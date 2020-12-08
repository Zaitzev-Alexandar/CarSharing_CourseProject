using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CarSharing.ViewModels.Filters
{
    public class RentFilterViewModel
    {
        public DateTime RentDeliveryDate { get; set; } = default!;
        public DateTime RentReturnDate { get; set; } = default!;
        public decimal RentPrice { get; set; } = default!;

    }
}
