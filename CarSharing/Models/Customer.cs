using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;


namespace CarSharing.Models
{
    public partial class Customer
    {
        public Customer()
        {
            Rents = new List<Rent>();
        }

        public int CustomerId { get; set; }
        [Display(Name = "Customer name")]
        public string Name { get; set; }
        public string Surname { get; set; }
        public string Patronymic { get; set; }
        [Display(Name = "Phone number")]
        public string PhoneNum { get; set; }
        public string Address { get; set; }
        [Display(Name = "Birth date")]
        public DateTime? BirthDate { get; set; }
        [Display(Name = "Passport info")]
        public string PassportInfo { get; set; }
        public bool Gender { get; set; }

        public virtual ICollection<Rent> Rents { get; set; }
    }
}
