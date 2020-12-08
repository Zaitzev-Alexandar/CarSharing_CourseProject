using System;
using System.Collections.Generic;

namespace CarSharing.Models
{
    public partial class Rent
    {
        public Rent()
        {
            AdditionalServices = new HashSet<AdditionalService>();
        }

        public int RentId { get; set; }
        public DateTime ReturnDate { get; set; }
        public DateTime DeliveryDate { get; set; }
        public int? CarId { get; set; }
        public int? CustomerId { get; set; }
        public int? EmployeeId { get; set; }
        public decimal Price { get; set; }

        public virtual Car Car { get; set; }
        public virtual Customer Customer { get; set; }
        public virtual Employee Employee { get; set; }
        public virtual ICollection<AdditionalService> AdditionalServices { get; set; }
    }
}
