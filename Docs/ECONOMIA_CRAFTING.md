# Economía, minería y fabricación

Dirección vigente al 9 de noviembre de 2026: pesca → venta → pico → minerales → fabricación de equipo → mayor profundidad. Los puzles y cofres de progresión se retiraron del mundo. Las ruinas del naufragio y la geometría de la fosa se conservan.

## Recorrido

1. Captura peces en la orilla con las manos y vende en Don Lucho. Tridente, arpón y red facilitan la pesca; el pico no exige comprar el tridente.
2. En La Veta · Minería, compra el **pico submarino por $45**. Se utiliza automáticamente al apuntar a una veta y pulsar E; no ocupa una tecla de arma ni un espacio de mochila.
3. Extrae cobre cerca de 24 m, hierro cerca de 52 m y cristal cerca de 88 m. Hay diez depósitos de cada tipo en esas franjas y cinco depósitos adicionales de cristal en el fondo a 650 m.
4. Regresa a Marea · Buceo y abre **FABRICAR TRAJES**. Cada receta muestra materiales actuales/necesarios, precio, profundidad y reserva de aire resultantes.
5. Fabrica las mejoras en orden. Se equipan permanentemente y no ocupan espacios; la bombona comprada suma su reserva adicional.

## Recetas iniciales

| Mejora | Dinero | Materiales | Equipo resultante |
|---|---:|---|---|
| Traje reforzado | $120 | 8 cobre | 64 m / 155 u de aire |
| Equipo presurizado | $300 | 6 cobre + 12 hierro | 100 m / 200 u de aire |
| Traje abisal | $650 | 8 hierro + 16 cristal abisal | 650 m / 1400 u de aire |

Valores provisionales; no representan un equilibrio humano validado. El cobre es alcanzable con el equipo inicial; hierro con el primer traje y cristal con el segundo. Sigue siendo posible exceder la profundidad segura, consumiendo mucho más aire. El final de estas tres recetas habilita la exploración de la geometría profunda, no biomas nuevos ya terminados.

## Extracción y fabricación

Cada depósito entrega tres unidades si hay espacio para todas. Exige pico, estar bajo el agua, menos de 4,5 m y visibilidad libre. Un intento fallido no agota la veta ni concede material parcial. Tras extraer se oscurece y regenera a los 180 segundos de reloj real; el plazo UTC se guarda y cargar no lo reinicia. Los minerales no se incluyen en VENDER TODOS LOS PECES.

La fabricación comprueba orden, propiedad, dinero y todos los ingredientes antes de consumir. Falta de fondos/materiales o una mejora ya obtenida no descuentan nada. La receta se aplica y guarda como una transacción completa de economía/equipo.

## Partidas y pruebas

El formato v2 guarda minerales, enfriamientos y mejoras fabricadas. Las partidas v1 se leen y sus mejoras ganadas pasan a las mejoras equivalentes; los giros de puzles dejan de controlar el juego. Se conservan dinero, compras, objetos, descubrimientos, vehículos y amarre.

Develop conserva una copia anterior a la prueba. F1 ofrece **Inicio sin equipo** y **Primera minería**; F2 alterna las reglas de Jugador. Minar o fabricar durante una prueba no escribe sobre la partida normal. Terminar pruebas la restaura en la playa.

Los informes opcionales `--expedition-report` pasan a versión 2 e incluyen `mineral` y `craft`; `initialEquipmentUpgrades` describe el equipo inicial. Los eventos antiguos de sello/cofre no forman parte del recorrido nuevo. Los escenarios Develop suspenden el informe para no mezclar pruebas con expediciones normales.

## Código

`GameConfig.minerals/recipes`, `CraftingRecipe` y `EconomyProgressionSetup` definen el contenido editable. `WorldBuilder.Minerals` crea recursos con IDs estables; `MineralDeposit` gestiona interacción y regeneración. `Economy.Crafting` e `Inventory.TryConsume` aplican requisitos/costes. `GameSession.Persistence` y `SaveValidation` conservan y migran el progreso. `DiveHUD.Shop` presenta tienda y taller.

Validación ejecutada en editor y compilación: `unity-economy-final-build.log`. Recorrido humano, presentación visual, sonido/animación específica de minería y balance profundo pendientes.
Actualización de ciudad: comercios separados, materiales en herramientas avanzadas y cinco obras de muelle/base. Ver `CIUDAD_Y_MUELLE.md`. La venta de minerales es opcional en La Veta; conservar ingredientes para trajes y obras. Añadir a la ficha humana visitas a los cuatro locales, pesca automática, las cinco construcciones y enlace/descensor.
