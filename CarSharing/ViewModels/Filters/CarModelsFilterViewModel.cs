using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace CarSharing.ViewModels.Filters
{
    public class CarModelsFilterViewModel
    {
        [Display(Name = "Name")]
        public string CarModelName { get; set; } = null!;
        [Display(Name = "Description")]
        public string CarModelDescription { get; set; } = null!;
        [Display(Name = "MarkName")]
        public string CarMarkName { get; set; } = null!;

    }
}
