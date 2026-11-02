# Cómo poner a correr este sistema

**BISOF 09 Programación con Patrones** · Universidad Latina de Costa Rica

Hay dos formas. Use la que le funcione; el resultado es el mismo.

---

## Opción A — En el navegador, sin instalar nada (GitHub Codespaces)

Es la vía recomendada para la primera sesión: no depende de la máquina en que esté sentado y le deja lista la cuenta de GitHub, que va a necesitar de todas formas para el repositorio de su equipo.

1. Cree una cuenta gratuita en **github.com** si no tiene una. Use su correo institucional.
2. Abra el repositorio del curso que le indique el docente.
3. Botón verde **Code** → pestaña **Codespaces** → **Create codespace on main**.
4. Espere. La primera vez tarda dos o tres minutos: está armando una máquina con .NET 8 y Visual Studio Code dentro del navegador.
5. Cuando abra el editor, en la terminal de abajo escriba:

```
cd src
dotnet run
```

Debe ver varias reservas creadas, un reporte del día y un comprobante. Si lo ve, terminó.

**Sobre el consumo:** una cuenta gratuita de GitHub incluye 120 horas de cómputo y 15 GB de almacenamiento al mes. Este curso usa una fracción de eso, pero **cierre el codespace cuando termine** (menú de Codespaces → Stop codespace) para no gastar horas con la máquina encendida sin usarla.

---

## Opción B — En su propia máquina

Si ya tiene o puede instalar el entorno:

1. **SDK de .NET 8 o superior** — descárguelo de `dotnet.microsoft.com/download`. Verifique con:

```
dotnet --version
```

2. **Visual Studio Code** con la extensión de C#, o Visual Studio Community.
3. **Git**.

Luego:

```
git clone <url del repositorio del curso>
cd SalaEventos/src
dotnet run
```

---

## Qué debe pasar

El programa compila y corre **sin que usted toque nada**. Crea reservas, rechaza una que choca de horario, emite comprobantes en tres formatos y saca un reporte del día. Deja además dos rastros en el disco: `reservas.csv` y una carpeta `comprobantes/`.

Que funcione es el punto de partida del curso, no el logro. Lo que vamos a discutir durante quince semanas es **cómo está construido por dentro**.

---

## Si algo sale mal

- **`dotnet` no se reconoce como comando:** el SDK no está instalado o no está en el PATH. Use la Opción A.
- **El codespace tarda muchísimo:** es normal la primera vez. Si pasa de cinco minutos, ciérrelo y créelo de nuevo.
- **Compila pero no corre:** revise que esté dentro de la carpeta `src`.
- **No logró ninguna de las dos:** avise al docente antes de la segunda sesión. Esa sesión asume el sistema corriendo en su máquina, y no conviene arrastrar el problema.
