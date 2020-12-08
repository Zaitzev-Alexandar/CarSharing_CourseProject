using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CarSharing.ViewModels.Filters
{
    public class AdditionalServicesFilterViewModel
    {
        public string AdditionalServiceServiceName { get; set; } = null!;
        public int AdditionalServiceRentId { get; set; } = default!;
    }
}
