using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using CarSharing.Models;
using CarSharing.ViewModels.Filters;

namespace CarSharing.ViewModels.Entities
{
    public class EmployeeViewModel : IEntitiesViewModel<Employee>
    {
        [Display(Name = "Employees")]
        public IEnumerable<Employee> Entities { get; set; }
        [Display(Name = "Employee")]
        public Employee Entity { get; set; }
        public PageViewModel PageViewModel { get; set; }
        public DeleteViewModel DeleteViewModel { get; set; }
        public SortViewModel SortViewModel { get; set; }
        public EmployeesFilterViewModel EmployeesFilterViewModel { get; set; }
    }
}
