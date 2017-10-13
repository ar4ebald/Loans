using System.Collections.Generic;

namespace Loans.DataTransferObjects.User
{
    public class UserSearchResponse
    {
        public int Count { get; set; }

        public IEnumerable<UserModel> Users { get; set; }
    }
}
