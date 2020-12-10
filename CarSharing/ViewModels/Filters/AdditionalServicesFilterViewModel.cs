using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace CarSharing.ViewModels.Filters
{
    public class AdditionalServicesFilterViewModel
    {
        [Display(Name = "Service name")]
        public string AdditionalServiceServiceName { get; set; } = null!;
        [Display(Name = "Rent Id")]
        public int AdditionalServiceRentId { get; set; } = default!;
    }
}
