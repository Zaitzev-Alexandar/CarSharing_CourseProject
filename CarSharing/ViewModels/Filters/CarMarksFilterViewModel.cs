using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace CarSharing.ViewModels.Filters
{
    public class CarMarksFilterViewModel
    {
        [Display(Name = "CarMark")]
        public string CarMarkName { get; set; } = null!;

    }
}
