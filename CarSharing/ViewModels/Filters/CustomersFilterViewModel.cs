using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace CarSharing.ViewModels.Filters
{
    public class CustomersFilterViewModel
    {
        public string CustomerName { get; set; } = null!;
        public string CustomerSurname { get; set; } = null!;
        public string CustomerPatronymic { get; set; } = null!;
        public string CustomerPhoneNum { get; set; } = null!;
        public string CustomerAddress { get; set; } = null!;
        public string CustomerPassportInfo { get; set; } = null!;
        public DateTime CustomerBirthDate { get; set; } = default!;
        public bool CustomerGender { get; set; } = default!;

    }
}
