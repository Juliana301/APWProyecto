using System.ComponentModel.DataAnnotations;

namespace NewsHub.Web.ViewModels.Authentication
{
    public class ForgotPasswordViewModel
    {
        [Required(ErrorMessage = "El correo es obligatorio.")]
        public string Email { get; set; } = null!;
    }
}
