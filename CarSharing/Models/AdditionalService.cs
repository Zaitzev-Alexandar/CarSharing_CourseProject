using System;
using System.Collections.Generic;

namespace CarSharing.Models
{
    public partial class AdditionalService
    {
        public int Id { get; set; }
        public int? RentId { get; set; }
        public int? ServiceId { get; set; }

        public virtual Rent Rent { get; set; }
        public virtual Service Service { get; set; }
    }
}
