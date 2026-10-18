# Guardado de la expedición

Etapa 1 del plan. Una partida local por usuario del sistema. El menú permite **Nueva partida** y **Continuar**; una nueva partida solicita confirmación cuando existe el archivo principal o su respaldo. Cancelar conserva los archivos. La pausa incluye **Guardar y volver al menú** y permanece en el juego si falla la escritura.

## Qué se conserva

- Saldo, pilas de inventario, compras únicas y herramienta seleccionada.
- Tesoros reclamados y mejoras de aire/profundidad, aunque se haya descartado el objeto.
- Giros, cantidad de movimientos y apertura de cada mecanismo; tapa de cada cofre.
- Acceso a las compuertas de descenso, derivado de esos mecanismos y tesoros sin añadir estado duplicado al formato v1.
- Zonas descubiertas: se registran al acercarse a menos de 18 m del santuario, o al recuperar su tesoro.
- Las nueve conchas de la costa: las recogidas no se vuelven a entregar al cargar.
- Propiedad, posición y orientación del bote y de la moto de agua.

Al continuar, el buzo vuelve a la playa con salud y aire completos. No se conserva su posición bajo el agua, un pasajero embarcado, la cámara, la recarga de herramientas ni el estado temporal de la fauna. Los peces se regeneran; ya eran un recurso renovable. No existe almacenamiento/carga de embarcaciones: corresponde a la etapa 4.

## Cuándo se guarda

Compras, ventas y tesoros guardan al completar su transacción. Inventario, conchas, herramienta, giros y sellos marcan cambios para guardar al final del frame, evitando capturar una operación a medio terminar. También se guarda al regresar a la superficie/playa, desembarcar, perder el foco, pausar la aplicación y cerrar normalmente; cada 20 segundos se actualiza la posición de los vehículos. La pausa permite guardar explícitamente al volver al menú.

Un cierre forzado puede perder los cambios posteriores al último guardado confirmado. El guardado no simula progreso mientras el juego está cerrado.

## Archivo y recuperación

Se usa `Application.persistentDataPath/expedition.json`. En Windows, con los ajustes actuales del proyecto, corresponde a `%USERPROFILE%/AppData/LocalLow/Caleta Bruma/La Fosa/expedition.json`. No está dentro del repositorio ni dentro del ejecutable.

- `expedition.json`: principal, envoltura JSON con datos de versión 1 y checksum SHA-256.
- `expedition.json.bak`: copia de la última escritura válida anterior; en una nueva partida ambos archivos representan la expedición nueva.
- `expedition.json.tmp`: escritura temporal. Se vacía el búfer a disco antes de reemplazar el principal.

El checksum detecta corrupción; no es una protección contra modificaciones deliberadas. Antes de tocar la sesión se validan versión, semilla, IDs, cantidades, capacidad, requisitos de equipo, progresión, circuitos resueltos y coordenadas finitas de vehículos. Las mejoras se derivan de compras y tesoros, sin acumularlas de nuevo en cada carga.

Si el principal está ausente, truncado, dañado o es incompatible, se intenta el respaldo y se avisa en el menú. Guardar después de esa recuperación conserva el respaldo válido. Si ambos fallan, Continuar queda deshabilitado; los archivos se conservan hasta que el jugador confirme Nueva partida. Un fallo de escritura mantiene el progreso en memoria y muestra un aviso; se puede reintentar desde pausa.

## Identificadores y ampliaciones

Los objetos usan `ItemDefinition.id`. Las zonas usan `DiveZoneDefinition.id`: `huiros`, `naufragio`, `abismo`; cada zona tiene exactamente un mecanismo y un tesoro, y su ID identifica ambos estados. Las embarcaciones y compras usan nombres estables de `GearKind`, no sus índices numéricos. Las conchas usan `coast-shell-0` a `coast-shell-8`.

No renombrar estos IDs al cambiar un título visible. El formato actual rechaza semillas distintas, IDs desconocidos y tableros con dimensiones incompatibles. No hay migraciones de formatos futuros ni adaptación automática a cambios de contenido. Al ampliar zonas o modificar la geometría de navegación debe diseñarse y probarse una migración antes de distribuir esa versión.

## Pruebas

`SmokeCheck.Persistence` amplía `--smoke-test` con transacciones, restauración, duplicados, nueva partida, respaldo, archivos ausentes/dañados, valores inválidos y fallo de escritura. Cada ejecución completa usa una carpeta aislada bajo `Builds/Windows/Verification/SaveTests-*`, sin leer ni sobrescribir la partida personal.

Para comprobar un cierre real, ejecutar en orden el mismo binario con `--smoke-test --save-restart-write` y después `--smoke-test --save-restart-read`. Estos dos modos comparten solamente `Verification/RestartSave`, registran su resultado en `restart-write.txt` y `restart-read.txt` y se cierran automáticamente. El segundo proceso compara todos los datos guardados y comprueba recompensa única y mejoras.

La prueba humana del menú, cancelación, avisos y navegación se detalla en `Docs/PRUEBAS.md`.
