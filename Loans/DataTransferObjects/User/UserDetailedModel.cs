using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using Loans.DataTransferObjects.Requisite;
using Loans.Extensions;
using Loans.Models;

namespace Loans.DataTransferObjects.User
{
    public class UserDetailedModel
    {
        public UserModel Info { get; set; }

        public IEnumerable<RequisiteModel> Requisites { get; set; }
    }
}