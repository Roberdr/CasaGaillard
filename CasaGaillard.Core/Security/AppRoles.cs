namespace CasaGaillard.Core.Security;

public static class AppRoles
{
    public const string Administrador = "Administrador";
    public const string Mantenimiento = "Mantenimiento";
    public const string Consulta = "Consulta";
    public const string LecturaMantenimiento = Administrador + "," + Mantenimiento + "," + Consulta;
    public const string GestionMantenimiento = Administrador + "," + Mantenimiento;
}
