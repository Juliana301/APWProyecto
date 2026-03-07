using System.ComponentModel.DataAnnotations;

namespace NewsHub.Web.ViewModels.Authentication
{
    public class LoginViewModel
    {
        [Required(ErrorMessage = "El nombre de usuario es requerido.")]
        public string UserName { get; set; } = null!;

        [Required(ErrorMessage = "La contraseña es requerida.")]
        public string Password { get; set; } = null!;

        public bool RememberMe { get; set; } = false;
    }
}
