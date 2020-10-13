using System;
using System.Collections.Generic;

namespace CarSharing.Models
{
    public partial class Service
    {
        public Service()
        {
            AdditionalServices = new HashSet<AdditionalService>();
        }

        public int ServiceId { get; set; }
        public string Name { get; set; }
        public decimal? Price { get; set; }
        public string Description { get; set; }

        public virtual ICollection<AdditionalService> AdditionalServices { get; set; }
    }
}
