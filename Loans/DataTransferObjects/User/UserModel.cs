using System;
using System.Linq.Expressions;
using Loans.Extensions;
using Loans.Models;

namespace Loans.DataTransferObjects.User
{
    public class UserModel
    {
        public static readonly Expression<Func<ApplicationUser, UserModel>> FromQuery = user => new UserModel
        {
            Id = user.Id,
            FirstName = user.FirstName,
            LastName = user.LastName,
            UserName = user.UserName,
            Email = user.Email
        };

        public int Id { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string UserName { get; set; }

        public string Email { get; set; }

        public string AvatarUri => $"{Startup.Domain}/avatars/{Id}.jpg";
    }
}