using System.Collections.Generic;
using Loans.DataTransferObjects.User;

namespace Loans.DataTransferObjects.Community
{
    public class GetCommunityResponse
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public IEnumerable<UserModel> Members { get; set; }
    }
}
