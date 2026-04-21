using System.ComponentModel;

namespace NewsHub.Domain.Enums
{
    public enum RolesEnums
    {
        [Description("Administrador")]
        Admin = 1,
        [Description("Editor")]
        Editor = 2,
        [Description("Usuario")]
        User = 3
    }
}
