using System.ComponentModel.DataAnnotations;

namespace NewsHub.Web.ViewModels.Authentication
{
    public class PasswordResetViewModel
    {
        public string Token { get; set; } = null!;

        [Required(ErrorMessage = "La contraseña es requerida.")]
        [RegularExpression(@"^(?=\S+$)(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[!@#$%^&*()_+\-=\[\]{};':""\\|,.<>\/?]).{6,64}$",
            ErrorMessage = "La contraseña debe estar entre 6 y 64 caracteres, incluir mayúscula, minúscula, número, carácter especial y no contener espacios.")]
        public string Password { get; set; } = null!;

        [Required]
        [Compare("Password", ErrorMessage = "Las contraseñas no coinciden.")]
        public string ConfirmPassword { get; set; } = null!;

        public DateTime ExpiresAt { get; set; }
    }
}
