# Primera expedición: guía de la etapa 3

Primera entrega del 18 de octubre de 2026. Implementa orientación contextual; el cierre de la etapa exige todavía juego humano y revisión del HUD.

## Recorrido

1. En Nueva partida, el cuaderno explica ratón, WASD, bajar con C y subir a respirar con ESPACIO. El marcador apunta a la pesca de orilla y, bajo el agua cerca de esa zona, al pez costero disponible más cercano. Una señal junto a las primeras boyas recuerda los controles.
2. La guía calcula el dinero que falta sumando saldo y valor real de la pesca. No promete un número fijo de peces. Al apuntar a un pez se muestra distancia al centro del pez y alcance de la herramienta; la captura real sigue usando su volumen físico y respetando obstáculos.
3. Cuando la pesca permite comprar el tridente, o la mochila de pesca está llena, el destino cambia a la cabaña. Vender y comprar actualizan el objetivo. Una mochila llena de conchas ofrece descartar una pila mediante TAB.
4. El tridente lleva al santuario de huiros: E gira piedras, después se activa ABRIR SELLO y se recoge el cofre. Si no queda espacio, la guía explica cómo liberarlo y la recompensa espera. Resolver libremente el sello sin comprar sigue permitiendo recoger su tesoro.
5. El primer tesoro conserva su mejora permanente de aire/profundidad y prepara al jugador para el naufragio. El primer descenso es libre, sin compuerta; exceder la profundidad segura consume aire ×8. La guía recomienda respirar y regresar a la caleta; al llegar cerca del inicio propone preparar la siguiente salida. Bajo la losa del abismo abierto indica su escotilla de retorno antes de la superficie.
6. El siguiente descenso recuerda que el arpón tiene 40 m de alcance para peces rápidos. Comprar herramientas sigue siendo independiente de abrir sellos.

Con aire por debajo de 30 s o presión excesiva, el ascenso tiene prioridad sobre la guía normal. R sigue rescatando con inventario conservado. Un destino detrás de la cámara muestra hacia dónde girar; no desaparece.

Entrega 3.3a (23 de octubre): el HUD mantiene proporciones al cambiar el formato de la ventana, amplía el texto del cuaderno y adapta su altura. El marcador tiene fondo propio y se mantiene por encima de los paneles de captura/interacción; añade indicaciones de mirada para destinos fuera de pantalla. La barra y el aviso de aire usan el mismo umbral de 30 s de la guía. Los controles y la alarma tienen fondo oscuro para poder leerse sobre la playa o el agua. Cambios compilados; comprobación visual en juego pendiente.

## Estado y compatibilidad

`GameSession.Expedition` deriva los objetivos de inventario, saldo, compras y puzles. El precio se consulta en `Tienda.asset`. No escribe pasos en el guardado ni modifica el formato v1.

El recordatorio de regreso tras el primer cofre existe durante esa sesión. Continuar ya sitúa al buzo en la playa y retoma el siguiente objetivo; no repite el regreso. Tampoco vuelve a exigir pesca inicial a quien consiguió el tesoro sin seguir la guía. Nueva partida reconstruye todo desde el estado inicial.

Se conservan el rescate, los precios, la capacidad de 12 espacios, el aire inicial y los alcances de manos/tridente/arpón. No se añadió dinero extra por abrir el cofre; su recompensa sigue siendo la mejora de equipo y el progreso. El ajuste de descenso libre reemplaza el daño directo por presión por consumo de aire multiplicado; los puzles conservan su orden.

## Probar y evaluar

Registrar el recorrido en esta ficha antes de ajustar el equilibrio. Dejar los datos sin medir como «pendiente»; las comprobaciones automáticas no rellenan esta ficha.

| Dato | Resultado |
|---|---|
| Persona nueva, fecha, versión y resolución | Pendiente |
| Tiempo hasta primera captura / venta / tridente | Pendiente |
| Intentos de captura y viajes a la cabaña | Pendiente |
| Tiempo hasta sello / cofre / regreso | Pendiente |
| Aire al llegar al santuario y al recoger el cofre | Pendiente |
| Rescates y causa | Pendiente |
| Texto difícil de leer o destino confuso | Pendiente |
| Guardar, cerrar y continuar conservó equipo y objetivo | Pendiente |
| Ajuste propuesto y motivo observado | Pendiente |

La compilación de esta entrega está en `Builds/FirstExpedition/La Fosa.exe`, separada de `Builds/Windows` porque había un juego abierto. Ambas usan la misma ranura personal; cerrar el ejecutable anterior antes de jugar en el nuevo evita que dos sesiones escriban progreso distinto. Las pruebas con `--smoke-test` usan archivos aislados.

- Empezar desde cero y seguir la guía: pescar hasta financiar el tridente en una salida, volver, vender y comprar. Comprobar legibilidad y que el marcador no tape el cuaderno al girarse.
- Sumergirse, respirar y comprobar la indicación de distancia. Valorar si se localizan y alcanzan los peces sin explicaciones externas.
- Llegar al santuario, conectar las piedras, accionar la palanca y recoger el cofre. Anotar el tiempo y el aire restante; salir a respirar debe ser viable sin borrar giros.
- Volver a la playa, guardar, cerrar y continuar. Deben conservarse tridente, sello y mejora; el objetivo debe ser preparar el siguiente descenso.
- Medir viajes a la cabaña, intentos de captura, uso de rescate y dificultad para orientarse. Ajustar precios, oxígeno o distancias solo a partir de esos resultados.

Las capturas de pantalla del diagnóstico `guia-*.png` requieren una ventana renderizada. En las ejecuciones ocultas de esta entrega resultaron negras: no sirven como evidencia visual. La revisión visual y la evaluación humana siguen pendientes.
