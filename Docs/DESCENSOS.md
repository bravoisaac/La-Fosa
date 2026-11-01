# Rutas entre capas

**Decisión vigente del usuario, 28 de octubre de 2026: el límite de la fosa es el aire disponible. Todas las capas tienen descenso y ascenso libres, incluido el abismo.** No se construyen losas, escotillas ni paredes para exigir una mejora antes de nadar hacia abajo.

**Ampliación según `fosa mapa.png`: el fondo pasa de 88 a 650 m.** La costa y los tres santuarios existentes conservan sus posiciones. Bajo el último hay una abertura de 8 × 8 m centrada en `(0, -88, 76)` hacia una cavidad 3D irregular: estrechamientos, cámaras laterales y dos pilares unidos a las paredes que dividen rutas y vuelven a reunirlas antes de la cámara inferior. Las profundidades y el equipo de esta primera implementación son provisionales, pendientes de juego humano.

Superar la profundidad segura añade un factor ×8 al consumo (×12 con aceleración). Ahora ese factor se combina con el coste de la franja de presión. No hay daño directo por presión mientras quede aire; agotarlo produce asfixia y finalmente rescate con inventario conservado.

## Las cinco líneas del dibujo

Las líneas marcan cambios de presión y gasto, sin planos, suelos, puertas ni pérdidas instantáneas de aire al cruzarlas. Se puede subir y bajar por cualquier umbral. N0 es el tramo inicial por encima de la primera línea.

| Nivel | Profundidad de inicio | Consumo con equipo preparado, sin acelerar |
|---|---:|---:|
| N0 | Superficie | 1 unidad de aire/s |
| N1 | 60 m | 1,25 unidades/s |
| N2 | 150 m | 1,6 unidades/s |
| N3 | 280 m | 2,1 unidades/s |
| N4 | 420 m | 2,8 unidades/s |
| N5 | 560 m, hasta el fondo a 650 m | 3,8 unidades/s |

Son niveles de presión de juego, no atmósferas físicas. Por ejemplo, en N3 excediendo el equipo se gastan 16,8 unidades/s, o 25,2 acelerando. El HUD muestra nivel, multiplicador sin acelerar, consumo actual en unidades/s (incluye acelerar), próximo umbral y reserva aproximada de regreso. La duración `~min:seg AL RITMO ACTUAL` divide el aire restante por el consumo actual: cambia con la profundidad/equipo/aceleración y no es tiempo garantizado para regresar. En superficie se sustituye por el estado de recuperación. Inventario, requisitos y tienda expresan capacidad en unidades de aire.

El aviso de regreso usa el mayor valor entre 30 unidades y la estimación de un ascenso directo sin acelerar, con un 20 % de margen. Integra las franjas y considera aletas/equipo; no garantiza cubrir desvíos ni tiempo resolviendo puzles. En la cavidad profunda el marcador lleva primero al eje despejado `(0, z=76)` y luego a superficie, para evitar recomendar ascender contra un saliente lateral.

Vista estática de la geometría, sin Play Mode: `Docs/Mapa/fosa-seccion.png`. En esa imagen se retira el frente para ver el interior y se exagera el ancho ×1,6, indicado en el propio rótulo; no es una captura de partida.

## Santuarios y equipo

| Capa | Entrada libre | Preparación del mecanismo | Prueba y recompensa | Regreso |
|---|---|---|---|---|
| Bosque de huiros | Boyas cerca de `(0, -2, 46)` | 24 m / 115 s; equipo inicial | Mural de 6 piedras, activar sello y recoger Perla: 64 m / 155 s | Ascender hacia superficie; R permite rescate |
| Ecos del naufragio | Referencia `(0, -32, 75)` y laterales | 52 m / 155 s y Perla recuperada | Mural de 9 piedras y Brújula: 100 m / 200 s | Ascenso libre hacia superficie; R permite rescate |
| Corazón del abismo | Referencia `(0, -64, 75)` y laterales | 88 m / 200 unidades y Brújula recuperada | Mural de 12 piedras y Corazón: equipo de 650 m / 1400 unidades | Ascenso libre hacia superficie; R permite rescate |

Los requisitos de esta tabla corresponden a los mecanismos y sus recompensas; nunca bloquean el movimiento. Se puede explorar más profundo con el equipo inicial si se asume el gasto de aire. La bombona de tienda suma 60 s y no resuelve puzles. Un tablero conectado necesita activar ABRIR SELLO y recoger su cofre para recibir la mejora. Con la mochila llena, el cofre y la solución esperan.

Los primeros santuarios siguen introduciendo pesca, venta y puzles. El equipo del tercer tesoro se amplía para permitir explorar la nueva escala; su capacidad cubre el recorrido directo teórico de ida/vuelta con margen. No se añaden todavía cinco biomas completos, tesoros o fauna profunda nuevos. Las balizas y señales de profundidad carecen de collider; las paredes y pilares naturales de la cavidad sí tienen colisión.

## Persistencia

Se mantienen tres IDs de zona y el guardado v1. Equipo, tesoros y puzles conservan su estado. Quien ya recuperó el tercer tesoro recibe sus nuevos valores de equipo al Continuar, porque se reconstruyen desde las recompensas vigentes. No se guardan posiciones del buzo: Continuar vuelve a la playa como antes. El fondo y la reserva cambian, por lo que el equilibrio anterior no se considera validado para esta ampliación.

## Configuración y código

`DiveZoneDefinition.entryPosition` sitúa la señal de referencia. `requiredSafeDepth` y `requiredOxygen` siguen definiendo la preparación de los mecanismos; `unlockedDepth` y `unlockedOxygen` definen la recompensa. Se eliminaron `hasPhysicalGate` y `barrierWidth`, así como `DiveGate`, para que una configuración antigua no vuelva a crear paredes.

`WorldBuilder.Descents.BuildDescentGuides` construye únicamente señales y balizas sin colisión. `GameSession.NextWaypoint` orienta al santuario y `GameSession.Expedition.AscentWaypoint` a superficie. `DiverController.AdvanceVitals` conserva el coste por aire definido por `GameConfig.pressureOxygenMultiplier`. La fauna respeta el terreno y ya no evita alturas reservadas a losas inexistentes.

`DeepTrenchProfile` define entrada, anillos, cámaras, pilares y fondo. `WorldBuilder.DeepTrench` genera la malla 3D interior con MeshCollider y referencias luminosas; `WorldBuilder.CreateTerrainMesh` deja libre su abertura. `FloorHeight` se conserva para terreno/fauna superficiales: no representa techos o paredes interiores de la cavidad. `GameConfig.pressureBands` contiene los cinco umbrales editables; el editor exige profundidades y consumo crecientes. El rescate por salida del mundo queda 50 m bajo el nuevo fondo, no a los antiguos 120 m.

## Validación y límites

`DescentChecks`, incluido en `ProjectSetup.Validate`, construye solo las guías en editor: comprueba las dos transiciones, ausencia de colliders/interacciones de bloqueo y espacio bajo los puntos de cruce en la función de terreno. No entra en Play Mode ni construye la partida completa.

`DeepTrenchChecks` comprueba umbrales exactos, coste con/sin equipo, reserva, malla de 650 m, correspondencia de bordes y rayos sobre MeshCollider. Incluye la abertura en la malla costera real, repisa lateral y eje continuo desde el fondo. No sustituye un recorrido con CharacterController por todos los ramales. `DeepTrenchPreview.Export` produce la sección estática; `BuildAndExport` compila y exporta en una misma ejecución de editor.

`SmokeCheck.Descents` queda preparado para verificar en una futura ejecución autorizada el cruce por centro/laterales de ambas transiciones desde Nueva partida, el acceso a santuarios con equipo, ascenso y persistencia. Actualizar el diagnóstico no equivale a haberlo ejecutado. Revisión visual, colisiones del mundo completo y equilibrio de aire siguen pendientes de recorrido humano. Evidencia de compilación/editor en `Docs/PRUEBAS.md`.

Las comprobaciones antiguas sobre losas cerradas y escotillas pertenecen a versiones anteriores; no describen el diseño vigente.
