using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace CarSharing.Models
{
    public class CarModel
    {
        public int CarModelId { get; set; }
        [Display(Name = "CarModelName")]
        public string Name { get; set; }
        public string Description { get; set; }
        public int CarMarkId { get; set; }
        public CarMark CarMark { get; set; }
        public virtual ICollection<Car> Cars { get; set; }
        public CarModel()
        {
            Cars = new List<Car>();
        }
    }
}
