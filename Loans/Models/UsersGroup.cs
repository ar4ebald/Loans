using System.Collections.Generic;

namespace Loans.Models
{
    public class UsersGroup
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public ICollection<UsersGroupEnrollment> Members { get; set; }
    }
}