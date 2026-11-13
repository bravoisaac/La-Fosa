# Caleta Bruma: comercios, muelle y base

Entrega del 9 de noviembre de 2026. La ciudad ahora tiene plaza, calles, casas, faroles y cuatro comercios físicos. Las compras se completan en el local correspondiente; no existe un menú único que venda todo.

## Comercios y productos

| Local | Ubicación desde el inicio | Servicios |
|---|---|---|
| Don Lucho · Pesca | Izquierda, primera fila | Vender todos los peces; tridente, arpón y red |
| La Veta · Minería | Derecha, primera fila | Pico; vender cobre, hierro y cristal por unidad o toda la pila |
| Marea · Buceo | Izquierda, al fondo de la plaza | Bombona, aletas y FABRICAR TRAJES |
| Astillero Bruma | Derecha, al fondo | Embarcaciones y OBRAS DEL MUELLE |

Acércate al mostrador, apunta y pulsa **E**. El local muestra precio, materiales que tienes/necesitas y requisitos. ESC cierra. El tridente ($60) y el pico ($45) no piden materiales para permitir empezar desde cero. El pico no exige tridente.

| Herramienta o accesorio | Dinero | Materiales |
|---|---:|---|
| Arpón | $150 | 6 hierro; requiere tridente |
| Red | $120 | 6 cobre; requiere tridente |
| Bombona | $90 | 4 cobre |
| Aletas | $75 | 3 cobre |
| Lancha | $240 | 8 cobre + 6 hierro |
| Moto de agua | $360 | 12 hierro + 4 cristal |

La Veta compra cobre por $8/u, hierro por $14/u y cristal por $26/u. Reserva lo necesario para las recetas y obras antes de vender. La venta de peces no consume minerales. Las tres recetas de traje conservan sus costes y se fabrican en Marea: `ECONOMIA_CRAFTING.md`.

## Cinco obras en orden

| Obra | Dinero y materiales | Resultado |
|---|---|---|
| 1 · Ampliar el muelle | $180 + 12 cobre | Alarga 12 m el muelle existente |
| 2 · Primera zona de pesca | $280 + 10 cobre + 6 hierro | Plataforma, red y depósito automático |
| 3 · Muelle pesquero | $450 + 8 cobre + 18 hierro | Otros 12 m y segundo puesto automático |
| 4 · Base sobre la fosa | $900 + 24 hierro + 12 cristal | Base flotante con refugio y enlace ciudad ↔ base |
| 5 · Descensor rápido | $700 + 12 cobre + 18 cristal | Transporte desde la base a profundidades ya exploradas |

Las obras aparecen al comprarlas y se conservan. Solo se puede encargar la siguiente; no se cobra de nuevo una obra terminada. Dinero y todos los ingredientes se comprueban antes de descontar. La base queda a un lado de la entrada, sin bloquear el conducto central ni el amarre de la lancha.

## Pesca automática

Cada zona produce **un pez cada 60 segundos de juego activo**, hasta **10 peces** por depósito. No produce durante tienda, inventario, pausa, menú o con el juego cerrado. El tiempo parcial se guarda; cargar no regala capturas ni reinicia ese progreso. Al llenarse deja de acumular tiempo.

Apunta al cajón del puesto y pulsa **E** para pasar toda la captura a la mochila. Si no hay espacio para todos, los peces permanecen en el puesto. Véndelos después en Don Lucho. Los dos puestos funcionan independientemente.

## Base y viajes

Tras comprar la base, el terminal de la plaza permite ir a ella sin coste. El terminal de la base permite volver a la ciudad. La plataforma incluye rampa desde el agua y un refugio; no añade todavía bodega ni reposición especial de suministros.

Con la quinta obra, el terminal de la base ofrece **24, 52, 88, 150, 280, 420 y 620 m**. Cada destino exige haber alcanzado esa profundidad y tener equipo seguro para ella, además de reserva de aire para ascender. El viaje conserva inventario, salud y aire; no es una recarga. Si la llegada está ocupada, no desplaza al buzo. No requiere paredes ni llaves para el descenso manual.

El registro de exploración comienza con esta actualización: los guardados antiguos conservan sus mejoras, pero deberán visitar de nuevo las cotas para registrar destinos de transporte.

## Guardado, Develop y validación

Se conservan nivel de obras, capturas de cada puesto, segundos parciales de producción y máxima profundidad explorada. Son campos opcionales del formato v2; partidas anteriores empiezan sin infraestructura. Nueva partida y escenarios iniciales Develop los reinician. Terminar pruebas recupera todo el estado normal.

En F1, **+10 DE CADA MINERAL · PRUEBA** permite financiar materiales de prueba junto con el botón de dinero. Solo funciona en Develop y no escribe sobre la partida personal.

`unity-city-final.log`: **78 comprobaciones nuevas de ciudad**, además de 39 económicas, 16 Develop, 18 amarre, 71 fosa, 20 informe y 11 señales. Validan comercios, costes, generación de estructuras, acceso a mostradores, cubierta, eje libre, producción, capacidad, viajes, datos inválidos y continuidad/aislamiento. Windows actualizado en `Builds/Windows/La Fosa.exe`; sin Play Mode ni partida de prueba.

`CityPreview.Export` genera `Docs/Ciudad/ciudad.png` y `muelle-base.png`: vistas estáticas con todas las obras visibles, sin jugar. No sustituyen recorrido humano, revisión del HUD o balance de precios/producción. Esos puntos siguen pendientes.

## Archivos principales

`CityProgressionSetup` y `InfrastructureOffer`: contenido. `Economy.City`: ventas/requisitos/costes. `GameSession.City`: estado, producción y viajes. `WorldBuilder.City`, `InfrastructureView` y `CityInteraction`: ciudad y obras. `DiveHUD.Shop`: comercios/taller/terminales. `CityProgressionChecks`: pruebas aisladas.
