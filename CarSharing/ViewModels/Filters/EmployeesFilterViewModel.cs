using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace CarSharing.ViewModels.Filters
{
    public class EmployeesFilterViewModel
    {
        public string EmployeePost { get; set; } = null!;
        public string EmployeeName { get; set; } = null!;
        public string EmployeeSurname { get; set; } = null!;
        public string EmployeePatronymic { get; set; } = null!;
        public DateTime EmployeeEmploymentDate { get; set; } = default!;

    }
}
