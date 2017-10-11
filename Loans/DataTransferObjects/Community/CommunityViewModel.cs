using System;
using System.Linq.Expressions;

namespace Loans.DataTransferObjects.UsersGroup
{
    public class CommunityViewModel
    {
        public static readonly Expression<Func<Models.Community, CommunityViewModel>> FromQuery = community =>
            new CommunityViewModel
            {
                Id = community.Id,
                Name = community.Name
            };

        public int Id { get; set; }

        public string Name { get; set; }
    }
}
