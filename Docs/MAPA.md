# Mapa dinámico

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
