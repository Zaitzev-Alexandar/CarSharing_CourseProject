using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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

namespace Lab5_WebApp.Controllers
{
    [Authorize]
    public class CarModelsController : Controller
    {
        private readonly car_sharingContext db;
        private readonly CacheProvider cache;

        private const string filterKey = "carModels";

        public CarModelsController(car_sharingContext context, CacheProvider cacheProvider)
        {
            db = context;
            cache = cacheProvider;
        }

        public IActionResult Index(SortState sortState = SortState.CarModelsNameAsc, int page = 1)
        {
            CarModelsFilterViewModel filter = HttpContext.Session.Get<CarModelsFilterViewModel>(filterKey);
            if (filter == null)
            {
                filter = new CarModelsFilterViewModel { CarModelName = string.Empty, CarModelDescription = string.Empty, CarMarkName = string.Empty };
                HttpContext.Session.Set(filterKey, filter);
            }

            string modelKey = $"{typeof(CarModel).Name}-{page}-{sortState}-{filter.CarModelName}-{filter.CarModelDescription}";
            if (!cache.TryGetValue(modelKey, out CarModelViewModel model))
            {
                model = new CarModelViewModel();

                IQueryable<CarModel> carModels = GetSortedEntities(sortState, filter.CarModelName, filter.CarModelDescription);

                int count = carModels.Count();
                int pageSize = 10;
                model.PageViewModel = new PageViewModel(page, count, pageSize);

                model.Entities = count == 0 ? new List<CarModel>() : carModels.Skip((model.PageViewModel.CurrentPage - 1) * pageSize).Take(pageSize).ToList();
                model.SortViewModel = new SortViewModel(sortState);
                model.CarModelsFilterViewModel = filter;

                cache.Set(modelKey, model);
            }

            return View(model);
        }

        [HttpPost]
        public IActionResult Index(CarModelsFilterViewModel filterModel, int page)
        {
            CarModelsFilterViewModel filter = HttpContext.Session.Get<CarModelsFilterViewModel>(filterKey);
            if (filter != null)
            {
                filter.CarModelName = filterModel.CarModelName;
                filter.CarModelDescription = filterModel.CarModelDescription;

                HttpContext.Session.Remove(filterKey);
                HttpContext.Session.Set(filterKey, filter);
            }

            return RedirectToAction("Index", new { page });
        }
        [Authorize(Roles = "admin")]
        public IActionResult Create(int page)
        {
            CarModelViewModel model = new CarModelViewModel
            {
                PageViewModel = new PageViewModel { CurrentPage = page }
            };
            model.SelectList = db.CarMarks.ToList();

            return View(model);
        }

        [HttpPost]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> Create(CarModelViewModel model)
        {
            model.SelectList = db.CarMarks.ToList();
            var carMark = db.CarMarks.FirstOrDefault(g => g.Name == model.CarMarkName);
            if (carMark == null)
            {
                ModelState.AddModelError(string.Empty, "Please select carMark from list.");
                return View(model);
            }
            if (ModelState.IsValid & CheckUniqueValues(model.Entity))
            {
                model.Entity.CarMarkId = carMark.CarMarkId;
                await db.CarModels.AddAsync(model.Entity);
                await db.SaveChangesAsync();

                cache.Clean();

                return RedirectToAction("Index", "CarModels");
            }

            return View(model);
        }
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> Edit(int id, int page)
        {
            CarModel carModel = await db.CarModels.FindAsync(id);
            if (carModel != null)
            {
                CarModelViewModel model = new CarModelViewModel();
                model.PageViewModel = new PageViewModel { CurrentPage = page };
                model.Entity = carModel;

                model.SelectList = db.CarMarks.ToList();
                model.CarMarkName= model.Entity.CarMark.Name;

                return View(model);
            }

            return NotFound();
        }

        [HttpPost]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> Edit(CarModelViewModel model)
        {
            model.SelectList = db.CarMarks.ToList();

            var carMark = db.CarMarks.FirstOrDefault(g => g.Name == model.CarMarkName);
            if (carMark == null)
            {
                ModelState.AddModelError(string.Empty, "Please select genre from list.");
                return View(model);
            }

            if (ModelState.IsValid & CheckUniqueValues(model.Entity))
            {
                CarModel carModel = await db.CarModels.FindAsync(model.Entity.CarModelId);
                if (carModel != null)
                {

                    carModel.Name = model.Entity.Name;
                    carModel.Description = model.Entity.Description;
                    carModel.CarMarkId = model.Entity.CarMarkId;
                    db.CarModels.Update(carModel);
                    await db.SaveChangesAsync();

                    cache.Clean();

                    return RedirectToAction("Index", "CarModels", new { page = model.PageViewModel.CurrentPage });

                }
                else
                {
                    return NotFound();
                }
            }

            return View(model);
        }

        public async Task<IActionResult> Details(int id, int page)
        {
            CarModel carModel = await db.CarModels.Include(s => s.CarMark).FirstOrDefaultAsync(s => s.CarModelId == id);
            if (carModel == null)
                return NotFound();

            CarModelViewModel model = new CarModelViewModel();
            model.Entity = carModel;
            model.PageViewModel = new PageViewModel { CurrentPage = page };

            return View(model);
        }
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> Delete(int id, int page)
        {
            CarModel carModel = await db.CarModels.FindAsync(id);
            if (carModel == null)
                return NotFound();

            bool deleteFlag = false;
            string message = "Do you want to delete this entity";

            if (db.Cars.Any(s => s.CarModelId == carModel.CarModelId))
                message = "This entity has entities, which dependents from this. Do you want to delete this entity and other, which dependents from this?";

            CarModelViewModel model = new CarModelViewModel();
            model.Entity = carModel;
            model.PageViewModel = new PageViewModel { CurrentPage = page };
            model.DeleteViewModel = new DeleteViewModel { Message = message, IsDeleted = deleteFlag };

            return View(model);
        }

        [HttpPost]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> Delete(CarModelViewModel model)
        {
            CarModel carModel = await db.CarModels.FindAsync(model.Entity.CarModelId);
            if (carModel == null)
                return NotFound();

            db.CarModels.Remove(carModel);
            await db.SaveChangesAsync();

            cache.Clean();

            model.DeleteViewModel = new DeleteViewModel { Message = "The entity was successfully deleted.", IsDeleted = true };

            return View(model);
        }



        private bool CheckUniqueValues(CarModel carModel)
        {
            bool firstFlag = true;

            CarModel tempCarModel = db.CarModels.FirstOrDefault(g => g.Name == carModel.Name);
            if (tempCarModel != null)
            {
                if (tempCarModel.CarMarkId != carModel.CarMarkId)
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

        private IQueryable<CarModel> GetSortedEntities(SortState sortState, string carModelName, string carModelDescription)
        {
            IQueryable<CarModel> carModels = db.CarModels.Include(g => g.CarMark).AsQueryable();

            switch (sortState)
            {
                case SortState.CarModelsNameAsc:
                    carModels = carModels.OrderBy(g => g.Name);
                    break;
                case SortState.CarModelsNameDesc:
                    carModels = carModels.OrderByDescending(g => g.Name);
                    break;
                case SortState.CarModelsDescriptionAsc:
                    carModels = carModels.OrderBy(g => g.Description);
                    break;
                case SortState.CarModelsDescriptionDesc:
                    carModels = carModels.OrderByDescending(g => g.Description);
                    break;
            }

            if (!string.IsNullOrEmpty(carModelName))
                carModels = carModels.Where(g => g.Name.Contains(carModelName)).AsQueryable();
            if (!string.IsNullOrEmpty(carModelDescription))
                carModels = carModels.Where(g => g.Description.Contains(carModelDescription)).AsQueryable();
            return carModels;
        }
    }
}
