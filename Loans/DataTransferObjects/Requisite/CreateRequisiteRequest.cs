using System.ComponentModel.DataAnnotations;

namespace Loans.DataTransferObjects.Requisite
{
    public class CreateRequisiteRequest
    {
        [Required, StringLength(256, MinimumLength = 1)]
        public string Description { get; set; }
    }
}
