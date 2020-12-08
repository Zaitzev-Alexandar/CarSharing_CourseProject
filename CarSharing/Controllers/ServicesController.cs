using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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
    public class ServicesController : Controller
    {
        private readonly car_sharingContext db;
        private readonly CacheProvider cache;

        private const string filterKey = "services";

        public ServicesController(car_sharingContext context, CacheProvider cacheProvider)
        {
            db = context;
            cache = cacheProvider;
        }

        public IActionResult Index(SortState sortState = SortState.ServicesNameAsc, int page = 1)
        {
            ServicesFilterViewModel filter = HttpContext.Session.Get<ServicesFilterViewModel>(filterKey);
            if (filter == null)
            {
                filter = new ServicesFilterViewModel { ServiceName = string.Empty, ServiceDescription = string.Empty, ServicePrice = default};
                HttpContext.Session.Set(filterKey, filter);
            }

            string modelKey = $"{typeof(Service).Name}-{page}-{sortState}-{filter.ServiceName}-{filter.ServiceDescription}-{filter.ServicePrice}";
            if (!cache.TryGetValue(modelKey, out ServiceViewModel model))
            {
                model = new ServiceViewModel();

                IQueryable<Service> services = GetSortedEntities(sortState, filter.ServiceName, filter.ServicePrice, filter.ServiceDescription);

                int count = services.Count();
                int pageSize = 10;
                model.PageViewModel = new PageViewModel(page, count, pageSize);

                model.Entities = count == 0 ? new List<Service>() : services.Skip((model.PageViewModel.CurrentPage - 1) * pageSize).Take(pageSize).ToList();
                model.SortViewModel = new SortViewModel(sortState);
                model.ServicesFilterViewModel = filter;

                cache.Set(modelKey, model);
            }

            return View(model);
        }

        [HttpPost]
        public IActionResult Index(ServicesFilterViewModel filterModel, int page)
        {
            ServicesFilterViewModel filter = HttpContext.Session.Get<ServicesFilterViewModel>(filterKey);
            if (filter != null)
            {
                filter.ServiceName = filterModel.ServiceName;
                filter.ServiceDescription = filterModel.ServiceDescription;
                filter.ServicePrice = filterModel.ServicePrice;

                HttpContext.Session.Remove(filterKey);
                HttpContext.Session.Set(filterKey, filter);
            }

            return RedirectToAction("Index", new { page });
        }

        public IActionResult Create(int page)
        {
            ServiceViewModel model = new ServiceViewModel
            {
                PageViewModel = new PageViewModel { CurrentPage = page }
            };

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Create(ServiceViewModel model)
        {
            if (ModelState.IsValid & CheckUniqueValues(model.Entity))
            {
                await db.Services.AddAsync(model.Entity);
                await db.SaveChangesAsync();

                cache.Clean();

                return RedirectToAction("Index", "Services");
            }

            return View(model);
        }

        public async Task<IActionResult> Edit(int id, int page)
        {
            Service service = await db.Services.FindAsync(id);
            if (service != null)
            {
                ServiceViewModel model = new ServiceViewModel();
                model.PageViewModel = new PageViewModel { CurrentPage = page };
                model.Entity = service;

                return View(model);
            }

            return NotFound();
        }

        [HttpPost]
        public async Task<IActionResult> Edit(ServiceViewModel model)
        {
            if (ModelState.IsValid & CheckUniqueValues(model.Entity))
            {
                Service service = db.Services.Find(model.Entity.ServiceId);
                if (service != null)
                {
                    service.Name = model.Entity.Name;
                    service.Description = model.Entity.Description;
                    service.Price = model.Entity.Price;

                    db.Services.Update(service);
                    await db.SaveChangesAsync();

                    cache.Clean();

                    return RedirectToAction("Index", "Services", new { page = model.PageViewModel.CurrentPage });
                }
                else
                {
                    return NotFound();
                }
            }

            return View(model);
        }

        public async Task<IActionResult> Delete(int id, int page)
        {
            Service service = await db.Services.FindAsync(id);
            if (service == null)
                return NotFound();

            bool deleteFlag = false;
            string message = "Do you want to delete this entity";

            if (db.AdditionalServices.Any(s => s.ServiceId == service.ServiceId))
                message = "This entity has entities, which dependents from this. Do you want to delete this entity and other, which dependents from this?";

            ServiceViewModel model = new ServiceViewModel();
            model.Entity = service;
            model.PageViewModel = new PageViewModel { CurrentPage = page };
            model.DeleteViewModel = new DeleteViewModel { Message = message, IsDeleted = deleteFlag };

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Delete(ServiceViewModel model)
        {
            Service service = await db.Services.FindAsync(model.Entity.ServiceId);
            if (service == null)
                return NotFound();

            db.Services.Remove(service);
            await db.SaveChangesAsync();

            cache.Clean();

            model.DeleteViewModel = new DeleteViewModel { Message = "The entity was successfully deleted.", IsDeleted = true };

            return View(model);
        }



        private bool CheckUniqueValues(Service service)
        {
            bool firstFlag = true;

            Service tempService = db.Services.FirstOrDefault(g => g.Name == service.Name);
            if (tempService != null)
            {
                if (tempService.ServiceId != service.ServiceId)
                {
                    ModelState.AddModelError(string.Empty, "Another entity have this name. Please replace this to another.");
                    firstFlag = false;
                }
            }

            if (firstFlag )
                return true;
            else
                return false;
        }

        private IQueryable<Service> GetSortedEntities(SortState sortState, string serviceName, decimal servicePrice, string serviceDescription)
        {
            IQueryable<Service> services = db.Services.AsQueryable();

            switch (sortState)
            {
                case SortState.ServicesNameAsc:
                    services = services.OrderBy(g => g.Name);
                    break;
                case SortState.ServicesNameDesc:
                    services = services.OrderByDescending(g => g.Name);
                    break;
                case SortState.ServicesDescriptionAsc:
                    services = services.OrderBy(g => g.Description);
                    break;
                case SortState.ServicesDescriptionDesc:
                    services = services.OrderByDescending(g => g.Description);
                    break;
                case SortState.ServicesPriceAsc:
                    services = services.OrderBy(g => g.Price);
                    break;
                case SortState.ServicesPriceDesc:
                    services = services.OrderByDescending(g => g.Price);
                    break;
            }

            if (!string.IsNullOrEmpty(serviceName))
                services = services.Where(g => g.Name.Contains(serviceName)).AsQueryable();
            if (!string.IsNullOrEmpty(serviceDescription))
                services = services.Where(g => g.Name.Contains(serviceDescription)).AsQueryable();
            if (servicePrice > 0)
            {
                services = services.Where(g => g.Price >= (servicePrice - 1) && g.Price <= (servicePrice + 1)).AsQueryable();
            }
            return services;
        }
    }
}
