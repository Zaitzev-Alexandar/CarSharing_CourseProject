using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using System;
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
    public class CustomersController : Controller
    {
        private readonly car_sharingContext db;
        private readonly CacheProvider cache;

        private const string filterKey = "customers";

        public CustomersController(car_sharingContext context, CacheProvider cacheProvider)
        {
            db = context;
            cache = cacheProvider;
        }

        public IActionResult Index(SortState sortState = SortState.CustomersNameAsc, int page = 1)
        {
            CustomersFilterViewModel filter = HttpContext.Session.Get<CustomersFilterViewModel>(filterKey);
            if (filter == null)
            {
                filter = new CustomersFilterViewModel { CustomerName = string.Empty };
                HttpContext.Session.Set(filterKey, filter);
            }

            string modelKey = $"{typeof(Customer).Name}-{page}-{sortState}-{filter.CustomerName}-{filter.CustomerSurname} -{filter.CustomerPatronymic}-{filter.CustomerPhoneNum}-{filter.CustomerAddress}-{filter.CustomerPassportInfo}-{filter.CustomerBirthDate}";
            if (!cache.TryGetValue(modelKey, out CustomerViewModel model))
            {
                model = new CustomerViewModel();

                IQueryable<Customer> customers = GetSortedEntities(sortState, filter.CustomerName, filter.CustomerSurname, filter.CustomerPatronymic, filter.CustomerPhoneNum, filter.CustomerAddress, filter.CustomerPassportInfo, filter.CustomerBirthDate);

                int count = customers.Count();
                int pageSize = 10;
                model.PageViewModel = new PageViewModel(page, count, pageSize);

                model.Entities = count == 0 ? new List<Customer>() : customers.Skip((model.PageViewModel.CurrentPage - 1) * pageSize).Take(pageSize).ToList();
                model.SortViewModel = new SortViewModel(sortState);
                model.CustomersFilterViewModel = filter;

                cache.Set(modelKey, model);
            }

            return View(model);
        }

        [HttpPost]
        public IActionResult Index(CustomersFilterViewModel filterModel, int page)
        {
            CustomersFilterViewModel filter = HttpContext.Session.Get<CustomersFilterViewModel>(filterKey);
            if (filter != null)
            {

                filter.CustomerName = filterModel.CustomerName;

                filter.CustomerSurname = filterModel.CustomerSurname;
                filter.CustomerPatronymic = filterModel.CustomerPatronymic;

                filter.CustomerPhoneNum = filterModel.CustomerPhoneNum;
                filter.CustomerAddress = filterModel.CustomerAddress;
                filter.CustomerBirthDate = filterModel.CustomerBirthDate;
                filter.CustomerPassportInfo = filterModel.CustomerPassportInfo;
                filter.CustomerGender = filterModel.CustomerGender;

                HttpContext.Session.Remove(filterKey);
                HttpContext.Session.Set(filterKey, filter);
            }

            return RedirectToAction("Index", new { page });
        }
        [Authorize(Roles = "admin")]
        public IActionResult Create(int page)
        { 
            CustomerViewModel model = new CustomerViewModel
            {
                PageViewModel = new PageViewModel { CurrentPage = page }
            };

            return View(model);
        }

        [HttpPost]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> Create(CustomerViewModel model)
        {
            if (ModelState.IsValid & CheckUniqueValues(model.Entity))
            {
                await db.Customers.AddAsync(model.Entity);
                await db.SaveChangesAsync();

                cache.Clean();

                return RedirectToAction("Index", "Customers");
            }

            return View(model);
        }
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> Edit(int id, int page)
        {
            Customer customer = await db.Customers.FindAsync(id);
            if (customer != null)
            {
                CustomerViewModel model = new CustomerViewModel();
                model.PageViewModel = new PageViewModel { CurrentPage = page };
                model.Entity = customer;

                return View(model);
            }

            return NotFound();
        }

        [HttpPost]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> Edit(CustomerViewModel model)
        {
            if (ModelState.IsValid & CheckUniqueValues(model.Entity))
            {
                Customer customer = db.Customers.Find(model.Entity.CustomerId);
                if (customer != null)
                {
                    customer.Name = model.Entity.Name;
                    customer.Surname = model.Entity.Surname;
                    customer.Patronymic = model.Entity.Patronymic;
                    customer.PhoneNum = model.Entity.PhoneNum;
                    customer.Address = model.Entity.Address;
                    customer.BirthDate = model.Entity.BirthDate;
                    customer.PassportInfo = model.Entity.PassportInfo;
                    customer.Gender = model.Entity.Gender;

                    db.Customers.Update(customer);
                    await db.SaveChangesAsync();

                    cache.Clean();

                    return RedirectToAction("Index", "Customers", new { page = model.PageViewModel.CurrentPage });
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
            Customer customer = await db.Customers.FindAsync(id);
            if (customer == null)
                return NotFound();

            bool deleteFlag = false;
            string message = "Do you want to delete this entity";

            if (db.Rents.Any(s => s.CustomerId == customer.CustomerId))
                message = "This entity has entities, which dependents from this. Do you want to delete this entity and other, which dependents from this?";

            CustomerViewModel model = new CustomerViewModel();
            model.Entity = customer;
            model.PageViewModel = new PageViewModel { CurrentPage = page };
            model.DeleteViewModel = new DeleteViewModel { Message = message, IsDeleted = deleteFlag };

            return View(model);
        }

        [HttpPost]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> Delete(CustomerViewModel model)
        {
            Customer customer = await db.Customers.FindAsync(model.Entity.CustomerId);
            if (customer == null)
                return NotFound();

            db.Customers.Remove(customer);
            await db.SaveChangesAsync();

            cache.Clean();

            model.DeleteViewModel = new DeleteViewModel { Message = "The entity was successfully deleted.", IsDeleted = true };

            return View(model);
        }



        private bool CheckUniqueValues(Customer customer)
        {
            bool firstFlag = true;

            Customer tempCustomer = db.Customers.FirstOrDefault(g => g.Name == customer.Name);
            if (tempCustomer != null)
            {
                if (tempCustomer.CustomerId != customer.CustomerId)
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

        private IQueryable<Customer> GetSortedEntities(SortState sortState, string customerName, string customerSurname, string customerPatronymic, string customerPhoneNum, string customerAddress, string customerPassportInfo, DateTime customerBirthDate)
        {
            IQueryable<Customer> customers = db.Customers.AsQueryable();

            switch (sortState)
            {
                case SortState.CustomersNameAsc:
                    customers = customers.OrderBy(g => g.Name);
                    break;
                case SortState.CustomersNameDesc:
                    customers = customers.OrderByDescending(g => g.Name);
                    break;

                case SortState.CustomersSurnameAsc:
                    customers = customers.OrderBy(g => g.Surname);
                    break;
                case SortState.CustomersSurnameDesc:
                    customers = customers.OrderByDescending(g => g.Surname);
                    break;

                case SortState.CustomersPatronymicAsc:
                    customers = customers.OrderBy(g => g.Patronymic);
                    break;
                case SortState.CustomersPatronymicDesc:
                    customers = customers.OrderByDescending(g => g.Patronymic);
                    break;

                case SortState.CustomersPhoneNumAsc:
                    customers = customers.OrderBy(g => g.PhoneNum);
                    break;
                case SortState.CustomersPhoneNumDesc:
                    customers = customers.OrderByDescending(g => g.PhoneNum);
                    break;

                case SortState.CustomersAddressAsc:
                    customers = customers.OrderBy(g => g.Address);
                    break;
                case SortState.CustomersAddressDesc:
                    customers = customers.OrderByDescending(g => g.Address);
                    break;

                case SortState.CustomersBirthDateAsc:
                    customers = customers.OrderBy(g => g.BirthDate);
                    break;
                case SortState.CustomersBirthDateDesc:
                    customers = customers.OrderByDescending(g => g.BirthDate);
                    break;

                case SortState.CustomersPassportInfoAsc:
                    customers = customers.OrderBy(g => g.PassportInfo);
                    break;
                case SortState.CustomersPassportInfoDesc:
                    customers = customers.OrderByDescending(g => g.PassportInfo);
                    break;

                case SortState.CustomersGenderAsc:
                    customers = customers.OrderBy(g => g.Gender);
                    break;
                case SortState.CustomersGenderDesc:
                    customers = customers.OrderByDescending(g => g.Gender);
                    break;


            }

            if (!string.IsNullOrEmpty(customerName))
                customers = customers.Where(g => g.Name.Contains(customerName)).AsQueryable();
            if (!string.IsNullOrEmpty(customerSurname))
                customers = customers.Where(g => g.Surname.Contains(customerSurname)).AsQueryable();
            if (!string.IsNullOrEmpty(customerPatronymic))
                customers = customers.Where(g => g.Patronymic.Contains(customerPatronymic)).AsQueryable();

            if (!string.IsNullOrEmpty(customerPhoneNum))
                customers = customers.Where(g => g.Patronymic.Contains(customerPhoneNum)).AsQueryable();

            if (!string.IsNullOrEmpty(customerAddress))
                customers = customers.Where(g => g.Patronymic.Contains(customerAddress)).AsQueryable();

            if (!string.IsNullOrEmpty(customerPassportInfo))
                customers = customers.Where(g => g.Patronymic.Contains(customerPassportInfo)).AsQueryable();


            if (!string.IsNullOrEmpty(customerPassportInfo))
                customers = customers.Where(g => g.Patronymic.Contains(customerPassportInfo)).AsQueryable();

            if(customerBirthDate != default)
            {
                customers = customers.Where(g => g.BirthDate.Value.Date == customerBirthDate.Date).AsQueryable();
            }
            return customers;
        }
    }
}
