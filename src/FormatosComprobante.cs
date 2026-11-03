using System.Xml.Linq;

namespace SalaEventos;

// ¿Qué recibe? Los datos de reserva, cliente y sala, o los datos del reporte.
// ¿Qué devuelve? El contenido del formato solicitado y la extension de archivo.
// ¿Qué garantiza? Representar correctamente los datos con el formato de esta estrategia.
// ¿Qué promete NO hacer? Modificar la reserva, guardar archivos o enviar correos.
public interface IFormatoComprobante
{
    string Clave { get; }
    string GenerarComprobante(Reserva reserva, Cliente cliente, Sala sala);
    string GenerarVistaPrevia(Cliente cliente, Sala sala);
    string GenerarCuerpoCorreo();
    string GenerarReporte(DateTime fecha, int cantidad, decimal totalDia);
    string ObtenerExtension();
}

public sealed class FormatoTexto : IFormatoComprobante
{
    public string Clave => "TEXTO";

    public string GenerarComprobante(Reserva reserva, Cliente cliente, Sala sala) =>
        "COMPROBANTE DE RESERVA #" + reserva.Id + "\n" +
        "Cliente: " + cliente.Nombre + "\n" +
        "Sala: " + sala.Nombre + "\n" +
        "Fecha: " + reserva.Fecha.ToString("dd/MM/yyyy") + " de " + reserva.HoraInicio + ":00 por " + reserva.Horas + " horas\n" +
        "Subtotal: " + reserva.Subtotal + "\n" +
        "Impuesto: " + reserva.Impuesto + "\n" +
        "TOTAL: " + reserva.Total;

    public string GenerarVistaPrevia(Cliente cliente, Sala sala) =>
        "VISTA PREVIA\nCliente: " + cliente.Nombre + "\nSala: " + sala.Nombre;

    public string GenerarCuerpoCorreo() => "Adjunto su comprobante en texto plano.";

    public string GenerarReporte(DateTime fecha, int cantidad, decimal totalDia) =>
        "REPORTE " + fecha.ToString("dd/MM/yyyy") + "\nReservas: " + cantidad + "\nTotal: " + totalDia;

    public string ObtenerExtension() => ".txt";
}

public sealed class FormatoHtml : IFormatoComprobante
{
    public string Clave => "HTML";

    public string GenerarComprobante(Reserva reserva, Cliente cliente, Sala sala) =>
        "<div class='comprobante'><h1>Reserva #" + reserva.Id + "</h1>" +
        "<p>Cliente: " + cliente.Nombre + "</p>" +
        "<p>Sala: " + sala.Nombre + "</p>" +
        "<p>Fecha: " + reserva.Fecha.ToString("dd/MM/yyyy") + "</p>" +
        "<p>Total: " + reserva.Total + "</p></div>";

    public string GenerarVistaPrevia(Cliente cliente, Sala sala) =>
        "<div class='previa'><p>" + cliente.Nombre + "</p><p>" + sala.Nombre + "</p></div>";

    public string GenerarCuerpoCorreo() => "<p>Adjunto su comprobante.</p>";

    public string GenerarReporte(DateTime fecha, int cantidad, decimal totalDia) =>
        "<h2>Reporte " + fecha.ToString("dd/MM/yyyy") + "</h2><p>Reservas: " + cantidad + "</p><p>Total: " + totalDia + "</p>";

    public string ObtenerExtension() => ".html";
}

public sealed class FormatoJson : IFormatoComprobante
{
    public string Clave => "JSON";

    public string GenerarComprobante(Reserva reserva, Cliente cliente, Sala sala) =>
        "{\"id\":" + reserva.Id + ",\"cliente\":\"" + cliente.Nombre + "\"," +
        "\"sala\":\"" + sala.Nombre + "\",\"total\":" + reserva.Total + "}";

    public string GenerarVistaPrevia(Cliente cliente, Sala sala) =>
        "{\"cliente\":\"" + cliente.Nombre + "\",\"sala\":\"" + sala.Nombre + "\"}";

    public string GenerarCuerpoCorreo() => "{\"mensaje\":\"Adjunto su comprobante\"}";

    public string GenerarReporte(DateTime fecha, int cantidad, decimal totalDia) =>
        "{\"fecha\":\"" + fecha.ToString("yyyy-MM-dd") + "\",\"reservas\":" + cantidad + ",\"total\":" + totalDia + "}";

    public string ObtenerExtension() => ".json";
}

public sealed class FormatoXml : IFormatoComprobante
{
    public string Clave => "XML";

    public string GenerarComprobante(Reserva reserva, Cliente cliente, Sala sala) =>
        new XElement("comprobante",
            new XElement("id", reserva.Id),
            new XElement("cliente", cliente.Nombre),
            new XElement("sala", sala.Nombre),
            new XElement("fecha", reserva.Fecha.ToString("dd/MM/yyyy")),
            new XElement("horaInicio", reserva.HoraInicio),
            new XElement("horas", reserva.Horas),
            new XElement("subtotal", reserva.Subtotal),
            new XElement("impuesto", reserva.Impuesto),
            new XElement("total", reserva.Total))
        .ToString(SaveOptions.DisableFormatting);

    public string GenerarVistaPrevia(Cliente cliente, Sala sala) =>
        new XElement("vistaPrevia",
            new XElement("cliente", cliente.Nombre),
            new XElement("sala", sala.Nombre))
        .ToString(SaveOptions.DisableFormatting);

    public string GenerarCuerpoCorreo() =>
        new XElement("mensaje", "Adjunto su comprobante.")
        .ToString(SaveOptions.DisableFormatting);

    public string GenerarReporte(DateTime fecha, int cantidad, decimal totalDia) =>
        new XElement("reporte",
            new XElement("fecha", fecha.ToString("yyyy-MM-dd")),
            new XElement("reservas", cantidad),
            new XElement("total", totalDia))
        .ToString(SaveOptions.DisableFormatting);

    public string ObtenerExtension() => ".xml";
}

internal sealed class FormatoDesconocido : IFormatoComprobante
{
    private const string MensajeError = "ERROR: formato de comprobante desconocido";

    public string Clave => string.Empty;
    public string GenerarComprobante(Reserva reserva, Cliente cliente, Sala sala) => MensajeError;
    public string GenerarVistaPrevia(Cliente cliente, Sala sala) => MensajeError;
    public string GenerarCuerpoCorreo() => MensajeError;
    public string GenerarReporte(DateTime fecha, int cantidad, decimal totalDia) => MensajeError;
    public string ObtenerExtension() => ".txt";
}