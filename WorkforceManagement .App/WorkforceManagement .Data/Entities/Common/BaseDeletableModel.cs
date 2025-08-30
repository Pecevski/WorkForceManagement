using System;

namespace WorkforceManagement.Data.Entities.Common
{
    public class BaseDeletableModel : IBaseDeletableModel
    {
        public bool IsDeleted { get; set; }

        public DateTime? DeletedOn { get; set; }
    }
}
