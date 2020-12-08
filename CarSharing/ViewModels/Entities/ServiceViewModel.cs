using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using CarSharing.Models;
using CarSharing.ViewModels.Filters;

namespace CarSharing.ViewModels.Entities
{
    public class ServiceViewModel : IEntitiesViewModel<Service>
    {
        [Display(Name = "Services")]
        public IEnumerable<Service> Entities { get; set; }
        [Display(Name = "Service")]
        public Service Entity { get; set; }
        public PageViewModel PageViewModel { get; set; }
        public DeleteViewModel DeleteViewModel { get; set; }
        public SortViewModel SortViewModel { get; set; }
        public ServicesFilterViewModel ServicesFilterViewModel { get; set; }
    }
}
