# Primera expedición: pesca, minería y fabricación

Recorrido vigente al 9 de noviembre de 2026. Sustituye la guía de sellos/cofres. La implementación y comprobaciones de editor están terminadas; presentación visual y equilibrio requieren todavía juego humano. No hay marcador de misión proyectado: se usan cuaderno, retícula y señales físicas.

## Recorrido

1. Nueva partida comienza en la playa, con manos y sin dinero. WASD mueve, ratón mira, C baja y Espacio sube. Bajo el agua, apunta a peces de orilla y haz clic para capturarlos.
2. Reúne pesca para financiar el **pico de $45**. El cuaderno considera saldo y valor real de los peces; si falta espacio, TAB permite revisar o descartar pilas.
3. Vuelve a Don Lucho, pulsa E en el mostrador y vende peces. Compra el pico; tridente y otras herramientas de pesca son mejoras opcionales, no un requisito para minar.
4. Sigue las boyas y busca vetas de cobre cerca de 24 m. Con pico comprado, bajo el agua y a menos de 4,5 m, apunta y pulsa E. Cada depósito entrega tres unidades; reúne al menos ocho para el primer traje.
5. Regresa a la tienda con **$120 y 8 cobre**. En **FABRICAR**, crea Traje reforzado: 64 m de profundidad segura y 155 unidades de aire. Los costes se descuentan una vez y la mejora se equipa permanentemente.
6. Repite con hierro cerca de 52 m y después cristal cerca de 88 m. Las recetas posteriores combinan materiales y dinero; consulta `Docs/ECONOMIA_CRAFTING.md`. El traje final prepara para explorar hasta 650 m.

El paso por la fosa siempre está abierto. Exceder el equipo consume mucho más aire; el aviso de regreso prioriza ascender antes que recolectar. En cámaras laterales vuelve al conducto central para subir. R rescata conservando inventario. La bombona de tienda suma aire al equipo fabricado.

Las vetas agotadas se oscurecen y regeneran tras 180 segundos de reloj real; guardar/cargar no las repone inmediatamente. Vender todos los peces conserva los minerales. No hacen falta cofres ni soluciones de puzles.

## Estado y pruebas Develop

`GameSession.Expedition` deriva el objetivo de saldo, pesca, pico, materiales y mejoras. No guarda pasos de misión independientes. El recordatorio de regreso aparece tras minar; Continuar sitúa al buzo en la playa y reconstruye el objetivo. El guardado v2 lee partidas v1 y conserva mejoras ganadas.

F2 inicia una prueba temporal; F1 abre herramientas. **Inicio sin equipo** prepara un inicio vacío. **Primera minería** coloca al buzo con pico junto al cobre. Ambos reinician el progreso temporal y desactivan ventajas; F2 permite usar reglas de Jugador. **Terminar pruebas** restaura la partida anterior en la playa. No usar dinero/equipo de Develop para medir la economía normal.

## Registro opcional del recorrido

`--expedition-report` genera un informe JSON independiente por Nueva partida/Continuar en `%USERPROFILE%/AppData/LocalLow/Caleta Bruma/La Fosa/ExpeditionReports/`. Es opcional, no cambia la ranura de guardado, y se suspende en Develop. `--smoke-test` lo desactiva.

Informes v2: fecha UTC, compilación, resolución, semilla, modo new/continue y `initialEquipmentUpgrades`. Cada segmento mide desde su inicio; Continuar no reconstruye tiempos anteriores.

| Campo | Interpretación |
|---|---|
| `elapsedSeconds` | Tiempo total del segmento, incluidas pausas |
| `engagedSeconds` | Tiempo sin pausa; incluye inventario y tienda |
| `activeSeconds` | Tiempo con controles habilitados; excluye pausa, inventario y tienda |
| `attemptsByTool` | Intentos aceptados de manos, tridente, arpón y red; no incluye extracción |
| `captures`, `shopVisits`, `sales`, `rescues` | Capturas, aperturas de tienda, ventas exitosas y rescates |
| `events` | Captura, compra, venta, extracción `mineral`, fabricación `craft`, rescates y regreso con minerales |

`return` / `return_rescued`, con detalle `mineral_to_beach`, distinguen el primer regreso observado con/sin rescate desde la extracción. Se mide a menos de 12 m del inicio, fuera del agua. Eventos de santuario/sello/cofre ya no forman parte del recorrido.

El informe se escribe en eventos, periódicamente y al salir. `recording` tras un cierre inesperado es un segmento incompleto. Antes de equilibrar con sus cifras, contrastar captura, venta, minería, fabricación, pausa y regreso con observación humana. Las comprobaciones de editor validan modelo/archivo, no esa experiencia.

## Ficha humana pendiente

Usar `Builds/Windows/La Fosa.exe`; `Builds/FirstExpedition` puede conservar una versión anterior. Ambas salidas comparten la ranura personal. Los diagnósticos usan archivos aislados.

| Dato | Resultado |
|---|---|
| Persona, fecha, compilación, resolución e informe new/continue | Pendiente |
| Tiempo hasta primera captura / venta / pico | Pendiente |
| Tiempo y aire hasta primer cobre / regreso / traje | Pendiente |
| Ingresos, viajes y recursos para las tres recetas | Pendiente |
| Comprensión de vetas agotadas y regeneración | Pendiente |
| Lectura de materiales/precio/resultado en FABRICAR | Pendiente |
| Rescates, causa y orientación para ascender | Pendiente |
| Guardar, cerrar y continuar conserva equipo, minerales y plazos | Pendiente |
| HUD legible en 1280×720, 1920×1080 y ultrapanorámico | Pendiente |
| Ajuste propuesto y motivo observado | Pendiente |

Comprobar consumo en superficie, al acelerar y al cruzar los cinco umbrales. La duración del aire cambia según el ritmo actual y no garantiza el regreso. Medir antes de ajustar precios o reservas; las pruebas automáticas no rellenan esta ficha.
