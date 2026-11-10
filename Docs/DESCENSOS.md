# Rutas entre capas

**Decisión vigente del usuario, 28 de octubre de 2026: el límite de la fosa es el aire disponible. Todas las capas tienen descenso y ascenso libres, incluido el abismo.** No se construyen losas, escotillas ni paredes para exigir una mejora antes de nadar hacia abajo.

**Ampliación según `fosa mapa.png`: el fondo pasa de 88 a 650 m.** La costa y las ruinas del naufragio se conservan. Cerca de las vetas de cristal hay una abertura de 8 × 8 m centrada en `(0, -88, 76)` hacia una cavidad 3D irregular: estrechamientos, cámaras laterales y dos pilares unidos a las paredes que dividen rutas y vuelven a reunirlas antes de la cámara inferior. Las profundidades y el equipo de esta primera implementación son provisionales, pendientes de juego humano.

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

El aviso de regreso usa el mayor valor entre 30 unidades y la estimación de un ascenso directo sin acelerar, con un 20 % de margen. Integra las franjas y considera aletas/equipo; no garantiza cubrir desvíos ni tiempo extrayendo minerales. En la cavidad profunda, el cuaderno recuerda volver al conducto central despejado `(0, z=76)` para ascender. No hay marcador proyectado de misión.

Vista estática de la geometría, sin Play Mode: `Docs/Mapa/fosa-seccion.png`. En esa imagen se retira el frente para ver el interior y se exagera el ancho ×1,6, indicado en el propio rótulo; no es una captura de partida.

## Minerales y equipo

| Recurso | Ubicación inicial aproximada | Preparación prevista |
|---|---|---|
| Cobre | 24 m, diez depósitos | Equipo inicial y pico submarino |
| Hierro | 52 m, diez depósitos | Traje reforzado: 64 m / 155 u de aire |
| Cristal abisal | 88 m, diez depósitos | Equipo presurizado: 100 m / 200 u |
| Cristal del fondo | 650 m, cinco depósitos | Traje abisal: 650 m / 1400 u |

Los tres equipos se fabrican con dinero y minerales en Don Lucho, en orden. La bombona suma 60 unidades adicionales. Los requisitos solo controlan fabricación: nunca impiden nadar. Valores y costes en `Docs/ECONOMIA_CRAFTING.md`.

Se retiraron santuarios, tableros y cofres de progresión. La geometría profunda no incluye todavía cinco biomas completos. Las balizas y señales no tienen colisión; las paredes naturales y pilares sí.

## Persistencia

El guardado v2 conserva mejoras fabricadas y regeneración de minerales. Lee partidas v1 convirtiendo las mejoras ganadas a sus equivalentes, sin exigir repetirlas. Conserva los IDs de mejora `huiros`, `naufragio` y `abismo`. Continuar devuelve al buzo a la playa. Ver `Docs/GUARDADO.md`.

## Configuración y código

`DiveZoneDefinition.entryPosition` sitúa la señal de referencia. `unlockedDepth` y `unlockedOxygen` definen el equipo de la receta asociada. Los antiguos requisitos de mecanismos se conservan como datos de compatibilidad, sin bloquear acceso ni activar puzles. Se eliminaron `hasPhysicalGate` y `barrierWidth`, así como `DiveGate`, para que una configuración antigua no vuelva a crear paredes.

`WorldBuilder.Descents.BuildDescentGuides` construye únicamente señales y balizas sin colisión. `GameSession.Expedition` guía extracción, fabricación y ascenso mediante texto contextual. `DiverController.AdvanceVitals` conserva el coste por aire definido por `GameConfig.pressureOxygenMultiplier`. La fauna respeta el terreno y ya no evita alturas reservadas a losas inexistentes.

`DeepTrenchProfile` define entrada, anillos, cámaras, pilares y fondo. `WorldBuilder.DeepTrench` genera la malla 3D interior con MeshCollider y referencias luminosas; `WorldBuilder.CreateTerrainMesh` deja libre su abertura. `FloorHeight` se conserva para terreno/fauna superficiales: no representa techos o paredes interiores de la cavidad. `GameConfig.pressureBands` contiene los cinco umbrales editables; el editor exige profundidades y consumo crecientes. El rescate por salida del mundo queda 50 m bajo el nuevo fondo, no a los antiguos 120 m.

## Validación y límites

`DescentChecks`, incluido en `ProjectSetup.Validate`, construye solo las guías en editor: comprueba las dos transiciones, ausencia de colliders/interacciones de bloqueo y espacio bajo los puntos de cruce en la función de terreno. No entra en Play Mode ni construye la partida completa.

`DeepTrenchChecks` comprueba umbrales exactos, coste con/sin equipo, reserva, malla de 650 m, correspondencia de bordes y rayos sobre MeshCollider. Incluye la abertura en la malla costera real, repisa lateral y eje continuo desde el fondo. No sustituye un recorrido con CharacterController por todos los ramales. `DeepTrenchPreview.Export` produce la sección estática; `BuildAndExport` compila y exporta en una misma ejecución de editor.

El diagnóstico actual `SmokeCheck.Start` está adaptado y compilado para el recorrido económico, pero no se ejecutó en esta entrega. Los antiguos auxiliares de puzles/descensos son históricos y no validan el flujo nuevo. Revisión visual, recorrido con CharacterController por todos los ramales y balance de aire siguen pendientes de juego humano. Evidencia de editor y compilación en `Docs/PRUEBAS.md`.

Las comprobaciones antiguas sobre losas cerradas y escotillas pertenecen a versiones anteriores; no describen el diseño vigente.
