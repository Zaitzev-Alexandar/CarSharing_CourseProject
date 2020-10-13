﻿using System;
using System.Collections.Generic;

namespace CarSharing.Models
{
    public partial class Car
    {
        public Car()
        {
            Rents = new HashSet<Rent>();
        }

        public int CarId { get; set; }
        public int? CarModelId { get; set; }
        public int? RegNum { get; set; }
        public string Vincode { get; set; }
        public int? EngineNum { get; set; }
        public decimal? Price { get; set; }
        public decimal? RentalPrice { get; set; }
        public DateTime? IssueDate { get; set; }
        public string Specs { get; set; }
        public DateTime? TechnicalMaintenanceDate { get; set; }
        public bool? SpecMark { get; set; }
        public bool? ReturnMark { get; set; }
        public int? EmployeeId { get; set; }

        public virtual CarModel CarModel { get; set; }
        public virtual Employee Employee { get; set; }
        public virtual ICollection<Rent> Rents { get; set; }
    }
}