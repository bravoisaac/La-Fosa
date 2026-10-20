# La Fosa

Prototipo original en **Unity 6.3 LTS (6000.3.26f1)**. Exploración low poly en primera persona, ambientada en Caleta Bruma, una playa ficticia de Chile. La profundidad está comprimida para jugar: no es una simulación realista de buceo ni de geografía.

Dirección del proyecto: [contexto de diseño para Codex](Docs/CONTEXTO_JUEGO.md) y [plan de mejora por etapas](Docs/PLAN_MEJORAS.md). Para continuar el desarrollo, empezar por **Reanudar aquí**, al inicio del plan: contiene objetivo, pasos marcados, evidencia y pendiente inmediato. Estos documentos distinguen la visión futura de las funciones implementadas que se describen aquí.

## Jugar

1. En Unity Hub, selecciona **Add / Add project from disk** y esta carpeta.
2. Abre `Assets/LaFosa/Scenes/LaFosa.unity`.
3. Pulsa **Play** y **Nueva partida** o **Continuar**. Reemplazar una partida existente requiere confirmación.

Si aún no existe la escena, usa **La Fosa → Preparar escena inicial**. El mundo se construye al pulsar Play; es normal que la escena en edición contenga solamente el objeto de inicio.

Si está disponible la compilación, abre `Builds/Windows/La Fosa.exe` directamente.

La entrega de guía de la etapa 3 se compiló por separado en `Builds/FirstExpedition/La Fosa.exe`. Cierra el juego anterior antes de usarla: ambos ejecutables comparten la misma partida local.

## Controles

| Acción | Control |
|---|---|
| Caminar / nadar | WASD |
| Mirar | Ratón |
| Saltar / ascender | Espacio |
| Descender | Ctrl izquierdo o C |
| Acelerar (consume más aire al bucear) | Shift izquierdo |
| Interactuar / activar sello / recoger | E |
| Atrapar con las manos / usar herramienta | Clic izquierdo |
| Manos / tridente / arpón / red | 1 / 2 / 3 / 4 |
| Abrir tienda / subir o bajar de vehículo | E |
| Conducir bote o moto | W/S avanzar-retroceder, A/D girar |
| Linterna | F |
| Inventario | Tab |
| Pausa | Esc |
| Rescate a la playa | R |

## Bucle de juego

El cuaderno guía la **primera expedición**: pesca lenta en la orilla → vender → comprar tridente → resolver sello → recoger tesoro → regresar. Calcula cuánto falta reunir según el valor real de las especies y recuerda los controles necesarios. El marcador sigue el objetivo actual, incluso si queda detrás de ti. Aire bajo o presión excesiva priorizan el ascenso. Al cargar se reconstruye el objetivo a partir del progreso. [Recorrido, compatibilidad y revisión pendiente](Docs/PRIMERA_EXPEDICION.md).

Recoge conchas en la playa, sigue las boyas hasta el punto de inmersión y explora. Empiezas sin armas: acércate a un pez bajo el agua, apúntalo y haz clic para atraparlo con las manos (alcance de 1,6 m y 0,8 s entre intentos). Las manos no disparan y no atraviesan obstáculos. Vende tu pesca para obtener el tridente y, más adelante, el arpón.

El mar incluye **49 especies o grupos del documento aportado, con 49 modelos 3D propios y 132 peces**, repartidos desde la orilla hasta el abismo. Cada especie tiene su forma, colores, precio y franja de profundidad. Apunta a un pez cercano para identificarlo. Los modelos reutilizables están en `Assets/LaFosa/Models/Fish/`; consulta [la guía de fauna](Docs/FAUNA.md) y [la lámina de modelos](Docs/peces-catalogo.jpg).

Los peces huyen al ver al buzo. **Los de la orilla escapan despacio (1,8 m/s)**: puedes alcanzarlos con las manos y mejorar después al tridente. **Los del mar escapan a 16 m/s**, por encima del nado con aletas: usa el arpón de 40 m para capturarlos a distancia. Al perderte de vista se tranquilizan y regresan nadando. La detección respeta los obstáculos y la huida permanece dentro del hábitat.

Hay tres santuarios: Bosque de huiros (~24 m), Ecos del naufragio (~52 m) y Corazón del abismo (~88 m). Cada uno tiene un mosaico de piedras giratorias: **6, 9 y 12 piezas**, respectivamente. Apunta a una piedra y pulsa **E** para girarla 90°. Une los canales desde **ENTRADA** hasta **SELLO** pasando por todas las piedras y sin extremos abiertos. La corriente ilumina el recorrido conectado. Cuando esté completo, acciona la palanca **ABRIR SELLO** y abre el cofre con E.

Los giros equivocados no borran tus avances. La piedra **REINICIAR** restaura solo ese tablero si quieres empezar de nuevo. Puedes leer la pista mirando el cofre cerrado y pulsando E. Los giros y los sellos abiertos se guardan también al cerrar el juego. Consulta [los puzles de piedras](Docs/PUZLES.md).

Los tesoros mejoran automáticamente la reserva de aire y la profundidad segura. Abrir un cofre repone oxígeno y salud. Superar la profundidad segura consume aire ×8 (×12 al acelerar), sin daño directo de presión mientras quede oxígeno. La falta de aire causa daño. La superficie repone aire y salud; si agotas la salud, regresas a la playa conservando tus objetos. R permite regresar en cualquier momento.

**El descenso al naufragio es libre; el abismo conserva su compuerta a 64 m.** Sin la mejora de huiros puedes explorar hacia abajo, pero gastarás aire rápidamente y no podrás saltarte el orden de los mecanismos. La losa del abismo deja una escotilla central: abre el sello del naufragio y recoge su tesoro para liberar el paso. Comprar la bombona no resuelve el mecanismo. E sobre la compuerta, o chocar con ella, muestra lo que falta. El marcador guía a esa escotilla antes del santuario; vuelve por la misma abertura iluminada. El estado persiste al cargar y aunque descartes el objeto del tesoro. Consulta [rutas y requisitos de descenso](Docs/DESCENSOS.md).

La mochila tiene 12 espacios y pilas limitadas. Un inventario lleno no consume tesoros ni capturas. Puedes descartar pilas desde Tab. Las mejoras adquiridas permanecen aunque descartes su tesoro. Inventario y pausa detienen la simulación.

**El progreso se guarda en el equipo**, con versión de formato y copia de respaldo. Se conservan dinero, inventario, compras, tesoros, piedras, zonas descubiertas, conchas recogidas y propiedad/posición de vehículos. **Continuar** devuelve al buzo a la playa con aire y salud recuperados. En pausa, **Guardar y volver al menú** permite comprobar el guardado antes de salir. Un error de escritura se muestra en pantalla; un archivo principal inválido permite recuperar el respaldo. Consulta [guardado y recuperación](Docs/GUARDADO.md).

No incluye multijugador, sonido ni animación de brazos. Es una base jugable para ampliar; los objetos y paisajes se generan con geometría simple, sin recursos externos.

## Cabaña y tienda de Don Lucho

La cabaña de madera está a la izquierda del punto de inicio, con techo de zinc a dos aguas, corredor, mostrador y un pescador que atiende. Acércate al mostrador, mira hacia él y pulsa **E**. La tienda pausa el mundo; **Esc** permite salir.

Empiezas **sin dinero, sin arpón y pescando con las manos**. Los peces de la orilla y el jurel se venden por **$30 de moneda del juego**; otras especies tienen precios propios. Dos capturas de la orilla pagan el **tridente de $60**, la primera herramienta, que se equipa al comprarlo. Solo entonces puedes comprar el arpón o la red. El botón de venta suma los precios de toda tu pesca y deja intactos conchas y tesoros. Los peces reaparecen tras 25 segundos de simulación para mantener el ciclo de pesca y comercio.

| Compra | Precio | Función |
|---|---:|---|
| Tridente | $60 | Primera herramienta; tecla 2, alcance de 4 m y recarga de 0,45 s |
| Arpón | $150 | Requiere tridente; tecla 3, alcance de 40 m y recarga de 0,65 s |
| Bombona de O₂ ampliada | $90 | +60 segundos de reserva, acumulables con cada tesoro |
| Aletas | $75 | +35% de velocidad de nado, también al acelerar |
| Red de pesca | $120 | Requiere tridente; tecla 4, hasta 5 peces en un radio de 4 m, recarga de 5 s |
| Bote de caleta | $240 | Embarcación de superficie a 12 m/s |
| Moto de agua | $360 | Embarcación de superficie a 22 m/s |

Las mejoras se compran una sola vez y no consumen espacios de mochila. Puedes volver a las manos con la tecla 1. La bombona amplía permanentemente la capacidad de la partida; no es un consumible. La red es reutilizable, funciona buceando o desde una embarcación y respeta los obstáculos. Las manos, el arpón y el tridente requieren estar bajo el agua. Cambiar de herramienta no elimina la recarga pendiente.

El bote comprado aparece a la izquierda del muelle y la moto a la derecha. Acércate, mira la embarcación y pulsa **E**. Usa **W/S** para avanzar/retroceder, **A/D** para girar y el ratón para mirar. **E** te deja nadando a un costado; el vehículo queda donde lo abandonaste. **R** rescata al buzo y conserva la propiedad. La conducción usa colisiones y límites de costa; no incluye una simulación de oleaje o combustible.

## Ampliar

- `Assets/LaFosa/Data/Expedicion.asset`: semilla, punto de aparición, oxígeno inicial, profundidad segura, capacidad y lista ordenada de zonas.
- Desde fuera del agua, el mar tiene un tono y una turbidez uniformes; solo los primeros 20 m junto a la orilla son progresivamente más transparentes, sin una mancha oscura sobre la fosa. Bajo el agua, `coastalFogDensity` (0,035), `shallowFogDensity` (0,065) y `deepFogDensity` (0,115) en `Expedicion.asset` ajustan exterior, fosa somera y fosa profunda, conservando la transición gradual entre zonas. La bruma del aire sigue en 0,0035 y Graphics Settings conserva explícitamente el modo Exp2.
- `Assets/LaFosa/Data/Tienda.asset`: precios del equipo, bono de oxígeno y multiplicador de aletas. Los precios de venta están en cada `ItemDefinition.salePrice` (el campo antiguo `fishValue` se conserva como referencia del jurel inicial). `ShopCatalog`, `Economy` y `GearKind` permiten ampliar el equipo; añade también su efecto en el componente correspondiente.
- `WorldBuilder.Shop.cs`: cabaña, vendedor, modelos de herramientas y embarcaciones. `SurfaceVehicle` concentra navegación, embarque, colisiones y desembarque. `DiveHUD.Shop.cs` contiene la tienda con lista desplazable.
- **Create → La Fosa → Objeto**: crea recursos, peces y tesoros con identificador estable, nombre, descripción, color y tamaño de pila.
- **Create → La Fosa → Zona de buceo**: crea una zona con ID estable único, posición, profundidad, tesoro y mejoras. Asígnale un **Puzle de piedras** y añádela a `Expedicion.zones` en orden de profundidad. Se genera su santuario automáticamente. El campo antiguo `sequence` ya no controla los nuevos mecanismos. Cambiar IDs o estructura del contenido exige planificar una migración del guardado.
- `GameSession.Persistence`, `GameSession.SaveValidation` y `SaveStore`: captura/restauración de la partida, validación previa y escritura con respaldo. Los cambios de inventario/puzles se guardan al final del frame; compras, ventas y tesoros se guardan tras completar la transacción.
- `DiveGate` y `WorldBuilder.Gates`: compuertas, losas y señalización de retorno solo donde `hasPhysicalGate` esté activo. Cada zona define entrada, requisitos de profundidad/aire, ancho de barrera y pista de regreso; validar de nuevo el perímetro si cambia el terreno. `GameConfig.pressureOxygenMultiplier` ajusta el consumo fuera de la profundidad segura.
- `WorldBuilder.FloorHeight`: amplía el fondo si añades niveles por debajo de los 88 m. Ajusta también los límites de rescate de `DiverController` para profundidades mayores a 120 m. Las posiciones y alturas del paisaje base asumen agua a 0 m.
- `Interactable`: hereda esta clase para añadir puertas, herramientas, misiones o nuevos puzles. El buzo detecta automáticamente los objetos a menos de 4,5 m.
- `StonePuzzleDefinition` define dimensiones, recorrido y mezcla; `StoneCircuitPuzzle` evalúa conexiones y corriente; `RuneStone` gira las piedras; `StonePuzzleLever` activa o reinicia; `StonePuzzleView` anima el cierre; `TreasureChest` entrega la recompensa. `SequencePuzzle` conserva el código anterior por compatibilidad, pero ya no se usa en los santuarios.
- `FishAgent` y `HarpoonGun`: puntos de extensión para especies, comportamiento, daño y proyectiles físicos. Manos/tridente/arpón usan barrido de esfera y la red consulta peces cercanos con comprobación de visibilidad. Manos y tridente tienen un movimiento corto de alcance; el arpón muestra la línea del disparo.
- `DiveHUD`: interfaz de prototipo con IMGUI escalada a la ventana. Para un inventario mayor a 12 espacios, añade paginación o migra a UI Toolkit/uGUI.

La escena usa el renderizador integrado de Unity y entrada clásica, sin paquetes externos. Si importas los scripts en otro proyecto, habilita **Active Input Handling: Input Manager (Old)** o **Both**, y conserva los ejes Mouse X / Mouse Y. Las referencias a shaders en la configuración evitan que la compilación los elimine.

## Verificación y compilación

- **La Fosa → Validar mecánicas**: comprueba inventario, fauna, circuitos de piedras, reinicio, bloqueo de soluciones y accesibilidad de cada nivel.
- **La Fosa → Compilar Windows**: valida y crea el ejecutable en `Builds/Windows`.
- `Docs/PRUEBAS.md`: resultados y recorrido de pruebas manuales.
- `Docs/MAPA.md`: estructura y dependencias del proyecto.

Referencia técnica: [CharacterController.Move, documentación oficial de Unity](https://docs.unity.com/en-us/engine/6000.0/script-reference/unityengine/charactercontroller/move).
