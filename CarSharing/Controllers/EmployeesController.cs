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
    public class EmployeesController : Controller
    {
        private readonly car_sharingContext db;
        private readonly CacheProvider cache;

        private const string filterKey = "employees";

        public EmployeesController(car_sharingContext context, CacheProvider cacheProvider)
        {
            db = context;
            cache = cacheProvider;
        }

        public IActionResult Index(SortState sortState = SortState.EmployeesNameAsc, int page = 1)
        {
            EmployeesFilterViewModel filter = HttpContext.Session.Get<EmployeesFilterViewModel>(filterKey);
            if (filter == null)
            {
                filter = new EmployeesFilterViewModel { EmployeeName = string.Empty };
                HttpContext.Session.Set(filterKey, filter);
            }

            string modelKey = $"{typeof(Employee).Name}-{page}-{sortState}-{filter.EmployeePost}-{filter.EmployeeName}-{filter.EmployeeSurname} -{filter.EmployeePatronymic}";
            if (!cache.TryGetValue(modelKey, out EmployeeViewModel model))
            {
                model = new EmployeeViewModel();

                IQueryable<Employee> employees = GetSortedEntities(sortState, filter.EmployeePost, filter.EmployeeName, filter.EmployeeSurname, filter.EmployeePatronymic);

                int count = employees.Count();
                int pageSize = 10;
                model.PageViewModel = new PageViewModel(page, count, pageSize);

                model.Entities = count == 0 ? new List<Employee>() : employees.Skip((model.PageViewModel.CurrentPage - 1) * pageSize).Take(pageSize).ToList();
                model.SortViewModel = new SortViewModel(sortState);
                model.EmployeesFilterViewModel = filter;

                cache.Set(modelKey, model);
            }

            return View(model);
        }

        [HttpPost]
        public IActionResult Index(EmployeesFilterViewModel filterModel, int page)
        {
            EmployeesFilterViewModel filter = HttpContext.Session.Get<EmployeesFilterViewModel>(filterKey);
            if (filter != null)
            {
                filter.EmployeePost = filterModel.EmployeePost;
                filter.EmployeeName = filterModel.EmployeeName;

                filter.EmployeeSurname = filterModel.EmployeeSurname;
                filter.EmployeePatronymic = filterModel.EmployeePatronymic;

                HttpContext.Session.Remove(filterKey);
                HttpContext.Session.Set(filterKey, filter);
            }

            return RedirectToAction("Index", new { page });
        }

        public IActionResult Create(int page)
        {
            EmployeeViewModel model = new EmployeeViewModel
            {
                PageViewModel = new PageViewModel { CurrentPage = page }
            };

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Create(EmployeeViewModel model)
        {
            if (ModelState.IsValid & CheckUniqueValues(model.Entity))
            {
                await db.Employees.AddAsync(model.Entity);
                await db.SaveChangesAsync();

                cache.Clean();

                return RedirectToAction("Index", "Employees");
            }

            return View(model);
        }

        public async Task<IActionResult> Edit(int id, int page)
        {
            Employee employee = await db.Employees.FindAsync(id);
            if (employee != null)
            {
                EmployeeViewModel model = new EmployeeViewModel();
                model.PageViewModel = new PageViewModel { CurrentPage = page };
                model.Entity = employee;

                return View(model);
            }

            return NotFound();
        }

        [HttpPost]
        public async Task<IActionResult> Edit(EmployeeViewModel model)
        {
            if (ModelState.IsValid & CheckUniqueValues(model.Entity))
            {
                Employee employee = db.Employees.Find(model.Entity.EmployeeId);
                if (employee != null)
                {
                    employee.Name = model.Entity.Name;
                    employee.Surname = model.Entity.Surname;
                    employee.Patronymic = model.Entity.Patronymic;
                    employee.Post = model.Entity.Post;
                    employee.EmploymentDate = model.Entity.EmploymentDate;

                    db.Employees.Update(employee);
                    await db.SaveChangesAsync();

                    cache.Clean();

                    return RedirectToAction("Index", "Employees", new { page = model.PageViewModel.CurrentPage });
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
            Employee employee = await db.Employees.FindAsync(id);
            if (employee == null)
                return NotFound();

            bool deleteFlag = false;
            string message = "Do you want to delete this entity";

            if (db.Cars.Any(s => s.EmployeeId == employee.EmployeeId))
                message = "This entity has entities, which dependents from this. Do you want to delete this entity and other, which dependents from this?";

            EmployeeViewModel model = new EmployeeViewModel();
            model.Entity = employee;
            model.PageViewModel = new PageViewModel { CurrentPage = page };
            model.DeleteViewModel = new DeleteViewModel { Message = message, IsDeleted = deleteFlag };

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Delete(EmployeeViewModel model)
        {
            Employee employee = await db.Employees.FindAsync(model.Entity.EmployeeId);
            if (employee == null)
                return NotFound();

            db.Employees.Remove(employee);
            await db.SaveChangesAsync();

            cache.Clean();

            model.DeleteViewModel = new DeleteViewModel { Message = "The entity was successfully deleted.", IsDeleted = true };

            return View(model);
        }



        private bool CheckUniqueValues(Employee employee)
        {
            bool firstFlag = true;

            Employee tempEmployee = db.Employees.FirstOrDefault(g => g.Name == employee.Name);
            if (tempEmployee != null)
            {
                if (tempEmployee.EmployeeId != employee.EmployeeId)
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

        private IQueryable<Employee> GetSortedEntities(SortState sortState,string employeePost, string employeeName, string employeeSurname, string employeePatronymic)
        {
            IQueryable<Employee> employees = db.Employees.AsQueryable();

            switch (sortState)
            {
                case SortState.EmployeesPostAsc:
                    employees = employees.OrderBy(g => g.Post);
                    break;
                case SortState.EmployeesPostDesc:
                    employees = employees.OrderByDescending(g => g.Post);
                    break;
                case SortState.EmployeesNameAsc:
                    employees = employees.OrderBy(g => g.Name);
                    break;
                case SortState.EmployeesNameDesc:
                    employees = employees.OrderByDescending(g => g.Name);
                    break;
                case SortState.EmployeesSurnameAsc:
                    employees = employees.OrderBy(g => g.Surname);
                    break;
                case SortState.EmployeesSurnameDesc:
                    employees = employees.OrderByDescending(g => g.Surname);
                    break;
                case SortState.EmployeesPatronymicAsc:
                    employees = employees.OrderBy(g => g.Patronymic);
                    break;
                case SortState.EmployeesPatronymicDesc:
                    employees = employees.OrderByDescending(g => g.Patronymic);
                    break;
            }

            if (!string.IsNullOrEmpty(employeePost))
                employees = employees.Where(g => g.Post.Contains(employeePost)).AsQueryable();
            if (!string.IsNullOrEmpty(employeeName))
                employees = employees.Where(g => g.Name.Contains(employeeName)).AsQueryable();
            if (!string.IsNullOrEmpty(employeeSurname))
                employees = employees.Where(g => g.Surname.Contains(employeeSurname)).AsQueryable();
            if (!string.IsNullOrEmpty(employeePatronymic))
                employees = employees.Where(g => g.Patronymic.Contains(employeePatronymic)).AsQueryable();

            return employees;
        }
    }
}
