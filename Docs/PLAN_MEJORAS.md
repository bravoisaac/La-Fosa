# La Fosa · plan de mejoras

## Reanudar aquí — 13 de noviembre de 2026

**Objetivo activo:** desarrollar una ciudad comercial: vender pesca y minerales, comprar herramientas en tiendas distintas y pagar dinero/materiales para ampliar el muelle, automatizar pesca y construir una base con viajes rápidos sobre la fosa de 650 m. Petición vigente del usuario: quitar los puzles. El límite del descenso es el aire, nunca una pared ni un sello.

**Última entrega: 4.2b terminada.** Bodega de lancha de 24 espacios, B a bordo, transferencias de una unidad o pila completa de pesca/minerales. Capacidad visible, navegación pausada, guardado v2 compatible y restauración Develop. Ciudad C1–C5 y economía E1–E4 ya implementadas. `Builds/Windows/La Fosa.exe` actualizado.

**Evidencia:** `unity-cargo-build.log`, salida 0: 39 comprobaciones nuevas de bodega, más 78 ciudad, 39 económicas, 16 Develop, 18 amarre, 71 fosa, 20 informe y 11 señales. `LA_FOSA_TESTS_OK` y `LA_FOSA_BUILD_OK`. Sin Play Mode ni ejecución del jugador. Detalles y prueba manual: `Docs/BODEGA_LANCHA.md`.

**Siguiente implementación:** 8b, almacenamiento y preparación de suministros en la base, según el orden acordado; 4.3 (preparación en lancha) y 4.4 (recuperación) siguen pendientes. El diseño concreto de suministros aún no está definido: la bombona actual es una mejora permanente y la superficie ya repone aire/salud. No convertirla en consumible ni añadir combustible por inferencia.

**Lectura mínima para la próxima fase:** este resumen + `Docs/BODEGA_LANCHA.md` + sección de base en `Docs/CIUDAD_Y_MUELLE.md`. Código inicial: `GameSession.Cargo.cs`, `Inventory.cs`, `GameSession.City.cs`, `WorldBuilder.City.cs` y `CityInteraction.cs`. Ampliar a persistencia/HUD solo al implementar. No releer logs completos ni historial de MAPA/PRUEBAS; buscar marcadores `LA_FOSA_` si hace falta. Hay cambios acumulados sin commit de fases anteriores: conservarlos.

**Revisión humana pendiente:** C6/E5 y bodega: compras/ventas, minería, fabricación, obras, redes, viajes, legibilidad, B/ESC y recorrido guardar/cargar. No marcarla completada con pruebas de editor.

**Uso del juego:** abrirlo solo cuando el usuario lo pida; no confundir abrir con probar. Esta entrega se comprobó en editor, sin Play Mode. No hay nueva evaluación humana del balance.

- [x] C1 · Ciudad con plaza, calles, casas y cuatro locales: pesca, minería, buceo y astillero. Cada local ofrece sus productos; guía actualizada.
- [x] C2 · Herramientas avanzadas y obras con dinero + materiales; ventas de minerales; cinco obras únicas y secuenciales con costes completos.
- [x] C3 · Muelle ampliable en dos tramos de 12 m; dos redes automáticas, 1 pez/min activo y depósito de 10 por puesto; E recoge sin pérdida por mochila llena.
- [x] C4 · Base junto a la entrada de la fosa, enlace gratis ciudad/base y descensor a cotas exploradas con equipo/aire suficientes. Llegadas comprobadas y eje libre.
- [x] C5 · Guardado compatible con partidas anteriores; obras/stock/tiempo/exploración persistentes; Develop aislado y botón de minerales; pruebas y compilación Windows.
- [ ] C6 · Recorrido humano de la ciudad, precios, producción, accesibilidad en movimiento y viajes; vistas estáticas no sustituyen jugar.
- [x] E1 · Retirar puzles, santuarios y cofres de la generación del mundo. Conservar ruinas del naufragio, terreno y descenso libre.
- [x] E2 · Pico comprable ($45), 35 depósitos de cobre/hierro/cristal, extracción con E, bloqueo por distancia/obstáculos/mochila llena y regeneración persistente de 180 s. Depósitos agotados oscurecidos.
- [x] E3 · Taller FABRICAR, trasladado a Marea · Buceo en C1. Tres mejoras de equipo por dinero y minerales; requisitos visibles, orden de mejoras, costes completos y sin fabricación duplicada. Guía y Develop adaptados a minería.
- [x] E4 · Guardado v2 de materiales, mejoras y regeneración; carga v1 convierte mejoras ganadas sin repetirlas. Preserva economía, inventario, vehículos y amarre. Pruebas Develop no sobrescriben la partida normal.
- [ ] E5 · Recorrido humano completo, lectura de vetas/taller, guardado/carga manual y medición de ingresos, viajes y aire. Ajustar valores después de medir.
- [x] 4.1 · Lancha de caleta reutiliza la embarcación comprable.
- [x] 4.2a · Amarre sobre la fosa, tecla G, comprobación de obstáculos y persistencia. Verificación humana pendiente.
- [x] 4.2b · Bodega: 24 espacios, B a bordo, transferir pesca/minerales por unidad/pila, límites visibles, guardado compatible y 39 comprobaciones sin pérdidas/duplicaciones. Revisión visual y jugable pendiente.
- [ ] 4.3 · Preparar salida y reponer suministros en un punto definido de la lancha.
- [ ] 4.4 · Recuperar lancha abandonada y comprobar carga/propiedad/posición entre expediciones.
- [ ] 5 · Ampliar fabricación: nuevos materiales, recetas y herramientas que aporten usos distintos. La antigua etapa de nuevos puzles queda cancelada.
- [ ] 6 · Biomas, recursos y fauna por profundidad, rutas y referencias ambientales. Sin puzles obligatorios.
- [ ] 7 · Fauna fantástica y peligro legible; evasión antes de daño y herramientas con funciones distintas.
- [x] 8a · Base sobre la fosa y conexiones a zonas exploradas, entregadas en C4.
- [ ] 8b · Almacenamiento y preparación de suministros en la base.
- [ ] 9 · Exploradores contratados para apoyo económico en zonas conocidas, sin sustituir la exploración personal.
- [ ] 10 · Equilibrio, rendimiento, sonido y opciones; comprobar la expedición completa y sus guardados.

## Reglas para continuar

Leer este resumen y el documento del paso activo. Terminar una entrega acotada, comprobarla, marcar `[x]`, registrar evidencia y actualizar el siguiente paso. Conservar pendientes las pruebas humanas no ejecutadas. No reintroducir puzles o barreras de progresión salvo nueva petición explícita.

Los valores de recetas y regeneración son provisionales. La referencia `fosa mapa.png` conserva cinco umbrales de presión/consumo; la geometría profunda no equivale a cinco biomas terminados.

La evaluación de las antiguas etapas 1–3 se sustituye por el recorrido económico E5: no exige resolver un contenido que el usuario retiró. Las prestaciones previas de manos, HUD, pesca, presión, guardado y Develop se conservan.

## Referencias

- `Docs/CIUDAD_Y_MUELLE.md`: comercios, obras, pesca automática y transporte.
- `Docs/BODEGA_LANCHA.md`: entrega 4.2b, decisiones, archivos clave y prueba humana pendiente.
- `Docs/ECONOMIA_CRAFTING.md`: bucle, recursos, recetas y límites.
- `Docs/PRIMERA_EXPEDICION.md`: ficha humana vigente.
- `Docs/GUARDADO.md`: compatibilidad v1 → v2 y aislamiento de pruebas.
- `Docs/PRUEBAS.md` y `Docs/MAPA.md`: evidencia y dependencias.
- `Docs/PLAN_ANTERIOR_PUZLES.md`: historial del plan anterior; sus propuestas de puzles ya no son instrucciones vigentes.
