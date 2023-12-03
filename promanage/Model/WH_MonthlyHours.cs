﻿using System;
using System.ComponentModel.DataAnnotations;

namespace ActionTrakingSystem.Model
{
    public class WH_MonthlyHours
    {
        [Key]
        public int monthlyHourId { get; set; }
        public int startingHourId { get; set; }
        public int monthId { get; set; }
        public int yearId { get; set; }
        public decimal runningHour { get; set; }
        public DateTime createdOn { get; set; }
        public int createdBy { get; set; }
        public DateTime? modifiedOn { get; set; }
        public int? modifiedBy { get; set; }
    }
}
