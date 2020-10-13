using System;
using System.Collections.Generic;

namespace CarSharing.Models
{
    public partial class Customer
    {
        public Customer()
        {
            Rents = new HashSet<Rent>();
        }

        public int CustomerId { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }
        public string Patronymic { get; set; }
        public string PhoneNum { get; set; }
        public string Address { get; set; }
        public DateTime? BirthDate { get; set; }
        public string PassportInfo { get; set; }
        public bool? Gender { get; set; }

        public virtual ICollection<Rent> Rents { get; set; }
    }
}
