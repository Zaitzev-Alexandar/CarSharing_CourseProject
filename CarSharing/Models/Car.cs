using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace CarSharing.Models
{
    public class Car
    {
        public int CarId { get; set; }
        [Display(Name = "Registration number")]
        public int RegNum { get; set; }
        public string VINcode { get; set; }
        [Display(Name = "Engine number")]
        public int EngineNum { get; set; }
        public decimal Price { get; set; }
        [Display(Name = "Rental price")]
        public decimal RentalPrice { get; set; }
        [Display(Name = "Issue date")]
        public DateTime IssueDate { get; set; }
        public string Specs { get; set; }
        [Display(Name = "Technical maintenance date")]

        public DateTime TechnicalMaintenanceDate { get; set; }
        [Display(Name = "Special mark")]
        public bool SpecMark { get; set; }
        [Display(Name = "Return mark")]
        public bool ReturnMark { get; set; }
        public int EmployeeId { get; set; }
        public int CarModelId { get; set; }
        public virtual Employee Employee { get; set; }
        public virtual CarModel CarModel { get; set; }

        public virtual ICollection<Rent> Rents { get; set; }
        public Car()
        {
            Rents = new List<Rent>();
        }

    }
}
