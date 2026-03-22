using System;
using System.Collections.Generic;
using System.Text;

namespace NewsHub.Domain.Entities
{
    public class UserRoleEnt
    {
        public int UserId { get; private set; }
        public int RoleId { get; private set; }

        public UserEnt User { get; private set; } = null!;
        public RoleEnt Role { get; private set; } = null!;

        private UserRoleEnt() { }

        public UserRoleEnt(int userId, int roleId)
        {
            UserId = userId;
            RoleId = roleId;
        }
    }
}
