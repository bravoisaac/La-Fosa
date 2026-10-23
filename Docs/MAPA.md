# Mapa dinámico

## Entrega activa 3.4a: preparar medición del recorrido (23 de octubre)

- Objetivo: registrar una futura prueba humana con tiempos, intentos, capturas, compras, visitas y rescates; no ejecutar el juego ni simular resultados humanos.
- Entrada/patrón: Unity C#, parcial de `GameSession` y modelo local serializable; activación explícita mediante `--expedition-report`, sin dependencias nuevas.
- Conexiones: inicio/carga/menú/pausa → reloj del informe; `HarpoonGun`, `FishAgent`, `Economy`, puzle, recompensa y `DiverController` → sucesos confirmados.
- Estado: cada Nueva partida/Continuar abre un segmento independiente con progreso inicial; separar tiempo total, tiempo sin pausa y tiempo de movimiento. No contar restauraciones como rescates.
- Archivos: nuevo modelo en Diagnostics, parcial de sesión y validación de editor; puntos de integración citados y documentación del plan/prueba.
- Conservar: formato de guardado v1, HUD, precios, alcance, manos y mecánicas. Riesgo medio: clasificación de sucesos y ciclo de vida; el informe debe poder fallar sin afectar la partida.
- Validación prevista: casos sintéticos del modelo/escritura en editor y compilación Windows; integración jugable y medición humana pendientes.

## Entrega activa 3.3a: legibilidad y distribución del HUD (23 de octubre)

- Objetivo: corregir desde el código solapamientos, estiramiento y avisos inconsistentes, sin abrir el juego.
- Framework/entrada: Unity IMGUI; `DiveHUD.OnGUI` → escala uniforme y paneles → `DiveHUD.Shop` para tienda.
- Estado conectado: guía de `GameSession.Expedition`, destino proyectado, interacción/pez observado, profundidad/oxígeno y menús; no se cambia el progreso.
- Archivos revisados/modificables: `DiveHUD.cs`, `DiveHUD.Shop.cs`, plan, mapa, pruebas, guía y README.
- Patrón: conservar coordenadas virtuales 1600 × 900, paleta marina, parciales y controles actuales; no migrar sistema de UI ni añadir paquetes.
- Cambios: margen de marcadores separado de interacción/avisos, fondo para textos sobre el mundo, cuaderno con altura según texto, escala sin deformar, umbral visual de aire coherente a 30 s.
- Conservar: manos, mecánicas, economía, guardado y rescate. Riesgo bajo/medio: distribución y coordenadas de botones en formatos distintos.
- Validación: compilación Windows y comprobaciones existentes del editor; revisión visual en partida expresamente pendiente por la indicación de no usar el juego.
- Resultado: `LA_FOSA_TESTS_OK` y `LA_FOSA_BUILD_OK` en `unity-hud-layout-build.log`; cambios de distribución revisados en el código y `git diff --check` sin problemas. 3.3a terminada; 3.3b visual pendiente. Ejecutable actualizado sin lanzarlo.

## Mejora actual: manos de primera persona

Actualización del 23 de octubre: revisar anatomía y cierre sobre el modelo ya existente. `HandsPreview` exporta vistas estáticas en editor, sin Play Mode. Detectado y corregido GUID inválido de `WorldBuilder.Hands.cs.meta`, que impedía compilar `BuildFishingTools`. Mantener pendientes las comprobaciones dentro del juego.

- Resultado visual: dedos extendidos con separación y longitudes distintas, uñas discretas, pulgar con oposición en tres ejes y muñecas que acompañan el cierre. `DiverHands` guarda/restaura posturas locales sin acumular desplazamientos.
- Animación: `HarpoonGun` conserva la captura inmediata y recarga de 0,8 s; solo el gesto manual pasa a 0,48 s, abre al alcanzar y cierra al recoger. Cambiar herramienta cancela el gesto anterior conservando la recarga.
- Artefactos: `HandsPreview` genera `Docs/Manos/manos-0.png`, `manos-50.png` y `manos-100.png` con el FOV de primera persona (76°). Las capturas muestran el modelo aislado, sin HUD ni iluminación del mundo.
- Verificación final: compilación Windows y validación editorial correctas (`unity-hands-build.log`); vistas estáticas inspeccionadas. No se ejecutó el juego ni las pruebas de runtime en esta entrega.

- Objetivo: reemplazar las manos cúbicas por anatomía low poly legible y un gesto corto de captura, sin ejecutar el juego por indicación del usuario.
- Entrada: `WorldBuilder.BuildFishingTools` → nuevo parcial de manos → modelo 0; `HarpoonGun` conduce el gesto con el tiempo de alcance existente.
- Estado/componentes: muñecas, palmas, pulgar y cuatro dedos por mano; articulaciones locales y materiales existentes; no se añaden colisiones a las manos.
- Archivos a modificar: `WorldBuilder.Shop`, nuevo `WorldBuilder.Hands`, nuevo `DiverHands`, `HarpoonGun`; mapa, plan, README y resultados.
- Conservar: herramientas 1–3, alcance manual de 1,6 m, recarga de 0,8 s, pesca, inventario y persistencia. Riesgo bajo/medio: orientación de mallas, oclusión y postura de los dedos.
- Validación: compilación de Unity sin abrir el juego; revisión de geometría en editor si es posible. La revisión jugable queda pendiente.

## Entrega 3.3 interrumpida: revisión visual del HUD

El usuario pidió dejar de usar el juego y mejorar las manos. La inspección del HUD quedó incompleta y no se marca como terminada.

- Objetivo: comprobar cuaderno, avisos, marcadores y menús renderizados sin tocar la partida personal.
- Framework/entrada: Unity IMGUI, `DiveHUD.OnGUI` y `DiveHUD.Shop`; datos desde `GameSession.Expedition`, inventario, economía y controlador.
- Componentes/estado: objetivo y destino, interacción observada, aire/profundidad, pausa/tienda/inventario; colores y coordenadas virtuales 1600 × 900 existentes.
- Plataformas: Windows en ventana; revisar 1600 × 900 y 1280 × 720, y conservar proporciones al cambiar el formato de pantalla.
- Archivos candidatos/modificables: HUD y diagnóstico optativo de previsualización; mapa, plan, pruebas y guía de primera expedición.
- Patrón: parciales y herramientas del diagnóstico existente, guardado aislado mediante `--smoke-test`; no añadir dependencias.
- Conservar: precios, vitales, progresión, fauna y guardado v1. Riesgo bajo/medio: solapamientos, texto recortado o coordenadas de clic desajustadas; comprobar capturas y controles reales.

## Entrega activa: validación del primer descenso libre (19 de octubre de 2026)

- Objetivo: cerrar la verificación pendiente del cambio existente y mantener un punto de reanudación breve en `PLAN_MEJORAS.md`.
- Framework/entrada: Unity 6.3 LTS; `SmokeCheck.Start` → pruebas de expedición, rutas y vitales.
- Flujo conectado: `hasPhysicalGate` → mundo/colisiones → `NextWaypoint` y guía; profundidad → `AdvanceVitals` → oxígeno/salud/rescate.
- Estado: progreso persistente v1, inventario y recompensas existentes; la ausencia de losa no desbloquea el segundo puzle.
- Candidatos revisados: sesión, guía, controlador, generador de compuertas, assets de zonas, pruebas y compilador editorial.
- Archivos modificados: diagnósticos `SmokeCheck`, `SmokeCheck.Gates`, `SmokeCheck.Expedition` y `SmokeCheck.Puzzles`; plan, mapa y documentación de rutas/pruebas/primera expedición/README.
- Patrones: pruebas optativas con guardado aislado y movimiento mediante CharacterController; sin dependencias nuevas.
- Conservar: cambios locales previos, precios, arte, guardado personal, escena, reglas de progresión y equilibrio hasta la prueba humana.
- Riesgo medio: confundir acceso físico con progreso o dar por válida una compilación antigua. Validar ejecutable recompilado y registrar resultado real.
- Resultado: compilación/editor correctos, 458 comprobaciones de integración y 12 entre procesos aprobadas. El diagnóstico distingue en curso/fallo/éxito por modo. Paso 3.2 marcado en el plan; pendiente inmediato 3.3 (HUD), luego recorrido y equilibrio humanos. Detalles en `PRUEBAS.md`.

## Primer descenso libre y peligro por consumo de aire

- Objetivo solicitado: retirar toda la barrera entre huiros y naufragio; sustituir el bloqueo físico por consumo de oxígeno rápido fuera de la profundidad segura.
- Entrada/flujo: `DiveZoneDefinition.hasPhysicalGate` → generación de mundo y fauna → marcadores; `DiverController` → consumo de aire → daño por falta de aire → rescate existente.
- Estado: equipo y progreso guardados sin cambiar el formato v1. El abismo conserva su compuerta y los puzles mantienen sus requisitos de progreso.
- Archivos candidatos/modificados: configuración de zona y expedición, generador/editor, controlador, sesión/guía/HUD, diagnósticos de rutas/vitales y documentación.
- Patrón: datos editables, componentes y pruebas existentes; consumo ×8 al sobrepasar el límite, sin daño directo adicional de presión.
- Conservar: shader, precios, mejoras, pesca y rescate con inventario. Riesgo medio: límites de profundidad, retorno y coherencia de indicaciones.
- Validación prevista: cruzar libremente el antiguo centro y laterales sin tesoro, compuerta profunda intacta, consumo dentro/fuera del límite, recuperación al ascender, agotamiento/rescate y guardado.

## Etapa 3: primera expedición guiada

- Objetivo: orientar la primera pesca, venta, compra del tridente, sello, tesoro y regreso; entrega acotada de la etapa 3.
- Framework/entrada: Unity 6.3 LTS, `GameSession` → guía contextual → `DiveHUD` y marcador.
- Componentes conectados: `Economy`, inventario, controlador, puzles y cofres; catálogo como fuente del precio del tridente.
- Estado: objetivos derivados de saldo, pesca, compras y sellos guardados. El regreso tras el primer tesoro es una indicación de la sesión; cargar ya devuelve a la playa.
- Patrones: parciales pequeños, generación procedural, IMGUI y pruebas del ejecutable existentes. Sin dependencias nuevas.
- Candidatos revisados: sesión/persistencia, HUD/tienda, mundo/cabaña/fauna, herramientas, controlador, inventario, mecanismos y diagnóstico.
- Archivos modificados: nuevos `GameSession.Expedition.cs` y `SmokeCheck.Expedition.cs`; sesión/persistencia, HUD/tienda, señalización costera en `WorldBuilder`, entrada de pruebas, opción de compilación aislada en `ProjectSetup` y documentación.
- Conservar: shader del agua, fauna, precios, alcances, oxígeno, reglas de rescate y formato de guardado v1.
- Riesgo medio: orientar hacia un lugar incorrecto o repetir objetivos al cargar. Verificar transiciones, inventario lleno, avance fuera de orden, persistencia y capturas de la interfaz.
- Pendiente: recorrido humano para decidir ajustes de ritmo, precios y aire; no declarar cerrada la etapa sin esa prueba.
- Resultado: compilación aislada y validación del editor correctas; 448 comprobaciones de integración aprobadas (26 nuevas). La captura de pantalla de una ventana oculta resultó negra; revisión visual del HUD pendiente. Detalles en `Docs/PRIMERA_EXPEDICION.md` y `Docs/PRUEBAS.md`.

## Bruma y visibilidad submarina

- Objetivo: desde fuera, mar con tono y turbidez uniformes, sin mancha oscura sobre la fosa; solo la orilla conserva mayor transparencia. Se mantiene la atmósfera submarina existente.
- Framework/entrada: Unity Built-in; `DiverController.UpdateAtmosphere` actualiza niebla exponencial cuadrática, luz y color según profundidad y posición. `Water.shader` suaviza el fondo visto a través de la superficie, sin desenfocar tierra ni HUD.
- Datos: `GameConfig.coastalFogDensity` / `shallowFogDensity` / `deepFogDensity`, editables en `Expedicion.asset`, con valores 0,035 / 0,065 / 0,115. La fosa interpola por profundidad hasta 95 m; la transición radial entre exterior e interior va de 31 a 23 m alrededor del centro del terreno `(0,75)`.
- Mundo conectado: `WorldBuilder` calcula la orilla a partir de la pendiente del terreno y el nivel del agua. Desde arriba, la opacidad pasa de 0,65 junto a la orilla a 0,85 tras 20 m, con tono uniforme, suavizado de cinco muestras y un único GrabPass nombrado. El ajuste intermedio aumenta el desenfoque junto a la orilla (3,5 píxeles a 900p) y lo reduce mar adentro (4,5 píxeles), con una diferencia de turbidez más suave. La máscara radial de la fosa solo cambia el tono visto desde abajo; la superficie submarina mantiene opacidad 0,3 sin desenfoque.
- Configuración conectada: `GraphicsSettings.asset` conserva explícitamente las variantes Exp2, necesarias porque la escena procedural activa la niebla durante el juego y el modo automático puede eliminarlas al compilar.
- Archivos modificados: controlador, configuración, asset de expedición, `WorldBuilder`, `Water.shader`, configuración gráfica, diagnóstico visual y documentación. Conservados: atmósfera del aire, alcance de herramientas, reglas, progreso y guardado.
- Verificación visual: `--smoke-test --water-preview` captura panorama de playa, orilla, costa/fosa desde arriba y sumergido, incluida una pareja a la misma profundidad. No toca el guardado personal. Compilación y revisión de puzles/compuertas mediante el recorrido de integración existente; la dificultad de pesca requiere apreciación jugando.
- Riesgo: coste gráfico del GrabPass y menor visibilidad a distancia; no se midió rendimiento en otras GPU. Sin dependencias nuevas ni cambios de formato de partida.

## Etapa 2: requisitos y compuertas de descenso

- Objetivo: impedir el acceso físico a capas nuevas hasta resolver el sello anterior y recoger su mejora, con requisitos visibles y regreso viable.
- Framework: Unity 6.3 LTS; generación procedural, componentes pequeños, ScriptableObjects e IMGUI existentes.
- Entrada: `GameBootstrap` → `WorldBuilder.Gates` → dos `DiveGate`, conectadas al mecanismo de la capa anterior y al evento `GameSession.ProgressChanged`.
- Datos: `DiveZoneDefinition.entryPosition`, `requiredSafeDepth`, `requiredOxygen`, `barrierWidth` y `returnHint`. Configuración de las tres capas en sus assets y valores iniciales en `ProjectSetup`.
- Flujo: abrir sello → recoger tesoro → aplicar mejora permanente → actualizar acceso y colisiones. Continuar/Nueva partida actualizan las compuertas a partir del progreso existente, sin otra copia de su estado en el archivo de guardado.
- Pantallas/interacciones: `DiveHUD` apunta primero a la escotilla y después al santuario. `DiverController` muestra requisitos al chocar; compuerta, piedras, palanca y cofre consultan las condiciones de la zona. Rótulos distintos desde arriba y desde abajo.
- Mundo: losas de cuatro bloques encajadas en el terreno, abertura central de 8 m y panel móvil. `WorldBuilder.Fish` evita generar fauna dentro de las nuevas losas; conserva las 49 especies y 132 individuos.
- Archivos nuevos: `World/DiveGate.cs`, `World/WorldBuilder.Gates.cs`, `Diagnostics/SmokeCheck.Gates.cs`. Cambios en los componentes/datos enumerados, pruebas existentes y documentación.
- Conservado: precios, orden manos → tridente → arpón, reglas de puzles, rescate, inventario y formato de guardado v1. No se implementan biomas ni tutoriales de etapas posteriores.
- Riesgo medio: accesos físicos y retorno. Pruebas con CharacterController, muestreo de cobertura, perímetros enterrados, ida/vuelta, persistencia y objetos descartados; revisión de capturas. Pendiente recorrido humano y equilibrio de tiempos/oxígeno.

Descripción del recorrido y configuración: `Docs/DESCENSOS.md`.

## Etapa 1: progreso persistente

- Objetivo: cerrar y continuar una expedición coherente, sin repetir compras ni recompensas.
- Framework: Unity 6.3 LTS; componentes actuales, ScriptableObjects e IMGUI, sin dependencias nuevas.
- Entrada: `GameBootstrap.Build` genera el mundo y registra `GameSession.InitializePersistence`; `DiveHUD` ofrece Nueva partida, Continuar y Guardar y volver al menú.
- Flujo: componentes marcan cambios → `GameSession.Persistence` captura una transacción completa → `SaveStore` valida/escribe principal y respaldo → Continuar valida antes de restaurar y devuelve el buzo a la playa.
- Estado conectado: `Inventory`, `Economy`, `DiverController`, `HarpoonGun`, `StoneCircuitPuzzle`, `TreasureChest`, `Pickup`, `SurfaceVehicle`; IDs estables en objetos, zonas, compras y conchas.
- Archivos revisados/modificados: esos componentes, generadores de conchas/arranque, tres assets de zonas, `ProjectSetup`, HUD, pruebas y documentación. Nuevos: `SaveStore`, parciales `GameSession.Persistence`/`SaveValidation` y `SmokeCheck.Persistence`.
- Conservado: inicio con manos, requisito del tridente, precios, fauna, movimiento, geometría de santuarios y reglas de circuitos. No se amplían etapas 2–10.
- Riesgo medio: escritura, archivos incompatibles y consistencia de recompensas. Comprobaciones de respaldo, corrupción y reinicio en procesos separados; pruebas aisladas de la partida personal.
- Límites: una ranura local; el buzo siempre reaparece en la playa; sin migraciones de contenido futuro. Detalles en `Docs/GUARDADO.md`.

## IA de peces y progresión de pesca

- Objetivo: detectar al buzo y escapar; peces intermareales lentos para manos/tridente y peces del mar más rápidos que el nado con aletas.
- Entrada: `GameBootstrap` vincula el buzo y perfiles de comportamiento a `FishAgent` después de generar el mundo.
- Datos: dos `FishBehaviorProfile` en `GameConfig`; selección por `FishSpeciesDefinition.habitatBand` (0 = orilla).
- Estado: patrulla, huida con memoria breve, regreso gradual al hábitat; respetar pausa, límites de profundidad y obstáculos. Conservar captura atómica y reposición.
- Cambios previstos: IA, configuración/editor, descripción de pesca y pruebas de persecución real, alcance del arpón, bloqueo visual, pausa y límites del hábitat.
- Riesgo medio: una fuga no debe atravesar roca, salir del agua o volver a la posición inicial durante la persecución. No cambiar precios, compras, fauna ni puzles.
- Implementado: `PecesOrilla.asset` (huida a 1,8 m/s) y `PecesMar.asset` (16 m/s); visión a 3,5/12 m, memoria de alerta e histéresis. `FishAgent.Navigation` valida profundidad, terreno y obstáculos mediante consultas físicas. `SmokeCheck.FishAI` prueba persecuciones a 6 y 12,15 m/s, captura manual/arpón, alcance del tridente, paredes, pausa y regreso gradual.

## Mejora de puzles: mosaicos de piedras giratorias

- Sustituir la repetición de OLA/LUNA/SOL por canales tallados en piedras: 6, 9 y 12 piezas. Girar con E, conectar toda la corriente y accionar una palanca. Reinicio opcional por tablero.
- Datos: `StonePuzzleDefinition` por zona. Lógica: `StoneCircuitPuzzle`; interacción: `RuneStone` y `StonePuzzleLever`; aspecto: `WorldBuilder.Puzzles` y `StonePuzzleView`.
- Conservar pesca, tienda, fauna, progreso por niveles y recompensa única. Los campos de secuencia quedan como datos heredados fuera de los nuevos tableros.
- Validar rutas, giros, conexiones, mezcla, reinicio, bloqueo entre niveles, interacción física y cofres con mochila llena.
- Resultado: tres circuitos jugables (6/9/12 piedras), compilación Windows correcta y 342 comprobaciones aprobadas. Las 27 piedras y tres palancas se comprobaron por raycast; revisadas capturas de los tableros bajo el agua. Fichas y guía de ampliación en `Docs/PUZLES.md`.

## Fauna del documento aportado

- Objetivo: 49 modelos low poly individuales, reutilizables e integrados en pesca, inventario y compraventa.
- Fuente de alcance: documento DOCX del usuario, leído como lista de especies y rangos aproximados; no contiene imágenes.
- Datos: `FishSpeciesDefinition` vincula especie, taxón, banda real, profundidad jugable, modelo e ítem vendible.
- Generación: recetas editoriales → mallas facetadas distintas → assets Mesh, material y prefab por especie; galería para inspección.
- Mundo: `WorldBuilder.Fish.cs` reemplaza el jurel genérico por 132 individuos en seis bandas de profundidad comprimidas a la fosa existente.
- Sistemas conectados: `FishAgent`, `WorldBuilder`, inventario, `Economy`, HUD de identificación y tienda multiespecie.
- Preservar: manos → tridente → arpón, cabaña, vehículos, puzles, tesoros y precios del equipo.
- Riesgo: medio; comprobar las 49 referencias, geometrías, profundidad, captura y venta de especies distintas, sin vender conchas/tesoros.
- Resultado: 49 prefabs, mallas y materiales guardados; `FishCatalogBuilder` valida y conserva el catálogo ampliado; `SmokeCheck.Fish` verifica hábitats y transacciones y renderiza 49 vistas. Compilación correcta y 278 comprobaciones de integración aprobadas. `Docs/FAUNA.md` explica reutilización y ampliación.

## Corrección de progresión: manos → tridente → arpón

- Entrada: `HarpoonGun` y modelos de `WorldBuilder.Shop`; inicio sin herramientas compradas.
- Alcance: captura manual a corta distancia, tridente como primera herramienta, bloqueo de arpón/red hasta comprar tridente, catálogo y controles coherentes.
- Archivos afectados: pesca, economía, tienda, generador, catálogo existente, pruebas y documentación.
- Patrones conservados: equipo de sesión separado de inventario, catálogo editable y detección física que respeta obstáculos.
- Riesgo: medio; verificar manos sin alcance de arpón, captura cercana, requisitos con saldo suficiente y todos los cambios de herramienta.

## Ampliación: cabaña, tienda y equipo

- Objetivo: convertir la caseta en cabaña de caleta chilena; vender peces y comprar siete mejoras o herramientas utilizables.
- Entrada: `WorldBuilder.BuildCoast` → `ShopCounter` → `GameSession` / `DiveHUD`.
- Datos: nuevo `ShopCatalog` editable, saldo y equipo de sesión en `Economy`; inventario retira únicamente peces vendidos.
- Sistemas afectados: pesca, oxígeno, velocidad de nado, modelos de herramientas y vehículos de superficie.
- Patrón conservado: componentes pequeños, generación procedural y ScriptableObjects; sin paquetes nuevos.
- Archivos candidatos revisados: WorldBuilder, GameSession, DiverController, HarpoonGun, FishAgent, Inventory, DiveHUD, ProjectSetup y SmokeCheck.
- Cambios: catálogo y compras, cabaña, herramientas alternativas, vehículos, HUD, documentación y pruebas de transacciones.
- Evitar: sobrescribir objetos, secuencias y ajustes existentes de la expedición; conservar la escena.
- Riesgo: medio; comprobar saldo insuficiente, duplicados, venta exacta, acumulación de mejoras y embarque/desembarque.

### Conexiones finales de la tienda

`ShopCounter` abre `GameSession.ShopOpen` → `DiveHUD.Shop` consulta `Tienda.asset` → `Economy` vende mediante `Inventory.TryRemove` y registra compras únicas.

- `DiverController` aplica aletas y suma la bombona a la capacidad base de cada tesoro.
- `HarpoonGun` selecciona cuatro modelos: manos iniciales, tridente, arpón y red. La economía exige tridente antes del arpón o la red.
- `FishAgent` oculta una captura y la repone después de 25 s; impide recompensas duplicadas antes de reaparecer.
- `SurfaceVehicle` comparte navegación para bote/moto, con propiedad, entrega, colisiones, pausa y salida segura.
- `WorldBuilder.Shop` construye cabaña, mostrador, vendedor, herramientas y vehículos; `ProjectSetup` añade el catálogo si falta sin sustituir configuraciones ya existentes.
- `SmokeCheck` amplía la integración para comprobar venta, saldo insuficiente, compras duplicadas, equipo, captura con herramientas, reposición, navegación y rescate.

- Objetivo: prototipo ampliable de exploración submarina low poly, en primera persona, inspirado en la costa chilena.
- Framework detectado: carpeta vacía; Unity 6.3 LTS (6000.3.26f1) instalado.
- Punto de entrada: `Assets/LaFosa/Scenes/LaFosa.unity` → `GameBootstrap`.
- Flujo esperado: playa → bucear → resolver secuencias → abrir tesoros → mejorar equipo → descender; pesca opcional con arpón.
- Componentes: mundo procedural, controlador de buzo, interacción, HUD, peces, puzles y cofres.
- Modelos: `ItemDefinition`, `DiveZoneDefinition`, `GameConfig` (ScriptableObjects); inventario con capacidad y pilas.
- Estado: sesión local; oxígeno, salud, profundidad, equipo, inventario y progreso de puzles.
- Patrones existentes: ninguno; componentes separados y datos editables, sin assets externos.
- Archivos revisados: directorio inicial vacío e instalación local de Unity.
- Archivos a modificar: Assets/LaFosa, Packages, ProjectSettings y documentación.
- Archivos evitados: instalaciones de Unity y otros proyectos.
- Riesgo: medio; nueva base solicitada, generación reproducible y comprobación mediante Unity en batch.

## Estructura implementada

`LaFosa.unity` → `GameBootstrap` → `WorldBuilder` + `GameSession` + buzo + `DiveHUD`.

- `DiverController` controla caminar, nadar, cámara, oxígeno, salud, presión, luz e interacción por raycast.
- `HarpoonGun` detecta impactos y `FishAgent` entrega la captura mediante `Inventory.TryAdd`.
- `RuneStone` gira piezas de `StoneCircuitPuzzle`; `StonePuzzleLever` valida o reinicia; `TreasureChest` entrega el objeto y `GameSession` aplica mejoras y progreso.
- `GameConfig` referencia objetos y zonas como ScriptableObjects. Las zonas se editan sin modificar la lógica de puzles.
- `ProjectSetup` prepara la escena, valida reglas y compila para Windows.
- `SmokeCheck`, activado únicamente con `--smoke-test`, verifica el bucle en el ejecutable y exporta capturas de cámara.
- Shaders propios para terreno facetado, agua y rótulos con oclusión por geometría. Renderizador integrado.

La escena, sus referencias y todos los archivos `.meta` están generados por Unity. `Library`, logs y compilaciones se excluyen de Git.
