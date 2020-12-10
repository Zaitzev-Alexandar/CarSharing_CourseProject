using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace CarSharing.ViewModels.Filters
{
    public class CarsFilterViewModel
    {
        public int CarRegNum { get; set; } = default!;
        public string CarVINcode { get; set; } = null!;
        public int CarEngineNum { get; set; } = default!;
        public decimal CarPrice { get; set; } = default!;
        public decimal CarRentalPrice { get; set; } = default!;
        public DateTime CarIssueDate { get; set; } = default!;
        public string CarSpecs { get; set; } = null!;
        public DateTime CarTechnicalMaintenanceDate { get; set; } = default!;
        public bool CarSpecMark { get; set; } = default!;
        public bool CarReturnMark { get; set; } = default!;

        public DateTime RentDeliveryDate { get; set; } = default!;
        public DateTime RentReturnDate { get; set; } = default!;
        public bool AdditionalRentCount { get; set; } = default!;
    }
}
