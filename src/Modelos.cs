namespace SalaEventos;

// Estas clases solo guardan datos. Toda la lógica vive en GestorReservas.
public class Cliente
{
    public int Id;
    public string Nombre;
    public string Tipo;      // "REGULAR", "CORPORATIVO", "ESTUDIANTIL"
    public string Correo;
}

public class Sala
{
    public int Id;
    public string Nombre;
    public int Capacidad;
    public decimal TarifaHora;
}

public class Reserva
{
    public int Id;
    public int ClienteId;
    public int SalaId;
    public DateTime Fecha;
    public int HoraInicio;
    public int Horas;
    public bool ConCatering;
    public bool ConProyector;
    public decimal Subtotal;
    public decimal Impuesto;
    public decimal Total;
    public string Estado;    // "CONFIRMADA", "ANULADA"
}
