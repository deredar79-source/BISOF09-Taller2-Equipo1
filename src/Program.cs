namespace SalaEventos;

public class Program
{
    public static void Main(string[] args)
    {
        GestorReservas gestor = new GestorReservas();
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
    }

    private static DateTime ProximoDiaDeSemana(DayOfWeek dia)
    {
        DateTime f = DateTime.Today.AddDays(1);
        while (f.DayOfWeek != dia) { f = f.AddDays(1); }
        return f;
    }
}
