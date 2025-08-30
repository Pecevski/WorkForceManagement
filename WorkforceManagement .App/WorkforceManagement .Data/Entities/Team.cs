using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using WorkforceManagement.Data.Entities.Common;

namespace WorkforceManagement.Data.Entities
{
    public class Team : BaseDeletableModel
    {
        public Team()
        {
            Members = new HashSet<User>();
        }

        public int Id { get; set; }

        [Required]
        public string Title { get; set; }

        [Required]
        public string Description { get; set; }

        public DateTime CreatedOn { get; set; }

        public DateTime? LastModifiedOn { get; set; }

        [Required]
        public virtual User TeamLeader { get; set; }

        public virtual ICollection<User> Members { get; set; }
    }
}
