using System.ComponentModel.DataAnnotations;

namespace Quiz_Maker.ViewModel
{
    public class RegisterViewModel
    {
        [Required]
        [StringLength(100)]
        public string? DisplayName { get; set; }
        [Required]
        [EmailAddress]
        public string? Email { get; set; }
        [Required]
        [StringLength(100, MinimumLength = 6)]
        public string? Password { get; set; }
        [Required]
        [DataType(DataType.Password)]
        [Compare("Password")]
        public string? ConfirmPassword { get; set; }

    }
}
