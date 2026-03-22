using System;
using System.Collections.Generic;
using System.Text;

namespace NewsHub.Domain.Entities
{
    public class RoleEnt
    {
        public int Id { get; private set; }

        public string Name { get; private set; } = null!;

        public ICollection<UserRoleEnt> UserRoles { get; private set; } = new List<UserRoleEnt>();

        private RoleEnt() { }

        public RoleEnt(string name)
        {
            Name = name;
        }
    }
}
