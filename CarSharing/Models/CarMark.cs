﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace CarSharing.Models
{
    public class CarMark
    {
        public int CarMarkId { get; set; }
        [Display(Name = "Car mark name")]
        public string Name { get; set; }
        public virtual ICollection<CarModel> CarModels { get; set; }
        public CarMark()
        {
            CarModels = new List<CarModel>();
        }
    }
}
