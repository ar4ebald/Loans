using System;
using System.Linq.Expressions;

namespace Loans.DataTransferObjects.Requisite
{
    public class RequisiteModel
    {
        public static readonly Expression<Func<Models.Requisite, RequisiteModel>> FromQuery =
            requisite => new RequisiteModel
            {
                Id = requisite.Id,
                Description = requisite.Description
            };

        public int Id { get; set; }

        public string Description { get; set; }
    }
}
