using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CarSharing.Models
{
    public class CarMark
    {
        public int CarMarkId { get; set; }
        /// <summary>
        /// Марка авто-производителя
        /// </summary>
        public string Name { get; set; }
        public virtual ICollection<CarModel> CarModels { get; set; }
        public CarMark()
        {
            CarModels = new List<CarModel>();
        }
    }
}
