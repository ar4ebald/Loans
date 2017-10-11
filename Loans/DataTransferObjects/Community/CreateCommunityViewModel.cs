using System.ComponentModel.DataAnnotations;

namespace Loans.DataTransferObjects.Community
{
    public class CreateCommunityViewModel
    {
        [Required, StringLength(maximumLength: 32, MinimumLength = 3)]
        public string Name { get; set; }
    }
}
