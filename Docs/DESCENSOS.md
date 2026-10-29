# Rutas entre capas

**Decisión vigente del usuario, 28 de octubre de 2026: el límite de la fosa es el aire disponible. Todas las capas tienen descenso y ascenso libres, incluido el abismo.** No se construyen losas, escotillas ni paredes para exigir una mejora antes de nadar hacia abajo.

Superar la profundidad segura consume oxígeno ×8 (×12 al acelerar). No hay daño directo por presión mientras quede aire; agotarlo produce asfixia y finalmente rescate con inventario conservado. El equipo amplía el alcance práctico de la salida. El HUD prioriza el ascenso cuando queda poco aire o se supera la profundidad segura.

| Capa | Entrada libre | Preparación del mecanismo | Prueba y recompensa | Regreso |
|---|---|---|---|---|
| Bosque de huiros | Boyas cerca de `(0, -2, 46)` | 24 m / 115 s; equipo inicial | Mural de 6 piedras, activar sello y recoger Perla: 64 m / 155 s | Ascender hacia superficie; R permite rescate |
| Ecos del naufragio | Referencia `(0, -32, 75)` y laterales | 52 m / 155 s y Perla recuperada | Mural de 9 piedras y Brújula: 100 m / 200 s | Ascenso libre hacia superficie; R permite rescate |
| Corazón del abismo | Referencia `(0, -64, 75)` y laterales | 88 m / 200 s y Brújula recuperada | Mural de 12 piedras y Corazón: 110 m / 240 s | Ascenso libre hacia superficie; R permite rescate |

Los requisitos de esta tabla corresponden a los mecanismos y sus recompensas; nunca bloquean el movimiento. Se puede explorar más profundo con el equipo inicial si se asume el gasto de aire. La bombona de tienda suma 60 s y no resuelve puzles. Un tablero conectado necesita activar ABRIR SELLO y recoger su cofre para recibir la mejora. Con la mochila llena, el cofre y la solución esperan.

El marcador de descenso apunta al santuario; el regreso apunta directamente a la superficie sobre el buzo. Las balizas y señales recuerdan vigilar el aire y reservarlo para volver. Todos esos elementos de orientación carecen de collider.

## Persistencia

Se mantiene el guardado v1: equipo, tesoros y puzles conservan su estado. Las barreras se generaban desde el código y no estaban guardadas; retirarlas no requiere reiniciar la partida. Descartar un objeto de tesoro conserva su mejora. Continuar devuelve al jugador a la playa, como antes.

## Configuración y código

`DiveZoneDefinition.entryPosition` sitúa la señal de referencia. `requiredSafeDepth` y `requiredOxygen` siguen definiendo la preparación de los mecanismos; `unlockedDepth` y `unlockedOxygen` definen la recompensa. Se eliminaron `hasPhysicalGate` y `barrierWidth`, así como `DiveGate`, para que una configuración antigua no vuelva a crear paredes.

`WorldBuilder.Descents.BuildDescentGuides` construye únicamente señales y balizas sin colisión. `GameSession.NextWaypoint` orienta al santuario y `GameSession.Expedition.AscentWaypoint` a superficie. `DiverController.AdvanceVitals` conserva el coste por aire definido por `GameConfig.pressureOxygenMultiplier`. La fauna respeta el terreno y ya no evita alturas reservadas a losas inexistentes.

## Validación y límites

`DescentChecks`, incluido en `ProjectSetup.Validate`, construye solo las guías en editor: comprueba las dos transiciones, ausencia de colliders/interacciones de bloqueo y espacio bajo los puntos de cruce en la función de terreno. No entra en Play Mode ni construye la partida completa.

`SmokeCheck.Descents` queda preparado para verificar en una futura ejecución autorizada el cruce por centro/laterales de ambas transiciones desde Nueva partida, el acceso a santuarios con equipo, ascenso y persistencia. Actualizar el diagnóstico no equivale a haberlo ejecutado. Revisión visual, colisiones del mundo completo y equilibrio de aire siguen pendientes de recorrido humano. Evidencia de compilación/editor en `Docs/PRUEBAS.md`.

Las comprobaciones antiguas sobre losas cerradas y escotillas pertenecen a versiones anteriores; no describen el diseño vigente.
