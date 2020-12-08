using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using CarSharing.Models;
using CarSharing.ViewModels.Filters;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace CarSharing.ViewModels.Entities
{
    public class CustomerViewModel : IEntitiesViewModel<Customer>
    {
        [Display(Name = "Customers")]
        public IEnumerable<Customer> Entities { get; set; }
        [Display(Name = "Customer")]
        public Customer Entity { get; set; }
        public PageViewModel PageViewModel { get; set; }
        public DeleteViewModel DeleteViewModel { get; set; }
        public SortViewModel SortViewModel { get; set; }
        public CustomersFilterViewModel CustomersFilterViewModel { get; set; }
    }
}
