# Primera expedición: guía de la etapa 3

Primera entrega del 18 de octubre de 2026. Implementa orientación contextual; el cierre de la etapa exige todavía juego humano y revisión del HUD.

**Cambio vigente 3.6 (31 de octubre):** retirado el marcador proyectado a petición del usuario. Las referencias al rombo/flechas de destino en el historial siguiente describen versiones anteriores; hoy la orientación se apoya en cuaderno y señales físicas. F2 inicia/alterna Develop y Jugador en una prueba temporal; F1 abre herramientas. Al terminar pruebas se restaura el progreso normal en la playa. No medir el recorrido humano con equipo o dinero de Develop: terminar la prueba antes de evaluar el equilibrio normal.

## Recorrido

1. En Nueva partida, el cuaderno explica ratón, WASD, bajar con C y subir a respirar con ESPACIO. El marcador apunta a la pesca de orilla y, bajo el agua cerca de esa zona, al pez costero disponible más cercano. Una señal junto a las primeras boyas recuerda los controles.
2. La guía calcula el dinero que falta sumando saldo y valor real de la pesca. No promete un número fijo de peces. Al apuntar a un pez se muestra distancia al centro del pez y alcance de la herramienta; la captura real sigue usando su volumen físico y respetando obstáculos.
3. Cuando la pesca permite comprar el tridente, o la mochila de pesca está llena, el destino cambia a la cabaña. Vender y comprar actualizan el objetivo. Una mochila llena de conchas ofrece descartar una pila mediante TAB.
4. El tridente lleva al santuario de huiros: E gira piedras, después se activa ABRIR SELLO y se recoge el cofre. Si no queda espacio, la guía explica cómo liberarlo y la recompensa espera. Resolver libremente el sello sin comprar sigue permitiendo recoger su tesoro.
5. El primer tesoro conserva su mejora permanente de aire/profundidad y prepara al jugador para el naufragio. Toda la fosa, incluido el abismo, tiene descenso libre; exceder la profundidad segura consume aire ×8. La guía recomienda respirar y regresar a la caleta; al llegar cerca del inicio propone preparar la siguiente salida. El regreso apunta directamente a superficie, sin escotillas intermedias.
6. El siguiente descenso recuerda que el arpón tiene 40 m de alcance para peces rápidos. Comprar herramientas sigue siendo independiente de abrir sellos.

Con aire por debajo de 30 s o presión excesiva, el ascenso tiene prioridad sobre la guía normal. R sigue rescatando con inventario conservado. Un destino detrás de la cámara muestra hacia dónde girar; no desaparece.

Actualización 3.2c (28 de octubre): debajo de los santuarios la fosa continúa hasta 650 m. El consumo aumenta al cruzar cinco umbrales; el HUD expresa la reserva en unidades de aire. El aviso usa al menos 30 unidades, o la estimación de regreso con 20 % de margen si es mayor. En la cavidad profunda, el marcador de regreso lleva primero al eje de ascenso. El tercer tesoro permite explorar esa escala con equipo de 650 m y 1400 unidades. La ficha humana debe medir el balance nuevo; los tiempos anteriores no lo validan.

Entrega 3.3a (23 de octubre): el HUD mantiene proporciones al cambiar el formato de la ventana, amplía el texto del cuaderno y adapta su altura. El marcador tiene fondo propio y se mantiene por encima de los paneles de captura/interacción; añade indicaciones de mirada para destinos fuera de pantalla. La barra y el aviso de aire usan el mismo umbral de 30 s de la guía. Los controles y la alarma tienen fondo oscuro para poder leerse sobre la playa o el agua. Cambios compilados; comprobación visual en juego pendiente.

## Estado y compatibilidad

`GameSession.Expedition` deriva los objetivos de inventario, saldo, compras y puzles. El precio se consulta en `Tienda.asset`. No escribe pasos en el guardado ni modifica el formato v1.

El recordatorio de regreso tras el primer cofre existe durante esa sesión. Continuar ya sitúa al buzo en la playa y retoma el siguiente objetivo; no repite el regreso. Tampoco vuelve a exigir pesca inicial a quien consiguió el tesoro sin seguir la guía. Nueva partida reconstruye todo desde el estado inicial.

Se conservan el rescate, los precios, la capacidad de 12 espacios, el aire inicial y los alcances de manos/tridente/arpón. No se añadió dinero extra por abrir el cofre; su recompensa sigue siendo la mejora de equipo y el progreso. El ajuste de descenso libre reemplaza el daño directo por presión por consumo de aire multiplicado; los puzles conservan su orden.

## Probar y evaluar

### Registro opcional del recorrido (3.4a, 28 de octubre de 2026)

Preparado para una futura prueba humana; no se ha ejecutado el juego para obtener mediciones. Al iniciar el ejecutable con el argumento `--expedition-report`, cada **Nueva partida** o **Continuar** abre un informe JSON independiente en `%USERPROFILE%/AppData/LocalLow/Caleta Bruma/La Fosa/ExpeditionReports/`. Sin ese argumento no se generan informes; `--smoke-test` los desactiva también. Esta opción no cambia la ranura de guardado personal.

El archivo incluye fecha UTC, identificador de compilación, resolución inicial, semilla, modo `new`/`continue` y progreso inicial. Un segmento de Continuar empieza a medir desde esa carga: no reconstruye los tiempos anteriores ni registra la restauración en la playa como rescate. Conservar el identificador del informe en la ficha de la persona evaluadora.

| Campo | Interpretación |
|---|---|
| `elapsedSeconds` | Tiempo total desde el inicio del segmento, incluidas pausas. |
| `engagedSeconds` | Tiempo sin pausa; incluye leer inventario y comprar/vender. Usarlo como referencia de duración del recorrido. |
| `activeSeconds` | Tiempo con controles habilitados; excluye pausa, inventario y tienda. No representa solo desplazamiento físico. |
| `attemptsByTool` | Intentos aceptados por manos, tridente, arpón y red, en ese orden. Excluye clics durante recarga, menús o en una posición no válida para pescar. |
| `captures`, `shopVisits`, `sales`, `rescues` | Peces añadidos a mochila, aperturas de tienda, ventas exitosas y rescates. Abrir tienda cuenta una visita; no equivale a medir el trayecto hasta la cabaña. Una red puede capturar varios peces en un intento. |
| `events` | Primera captura, visitas, ventas, compra de tridente, llegada al santuario, sello, cofre, rescates y primer regreso. Cada evento conserva tiempos, aire, profundidad y una copia de los contadores. |

La llegada al santuario (`shrine`) se mide cuando la cámara queda a 5 m del marcador de la primera zona; no prueba que la persona haya entendido la señal. El aire del cofre (`treasure`) se toma **antes** de que la recompensa recargue el equipo. `return` indica llegada a menos de 12 m del inicio, fuera del agua; `return_rescued` distingue haber usado un rescate después de recoger el tesoro. Los contadores del evento de regreso no cambian con salidas posteriores. Para rescates se conserva `manual`, `world_boundary` u `oxygen_depleted` como causa.

El informe se actualiza en los eventos, cada 15 segundos, al perder foco y al salir. `status` distingue `recording`, `menu`, `quit` o destrucción de sesión; `recording` tras un cierre inesperado es un registro parcial, no una prueba completada. La escritura temporal conserva el informe anterior si falla el reemplazo; el fallo se avisa en el log y no altera el guardado de la expedición. Un cierre brusco puede perder datos desde la última escritura.

La ficha sigue necesitando observación humana para legibilidad, orientación, viajes reales y comprobación de carga. Antes de usar sus cifras para equilibrar, contrastar en una partida autorizada una captura, una venta, una pausa, un rescate y un regreso con sus eventos. La validación de editor comprueba el modelo y el archivo, no esa integración jugable.

### Ficha humana

Registrar el recorrido en esta ficha antes de ajustar el equilibrio. Dejar los datos sin medir como «pendiente»; las comprobaciones automáticas no rellenan esta ficha.

| Dato | Resultado |
|---|---|
| Persona nueva, fecha, versión y resolución | Pendiente |
| Identificador del informe y modo new/continue | Pendiente |
| Tiempo hasta primera captura / venta / tridente | Pendiente |
| Intentos de captura y viajes a la cabaña | Pendiente |
| Tiempo hasta sello / cofre / regreso | Pendiente |
| Aire al llegar al santuario y al recoger el cofre | Pendiente |
| Rescates y causa | Pendiente |
| Texto difícil de leer o destino confuso | Pendiente |
| Guardar, cerrar y continuar conservó equipo y objetivo | Pendiente |
| Ajuste propuesto y motivo observado | Pendiente |

La entrega vigente está en `Builds/Windows/La Fosa.exe`, actualizada el 31 de octubre con 3.2d y 3.3c. `Builds/FirstExpedition` conserva una salida alternativa anterior: no usarla para validar la entrega actual. Ambas usan la misma ranura personal; cerrar el ejecutable anterior antes de jugar en el nuevo evita que dos sesiones escriban progreso distinto. Las pruebas con `--smoke-test` usan archivos aislados.

- Empezar desde cero y seguir la guía: pescar hasta financiar el tridente en una salida, volver, vender y comprar. Comprobar legibilidad y que el marcador no tape el cuaderno al girarse.
- Sumergirse, respirar y comprobar la indicación de distancia. Valorar si se localizan y alcanzan los peces sin explicaciones externas.
- Comprobar aire en superficie, al acelerar y al cruzar un umbral: reserva en unidades, consumo en u/s y duración aproximada deben ser coherentes. El siguiente umbral cambia al cruzarlo; tras el quinto se indica último nivel. Revisar los paneles en 1280×720, 1920×1080 y formato ultrapanorámico, sin textos cortados.
- Llegar al santuario, conectar las piedras, accionar la palanca y recoger el cofre. Anotar el tiempo y el aire restante; salir a respirar debe ser viable sin borrar giros.
- Volver a la playa, guardar, cerrar y continuar. Deben conservarse tridente, sello y mejora; el objetivo debe ser preparar el siguiente descenso.
- Medir viajes a la cabaña, intentos de captura, uso de rescate y dificultad para orientarse. Ajustar precios, oxígeno o distancias solo a partir de esos resultados.

Las capturas de pantalla del diagnóstico `guia-*.png` requieren una ventana renderizada. En las ejecuciones ocultas de esta entrega resultaron negras: no sirven como evidencia visual. La revisión visual y la evaluación humana siguen pendientes.
