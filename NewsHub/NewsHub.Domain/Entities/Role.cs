using System;
using System.Collections.Generic;
using System.Text;

namespace NewsHub.Domain.Entities
{
    public class Role
    {
        public int Id { get; private set; }

        public string Name { get; private set; } = null!;

        public ICollection<UserRole> UserRoles { get; private set; } = new List<UserRole>();

        private Role() { }

        public Role(string name)
        {
            Name = name;
        }
    }
}
