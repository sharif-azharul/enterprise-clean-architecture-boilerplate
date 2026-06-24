using Enterprise.Domain.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace Enterprise.Domain.Entities
{
    public class Role : BaseEntity
    {
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;

        public ICollection<UserRole> UserRoles { get; set; }
        = new List<UserRole>();
    }
}
