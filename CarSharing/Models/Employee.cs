using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace CarSharing.Models
{
    public class Employee
    {
        public int EmployeeId { get; set; }
        public string Post { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }
        public string Patronymic { get; set; }
        public DateTime EmploymentDate { get; set; }
        public virtual ICollection<Car> Cars { get; set; }
        public virtual ICollection<Rent> Rents { get; set; }
        public Employee()
        {
            Cars = new List<Car>();
            Rents = new List<Rent>();
        }
    }
}
