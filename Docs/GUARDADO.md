# Guardado de la expedición

Estado vigente: formato v2, compatible con lectura de v1. Una partida local por usuario del sistema. Nueva partida solicita confirmación si existe el principal o respaldo; cancelar conserva ambos. Guardar y volver al menú permanece en el juego si falla la escritura.

## Qué se conserva

- Dinero, pilas de inventario, compras únicas y herramienta seleccionada.
- Mejoras de equipo fabricadas, sin ocupar espacios de mochila.
- Momento UTC de regeneración de cada depósito de minerales. Cargar no reinicia el plazo; los 180 segundos transcurren también con el juego cerrado.
- Zonas descubiertas, conchas recogidas y propiedad/posición/orientación de vehículos.
- Amarre de la lancha. En datos antiguos sin ese campo se interpreta como desamarrada.

Continuar devuelve al buzo a la playa con aire y salud completos. No guarda posición submarina, pasajero embarcado, cámara, recargas ni estado temporal de peces. La bodega de embarcaciones todavía no existe. El rescate conserva objetos.

## Cuándo se guarda

Compras, ventas, extracción y fabricación guardan después de completar la transacción. Cambios de inventario, conchas, herramienta y amarre se guardan al final del frame. También se guarda al regresar, desembarcar, perder foco, pausar la aplicación y cerrar normalmente; cada 20 segundos se actualizan posiciones de vehículos. La pausa permite guardar explícitamente.

Un cierre forzado puede perder cambios posteriores al último guardado confirmado. La regeneración de minerales usa reloj real; no se simulan pesca, ingresos ni expediciones sin conexión.

## Archivo y recuperación

`Application.persistentDataPath/expedition.json`: en Windows, `%USERPROFILE%/AppData/LocalLow/Caleta Bruma/La Fosa/expedition.json`. No está dentro del repositorio ni del ejecutable.

- `expedition.json`: envoltura JSON con datos v2 y checksum SHA-256.
- `expedition.json.bak`: última escritura válida anterior; Nueva partida establece ambos con la partida nueva.
- `expedition.json.tmp`: escritura temporal antes de sustituir el principal.

El checksum detecta corrupción, no impide modificaciones deliberadas. Antes de restaurar se validan versión, semilla, IDs, cantidades, capacidad, compras, orden de mejoras, plazos de minerales y estado/coordenadas de vehículos. El equipo se reconstruye sin sumar otra vez mejoras ya obtenidas.

Si el principal falla se intenta el respaldo y se avisa. Guardar después de recuperar conserva el respaldo válido. Si ambos fallan, Continuar queda deshabilitado; los archivos permanecen hasta confirmar Nueva partida. Un fallo de escritura mantiene el progreso en memoria y muestra aviso.

## Migración e identificadores

Las partidas v1 se validan con las definiciones antiguas de circuitos y recompensas. Sus mejoras ganadas se convierten a `equipmentUpgrades`; no se exige volver a fabricarlas. Se conservan dinero, compras, inventario, descubrimientos, conchas, vehículos y amarre. La siguiente escritura usa v2, con `treasures` y `puzzles` vacíos. Los objetos antiguos del inventario se conservan, pero ya no gobiernan la progresión. Un v1 sin datos de minerales empieza con vetas disponibles.

Los objetos usan `ItemDefinition.id`; las mejoras conservan IDs `huiros`, `naufragio`, `abismo`. Las vetas usan IDs únicos de `WorldBuilder.Minerals`. Compras/vehículos usan nombres estables de `GearKind`, y las conchas `coast-shell-0` a `coast-shell-8`. No cambiar IDs al renombrar títulos visibles. No hay adaptación automática para formatos futuros, semillas distintas o contenido desconocido.

## Develop y pruebas

Entrar a Develop guarda primero el progreso normal y toma una copia. Extracción, fabricación y cierre durante la prueba no escriben sobre la partida personal ni su respaldo. Terminar pruebas restaura el progreso y vuelve a la playa; al cargar se aplica el reloj real a la regeneración.

`EconomyProgressionChecks` verifica transacciones, plazos persistentes, fabricación duplicada, datos inválidos, migración v1 parcial/completa y recarga v2. `DeveloperModeChecks` comprueba aislamiento y restauración; ambos usan archivos temporales propios. Evidencia: `unity-economy-final-build.log`, sin Play Mode.

El recorrido actual de `SmokeCheck.Start` y los modos `--smoke-test --save-restart-write` / `--smoke-test --save-restart-read` están adaptados y compilados para economía. **No se ejecutaron en esta entrega.** Los antiguos auxiliares de diagnóstico de puzles no validan el mundo actual. La revisión humana del menú y continuidad sigue pendiente en `Docs/PRIMERA_EXPEDICION.md`.
