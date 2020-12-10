using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using CarSharing.Data;
using CarSharing.Infrastructure;
using CarSharing.Models;
using CarSharing.Services;
using CarSharing.ViewModels;
using CarSharing.ViewModels.Entities;
using CarSharing.ViewModels.Filters;

namespace CarSharing.Controllers
{
    [Authorize]
    public class AdditionalServicesController : Controller
    {
        private readonly car_sharingContext db;
        private readonly CacheProvider cache;
        private const string filterKey = "additionalServices";
        public AdditionalServicesController(car_sharingContext context, CacheProvider cacheProvider)
        {
            db = context;
            cache = cacheProvider;
        }

        public IActionResult Index(SortState sortState, int page = 1)
        {
            AdditionalServicesFilterViewModel filter = HttpContext.Session.Get<AdditionalServicesFilterViewModel>(filterKey);
            if (filter == null)
            {
                filter = new AdditionalServicesFilterViewModel { AdditionalServiceRentId = default, AdditionalServiceServiceName = string.Empty };
                HttpContext.Session.Set(filterKey, filter);
            }

            string modelKey = $"{typeof(Car).Name}-{page}-{sortState}-{filter.AdditionalServiceRentId}-{filter.AdditionalServiceServiceName}";
            if (!cache.TryGetValue(modelKey, out AdditionalServiceViewModel model))
            {
                model = new AdditionalServiceViewModel();

                IQueryable<AdditionalService> additionalServices = GetSortedEntities(sortState, filter.AdditionalServiceServiceName, filter.AdditionalServiceRentId);

                int count = additionalServices.Count();
                int pageSize = 10;
                model.PageViewModel = new PageViewModel(page, count, pageSize);

                model.Entities = count == 0 ? new List<AdditionalService>() : additionalServices.Skip((model.PageViewModel.CurrentPage - 1) * pageSize).Take(pageSize).ToList();
                model.SortViewModel = new SortViewModel(sortState);
                model.AdditionalServicesFilterViewModel = filter;

                cache.Set(modelKey, model);
            }

            return View(model);
        }

        [HttpPost]
        public IActionResult Index(AdditionalServicesFilterViewModel filterModel, int page)
        {
            AdditionalServicesFilterViewModel filter = HttpContext.Session.Get<AdditionalServicesFilterViewModel>(filterKey);
            if (filter != null)
            {
                filter.AdditionalServiceRentId = filterModel.AdditionalServiceRentId;
                filter.AdditionalServiceServiceName = filterModel.AdditionalServiceServiceName;


                HttpContext.Session.Remove(filterKey);
                HttpContext.Session.Set(filterKey, filter);
            }

            return RedirectToAction("Index", new { page });
        }


        [Authorize(Roles = "admin")]
        public IActionResult Create(int page)
        {
            AdditionalServiceViewModel model = new AdditionalServiceViewModel
            {
                PageViewModel = new PageViewModel { CurrentPage = page }
            };
            model.RentSelectList = db.Rents.ToList();
            model.ServiceSelectList = db.Services.ToList();

            return View(model);
        }

        [HttpPost]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> Create(AdditionalServiceViewModel model)
        {
            AdditionalService temp = new AdditionalService();
            model.RentSelectList = db.Rents.ToList();
            model.ServiceSelectList = db.Services.ToList();

            var rent = db.Rents.FirstOrDefault(g => g.RentId == model.RentId);
            if (rent == null)
            {
                ModelState.AddModelError(string.Empty, "Please select rent from list.");
                return View(model);
            }
            var service = db.Services.FirstOrDefault(g => g.Name == model.ServiceName);
            if (service == null)
            {
                ModelState.AddModelError(string.Empty, "Please select service from list.");
                return View(model);
            }
            if (ModelState.IsValid)
            {
                temp.RentId = rent.RentId;
                temp.ServiceId = service.ServiceId;

                await db.AdditionalServices.AddAsync(temp);
                await db.SaveChangesAsync();

                cache.Clean();

                return RedirectToAction("Index", "AdditionalServices");
            }

            return View(model);
        }
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> Edit(int id, int page)
        {
            AdditionalService additionalService = await db.AdditionalServices.FindAsync(id);
            if (additionalService != null)
            {
                AdditionalServiceViewModel model = new AdditionalServiceViewModel();
                model.PageViewModel = new PageViewModel { CurrentPage = page };
                model.Entity = additionalService;

                model.RentSelectList = db.Rents.ToList();
                model.ServiceSelectList = db.Services.ToList();
                model.RentId = model.Entity.Rent.RentId;
                model.ServiceName = model.Entity.Service.Name;

                return View(model);
            }

            return NotFound();
        }
        [HttpPost]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> Edit(AdditionalServiceViewModel model)
        {

            model.RentSelectList = db.Rents.ToList();
            model.ServiceSelectList = db.Services.ToList();

            var rent = db.Rents.FirstOrDefault(g => g.RentId == model.RentId);
            if (rent == null)
            {
                ModelState.AddModelError(string.Empty, "Please select rent from list.");
                return View(model);
            }
            var service = db.Services.FirstOrDefault(g => g.Name == model.ServiceName);
            if (service == null)
            {
                ModelState.AddModelError(string.Empty, "Please select service from list.");
                return View(model);
            }

            if (ModelState.IsValid & CheckUniqueValues(model.Entity))
            {
                AdditionalService additionalService = await db.AdditionalServices.FindAsync(model.Entity.AdditionalServiceId);
                if (additionalService != null)
                {

                    additionalService.RentId = rent.RentId;
                    additionalService.ServiceId = service.ServiceId;

                    db.AdditionalServices.Update(additionalService);
                    await db.SaveChangesAsync();

                    cache.Clean();

                    return RedirectToAction("Index", "AdditionalServices", new { page = model.PageViewModel.CurrentPage });

                }
                else
                {
                    return NotFound();
                }
            }

            return View(model);
        }
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> Delete(int id, int page)
        {
            AdditionalService additionalService = await db.AdditionalServices.FindAsync(id);
            if (additionalService == null)
                return NotFound();

            bool deleteFlag = false;
            string message = "Do you want to delete this entity";

            AdditionalServiceViewModel model = new AdditionalServiceViewModel();
            model.Entity = additionalService;
            model.PageViewModel = new PageViewModel { CurrentPage = page };
            model.DeleteViewModel = new DeleteViewModel { Message = message, IsDeleted = deleteFlag };

            return View(model);
        }

        [HttpPost]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> Delete(AdditionalServiceViewModel model)
        {
            AdditionalService additionalService = await db.AdditionalServices.FindAsync(model.Entity.AdditionalServiceId);
            if (additionalService == null)
                return NotFound();

            db.AdditionalServices.Remove(additionalService);
            await db.SaveChangesAsync();

            cache.Clean();

            model.DeleteViewModel = new DeleteViewModel { Message = "The entity was successfully deleted.", IsDeleted = true };

            return View(model);
        }


        private IQueryable<AdditionalService> GetSortedEntities(SortState sortState,
         string serviceName, int rentId)
        {
            IQueryable<AdditionalService> additionalServices = db.AdditionalServices.Include(a => a.Rent).Include(a => a.Service);

            switch (sortState)
            {
                case SortState.AdditionalServiceRentIdAsc:
                    additionalServices = additionalServices.OrderBy(g => g.RentId);
                    break;
                case SortState.AdditionalServiceRentIdDesc:
                    additionalServices = additionalServices.OrderByDescending(g => g.RentId);
                    break;

                case SortState.AdditionalServiceServiceNameAsc:
                    additionalServices = additionalServices.OrderBy(g => g.Service.Name);
                    break;
                case SortState.AdditionalServiceServiceNameDesc:
                    additionalServices = additionalServices.OrderByDescending(g => g.Service.Name);
                    break;


            }

            if (!string.IsNullOrEmpty(serviceName))
                additionalServices = additionalServices.Where(g => g.Service.Name.Contains(serviceName)).AsQueryable();

            if (rentId > 0)
            {
                additionalServices = additionalServices.Where(g => g.RentId == rentId).AsQueryable();
            }

            return additionalServices;
        }
        private bool CheckUniqueValues(AdditionalService additionalService)
        {
            bool firstFlag = true;

            AdditionalService tempAdditionalService = db.AdditionalServices.FirstOrDefault(g => g.AdditionalServiceId == additionalService.AdditionalServiceId);
            if (tempAdditionalService != null)
            {
                if (tempAdditionalService.AdditionalServiceId != additionalService.AdditionalServiceId)
                {
                    ModelState.AddModelError(string.Empty, "Another entity have this name. Please replace this to another.");
                    firstFlag = false;
                }
            }
            if (firstFlag)
                return true;
            else
                return false;
        }

    }
}
