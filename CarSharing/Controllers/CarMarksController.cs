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

namespace Lab5_WebApp.Controllers
{
    [Authorize]
    public class CarMarksController : Controller
    {
        private readonly car_sharingContext db;
        private readonly CacheProvider cache;

        private const string filterKey = "carMarks";

        public CarMarksController(car_sharingContext context, CacheProvider cacheProvider)
        {
            db = context;
            cache = cacheProvider;
        }

        public IActionResult Index(SortState sortState = SortState.CarMarkNameAsc, int page = 1)
        {
            CarMarksFilterViewModel filter = HttpContext.Session.Get<CarMarksFilterViewModel>(filterKey);
            if (filter == null)
            {
                filter = new CarMarksFilterViewModel { CarMarkName = string.Empty };
                HttpContext.Session.Set(filterKey, filter);
            }

            string modelKey = $"{typeof(CarMark).Name}-{page}-{sortState}-{filter.CarMarkName}";
            if (!cache.TryGetValue(modelKey, out CarMarkViewModel model))
            {
                model = new CarMarkViewModel();

                IQueryable<CarMark> carMarks = GetSortedEntities(sortState, filter.CarMarkName);

                int count = carMarks.Count();
                int pageSize = 10;
                model.PageViewModel = new PageViewModel(page, count, pageSize);

                model.Entities = count == 0 ? new List<CarMark>() : carMarks.Skip((model.PageViewModel.CurrentPage - 1) * pageSize).Take(pageSize).ToList();
                model.SortViewModel = new SortViewModel(sortState);
                model.CarMarksFilterViewModel = filter;

                cache.Set(modelKey, model);
            }

            return View(model);
        }

        [HttpPost]
        public IActionResult Index(CarMarksFilterViewModel filterModel, int page)
        {
            CarMarksFilterViewModel filter = HttpContext.Session.Get<CarMarksFilterViewModel>(filterKey);
            if (filter != null)
            {
                filter.CarMarkName = filterModel.CarMarkName;

                HttpContext.Session.Remove(filterKey);
                HttpContext.Session.Set(filterKey, filter);
            }

            return RedirectToAction("Index", new { page });
        }

        public IActionResult Create(int page)
        {
            CarMarkViewModel model = new CarMarkViewModel
            {
                PageViewModel = new PageViewModel { CurrentPage = page }
            };

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Create(CarMarkViewModel model)
        {
            if (ModelState.IsValid & CheckUniqueValues(model.Entity))
            {
                await db.CarMarks.AddAsync(model.Entity);
                await db.SaveChangesAsync();

                cache.Clean();

                return RedirectToAction("Index", "CarMarks");
            }

            return View(model);
        }

        public async Task<IActionResult> Edit(int id, int page)
        {
            CarMark carMark = await db.CarMarks.FindAsync(id);
            if (carMark != null)
            {
                CarMarkViewModel model = new CarMarkViewModel();
                model.PageViewModel = new PageViewModel { CurrentPage = page };
                model.Entity = carMark;

                return View(model);
            }

            return NotFound();
        }

        [HttpPost]
        public async Task<IActionResult> Edit(CarMarkViewModel model)
        {
            if (ModelState.IsValid & CheckUniqueValues(model.Entity))
            {
                CarMark carMark = db.CarMarks.Find(model.Entity.CarMarkId);
                if (carMark != null)
                {
                    carMark.Name = model.Entity.Name;

                    db.CarMarks.Update(carMark);
                    await db.SaveChangesAsync();

                    cache.Clean();

                    return RedirectToAction("Index", "CarMarks", new { page = model.PageViewModel.CurrentPage });
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
            CarMark carMark = await db.CarMarks.FindAsync(id);
            if (carMark == null)
                return NotFound();

            bool deleteFlag = false;
            string message = "Do you want to delete this entity";

            if (db.CarModels.Any(s => s.CarMarkId == carMark.CarMarkId))
                message = "This entity has entities, which dependents from this. Do you want to delete this entity and other, which dependents from this?";

            CarMarkViewModel model = new CarMarkViewModel();
            model.Entity = carMark;
            model.PageViewModel = new PageViewModel { CurrentPage = page };
            model.DeleteViewModel = new DeleteViewModel { Message = message, IsDeleted = deleteFlag };

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Delete(CarMarkViewModel model)
        {
            CarMark carMark = await db.CarMarks.FindAsync(model.Entity.CarMarkId);
            if (carMark == null)
                return NotFound();

            db.CarMarks.Remove(carMark);
            await db.SaveChangesAsync();

            cache.Clean();

            model.DeleteViewModel = new DeleteViewModel { Message = "The entity was successfully deleted.", IsDeleted = true };

            return View(model);
        }



        private bool CheckUniqueValues(CarMark carMark)
        {
            bool firstFlag = true;

            CarMark tempCarMark = db.CarMarks.FirstOrDefault(g => g.Name == carMark.Name);
            if (tempCarMark != null)
            {
                if (tempCarMark.CarMarkId != carMark.CarMarkId)
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

        private IQueryable<CarMark> GetSortedEntities(SortState sortState, string carMarkName)
        {
            IQueryable<CarMark> carMarks = db.CarMarks.AsQueryable();

            switch (sortState)
            {
                case SortState.CarMarkNameAsc:
                    carMarks = carMarks.OrderBy(g => g.Name);
                    break;
                case SortState.CarMarkNameDesc:
                    carMarks = carMarks.OrderByDescending(g => g.Name);
                    break;
            }

            if (!string.IsNullOrEmpty(carMarkName))
                carMarks = carMarks.Where(g => g.Name.Contains(carMarkName)).AsQueryable();

            return carMarks;
        }
    }
}
