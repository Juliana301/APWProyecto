using NewsHub.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace NewsHub.Application.DTOs.User
{
    public class UserDto
    {
        public int Id { get; init; }
        public string Email { get; init; } = null!;
        public string UserName { get; init; } = null!;
        public string FirstName { get; init; } = null!;
        public string LastName { get; init; } = null!;

        public List<string> Roles { get; init; } = new();

    }
}
