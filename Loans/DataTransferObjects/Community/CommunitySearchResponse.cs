using System.Collections.Generic;
using Loans.DataTransferObjects.User;

namespace Loans.DataTransferObjects.Community
{
    public class CommunitySearchResponse
    {
        public int Count { get; set; }

        public IEnumerable<CommunityViewModel> Communities { get; set; }
    }
}
