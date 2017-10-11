using System.Collections.Generic;

namespace Loans.Models
{
    public class Community
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public ICollection<CommunityEnrollment> Members { get; set; }
    }
}