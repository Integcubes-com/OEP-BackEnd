using System;
using System.ComponentModel.DataAnnotations;

namespace ActionTrakingSystem.Model
{
    public class TechnicalEvaluation
    {
        [Key]
        public int teId { get; set; }
        public int userId { get; set; }
        public string technicalEvaluation { get; set; }
        public int createdBy { get; set; }
        public DateTime createdDate { get; set; }
        public int? modifiedBy { get; set; }
        public DateTime? modifiedDate { get; set; }
        public int isDeleted { get; set; }
        public int tilId { get; set; }
    }
}
