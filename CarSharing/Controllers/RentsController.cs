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
using System;

namespace CarSharing.Controllers
{
    [Authorize]
    public class RentsController : Controller
    {
        private readonly car_sharingContext db;
        private readonly CacheProvider cache;

        private const string filterKey = "rents";

        public RentsController(car_sharingContext context, CacheProvider cacheProvider)
        {
            db = context;
            cache = cacheProvider;
        }

        public IActionResult Index(SortState sortState, int page = 1)
        {
            RentFilterViewModel filter = HttpContext.Session.Get<RentFilterViewModel>(filterKey);
            if (filter == null)
            {
                filter = new RentFilterViewModel { RentPrice = default, RentDeliveryDate = default, RentReturnDate = default};
                HttpContext.Session.Set(filterKey, filter);
            }

            string modelKey = $"{typeof(Rent).Name}-{page}-{sortState}-{filter.RentDeliveryDate}-{filter.RentReturnDate}-{filter.RentPrice}";
            if (!cache.TryGetValue(modelKey, out RentViewModel model))
            {
                model = new RentViewModel();

                IQueryable<Rent> rents = GetSortedEntities(sortState, filter.RentDeliveryDate, filter.RentReturnDate, filter.RentPrice);

                int count = rents.Count();
                int pageSize = 10;
                model.PageViewModel = new PageViewModel(page, count, pageSize);

                model.Entities = count == 0 ? new List<Rent>() : rents.Skip((model.PageViewModel.CurrentPage - 1) * pageSize).Take(pageSize).ToList();
                model.SortViewModel = new SortViewModel(sortState);
                model.RentFilterViewModel = filter;

                cache.Set(modelKey, model);
            }

            return View(model);
        }

        [HttpPost]
        public IActionResult Index(RentFilterViewModel filterModel, int page)
        {
            RentFilterViewModel filter = HttpContext.Session.Get<RentFilterViewModel>(filterKey);
            if (filter != null)
            {
                filter.RentDeliveryDate = filterModel.RentDeliveryDate;
                filter.RentReturnDate = filterModel.RentReturnDate;
                filter.RentPrice = filterModel.RentPrice;

                HttpContext.Session.Remove(filterKey);
                HttpContext.Session.Set(filterKey, filter);
            }

            return RedirectToAction("Index", new { page });
        }

        [Authorize(Roles = "admin")]
        public IActionResult Create(int page)
        {
            RentViewModel model = new RentViewModel
            {
                PageViewModel = new PageViewModel { CurrentPage = page }
            };

            model.CarSelectList = db.Cars.ToList();
            model.EmployeeSelectList = db.Employees.ToList();
            model.CustomerSelectList = db.Customers.ToList(); ;

            return View(model);
        }

        [HttpPost]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> Create(RentViewModel model)
        {
            model.CarSelectList = db.Cars.ToList();
            model.EmployeeSelectList = db.Employees.ToList();
            model.CustomerSelectList = db.Customers.ToList();

            var employee = db.Employees.FirstOrDefault(g => g.Name == model.EmplpoyeeName);
            if (employee == null)
            {
                ModelState.AddModelError(string.Empty, "Please select employee from list.");
                return View(model);
            }
            var customer = db.Customers.FirstOrDefault(g => g.Name == model.CustomerName);
            if (customer == null)
            {
                ModelState.AddModelError(string.Empty, "Please select customer from list.");
                return View(model);
            }

            var car = db.Cars.FirstOrDefault(g => g.VINcode == model.CarVINcode);
            if (car == null)
            {
                ModelState.AddModelError(string.Empty, "Please select car from list.");
                return View(model);
            }

            if (ModelState.IsValid & CheckUniqueValues(car, model.Entity))
            {
                model.Entity.CarId = car.CarId;
                model.Entity.EmployeeId = employee.EmployeeId;
                model.Entity.CustomerId = customer.CustomerId;

                await db.Rents.AddAsync(model.Entity);
                await db.SaveChangesAsync();

                cache.Clean();

                return RedirectToAction("Index", "Rents");
            }

            return View(model);
        }
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> Edit(int id, int page)
        {
            Rent rent = await db.Rents.FindAsync(id);
            if (rent != null)
            {
                RentViewModel model = new RentViewModel();
                model.PageViewModel = new PageViewModel { CurrentPage = page };
                model.Entity = rent;

                model.CarSelectList = db.Cars.ToList();
                model.EmployeeSelectList = db.Employees.ToList();
                model.CustomerSelectList = db.Customers.ToList();
                model.CarVINcode = model.Entity.Car.VINcode;
                model.EmplpoyeeName = model.Entity.Employee.Name;
                model.CustomerName = model.Entity.Customer.Name;

                return View(model);
            }

            return NotFound();
        }

        [HttpPost]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> Edit(RentViewModel model)
        {
            model.CarSelectList = db.Cars.ToList();
            model.EmployeeSelectList = db.Employees.ToList();
            model.CustomerSelectList = db.Customers.ToList();

            var employee = db.Employees.FirstOrDefault(g => g.Name == model.EmplpoyeeName);
            if (employee == null)
            {
                ModelState.AddModelError(string.Empty, "Please select employee from list.");
                return View(model);
            }
            var customer = db.Customers.FirstOrDefault(g => g.Name == model.CustomerName);
            if (customer == null)
            {
                ModelState.AddModelError(string.Empty, "Please select customer from list.");
                return View(model);
            }

            var car = db.Cars.FirstOrDefault(g => g.VINcode == model.CarVINcode);
            if (car == null)
            {
                ModelState.AddModelError(string.Empty, "Please select car from list.");
                return View(model);
            }

            if (ModelState.IsValid)
            {
                Rent rent = await db.Rents.FindAsync(model.Entity.CarId);
                if (rent != null && CheckUniqueValues(car, rent, model.Entity) == true)
                {
                    rent.CarId = model.Entity.CarId;
                    rent.CustomerId = model.Entity.CustomerId;
                    rent.EmployeeId = model.Entity.EmployeeId;
                    rent.ReturnDate = model.Entity.ReturnDate;
                    rent.DeliveryDate = model.Entity.DeliveryDate;
                    rent.Price = model.Entity.Price;
                    
                    db.Rents.Update(rent);
                    await db.SaveChangesAsync();

                    cache.Clean();

                    return RedirectToAction("Index", "Rents", new { page = model.PageViewModel.CurrentPage });

                }
                else
                {
                    return NotFound();
                }
            }

            return View(model);
        }
        private bool CheckUniqueValues(Car car, Rent rent)
        {
            bool firstFlag = true;
            bool secondFlag = true;
            if(rent.ReturnDate < rent.DeliveryDate)
            {
                firstFlag = false;
                ModelState.AddModelError(string.Empty, "Return date can't be less than delivery date.Change dates to anothers");
            }
            //Список аренд которые имеют дату возврата меньше чем дату отправки новой аренды
            List<Rent> carRents = db.Rents.Where(g => g.CarId == car.CarId).ToList() ;
            foreach(Rent carRent in carRents)
            {
                if(CheckRentCross(rent, carRent) == true)
                {
                    secondFlag = false;
                    ModelState.AddModelError(string.Empty, "Select car is already in another rent.Change dates of cars");
                    break;
                }
            }
                    //ModelState.AddModelError(string.Empty, "Another entity have this name. Please replace this to another.");
  
            if (firstFlag == true && secondFlag == true)
                return true;
            else
                return false;
        }
        private bool CheckUniqueValues(Car car, Rent oldRent, Rent newRent)
        {
            bool firstFlag = true;
            bool secondFlag = true;
            if (newRent.ReturnDate < newRent.DeliveryDate)
            {
                firstFlag = false;
                ModelState.AddModelError(string.Empty, "Return date can't be less than delivery date.Change dates to anothers");
            }
            //Список аренд которые имеют дату возврата меньше чем дату отправки новой аренды
            List<Rent> carRents = db.Rents.Where(g => g.CarId == car.CarId).ToList();
            foreach (Rent carRent in carRents)
            {
                if (CheckRentCross(newRent, carRent) == true && carRent!= oldRent)
                {
                    secondFlag = false;
                    ModelState.AddModelError(string.Empty, "Select car is already in another rent.Change dates of cars");
                    break;
                }
            }
            //ModelState.AddModelError(string.Empty, "Another entity have this name. Please replace this to another.");

            if (firstFlag == true && secondFlag == true)
                return true;
            else
                return false;
        }
        private bool CheckRentCross(Rent rent1, Rent rent2)
        {
            bool result = true;
            //if((rent1.DeliveryDate > rent2.ReturnDate) || (rent1.ReturnDate < rent2.DeliveryDate))
            if(rent1.DeliveryDate > rent2.ReturnDate)
            { result = false; }
            else
            { result = true; }
            if(result == true)
            {
                if(rent1.ReturnDate < rent2.DeliveryDate) { result = false; }
                else { result = true; }
            }
            return result;
        }



        public async Task<IActionResult> Delete(int id, int page)
        {
            Rent rent = await db.Rents.FindAsync(id);
            if (rent == null)
                return NotFound();

            bool deleteFlag = false;
            string message = "Do you want to delete this entity";

            if (db.Cars.Any(s => s.CarId == rent.CarId))
                message = "This entity has entities, which dependents from this. Do you want to delete this entity and other, which dependents from this?";

            RentViewModel model = new RentViewModel();
            model.Entity = rent;
            model.PageViewModel = new PageViewModel { CurrentPage = page };
            model.DeleteViewModel = new DeleteViewModel { Message = message, IsDeleted = deleteFlag };

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Delete(RentViewModel model)
        {
            Rent rent = await db.Rents.FindAsync(model.Entity.RentId);
            if (rent == null)
                return NotFound();

            db.Rents.Remove(rent);
            await db.SaveChangesAsync();

            cache.Clean();

            model.DeleteViewModel = new DeleteViewModel { Message = "The entity was successfully deleted.", IsDeleted = true };

            return View(model);
        }

        private IQueryable<Rent> GetSortedEntities(SortState sortState, DateTime rentDeliveryDate, DateTime rentReturnDate, decimal rentPrice)
        {
            IQueryable<Rent> rents = db.Rents.Include(r => r.Car).Include(r => r.Customer).Include(r => r.Employee);

            switch (sortState)
            {
                case SortState.RentDeliveryDateAsc:
                    rents = rents.OrderBy(g => g.DeliveryDate);
                    break;
                case SortState.RentDeliveryDateDesc:
                    rents = rents.OrderByDescending(g => g.DeliveryDate);
                    break;

                case SortState.RentReturnDateAsc:
                    rents = rents.OrderBy(g => g.ReturnDate);
                    break;
                case SortState.RentReturnDateDesc:
                    rents = rents.OrderByDescending(g => g.ReturnDate);
                    break;

                case SortState.RentPriceAsc:
                    rents = rents.OrderBy(g => g.Price);
                    break;
                case SortState.RentPriceDesc:
                    rents = rents.OrderByDescending(g => g.Price);
                    break;
            }

            if (rentDeliveryDate != default)
            {
                rents = rents.Where(g => g.DeliveryDate.Date == rentDeliveryDate.Date).AsQueryable();
            }
            if (rentReturnDate != default)
            {
                rents = rents.Where(g => g.ReturnDate.Date == rentReturnDate.Date).AsQueryable();
            }
            if (rentPrice > 0)
            {
                rents = rents.Where(g => g.Price >= (rentPrice - 1) && g.Price <= (rentPrice + 1)).AsQueryable();
            }

            return rents;
        }
    }
}
