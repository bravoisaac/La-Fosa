# La Fosa

Prototipo original en **Unity 6.3 LTS (6000.3.26f1)**. Exploración low poly en primera persona, ambientada en Caleta Bruma, una playa ficticia de Chile. La profundidad está comprimida para jugar: no es una simulación realista de buceo ni de geografía.

Dirección del proyecto: [contexto de diseño para Codex](Docs/CONTEXTO_JUEGO.md) y [plan de mejora por etapas](Docs/PLAN_MEJORAS.md). Para continuar el desarrollo, empezar por **Reanudar aquí**, al inicio del plan: contiene objetivo, pasos marcados, evidencia y pendiente inmediato. Estos documentos distinguen la visión futura de las funciones implementadas que se describen aquí.

La referencia de mapa del usuario se implementa como una **fosa de 650 m**, con cavidad irregular, cámaras laterales y rutas alrededor de pilares. Cinco umbrales aumentan presión y consumo de aire sin bloquear el paso. [Sección estática del mapa](Docs/Mapa/fosa-seccion.png) · [Profundidades, equipo y límites de validación](Docs/DESCENSOS.md). La costa y las ruinas del naufragio se conservan. La progresión usa pesca, minerales y fabricación; no se generan puzles ni cofres. La ampliación no equivale todavía a cinco biomas terminados.

## Ciudad, comercios y obras

Caleta Bruma tiene plaza, calles, casas y cuatro tiendas físicas: **Don Lucho (pesca), La Veta (minería), Marea (buceo) y Astillero Bruma (construcción y barcos)**. E abre cada mostrador. Peces y minerales se venden en sus comercios; herramientas avanzadas y obras cuestan **dinero y materiales**. Pico y tridente iniciales siguen siendo accesibles sin minerales.

En el astillero, **OBRAS DEL MUELLE** ofrece cinco mejoras en orden: ampliar muelle → primera red automática → segundo tramo y red → base sobre la fosa → descensor rápido. Cada red produce un pez/minuto de juego activo y guarda hasta diez; E en su cajón recoge la captura. No produce en menús, pausado ni sin conexión.

La base desbloquea enlace gratuito ciudad/base. El descensor permite regresar a cotas ya exploradas con equipo y aire suficientes; no recarga recursos ni bloquea el descenso manual. Obras y producción se guardan; las partidas anteriores empiezan sin ciudad mejorada y conservan su equipo. [Precios, materiales, transporte y pruebas](Docs/CIUDAD_Y_MUELLE.md).

Vistas de editor, sin jugar: [ciudad](Docs/Ciudad/ciudad.png) · [muelle y base con todas las obras](Docs/Ciudad/muelle-base.png). Revisión humana del recorrido y balance pendiente.

## Jugar

1. En Unity Hub, selecciona **Add / Add project from disk** y esta carpeta.
2. Abre `Assets/LaFosa/Scenes/LaFosa.unity`.
3. Pulsa **Play** y **Nueva partida** o **Continuar**. Reemplazar una partida existente requiere confirmación.

Si aún no existe la escena, usa **La Fosa → Preparar escena inicial**. El mundo se construye al pulsar Play; es normal que la escena en edición contenga solamente el objeto de inicio.

La ruta principal para las entregas es `Builds/Windows/La Fosa.exe`. Compila con **La Fosa → Compilar Windows** o `LaFosa.Editor.ProjectSetup.BuildWindows` sin `--expedition-preview`; el registro `LA_FOSA_BUILD_OK` muestra la ruta absoluta generada.

`Builds/FirstExpedition` es una salida alternativa para pruebas aisladas mediante `--expedition-preview`; compilar allí no actualiza `Builds/Windows`. Las entregas anteriores del 28 de octubre quedaron solo en esa salida, dejando la versión principal con las compuertas antiguas. Cierra el jugador antes de actualizar su carpeta. Ambos ejecutables comparten la misma partida local.

## Controles

| Acción | Control |
|---|---|
| Caminar / nadar | WASD |
| Mirar | Ratón |
| Saltar / ascender | Espacio |
| Descender | Ctrl izquierdo o C |
| Acelerar (consume más aire al bucear) | Shift izquierdo |
| Interactuar / extraer minerales / recoger | E |
| Atrapar con las manos / usar herramienta | Clic izquierdo |
| Manos / tridente / arpón / red | 1 / 2 / 3 / 4 |
| Abrir tienda / subir o bajar de vehículo | E |
| Conducir lancha o moto | W/S avanzar-retroceder, A/D girar |
| Amarrar / soltar la lancha junto a la boya de la fosa | G, desde el puesto de conducción |
| Bodega de la lancha | B a bordo; B o Esc para cerrar |
| Linterna | F |
| Inventario | Tab |
| Pausa | Esc |
| Rescate a la playa | R |
| Alternar Develop / Jugador de prueba | F2, después de iniciar o continuar partida |
| Panel de herramientas durante pruebas | F1 |

## Modo Develop

Durante una partida, **F2** guarda primero tu progreso normal y abre una sesión temporal de pruebas. También se puede entrar desde pausa. **F1** abre/cierra el panel: aire y salud fijos, velocidad ×3 con colisiones, recarga, $1000 de prueba, herramientas/equipo para 650 m, +10 de cada mineral de prueba y transporte a playa o distintas profundidades.

Para repetir mecánicas, **Inicio sin equipo** reinicia la prueba en la playa con manos, sin dinero y mochila vacía. **Primera minería** prepara el pico junto a las vetas de cobre. Ambos reinician mejoras y recursos de la prueba, apagan aire fijo/velocidad extra y mantienen abierto el panel; pulsa F2 para probar como Jugador. Reemplazan los cambios de la prueba actual, conservando tu partida normal. **Equipo para 650 m** mejora el equipo temporal sin reiniciar el progreso. Los escenarios no reinician toda la simulación de fauna.

**F2** alterna Develop y Jugador manteniendo la posición, el equipo y los cambios de la prueba. En Jugador se desactivan los privilegios: vuelve el consumo de aire, el daño y la velocidad normal. La etiqueta superior recuerda que la sesión sigue **sin guardar**. Para recuperar tu partida, pulsa **Terminar pruebas** en el panel: se restauran dinero, inventario, minerales, mejoras, equipo y vehículos anteriores y vuelves a la playa. Volver al menú también termina las pruebas. Cerrar el juego descarta los cambios temporales; no se escriben sobre la partida ni su respaldo. Los informes de expedición se interrumpen durante las pruebas.

## Bucle de juego

**Pescar → vender → comprar pico → extraer minerales → fabricar equipo → explorar más profundo.** El cuaderno calcula el siguiente objetivo según saldo, materiales y mejoras. Se mantienen retícula y señales físicas; el marcador de misión proyectado sigue retirado. Aire bajo o presión excesiva priorizan el ascenso. [Recorrido y evaluación pendiente](Docs/PRIMERA_EXPEDICION.md).

Empiezas con las manos: bajo el agua, apunta a un pez y haz clic. Alcance de 1,6 m y recarga de 0,8 s, respetando obstáculos. Los peces de la orilla huyen a 1,8 m/s; los del mar a 16 m/s, por lo que tridente, arpón y red ofrecen distintas formas de pescar. Hay 49 modelos propios y 132 individuos: [guía de fauna](Docs/FAUNA.md).

Vende en Don Lucho y compra en La Veta el **pico submarino por $45**, sin exigir tridente. Apunta a una veta y pulsa **E**: entrega tres unidades si tienes espacio. Hay cobre cerca de 24 m, hierro cerca de 52 m y cristal abisal cerca de 88 m, además de cristal en el fondo. Las vetas agotadas se oscurecen y regeneran tras 180 segundos de reloj real; guardar/cargar no reinicia ese plazo.

En Marea · Buceo, abre **FABRICAR TRAJES**. Dinero y minerales permiten crear tres mejoras permanentes, en orden:

| Equipo | Coste | Profundidad segura / reserva |
|---|---|---|
| Traje reforzado | $120 + 8 cobre | 64 m / 155 u de aire |
| Equipo presurizado | $300 + 6 cobre + 12 hierro | 100 m / 200 u |
| Traje abisal | $650 + 8 hierro + 16 cristal abisal | 650 m / 1400 u |

La bombona suma su reserva adicional. Una fabricación fallida no consume dinero ni materiales; no se puede volver a pagar una mejora obtenida. [Reglas y configuración económica](Docs/ECONOMIA_CRAFTING.md).

**Toda la fosa tiene descenso libre.** Superar la profundidad segura multiplica el consumo de aire por 8, o por 12 al acelerar, además del coste de la franja de presión. No hay daño directo de presión mientras quede aire. Desde cámaras profundas busca el conducto central para ascender. La superficie recupera aire y salud; agotar salud o pulsar R devuelve al buzo a la playa conservando objetos. [Profundidades y consumo](Docs/DESCENSOS.md).

La mochila tiene 12 espacios y pilas limitadas; puedes descartar pilas desde Tab. Inventario y pausa detienen la simulación, pero la regeneración de minerales usa reloj real. Las mejoras fabricadas no ocupan espacios.

El guardado v2 conserva dinero, inventario, compras, mejoras, enfriamiento de vetas, descubrimientos, conchas y vehículos con su amarre. Lee partidas v1 y convierte las mejoras ya ganadas. **Continuar** devuelve al buzo a la playa. [Guardado y respaldo](Docs/GUARDADO.md).

Las manos mantienen dedos articulados y gesto de captura. **La Fosa → Exportar vistas de manos (sin jugar)** crea vistas estáticas en `Docs/Manos`. No incluye multijugador ni sonido; animación específica de minería y equilibrio humano siguen pendientes.

El argumento opcional `--expedition-report` registra tiempos y eventos locales, incluidos extracción y fabricación. Los informes no se generan durante Develop. [Registro y ficha humana](Docs/PRIMERA_EXPEDICION.md#registro-opcional-del-recorrido).

## Pesca, herramientas y embarcaciones

La cabaña de madera está a la izquierda del punto de inicio, con techo de zinc a dos aguas, corredor, mostrador y un pescador que atiende. Acércate al mostrador, mira hacia él y pulsa **E**. La tienda pausa el mundo; **Esc** permite salir.

Empiezas **sin dinero, sin arpón y pescando con las manos**. Los peces de la orilla y el jurel se venden por **$30 de moneda del juego**; otras especies tienen precios propios. Dos capturas de la orilla pagan el **tridente de $60**, una mejora opcional de pesca, que se equipa al comprarlo. Solo entonces puedes comprar el arpón o la red. El botón de venta suma los precios de toda tu pesca y deja intactos conchas, minerales y objetos antiguos. Los peces reaparecen tras 25 segundos de simulación para mantener el ciclo de pesca y comercio.

| Compra | Precio | Función |
|---|---:|---|
| Pico submarino | $45 | Extrae minerales con E; no exige tridente |
| Tridente | $60 | Herramienta de pesca; tecla 2, alcance de 4 m y recarga de 0,45 s |
| Arpón | $150 | Requiere tridente; tecla 3, alcance de 40 m y recarga de 0,65 s |
| Bombona de O₂ ampliada | $90 | +60 unidades de aire, adicionales al equipo fabricado; duración según profundidad y aceleración |
| Aletas | $75 | +35% de velocidad de nado, también al acelerar |
| Red de pesca | $120 | Requiere tridente; tecla 4, hasta 5 peces en un radio de 4 m, recarga de 5 s |
| Lancha de caleta | $240 | Navega a 12 m/s; amarre sobre la fosa con G |
| Moto de agua | $360 | Embarcación de superficie a 22 m/s |

La tabla anterior muestra precios monetarios; los materiales y el local de cada compra figuran en [Ciudad y muelle](Docs/CIUDAD_Y_MUELLE.md). Las mejoras se compran una sola vez y no consumen espacios de mochila. Puedes volver a las manos con la tecla 1. La bombona amplía permanentemente la capacidad de la partida; no es un consumible. La red es reutilizable, funciona buceando o desde una embarcación y respeta los obstáculos. Las manos, el arpón y el tridente requieren estar bajo el agua. Cambiar de herramienta no elimina la recarga pendiente.

La lancha comprada aparece a la izquierda del muelle y la moto a la derecha. Acércate, mira la embarcación y pulsa **E**. Usa **W/S** para avanzar/retroceder, **A/D** para girar y el ratón para mirar. **E** te deja nadando a un costado; el vehículo queda donde lo abandonaste. **R** rescata al buzo y conserva la propiedad. La conducción usa colisiones y límites de costa; no incluye una simulación de oleaje o combustible.

La boya naranja sobre la fosa señala el **amarre de la lancha**, a un lado del conducto central. Desde el puesto de conducción, acércate a menos de 8 m del punto de amarre y pulsa **G**. El HUD avisa cuándo está disponible. Si el acercamiento o giro están obstruidos, la lancha permanece donde está. Amarrada queda inmóvil; puedes desembarcar con E, bucear, volver a subir y soltar con G. Posición y amarre se conservan al guardar/cargar; una partida anterior carga las embarcaciones desamarradas. La moto no usa este amarre. La lancha incluye una **bodega de 24 espacios**: pulsa **B a bordo** para transferir pesca y minerales por unidad o pila completa; **B/Esc** cierra. El panel pausa la navegación y muestra los límites de ambas cargas. Si no cabe la cantidad, no mueve nada. Retira a la mochila antes de vender o fabricar. Carga persistente y aislada en Develop. [Bodega y pruebas pendientes](Docs/BODEGA_LANCHA.md). La recuperación de una lancha abandonada sigue pendiente.

## Ampliar

- `GameConfig.minerals/recipes`, `CraftingRecipe` y `EconomyProgressionSetup`: minerales, ingredientes, precios y mejoras. `Expedicion.asset` conserva configuración, semilla, aire y zonas; `Tienda.asset` contiene compras.
- `WorldBuilder.Minerals` crea depósitos con IDs estables; `MineralDeposit` controla extracción, visibilidad, agotamiento y regeneración. `Economy.Crafting` e `Inventory.TryConsume` aplican costes completos.
- `DiveHUD.Shop` presenta TIENDA/FABRICAR. `GameSession.Expedition` deriva el cuaderno de economía, recursos y equipo, sin marcador proyectado.
- `GameSession.Persistence`, `GameSession.SaveValidation` y `SaveStore`: guardado v2, lectura v1, validación y respaldo. No renombrar IDs sin migración. Las definiciones antiguas de puzles se conservan para validar partidas v1; no generan contenido jugable.
- `DeepTrenchProfile`, `WorldBuilder.DeepTrench` y `WorldBuilder.Descents`: cavidad a 650 m, malla con colisión y señales sin bloqueo. `GameConfig.pressureBands` controla cinco umbrales de gasto; `pressureOxygenMultiplier` controla el sobrecoste fuera del equipo. El terreno superficial no representa techos o paredes de la cavidad.
- `WorldBuilder.City`, `InfrastructureView`, `CityInteraction`, `GameSession.City` y `Economy.City`: comercios separados, obras, producción y viajes. `CityProgressionSetup` define materiales y precios.
- `WorldBuilder.Shop` y `SurfaceVehicle`: tienda, embarcaciones, conducción y amarre. `GameSession.Cargo` y `DiveHUD.Cargo`: bodega de 24 espacios y transferencias atómicas con `Inventory.TryTransferTo`. Recuperación de lancha abandonada pendiente.
- `FishAgent` y `HarpoonGun`: fauna, comportamiento y captura. `ItemDefinition.salePrice` define el valor de cada especie.
- `DiveHUD`: IMGUI con lienzo base 1600 × 900; aire en unidades, consumo actual y duración aproximada. Revisar formatos de pantalla mediante la ficha humana.

La escena usa el renderizador integrado y entrada clásica de Unity, sin paquetes externos. Al importar en otro proyecto, habilita **Active Input Handling: Input Manager (Old)** o **Both** y conserva Mouse X / Mouse Y. Las referencias a shaders evitan su eliminación al compilar.

## Verificación y compilación

- **La Fosa → Validar mecánicas**: comprueba inventario, fauna, economía, minería, fabricación, migración, aislamiento Develop, amarre y descenso.
- **La Fosa → Compilar Windows**: valida y crea el ejecutable en `Builds/Windows`.
- `Docs/PRUEBAS.md`: resultados y recorrido de pruebas manuales.
- `Docs/MAPA.md`: estructura y dependencias del proyecto.

Referencia técnica: [CharacterController.Move, documentación oficial de Unity](https://docs.unity.com/en-us/engine/6000.0/script-reference/unityengine/charactercontroller/move).
