# Sistema de reservas de salas de eventos

Proyecto del curso **BISOFT-11 Programación con Patrones** · III Cuatrimestre 2026

Este es el sistema con el que va a trabajar durante todo el cuatrimestre. **Funciona**: compila, corre y hace lo que dice que hace. Su trabajo no es arreglar errores, es mejorar el diseño sin cambiar el comportamiento.

---

## Qué hace el sistema

Administra las reservas de tres salas de eventos:

- Registra reservas de una sala para una fecha, una hora de inicio y una cantidad de horas.
- Verifica que la sala esté libre en ese horario.
- Calcula el precio según el tipo de cliente, la duración, los servicios adicionales y la fecha.
- Emite el comprobante en tres formatos: texto, HTML y JSON.
- Guarda las reservas en un archivo y los comprobantes en una carpeta.
- Produce un reporte del día y permite anular una reserva.

## Cómo ejecutarlo

Necesita el SDK de .NET 8 o superior.

```
cd src
dotnet run
```

La primera ejecución crea `reservas.csv` y la carpeta `comprobantes/` en el directorio desde el que ejecute.

## Estructura

```
src/
  SalaEventos.csproj
  Program.cs           punto de entrada con un escenario de ejemplo
  Modelos.cs           Cliente, Sala, Reserva
  GestorReservas.cs    el resto del sistema
```

---

## Reglas de negocio

Estas son las reglas que el sistema implementa hoy. **Su rediseño debe conservarlas exactamente.**

### Validaciones

| Regla | Mensaje |
|---|---|
| El cliente debe existir | `ERROR: el cliente no existe` |
| La sala debe existir | `ERROR: la sala no existe` |
| Entre 1 y 12 horas | `ERROR: las horas deben estar entre 1 y 12` |
| Hora de inicio entre 6 y 20 | `ERROR: hora de inicio fuera del horario` |
| La reserva no puede pasar de las 22 | `ERROR: la reserva se pasa del cierre` |
| No se reserva en el pasado | `ERROR: no se reserva en el pasado` |
| La sala no puede estar ocupada en ese rango | `ERROR: la sala ya esta reservada en ese horario` |

### Cálculo del precio

Se aplica en este orden:

1. **Base:** tarifa por hora de la sala multiplicada por la cantidad de horas.
2. **Descuento por duración:** 15 % si son 8 horas o más; 10 % si son 4 o más y menos de 8; ninguno si son menos de 4.
3. **Descuento por tipo de cliente**, aplicado sobre el resultado anterior:
   - Corporativo: 12 %. Si además la reserva es de 8 horas o más y la sala tiene capacidad de 100 o más, un 3 % adicional.
   - Estudiantil: 20 %, pero **solo** si la fecha cae de lunes a viernes y la sala tiene capacidad de 30 o menos. Este descuento no aplica en reservas de menos de 4 horas.
   - Regular: ninguno.
4. **Catering:** suma 8.500 por cada unidad de capacidad de la sala.
5. **Proyector:** suma 15.000, salvo que el cliente sea corporativo y la reserva sea de 4 horas o más, en cuyo caso es gratis.
6. **Recargo de fin de semana:** 25 % si la fecha cae sábado o domingo.
7. **Recargo de feriado:** 40 % si la fecha es 15 de setiembre, 1 de diciembre, 25 de diciembre o 1 de enero.
8. **Impuesto:** 13 % sobre el subtotal resultante.

Los montos finales se redondean a dos decimales.

### Comprobantes

Tres formatos: `TEXTO`, `HTML` y `JSON`. Un formato desconocido produce `ERROR: formato de comprobante desconocido`.

El comprobante se devuelve al llamador y además se guarda en `comprobantes/reserva-<id>.<extensión>`.

### Otras operaciones

- `VistaPrevia` genera una versión reducida del comprobante, en los mismos tres formatos.
- `EnviarComprobante` arma el cuerpo del mensaje, en los mismos tres formatos, y devuelve a quién se enviaría.
- `ReporteDelDia` cuenta las reservas confirmadas de una fecha y suma sus totales, en los mismos tres formatos.
- `AnularReserva` marca la reserva como anulada y deja constancia en el archivo.

---

## Lo primero que hay que hacer

Antes de tocar una línea de este código, hay que poder demostrar que sigue haciendo lo mismo después de cada cambio. Eso son **pruebas de caracterización**: pruebas que capturan el comportamiento actual, tal como es hoy, incluso si ese comportamiento le parece discutible.

Ese es el Laboratorio 1.
