# La Fosa · plan de mejoras

## Reanudar aquí — 9 de noviembre de 2026

**Objetivo activo:** progresar mediante pesca, venta, minerales del fondo marino y fabricación de equipo para explorar una fosa de 650 m. Petición vigente del usuario: quitar los puzles. El límite del descenso es el aire, nunca una pared ni un sello.

**Última entrega: E1–E4 implementadas.** Mundo sin santuarios, tableros ni cofres de progresión; pico submarino, cobre/hierro/cristal, taller con tres recetas, guía económica, Develop adaptado y guardado v2 compatible con v1. Compilación principal `Builds/Windows/La Fosa.exe` aprobada. Evidencia: `unity-economy-final-build.log`; 39 comprobaciones económicas, 16 de Develop, 18 de amarre, 71 de fosa, 20 de informe y 11 de señales, además de validaciones generales.

**Siguiente implementación:** 4.2b, bodega de la lancha para transportar pesca y minerales con transferencias y carga persistente. **Revisión humana pendiente:** E5, recorrer pesca → pico → minería → fabricación, comprobar HUD y ajustar precios/cantidades según mediciones reales. La compilación no sustituye ese recorrido.

**Uso del juego:** abrirlo solo cuando el usuario lo pida; no confundir abrir con probar. Esta entrega se comprobó en editor, sin Play Mode. No hay nueva evaluación humana del balance.

- [x] E1 · Retirar puzles, santuarios y cofres de la generación del mundo. Conservar ruinas del naufragio, terreno y descenso libre.
- [x] E2 · Pico comprable ($45), 35 depósitos de cobre/hierro/cristal, extracción con E, bloqueo por distancia/obstáculos/mochila llena y regeneración persistente de 180 s. Depósitos agotados oscurecidos.
- [x] E3 · Taller FABRICAR en Don Lucho. Tres mejoras de equipo por dinero y minerales; requisitos visibles, orden de mejoras, costes completos y sin fabricación duplicada. Guía y Develop adaptados a minería.
- [x] E4 · Guardado v2 de materiales, mejoras y regeneración; carga v1 convierte mejoras ganadas sin repetirlas. Preserva economía, inventario, vehículos y amarre. Pruebas Develop no sobrescriben la partida normal.
- [ ] E5 · Recorrido humano completo, lectura de vetas/taller, guardado/carga manual y medición de ingresos, viajes y aire. Ajustar valores después de medir.
- [x] 4.1 · Lancha de caleta reutiliza la embarcación comprable.
- [x] 4.2a · Amarre sobre la fosa, tecla G, comprobación de obstáculos y persistencia. Verificación humana pendiente.
- [ ] 4.2b · Bodega: transferir pesca/minerales entre mochila y lancha, límites claros, guardado y cero duplicaciones.
- [ ] 4.3 · Preparar salida y reponer suministros en un punto definido de la lancha.
- [ ] 4.4 · Recuperar lancha abandonada y comprobar carga/propiedad/posición entre expediciones.
- [ ] 5 · Ampliar fabricación: nuevos materiales, recetas y herramientas que aporten usos distintos. La antigua etapa de nuevos puzles queda cancelada.
- [ ] 6 · Biomas, recursos y fauna por profundidad, rutas y referencias ambientales. Sin puzles obligatorios.
- [ ] 7 · Fauna fantástica y peligro legible; evasión antes de daño y herramientas con funciones distintas.
- [ ] 8 · Base sobre la fosa, almacenamiento, preparación y conexiones a zonas ya exploradas.
- [ ] 9 · Exploradores contratados para apoyo económico en zonas conocidas, sin sustituir la exploración personal.
- [ ] 10 · Equilibrio, rendimiento, sonido y opciones; comprobar la expedición completa y sus guardados.

## Reglas para continuar

Leer este resumen y el documento del paso activo. Terminar una entrega acotada, comprobarla, marcar `[x]`, registrar evidencia y actualizar el siguiente paso. Conservar pendientes las pruebas humanas no ejecutadas. No reintroducir puzles o barreras de progresión salvo nueva petición explícita.

Los valores de recetas y regeneración son provisionales. La referencia `fosa mapa.png` conserva cinco umbrales de presión/consumo; la geometría profunda no equivale a cinco biomas terminados.

La evaluación de las antiguas etapas 1–3 se sustituye por el recorrido económico E5: no exige resolver un contenido que el usuario retiró. Las prestaciones previas de manos, HUD, pesca, presión, guardado y Develop se conservan.

## Referencias

- `Docs/ECONOMIA_CRAFTING.md`: bucle, recursos, recetas y límites.
- `Docs/PRIMERA_EXPEDICION.md`: ficha humana vigente.
- `Docs/GUARDADO.md`: compatibilidad v1 → v2 y aislamiento de pruebas.
- `Docs/PRUEBAS.md` y `Docs/MAPA.md`: evidencia y dependencias.
- `Docs/PLAN_ANTERIOR_PUZLES.md`: historial del plan anterior; sus propuestas de puzles ya no son instrucciones vigentes.