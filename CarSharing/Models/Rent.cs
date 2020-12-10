using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace CarSharing.Models
{
    public partial class Rent
    {
        public Rent()
        {
            AdditionalServices = new List<AdditionalService>();
        }

        public int RentId { get; set; }
        [Display(Name = "Return date")]
        public DateTime ReturnDate { get; set; }
        [Display(Name = "Delivery date")]
        public DateTime DeliveryDate { get; set; }
        public int? CarId { get; set; }
        public int? CustomerId { get; set; }
        public int? EmployeeId { get; set; }
        public decimal Price { get; set; }
        [Display(Name = "Total price")]
        public decimal TotalPrice
        {
            get
            {
                decimal result = 0;
                result += Price;
                result += Car.RentalPrice;
                foreach(AdditionalService additionalService in AdditionalServices)
                {
                    result += additionalService.Service.Price;
                }
                return result;
            }
        }
        public virtual Car Car { get; set; }
        public virtual Customer Customer { get; set; }
        public virtual Employee Employee { get; set; }
        public virtual ICollection<AdditionalService> AdditionalServices { get; set; }
    }
}
