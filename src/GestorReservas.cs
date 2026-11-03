namespace SalaEventos;

// Clase central del sistema. Hace validación, cálculo de precios, persistencia,
// formato de comprobantes y reportes.
public class GestorReservas
{
    public static List<Cliente> Clientes = new List<Cliente>();
    public static List<Sala> Salas = new List<Sala>();
    public static List<Reserva> Reservas = new List<Reserva>();
    public static int SiguienteId = 1;

    private string archivoReservas = "reservas.csv";
    private string archivoComprobantes = "comprobantes";
    private readonly IReadOnlyDictionary<string, IFormatoComprobante> formatosComprobante;
    private readonly IFormatoComprobante formatoDesconocido = new FormatoDesconocido();

    public GestorReservas(IEnumerable<IFormatoComprobante> formatos = null)
    {
        formatos ??= new IFormatoComprobante[]
        {
            new FormatoTexto(),
            new FormatoHtml(),
            new FormatoJson(),
            new FormatoXml()
        };
        formatosComprobante = formatos.ToDictionary(formato => formato.Clave, StringComparer.Ordinal);
    }

    private IFormatoComprobante ObtenerFormato(string formatoComprobante) =>
        formatosComprobante.GetValueOrDefault(formatoComprobante ?? string.Empty, formatoDesconocido);

    public void CargarDatosIniciales()
    {
        Clientes.Add(new Cliente { Id = 1, Nombre = "Ana Vargas", Tipo = "REGULAR", Correo = "ana@correo.cr" });
        Clientes.Add(new Cliente { Id = 2, Nombre = "Constructora del Este", Tipo = "CORPORATIVO", Correo = "info@cde.cr" });
        Clientes.Add(new Cliente { Id = 3, Nombre = "Diego Mora", Tipo = "ESTUDIANTIL", Correo = "diego@correo.cr" });
        Clientes.Add(new Cliente { Id = 4, Nombre = "Hotel La Colina", Tipo = "CORPORATIVO", Correo = "eventos@lacolina.cr" });

        Salas.Add(new Sala { Id = 1, Nombre = "Sala Pequena", Capacidad = 20, TarifaHora = 12000m });
        Salas.Add(new Sala { Id = 2, Nombre = "Sala Mediana", Capacidad = 60, TarifaHora = 28000m });
        Salas.Add(new Sala { Id = 3, Nombre = "Auditorio", Capacidad = 150, TarifaHora = 65000m });
    }

    // Crea la reserva: valida, calcula, guarda en archivo y devuelve el comprobante.
    public string CrearReserva(int clienteId, int salaId, DateTime fecha, int horaInicio, int horas,
                               bool conCatering, bool conProyector, string formatoComprobante)
    {
        // ---- validaciones ----
        Cliente cliente = null;
        for (int i = 0; i < Clientes.Count; i++)
        {
            if (Clientes[i].Id == clienteId) { cliente = Clientes[i]; }
        }
        if (cliente == null) { return "ERROR: el cliente no existe"; }

        Sala sala = null;
        for (int i = 0; i < Salas.Count; i++)
        {
            if (Salas[i].Id == salaId) { sala = Salas[i]; }
        }
        if (sala == null) { return "ERROR: la sala no existe"; }

        if (horas < 1 || horas > 12) { return "ERROR: las horas deben estar entre 1 y 12"; }
        if (horaInicio < 6 || horaInicio > 20) { return "ERROR: hora de inicio fuera del horario"; }
        if (horaInicio + horas > 22) { return "ERROR: la reserva se pasa del cierre"; }
        if (fecha < DateTime.Today) { return "ERROR: no se reserva en el pasado"; }

        for (int i = 0; i < Reservas.Count; i++)
        {
            if (Reservas[i].SalaId == salaId && Reservas[i].Fecha.Date == fecha.Date && Reservas[i].Estado == "CONFIRMADA")
            {
                int finExistente = Reservas[i].HoraInicio + Reservas[i].Horas;
                int finNueva = horaInicio + horas;
                if (horaInicio < finExistente && Reservas[i].HoraInicio < finNueva)
                {
                    return "ERROR: la sala ya esta reservada en ese horario";
                }
            }
        }

        // ---- calculo del precio ----
        decimal subtotal = sala.TarifaHora * horas;

        if (horas >= 8)
        {
            subtotal = subtotal - (subtotal * 0.15m);
            if (cliente.Tipo == "CORPORATIVO")
            {
                subtotal = subtotal - (subtotal * 0.12m);
                if (sala.Capacidad >= 100)
                {
                    subtotal = subtotal - (subtotal * 0.03m);
                }
            }
            else
            {
                if (cliente.Tipo == "ESTUDIANTIL")
                {
                    if (fecha.DayOfWeek != DayOfWeek.Saturday && fecha.DayOfWeek != DayOfWeek.Sunday)
                    {
                        if (sala.Capacidad <= 30)
                        {
                            subtotal = subtotal - (subtotal * 0.20m);
                        }
                    }
                }
            }
        }
        else
        {
            if (horas >= 4)
            {
                subtotal = subtotal - (subtotal * 0.10m);
                if (cliente.Tipo == "CORPORATIVO")
                {
                    subtotal = subtotal - (subtotal * 0.12m);
                }
                else
                {
                    if (cliente.Tipo == "ESTUDIANTIL")
                    {
                        if (fecha.DayOfWeek != DayOfWeek.Saturday && fecha.DayOfWeek != DayOfWeek.Sunday)
                        {
                            if (sala.Capacidad <= 30)
                            {
                                subtotal = subtotal - (subtotal * 0.20m);
                            }
                        }
                    }
                }
            }
            else
            {
                if (cliente.Tipo == "CORPORATIVO")
                {
                    subtotal = subtotal - (subtotal * 0.12m);
                }
            }
        }

        if (conCatering)
        {
            subtotal = subtotal + (8500m * sala.Capacidad);
        }

        if (conProyector)
        {
            if (cliente.Tipo == "CORPORATIVO" && horas >= 4)
            {
                subtotal = subtotal + 0m;
            }
            else
            {
                subtotal = subtotal + 15000m;
            }
        }

        if (fecha.DayOfWeek == DayOfWeek.Saturday || fecha.DayOfWeek == DayOfWeek.Sunday)
        {
            subtotal = subtotal + (subtotal * 0.25m);
        }

        if ((fecha.Month == 9 && fecha.Day == 15) || (fecha.Month == 12 && fecha.Day == 25) ||
            (fecha.Month == 12 && fecha.Day == 1) || (fecha.Month == 1 && fecha.Day == 1))
        {
            subtotal = subtotal + (subtotal * 0.40m);
        }

        decimal impuesto = subtotal * 0.13m;
        decimal total = subtotal + impuesto;

        // ---- persistencia ----
        Reserva reserva = new Reserva
        {
            Id = SiguienteId,
            ClienteId = clienteId,
            SalaId = salaId,
            Fecha = fecha,
            HoraInicio = horaInicio,
            Horas = horas,
            ConCatering = conCatering,
            ConProyector = conProyector,
            Subtotal = Math.Round(subtotal, 2),
            Impuesto = Math.Round(impuesto, 2),
            Total = Math.Round(total, 2),
            Estado = "CONFIRMADA"
        };
        SiguienteId = SiguienteId + 1;
        Reservas.Add(reserva);

        string linea = reserva.Id + "," + reserva.ClienteId + "," + reserva.SalaId + "," +
                       reserva.Fecha.ToString("yyyy-MM-dd") + "," + reserva.HoraInicio + "," +
                       reserva.Horas + "," + reserva.Total + "," + reserva.Estado;
        File.AppendAllText(archivoReservas, linea + Environment.NewLine);

        // ---- comprobante ----
        IFormatoComprobante estrategiaFormato = ObtenerFormato(formatoComprobante);
        string comprobante = estrategiaFormato.GenerarComprobante(reserva, cliente, sala);

        if (!Directory.Exists(archivoComprobantes)) { Directory.CreateDirectory(archivoComprobantes); }
        File.WriteAllText(Path.Combine(archivoComprobantes, "reserva-" + reserva.Id + estrategiaFormato.ObtenerExtension()), comprobante);

        return comprobante;
    }

    public string VistaPrevia(int clienteId, int salaId, string formatoComprobante)
    {
        Cliente cliente = null;
        for (int i = 0; i < Clientes.Count; i++)
        {
            if (Clientes[i].Id == clienteId) { cliente = Clientes[i]; }
        }
        Sala sala = null;
        for (int i = 0; i < Salas.Count; i++)
        {
            if (Salas[i].Id == salaId) { sala = Salas[i]; }
        }
        if (cliente == null || sala == null) { return "ERROR: datos incompletos"; }

        return ObtenerFormato(formatoComprobante).GenerarVistaPrevia(cliente, sala);
    }

    public string EnviarComprobante(int reservaId, string formatoComprobante)
    {
        Reserva reserva = null;
        for (int i = 0; i < Reservas.Count; i++)
        {
            if (Reservas[i].Id == reservaId) { reserva = Reservas[i]; }
        }
        if (reserva == null) { return "ERROR: la reserva no existe"; }

        Cliente cliente = null;
        for (int i = 0; i < Clientes.Count; i++)
        {
            if (Clientes[i].Id == reserva.ClienteId) { cliente = Clientes[i]; }
        }

        string cuerpo = ObtenerFormato(formatoComprobante).GenerarCuerpoCorreo();
        // Aqui iria el envio real de correo.
        return "Enviado a " + cliente.Correo + ": " + cuerpo;
    }

    public string ReporteDelDia(DateTime fecha, string formatoComprobante)
    {
        decimal totalDia = 0m;
        int cantidad = 0;
        for (int i = 0; i < Reservas.Count; i++)
        {
            if (Reservas[i].Fecha.Date == fecha.Date && Reservas[i].Estado == "CONFIRMADA")
            {
                totalDia = totalDia + Reservas[i].Total;
                cantidad = cantidad + 1;
            }
        }

        return ObtenerFormato(formatoComprobante).GenerarReporte(fecha, cantidad, totalDia);
    }

    public string AnularReserva(int reservaId)
    {
        for (int i = 0; i < Reservas.Count; i++)
        {
            if (Reservas[i].Id == reservaId)
            {
                Reservas[i].Estado = "ANULADA";
                File.AppendAllText(archivoReservas, "ANULACION," + reservaId + Environment.NewLine);
                return "Reserva " + reservaId + " anulada";
            }
        }
        return "ERROR: la reserva no existe";
    }
}
