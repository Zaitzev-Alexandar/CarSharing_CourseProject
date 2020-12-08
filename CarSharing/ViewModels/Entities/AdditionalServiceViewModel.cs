using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using CarSharing.Models;
using CarSharing.ViewModels.Filters;

namespace CarSharing.ViewModels.Entities
{
    public class AdditionalServiceViewModel : IEntitiesViewModel<AdditionalService>
    {
        [Display(Name = "AdditionalServices")]
        public IEnumerable<AdditionalService> Entities { get; set; }
        [Display(Name = "AdditionalService")]
        public AdditionalService Entity { get; set; }

        [Display(Name = "Rents")]
        public IEnumerable<Rent> RentSelectList { get; set; }
        [Display(Name = "Services")]
        public IEnumerable<Service> ServiceSelectList { get; set; }

        public string ServiceName { get; set; }
        public int RentId { get; set; }
        public PageViewModel PageViewModel { get; set; }
        public DeleteViewModel DeleteViewModel { get; set; }
        public SortViewModel SortViewModel { get; set; }
        public AdditionalServicesFilterViewModel AdditionalServicesFilterViewModel { get; set; }
    }
}
