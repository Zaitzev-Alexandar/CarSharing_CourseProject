using System;
using System.Collections.Generic;

namespace CarSharing.Models
{
    public partial class Employee
    {
        public Employee()
        {
            Cars = new HashSet<Car>();
            Rents = new HashSet<Rent>();
        }

        public int EmployeeId { get; set; }
        public string Post { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }
        public string Patronymic { get; set; }
        public DateTime? EmploymentDate { get; set; }

        public virtual ICollection<Car> Cars { get; set; }
        public virtual ICollection<Rent> Rents { get; set; }
    }
}
