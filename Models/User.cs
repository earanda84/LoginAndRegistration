#pragma warning disable CS8618

using System.ComponentModel.DataAnnotations;

using System.ComponentModel.DataAnnotations.Schema;

namespace LoginAndRegistration.Models
{
    public class User
    {
        [Key]
        public int UserId { get; set; }

        // FirstName
        [Required]
        [MinLength(2, ErrorMessage = "El Firstname debe tener al menos 2 caracteres")]
        [Display(Name = "Firstname:")]
        public string FirstName { get; set; }

        // Lastname
        [Required]
        [MinLength(2, ErrorMessage = "El Lastname debe tener al menos 2 caracteres")]
        [Display(Name = "Lastname:")]
        public string LastName { get; set; }

        // Email address
        [Required]
        [EmailAddress]
        [UniqueEmail]
        [Display(Name = "Email:")]
        public string Email { get; set; }

        // Password
        [Required]
        [MinLength(8, ErrorMessage = "La Password debe ser de al menos 8 caracteres")]
        [DataType(DataType.Password)]
        [Display(Name ="Password:")]
        public string Password { get; set; }

        // CreatedAt
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        // UpdatedAt
        public DateTime UpdatedAt { get; set; } = DateTime.Now;

        // Confirm Password
        [NotMapped]
        [DataType(DataType.Password)]
        [Compare("Password")]
        [Display(Name = "PW Confirm:")]
        public string Confirm { get; set; }
    }

    // Validaciones
    public class UniqueEmailAttribute : ValidationAttribute
    {
        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            if (value == null)
            {
                return new ValidationResult("El Email es requerido");
            }
            else if (value is string email)
            {
                MyContext _context = (MyContext)validationContext.GetService(typeof(MyContext));


                bool emailDb = _context.Users.Any(e => e.Email == email.ToString());

                if (emailDb == true)
                {
                    return new ValidationResult("El Email debe ser único");
                }
            }

            return ValidationResult.Success;
        }
    }
}