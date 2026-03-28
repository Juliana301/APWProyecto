using System.ComponentModel.DataAnnotations;
using NewsHub.Domain.Enums;

namespace NewsHub.Web.ViewModels.Source
{
    public class SourceViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "La URL es obligatoria.")]
        [Url(ErrorMessage = "Debe ingresar una URL válida.")]
        [StringLength(500, ErrorMessage = "La URL no puede superar los 500 caracteres.")]
        [Display(Name = "URL")]
        public string Url { get; set; } = null!;

        [Required(ErrorMessage = "El nombre es obligatorio.")]
        [StringLength(200, MinimumLength = 3, ErrorMessage = "El nombre debe tener entre 3 y 200 caracteres.")]
        [Display(Name = "Nombre")]
        public string Name { get; set; } = null!;

        [StringLength(500, ErrorMessage = "La descripción no puede superar los 500 caracteres.")]
        [Display(Name = "Descripción")]
        public string? Description { get; set; }

        [Display(Name = "Configuración API (JSON)")]
        public string? ApiConfigJson { get; set; } = string.Empty;

        [Required(ErrorMessage = "Debe seleccionar un tipo de componente.")]
        [Display(Name = "Tipo de componente")]
        public SourceType ComponentType { get; set; }

        [Display(Name = "Requiere secreto")]
        public bool RequiresSecret { get; set; }
    }
}