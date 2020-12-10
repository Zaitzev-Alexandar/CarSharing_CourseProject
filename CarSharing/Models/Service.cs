using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;


namespace CarSharing.Models
{
    public partial class Service
    {
        public Service()
        {
            AdditionalServices = new List<AdditionalService>();
        }

        public int ServiceId { get; set; }
        [Display(Name = "Service name")]
        public string Name { get; set; }
        public decimal Price { get; set; }
        public string Description { get; set; }

        public virtual ICollection<AdditionalService> AdditionalServices { get; set; }
    }
}
