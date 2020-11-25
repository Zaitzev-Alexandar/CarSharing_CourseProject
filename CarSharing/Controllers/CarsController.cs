﻿using Microsoft.AspNetCore.Authorization;
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
using System;

namespace Lab5_WebApp.Controllers
{
    [Authorize]
    public class CarsController : Controller
    {
        private readonly car_sharingContext db;
        private readonly CacheProvider cache;

        private const string filterKey = "cars";

        public CarsController(car_sharingContext context, CacheProvider cacheProvider)
        {
            db = context;
            cache = cacheProvider;
        }

        public IActionResult Index(SortState sortState = SortState.CarsVINcodeAsc, int page = 1)
        {
            CarsFilterViewModel filter = HttpContext.Session.Get<CarsFilterViewModel>(filterKey);
            if (filter == null)
            {
                filter = new CarsFilterViewModel { CarEngineNum = default, CarIssueDate = default, CarPrice = default, CarRegNum = default, CarRentalPrice = default, CarReturnMark = default, CarSpecMark = default, CarSpecs = string.Empty, CarTechnicalMaintenanceDate = default, CarVINcode = string.Empty };
                HttpContext.Session.Set(filterKey, filter);
            }

            string modelKey = $"{typeof(CarModel).Name}-{page}-{sortState}-{filter.CarEngineNum}-{filter.CarIssueDate}-{filter.CarPrice}-{filter.CarRegNum}-{filter.CarRentalPrice}-{filter.CarReturnMark}-{filter.CarSpecMark}-{filter.CarSpecs}-{filter.CarTechnicalMaintenanceDate}-{filter.CarTechnicalMaintenanceDate}-{filter.CarVINcode}";
            if (!cache.TryGetValue(modelKey, out CarViewModel model))
            {
                model = new CarViewModel();

                IQueryable<Car> cars = GetSortedEntities(sortState, filter.CarVINcode, filter.CarEngineNum, filter.CarPrice, filter.CarRentalPrice, filter.CarIssueDate, filter.CarSpecs, filter.CarTechnicalMaintenanceDate, filter.CarSpecMark, filter.CarReturnMark);

                int count = cars.Count();
                int pageSize = 10;
                model.PageViewModel = new PageViewModel(page, count, pageSize);

                model.Entities = count == 0 ? new List<Car>() : cars.Skip((model.PageViewModel.CurrentPage - 1) * pageSize).Take(pageSize).ToList();
                model.SortViewModel = new SortViewModel(sortState);
                model.CarsFilterViewModel = filter;

                cache.Set(modelKey, model);
            }

            return View(model);
        }

        [HttpPost]
        public IActionResult Index(CarsFilterViewModel filterModel, int page)
        {
            CarsFilterViewModel filter = HttpContext.Session.Get<CarsFilterViewModel>(filterKey);
            if (filter != null)
            {
                filter.CarEngineNum = filterModel.CarEngineNum;
                filter.CarIssueDate = filterModel.CarIssueDate;
                filter.CarPrice = filterModel.CarPrice;
                filter.CarRegNum = filterModel.CarRegNum;
                filter.CarRentalPrice = filterModel.CarRentalPrice;
                filter.CarReturnMark = filterModel.CarReturnMark;
                filter.CarSpecMark = filterModel.CarSpecMark;
                filter.CarSpecs = filterModel.CarSpecs;
                filter.CarTechnicalMaintenanceDate = filterModel.CarTechnicalMaintenanceDate;
                filter.CarVINcode = filterModel.CarVINcode;

                HttpContext.Session.Remove(filterKey);
                HttpContext.Session.Set(filterKey, filter);
            }

            return RedirectToAction("Index", new { page });
        }

        public IActionResult Create(int page)
        {
            CarViewModel model = new CarViewModel
            {
                PageViewModel = new PageViewModel { CurrentPage = page }
            };
            model.SelectList1 = db.CarModels.ToList();
            model.SelectList2 = db.Employees.ToList();

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Create(CarViewModel model)
        {
            model.SelectList1 = db.CarModels.ToList();
            model.SelectList2 = db.Employees.ToList();

            var carModel = db.CarModels.FirstOrDefault(g => g.Name == model.CarModelName);
            if (carModel == null)
            {
                ModelState.AddModelError(string.Empty, "Please select car model from list.");
                return View(model);
            }
            var employee = db.Employees.FirstOrDefault(g => g.Name == model.EmplpoyeeName);
            if (employee == null)
            {
                ModelState.AddModelError(string.Empty, "Please select employee from list.");
                return View(model);
            }
            if (ModelState.IsValid & CheckUniqueValues(model.Entity))
            {
                model.Entity.CarModelId = carModel.CarModelId;
                model.Entity.EmployeeId = employee.EmployeeId;
                await db.Cars.AddAsync(model.Entity);
                await db.SaveChangesAsync();

                cache.Clean();

                return RedirectToAction("Index", "Cars");
            }

            return View(model);
        }

        public async Task<IActionResult> Edit(int id, int page)
        {
            Car car = await db.Cars.FindAsync(id);
            if (car != null)
            {
                CarViewModel model = new CarViewModel();
                model.PageViewModel = new PageViewModel { CurrentPage = page };
                model.Entity = car;

                model.SelectList1 = db.CarModels.ToList();
                model.SelectList2 = db.Employees.ToList();
                model.CarModelName= model.Entity.CarModel.Name;
                model.EmplpoyeeName = model.Entity.Employee.Name;

                return View(model);
            }

            return NotFound();
        }

        [HttpPost]
        public async Task<IActionResult> Edit(CarViewModel model)
        {
            model.SelectList1 = db.CarModels.ToList();
            model.SelectList2 = db.Employees.ToList();

            var carModel = db.CarModels.FirstOrDefault(g => g.Name == model.CarModelName);
            if (carModel == null)
            {
                ModelState.AddModelError(string.Empty, "Please select car model from list.");
                return View(model);
            }
            var employee = db.Employees.FirstOrDefault(g => g.Name == model.EmplpoyeeName);
            if (employee == null)
            {
                ModelState.AddModelError(string.Empty, "Please select employee from list.");
                return View(model);
            }

            if (ModelState.IsValid & CheckUniqueValues(model.Entity))
            {
                Car car = await db.Cars.FindAsync(model.Entity.CarId);
                if (car != null)
                {

                    car.CarModelId = model.Entity.CarModelId;
                    car.EmployeeId = model.Entity.EmployeeId;
                    car.EngineNum = model.Entity.EngineNum;
                    car.IssueDate = model.Entity.IssueDate;
                    car.Price = model.Entity.Price;
                    car.RegNum = model.Entity.RegNum;
                    car.RentalPrice = model.Entity.RentalPrice;
                    car.ReturnMark = model.Entity.ReturnMark;
                    car.SpecMark = model.Entity.SpecMark;
                    car.Specs = model.Entity.Specs;
                    car.TechnicalMaintenanceDate = model.Entity.TechnicalMaintenanceDate;
                    car.VINcode = model.Entity.VINcode;
                    db.Cars.Update(car);
                    await db.SaveChangesAsync();

                    cache.Clean();

                    return RedirectToAction("Index", "Cars", new { page = model.PageViewModel.CurrentPage });

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
            Car car = await db.Cars.FindAsync(id);
            if (car == null)
                return NotFound();

            bool deleteFlag = false;
            string message = "Do you want to delete this entity";

            CarViewModel model = new CarViewModel();
            model.Entity = car;
            model.PageViewModel = new PageViewModel { CurrentPage = page };
            model.DeleteViewModel = new DeleteViewModel { Message = message, IsDeleted = deleteFlag };

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Delete(CarViewModel model)
        {
            Car car = await db.Cars.FindAsync(model.Entity.CarId);
            if (car == null)
                return NotFound();

            db.Cars.Remove(car);
            await db.SaveChangesAsync();

            cache.Clean();

            model.DeleteViewModel = new DeleteViewModel { Message = "The entity was successfully deleted.", IsDeleted = true };

            return View(model);
        }



        private bool CheckUniqueValues(Car car)
        {
            bool firstFlag = true;

            Car tempCar = db.Cars.FirstOrDefault(g => g.VINcode == car.VINcode);
            if (tempCar != null)
            {
                if (tempCar.CarId != car.CarId)
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

        private IQueryable<Car> GetSortedEntities(SortState sortState,
         string carVINcode,
         int carEngineNum,
         decimal carPrice,
         decimal carRentalPrice, 
         DateTime carIssueDate, 
         string carSpecs, 
         DateTime carTechnicalMaintenanceDate, 
         bool carSpecMark, 
         bool carReturnMark
        )
        {
            IQueryable<Car> cars = db.Cars.Include(g => g.CarModel).Include(c => c.Employee).AsQueryable();

            switch (sortState)
            {
                case SortState.CarsVINcodeAsc:
                    cars = cars.OrderBy(g => g.VINcode);
                    break;
                case SortState.CarsVINcodeDesc:
                    cars = cars.OrderByDescending(g => g.VINcode);
                    break;

                case SortState.CarsEngineNumAsc:
                    cars = cars.OrderBy(g => g.EngineNum);
                    break;
                case SortState.CarsEngineNumDesc:
                    cars = cars.OrderByDescending(g => g.EngineNum);
                    break;

                case SortState.CarsPriceAsc:
                    cars = cars.OrderBy(g => g.Price);
                    break;
                case SortState.CarsPriceDesc:
                    cars = cars.OrderByDescending(g => g.Price);
                    break;

                case SortState.CarsRentalPriceAsc:
                    cars = cars.OrderBy(g => g.RentalPrice);
                    break;
                case SortState.CarsRentalPriceDesc:
                    cars = cars.OrderByDescending(g => g.RentalPrice);
                    break;

                case SortState.CarsIssueDateAsc:
                    cars = cars.OrderBy(g => g.IssueDate);
                    break;
                case SortState.CarsIssueDateDesc:
                    cars = cars.OrderByDescending(g => g.IssueDate);
                    break;

                case SortState.CarsSpecsAsc:
                    cars = cars.OrderBy(g => g.Specs);
                    break;
                case SortState.CarsSpecsDesc:
                    cars = cars.OrderByDescending(g => g.Specs);
                    break;

                case SortState.CarsTechnicalMaintenanceDateAsc:
                    cars = cars.OrderBy(g => g.TechnicalMaintenanceDate);
                    break;
                case SortState.CarsTechnicalMaintenanceDateDesc:
                    cars = cars.OrderByDescending(g => g.TechnicalMaintenanceDate);
                    break;

                case SortState.CarsSpecMarkAsc:
                    cars = cars.OrderBy(g => g.SpecMark);
                    break;
                case SortState.CarsSpecMarkDesc:
                    cars = cars.OrderByDescending(g => g.SpecMark);
                    break;

                case SortState.CarsReturnMarkAsc:
                    cars = cars.OrderBy(g => g.ReturnMark);
                    break;
                case SortState.CarsReturnMarkDesc:
                    cars = cars.OrderByDescending(g => g.ReturnMark);
                    break;
            }

            if (!string.IsNullOrEmpty(carVINcode))
                cars = cars.Where(g => g.VINcode.Contains(carVINcode)).AsQueryable();

            if (!string.IsNullOrEmpty(carSpecs))
                cars = cars.Where(g => g.Specs.Contains(carSpecs)).AsQueryable();
            return cars;
        }
    }
}