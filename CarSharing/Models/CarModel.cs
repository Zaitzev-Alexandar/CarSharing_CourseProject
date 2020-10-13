using System;
using System.Collections.Generic;

namespace CarSharing.Models
{
    public partial class CarModel
    {
        public CarModel()
        {
            Cars = new HashSet<Car>();
        }

        public int CarModelId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }

        public virtual ICollection<Car> Cars { get; set; }
    }
}
