namespace SalaEventos;

public class Program
{
    public static void Main(string[] args)
    {
        // Forzar formato regional de Costa Rica globalmente para que los decimales usen coma (,)
        System.Globalization.CultureInfo.DefaultThreadCurrentCulture = new System.Globalization.CultureInfo("es-CR");
        System.Globalization.CultureInfo.DefaultThreadCurrentUICulture = new System.Globalization.CultureInfo("es-CR");

        GestorReservas gestor = new GestorReservas(new IFormatoComprobante[]
        {
            new FormatoTexto(),
            new FormatoHtml(),
            new FormatoJson(),
            new FormatoXml()
        });
        gestor.CargarDatosIniciales();

        DateTime martes = ProximoDiaDeSemana(DayOfWeek.Tuesday);
        DateTime sabado = ProximoDiaDeSemana(DayOfWeek.Saturday);

        Console.WriteLine("=== Sistema de reservas de salas de eventos ===");
        Console.WriteLine();

        Console.WriteLine(gestor.CrearReserva(1, 1, martes, 9, 3, false, true, "TEXTO"));
        Console.WriteLine();

        Console.WriteLine(gestor.CrearReserva(2, 3, martes, 13, 8, true, true, "TEXTO"));
        Console.WriteLine();

        Console.WriteLine(gestor.CrearReserva(3, 1, martes.AddDays(1), 14, 4, false, false, "JSON"));
        Console.WriteLine();

        Console.WriteLine(gestor.CrearReserva(4, 2, sabado, 8, 6, true, false, "HTML"));
        Console.WriteLine();

        Console.WriteLine(gestor.CrearReserva(1, 1, martes, 10, 2, false, false, "TEXTO"));
        Console.WriteLine();

        Console.WriteLine(gestor.ReporteDelDia(martes, "TEXTO"));
        Console.WriteLine();
        Console.WriteLine(gestor.EnviarComprobante(1, "TEXTO"));
        Console.WriteLine();
        Console.WriteLine(gestor.CrearReserva(1, 1, martes.AddDays(7), 15, 2, false, false, "XML"));
    }

    private static DateTime ProximoDiaDeSemana(DayOfWeek dia)
    {
        int dias = 1 + ((int)dia - (int)DateTime.Today.DayOfWeek - 1 + 7) % 7;
        return DateTime.Today.AddDays(dias);
    }
}