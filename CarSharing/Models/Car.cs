using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations.Schema;

namespace CarSharing.Models
{
    public class Car
    {
        public int CarId { get; set; }
        public int RegNum { get; set; }
        public string VINcode { get; set; }
        public int EngineNum { get; set; }
        [Column(TypeName = "money")]
        public decimal Price { get; set; }
        [Column(TypeName = "money")]
        public decimal RentalPrice { get; set; }
        public DateTime IssueDate { get; set; }
        public string Specs { get; set; }
        public DateTime TechnicalMaintenanceDate { get; set; }
        public bool SpecMark { get; set; }
        public bool ReturnMark { get; set; }
        public int EmployeeId { get; set; }
        public int CarModelId { get; set; }
        public virtual Employee Employee { get; set; }
        public virtual CarModel CarModel { get; set; }

    }
}
