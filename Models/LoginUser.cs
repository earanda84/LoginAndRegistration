#pragma warning disable CS8618

using System.ComponentModel.DataAnnotations;

namespace LoginAndRegistration.Models
{
    public class LoginUser
    {
        [Required(ErrorMessage = "El Email es requerido")]
        [EmailAddress]
        [Display(Name = "Email")]
        public string LEmail { get; set; }

        [Required(ErrorMessage = "El Password es requerido")]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string LPassword { get; set; }
    }
}