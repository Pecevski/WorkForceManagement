using System;

namespace WorkforceManagement.Data.Entities.Common
{
    public interface IBaseDeletableModel
    {
        DateTime? DeletedOn { get; set; }
        bool IsDeleted { get; set; }
    }
}