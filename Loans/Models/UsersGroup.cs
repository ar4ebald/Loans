using System.Collections.Generic;

namespace Loans.Models
{
    class UsersGroup
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public ICollection<UsersGroupEnrollment> Members { get; set; }
    }
}