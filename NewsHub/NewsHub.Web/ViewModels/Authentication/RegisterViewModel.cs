using System.ComponentModel.DataAnnotations;

namespace NewsHub.Web.ViewModels.Authentication
{
    public class RegisterViewModel
    {
        [Required(ErrorMessage = "El correo es obligatorio.")]
        [RegularExpression(@"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$",
            ErrorMessage = "Correo no válido. Debe tener el formato user@example.com")]
        public string Email { get; set; } = null!;

        [Required(ErrorMessage = "La contraseña es requerida.")]
        [RegularExpression(@"^(?=\S+$)(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[!@#$%^&*()_+\-=\[\]{};':""\\|,.<>\/?]).{6,64}$",
            ErrorMessage = "La contraseña debe estar entre 6 y 64 caracteres, incluir mayúscula, minúscula, número, carácter especial y no contener espacios.")]
        public string Password { get; set; } = null!;

        [Required]
        [Compare("Password", ErrorMessage = "Las contraseñas no coinciden.")]
        public string ConfirmPassword { get; set; } = null!;

        [Required(ErrorMessage = "El nombre es requerido.")]
        [RegularExpression(@"^(?=.{2,50}$)(?!.*\s{2,})[A-Za-zÁÉÍÓÚáéíóúÑñ]+(?:\s[A-Za-zÁÉÍÓÚáéíóúÑñ]+)*$",
            ErrorMessage = "El nombre solo puede contener letras y espacios, sin espacios dobles y mínimo 2 caracteres.")]
        public string FirstName { get; set; } = null!;

        [Required(ErrorMessage = "El apellido es requerido.")]
        [RegularExpression(@"^(?=.{2,50}$)(?!.*\s{2,})[A-Za-zÁÉÍÓÚáéíóúÑñ]+(?:\s[A-Za-zÁÉÍÓÚáéíóúÑñ]+)*$",
            ErrorMessage = "El apellido solo puede contener letras y espacios, sin espacios dobles y mínimo 2 caracteres.")]
        public string LastName { get; set; } = null!;

        [Required(ErrorMessage = "El nombre de usuario es requerido.")]
        [RegularExpression(@"^(?=.{3,30}$)[a-zA-Z0-9_]+$",
            ErrorMessage = "El nombre de usuario debe tener entre 3 y 30 caracteres y solo puede contener letras, números y guión bajo.")]
        public string UserName { get; set; } = null!;
    }
}