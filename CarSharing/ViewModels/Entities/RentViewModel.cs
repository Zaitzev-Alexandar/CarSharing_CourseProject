using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using CarSharing.Models;
using CarSharing.ViewModels.Filters;

namespace CarSharing.ViewModels.Entities
{
    public class RentViewModel
    {
        [Display(Name = "Rents")]
        public IEnumerable<Rent> Entities { get; set; }
        [Display(Name = "Rent")]
        public Rent Entity { get; set; }
        [Display(Name = "Cars")]
        public IEnumerable<Car> CarSelectList { get; set; }
        [Display(Name = "Employees")]
        public IEnumerable<Employee> EmployeeSelectList { get; set; }
        [Display(Name = "Customers")]
        public IEnumerable<Customer> CustomerSelectList { get; set; }
        public string CarVINcode { get; set; }
        public string EmplpoyeeName { get; set; }
        public string CustomerName { get; set; }
        public PageViewModel PageViewModel { get; set; }
        public DeleteViewModel DeleteViewModel { get; set; }
        public SortViewModel SortViewModel { get; set; }
        public RentFilterViewModel RentFilterViewModel { get; set; }
    }
}
