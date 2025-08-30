using System.ComponentModel.DataAnnotations;
using WorkforceManagement.Data.Entities.Common;

namespace WorkforceManagement.Data.Entities
{
    public class Approval : BaseDeletableModel
    {
        public int Id { get; set; }

        public bool IsApproved { get; set; }

        [Required]
        public virtual TimeOffRequest TimeOffRequest { get; set; }

        [Required]
        public virtual User Approver { get; set; }
    }
}
