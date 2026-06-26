using Enterprise.Domain.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace Enterprise.Domain.Entities
{
    public class User : AuditableEntity
    {
        public string FirstName { get; set; } = string.Empty;

        public string LastName { get; set; } = string.Empty;

        public string Email { get; set; } = string.Empty;
        public string PasswordHash { get; set; } = string.Empty;

        public ICollection<UserRole> UserRoles { get; set; }
            = new List<UserRole>();
    }
}
