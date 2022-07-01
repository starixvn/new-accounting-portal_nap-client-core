using System.ComponentModel.DataAnnotations;

namespace Fanex.NAP.Signin.AspNetCore.Models
{
    public class UpdatePasswordModel
    {
        [Display(Name = "Old Password")]
        [Required]
        [StringLength(50, MinimumLength = 2, ErrorMessage = "The {0} field must be at least {2} and maximum {1} characters long.")]
        public string OldUserPwd { get; set; }

        [Compare("UserPwd", ErrorMessage = "'{0}' and '{1}' do not match")]
        [Display(Name = "Confirm Password")]
        [Required]
        [StringLength(50, MinimumLength = 2, ErrorMessage = "The {0} field must be at least {2} and maximum {1}")]
        public string ConfirmUserPwd { get; set; }

        [Display(Name = "New Password")]
        [Required]
        [StringLength(50, MinimumLength = 2, ErrorMessage = "The {0} field must be at least {2} and maximum {1} characters long.")]
        public string UserPwd { get; set; }
    }
}