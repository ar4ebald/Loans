using System.Collections.Generic;
using Loans.DataTransferObjects.Requisite;

namespace Loans.DataTransferObjects.User
{
    public class UserDetailedModel
    {
        public int Id { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public IEnumerable<RequisiteModel> Requisites { get; set; }
    }
}