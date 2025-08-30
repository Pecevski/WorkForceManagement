using System;
using System.ComponentModel.DataAnnotations;
using WorkforceManagement.Data.Entities.Enums;

namespace WorkforceManagement.Models.DTO.Request
{
    public class TimeOffRequestRequestDTO
    {
        [Required]
        public string RequestType { get; set; }

        [Required]
        [DataType(DataType.Date)]
        public DateTime StartDate { get; set; }

        [Required]
        [DataType(DataType.Date)]
        public DateTime EndDate { get; set; }

        [Required]
        public string Reason { get; set; }

        [Required]
        public string RequesterUserId { get; set; }
    }
}
