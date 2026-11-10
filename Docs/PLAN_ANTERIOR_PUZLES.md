> HISTÓRICO: plan anterior al cambio a economía y fabricación. Sus propuestas de puzles quedan canceladas. Para continuar, leer PLAN_MEJORAS.md; no ejecutar las instrucciones de este archivo.

# La Fosa plan de mejora por etapas

El objetivo es transformar el prototipo actual en un juego de expediciones por una fosa misteriosa. Este plan desarrolla la visión de `Docs/CONTEXTO_JUEGO.md`. **Es un plan de trabajo: consultar el estado de cada etapa; las funciones de las etapas pendientes todavía no están implementadas.**

El orden de construcción no es idéntico al orden en que el jugador comprará el equipo. Primero hay que conseguir una expedición completa que se guarde, se entienda y permita avanzar; después se amplía el mundo.

## Reanudar aquí — 8 de noviembre de 2026

**Objetivo activo:** completar la primera expedición persistente (etapas 1–3): pescar con las manos, vender, comprar tridente, resolver el primer sello, recuperar el tesoro y volver para continuar otra salida.

**Última entrega terminada: 4.1 y 4.2a**, primera preparación técnica de la lancha: nombre uniforme, boya de amarre sobre la fosa, G para amarrar/soltar, comprobación de acercamiento/giro y estado persistente compatible con partidas v1 anteriores. 18 comprobaciones nuevas de editor, validaciones existentes y compilación principal Windows correctas (`unity-mooring-final-build.log`). **Siguiente implementación: 4.2b**, almacenamiento de la lancha con transferencias sin duplicaciones y carga persistente. **Revisión humana pendiente: 3.3b, 3.4b y recorrido de lancha**; el avance técnico no cierra esos criterios ni el equilibrio 3.5.

**Uso del juego:** el usuario autorizó abrirlo a petición y después pidió cerrarlo para actualizar. No interpretar esas aperturas como pruebas jugables realizadas. Esta entrega se validó en editor sin Play Mode; revisión humana pendiente.

- [x] Manos · Geometría, pulgar articulado, gesto de 0,48 s y compilación terminados; vistas de editor en `Docs/Manos`. Detalles en `Docs/MAPA.md` y `Docs/PRUEBAS.md`; sensación en partida pendiente.

- [x] 0 · Dirección y orden de desarrollo documentados.
- [x] 1 · Guardado implementado y probado automáticamente; falta comprobación humana.
- [x] 2 · Requisitos de progreso y ajuste de descenso libre verificados automáticamente; falta recorrido humano.
- [x] 3.1 · Guía de pesca → venta → tridente → sello → tesoro → regreso implementada.
- [x] 3.2 · Consumo de aire ×8 fuera del límite (×12 acelerando), aviso y rescate implementados. La entrega histórica conservaba una compuerta; 3.2b la sustituye.
- [x] 3.2b · Toda la fosa, incluido el abismo, tiene paso libre. Retiradas paredes/escotillas de progresión; señales sin colisión y marcadores directos a santuario/superficie. 11 comprobaciones de editor y compilación: `unity-air-descent-build.log`. Movimiento en partida pendiente.
- [x] 3.2c · Ampliación de 88 a 650 m según referencia: cavidad 3D, cinco umbrales, presión/consumo por nivel, reserva de regreso y equipo final adaptado. 71 comprobaciones de perfil/aire/MeshCollider, compilación y sección estática aprobadas (`unity-deep-map-final.log`). No ejecutado el recorrido humano.
- [x] 3.2d · Actualizar la carpeta principal `Builds/Windows`, que todavía contenía las compuertas antiguas. Compilación correcta, 71 comprobaciones de fosa, 11 de señales y 20 de informe aprobadas; binario actualizado verificado sin abrir el juego (`unity-windows-delivery-build.log`). Ruta de salida explícita en el log y README.
- [x] 3.3a · HUD: escala uniforme, cuaderno adaptable, marcadores separados de interacción, fondos legibles y aviso de aire coherente a 30 s. Compilación/editor correctos: `unity-hud-layout-build.log`.
- [ ] 3.3b · Revisar visualmente HUD, señales y orientación en una ventana renderizada. No ejecutado; conserva abierto el paso 3.3.
- [x] 3.3c · Aclarar aire y presión: unidades consistentes, tiempo estimado al ritmo actual, consumo incluyendo acelerar y distancia al próximo umbral. Vitales/HUD comparten fórmula; valores de balance conservados. Editor y compilación Windows aprobados (`unity-air-hud-build.log`); legibilidad en partida pendiente en 3.3b.
- [x] 3.4a · Preparar registro opcional de tiempos, intentos por herramienta, capturas, visitas, ventas, tridente, sello, cofre y rescates. Segmentos por inicio/carga, aire antes de recompensa y contadores por hito. 20 comprobaciones de editor y compilación: `unity-expedition-report-build.log`.
- [ ] 3.4b · Contrastar eventos con acciones reales y hacer el recorrido con una persona nueva; completar la ficha de tiempos, capturas, viajes y rescates. La instrumentación no completa esta prueba.
- [ ] 3.5 · Ajustar ritmo según esa prueba y cerrar las etapas 1–3 con guardado/carga manual.
- [x] 3.6 · Petición directa: retirar marcador proyectado y añadir Develop/Jugador (F2), herramientas (F1) y sesión temporal sin guardar. 19 comprobaciones nuevas de transición/guardado, validaciones existentes y compilación Windows aprobadas. Panel y manejo en partida pendientes de revisión humana.
- [x] 3.6b · Escenarios de primera expedición y primer santuario en Develop. Reinicio temporal repetible, ventajas apagadas, sin sobrescribir partida ni respaldo; regreso al progreso original comprobado con dinero/pesca/equipo/giros. 31 comprobaciones de Develop y compilación principal aprobadas (`unity-developer-scenarios-build.log`). Presentación y recorrido humano pendientes.
- [x] 4.1 · Bote reutilizado con nombre Lancha de caleta en tienda, mundo y HUD; ID/precio conservados.
- [x] 4.2a · Amarre con boya sobre la fosa, tecla G, colisiones y persistencia. 18 comprobaciones nuevas y compilación principal aprobadas (`unity-mooring-final-build.log`). Recorrido humano pendiente.
- [ ] 4.2b–4.4 · Bodega y transferencias, preparación de salida y recuperación de lancha abandonada; pendientes.
- [ ] 5–10 · Puzles, biomas, fauna, base, exploradores y equilibrio final; pendientes.

**Cómo continuar:** leer este bloque y solo el apartado/documento del paso pendiente. Marcar `[x]` al terminar una entrega y anotar evidencia y pendiente inmediato. Una implementación o prueba automática terminada no cierra una etapa que exige juego humano. Mantener el objetivo y el criterio de cierre al resumir; no borrar las decisiones de diseño.

**Evidencia histórica de 3.2:** 458 comprobaciones de integración y 12 de guardado/carga entre procesos aprobadas en la versión anterior. No validan el cambio 3.2b: este se comprobó en editor y compilación sin jugar. Ejecutable principal actualizado por 3.2d: `Builds/Windows/La Fosa.exe`; no confundir las comprobaciones históricas de runtime con las de esta entrega. Resultados y límites: `Docs/PRUEBAS.md`. Ficha humana: `Docs/PRIMERA_EXPEDICION.md`.

**Decisión vigente del usuario:** el bloqueo de la fosa es por aire, no por una pared. Huiros → naufragio → abismo tienen descenso/ascenso libres; exceder la profundidad segura multiplica el consumo de oxígeno. Los mecanismos conservan su orden de recompensas, pero no impiden nadar. No reintroducir barreras físicas para exigir equipo o tesoros al cruzar capas.

**Referencia visual vigente:** el dibujo `fosa mapa.png` representa una fosa mucho más honda e irregular. Sus cinco líneas son umbrales de presión/consumo; no construir plataformas que cierren la sección. La ampliación 3.2c procede por petición directa del usuario y no marca terminados los futuros biomas, fauna o equilibrio de etapas posteriores.

## Etapa 0 Consolidar la dirección

**Estado: documentación preparada.**

Usar el contexto como referencia para conservar Unity, primera persona, low poly, caleta chilena e inicio con manos. Separar los elementos existentes de los objetivos futuros y de las propuestas abiertas.

**Resultado:** una visión común y este orden de desarrollo. La preparación de este plan no cambia el funcionamiento del juego.

## Etapa 1 Guardar y recuperar el progreso

**Estado al 18 de octubre de 2026: implementada y verificada automáticamente. Pendiente la comprobación humana del menú y del recorrido.**

Guardado local versionado con escritura temporal, validación previa y respaldo; Nueva partida con confirmación, Continuar y Guardar y volver al menú. Incluye saldo, inventario, compras, herramienta, tesoros, giros/sellos, descubrimientos, conchas y propiedad/posición de vehículos. Continuar vuelve a la playa con aire y salud completos. Detalles y límites en `Docs/GUARDADO.md`.

Verificación: compilación Windows correcta; **389 comprobaciones de integración aprobadas** y **12 comprobaciones adicionales repartidas entre dos procesos** de escritura/carga tras cerrar. Sin duplicar tesoros ni acumular mejoras. Probados archivo ausente, corrupción, recuperación del respaldo, versión incompatible, datos incoherentes, fallo de escritura y protección contra nueva partida accidental.

**Objetivo:** que una expedición no se pierda al cerrar el juego.

- [x] 1. Definir identificadores estables para objetos, zonas, puzles y vehículos.
- [x] 2. Guardar dinero, inventario, equipo, tesoros, giros de piedras, zonas descubiertas y propiedad de embarcaciones.
- [x] 3. Añadir Nueva partida y Continuar, con protección frente a sobrescribir una partida por accidente.
- [x] 4. Guardar al volver a un lugar seguro, comprar equipo o recuperar un tesoro. Preparar versión del formato y copia de respaldo.

**Criterio de cierre:** cerrar y volver a abrir restaura una partida coherente, sin duplicar recompensas ni compras. Un archivo ausente o dañado recibe un tratamiento claro.

**Puntos de partida:** `GameSession`, `Inventory`, `Economy`, `StoneCircuitPuzzle`, `TreasureChest` y `SurfaceVehicle`.

## Etapa 2 Organizar los requisitos de cada descenso

**Estado al 18 de octubre de 2026: implementada y verificada automáticamente. Pendiente el recorrido humano, junto con la comprobación manual de la etapa 1.**

La entrega original añadió compuertas a 32 y 64 m. **Ambas están retiradas en el diseño vigente del 28 de octubre.** Se puede nadar entre todas las capas, con peligro por consumo de aire fuera de la profundidad segura. Los mecanismos conservan el requisito del tesoro anterior y entregan mejoras; las guías de profundidad/retorno no tienen colisión. El progreso se deriva del guardado existente, sin cambiar el formato v1. Detalles: `Docs/DESCENSOS.md`.

Verificación histórica: **422 comprobaciones de integración aprobadas** en la versión con compuertas, incluidas persistencia y compatibilidad con etapa 1. Las pruebas de losas/escotillas ya no describen el comportamiento vigente. La corrección 3.2b tiene comprobación de construcción en editor y compilación; el diagnóstico de movimiento está actualizado, pendiente de ejecución autorizada.

**Objetivo:** que el jugador sepa qué necesita para avanzar.

- [x] 1. Definir por capa su entrada, equipo necesario, prueba principal, recompensa y salida segura.
- [x] 2. Separar las mejoras de equipo de resolver mecanismos: tener más oxígeno no completa un puzle.
- [x] 3. Mantener todas las rutas físicamente libres; profundidad y aire determinan el alcance práctico de la salida.
- [x] 4. Avisar del consumo peligroso y señalar el ascenso; los requisitos de progreso se explican al interactuar con mecanismos.
- [x] 5. Mantener un regreso viable a la superficie y evitar que un objeto obligatorio pueda venderse o perderse sin recuperación.

**Criterio de cierre actualizado:** se puede nadar entre las tres capas y regresar sin paredes de progresión. El consumo de aire y la guía explican el peligro y hacen comprensible mejorar el equipo. Los mecanismos conservan su progreso al cargar. Hay que verificar jugando que el aire permita decisiones y un regreso viable.

**Puntos de partida:** `DiveZoneDefinition`, `DiverController.AdvanceVitals`, `WorldBuilder.Descents` y `GameSession.Expedition`. `GameSession.IsZoneUnlocked` corresponde solo a mecanismos/recompensas; no se usa para bloquear desplazamiento.

## Etapa 3 Completar la primera expedición

**Estado al 18 de octubre de 2026: primera entrega implementada, guía contextual de la expedición. Etapa todavía abierta: falta revisión visual del HUD, recorrido humano de las etapas 1–3 y ajuste de ritmo según esa prueba.**

**Actualización al 19 de octubre:** entrega 3.2 verificada (descenso libre, coste de aire y regresiones de progreso). La etapa sigue abierta; los pasos 3.3–3.5 del resumen inicial conservan el cierre visual y humano pendiente.

**Actualización al 23 de octubre:** 3.3a implementada y compilada sin ejecutar el juego. El HUD conserva proporciones, separa marcadores de interacción/avisos y mejora contraste y texto; la comprobación visual 3.3b sigue pendiente. No se adelanta el cierre humano ni el equilibrio.

**Actualización al 28 de octubre:** 3.4a terminada sin abrir el juego. `--expedition-report` prepara informes locales por inicio/carga, con relojes separados, contadores por hito y regreso asistido diferenciado. Validación del modelo/archivo en editor y compilación aprobadas; 3.3b, 3.4b y 3.5 siguen abiertos. No hay mediciones humanas nuevas ni cambios de equilibrio.

**Corrección del usuario, 3.2b:** retirado también el bloqueo físico del abismo; toda la fosa limita la salida por aire. No se cambiaron los multiplicadores ni se adelantó el cierre humano. Las referencias anteriores a compuertas son históricas.

La guía orienta hacia peces lentos de la orilla, calcula cuánto falta para el tridente según el catálogo y el valor real de la pesca, indica cuándo vender/comprar y acompaña el primer sello, cofre y regreso. Incluye instrucciones breves, distancia de captura, recuperación con mochila llena y prioridad de ascenso por presión/aire. Cargar una partida reconstruye los objetivos sin repetir compras ni recompensas. Detalles y prueba manual: `Docs/PRIMERA_EXPEDICION.md`.

Verificación: compilación Windows separada en `Builds/FirstExpedition`, validación del editor y **448 comprobaciones de integración aprobadas**. La captura de pantalla oculta no permitió revisar visualmente el HUD; no se sustituye con ello el criterio de cierre jugando.

Se conservan los precios, oxígeno, capacidad y alcance actuales para no confundir orientación con equilibrio sin jugar. El siguiente trabajo de esta etapa es medir el recorrido con una persona nueva, revisar legibilidad y ajustar los valores que realmente causen repetición.

**Objetivo:** pulir una experiencia pequeña de principio a fin antes de añadir sistemas grandes.

- [x] 1. Guiar al jugador desde la cabaña hasta una zona segura de pesca manual.
- [x] 2. Explicar mirar, nadar, respirar, capturar, vender y comprar mediante acciones cortas.
- [x] 3. Mantener manos → tridente → arpón y hacer visible la utilidad de cada compra.
- [x] 4. Integrar el primer puzle, su sello y una recompensa de equipo que permita preparar la siguiente salida.
- [ ] 5. Ajustar capacidad, distancia de captura, oxígeno y precios para reducir viajes repetitivos sin decisiones interesantes.

**Criterio de cierre:** una persona que no conozca el proyecto puede empezar sin equipo, capturar, vender, comprar el tridente, resolver el primer mecanismo, regresar y continuar su partida. Se comprueba jugando, además de con pruebas automáticas.

**Primer hito jugable:** al terminar las etapas 1–3 existe una expedición completa y persistente. Este es el primer objetivo de implementación recomendado.

## Etapa 4 Dar una función de expedición a la lancha

**Objetivo:** que la compra de la embarcación cambie la forma de explorar.

**Estado al 8 de noviembre:** primera entrega técnica implementada; no sustituye la evaluación humana pendiente de las etapas 1–3. El amarre conserva libre el eje de descenso y no añade paredes de progresión.

- [x] 1. Reutilizar el bote conducible como primera lancha, con un nombre consistente en tienda y juego.
- [x] 2a. Añadir amarre señalizado sobre la fosa: G desde lancha próxima, bloqueo de movimiento amarrada y desembarco/reembarco. Guardar/restaurar amarre y aceptar partidas antiguas desamarradas.
- [ ] 2b. Añadir zona de almacenamiento con transferencias entre mochila y bodega, límites claros y carga persistente sin duplicaciones.
- [ ] 3. Permitir preparar la salida y reponer aire en un lugar definido de la embarcación.
- [ ] 4. Guardar posición, propiedad y carga, y asegurar el regreso si el jugador abandona la lancha.

**Criterio de cierre:** comprar la lancha reduce desplazamientos desde la playa y permite transportar recursos sin duplicaciones. El jugador puede embarcar, bucear y volver a ella.

**Decisión de esta etapa:** qué suministros repone gratis y cuáles requieren recursos. La moto de agua queda como opción secundaria.

## Etapa 5 Diversificar los puzles y las pruebas

**Objetivo:** ofrecer nuevos problemas que comprender, además de tableros mayores.

- [ ] 1. Conservar y pulir la familia de canales de piedra.
- [ ] 2. Añadir una segunda familia, por ejemplo piedras que desvían corrientes para abrir un paso.
- [ ] 3. Añadir después una prueba basada en luz, pesos o comportamiento de fauna; elegir una y terminarla antes de iniciar la siguiente.
- [ ] 4. Diseñar una presentación sencilla de cada regla, seguida de una variación que combine lo aprendido.
- [ ] 5. Conectar cada solución a un cambio visible del mundo y guardar ese cambio.

**Criterio de cierre:** hay al menos dos familias con reglas distintas, pistas comprensibles, reinicio o recuperación y una consecuencia útil en la exploración.

**Puntos de partida:** `Interactable`, `StonePuzzleDefinition`, `StoneCircuitPuzzle` y los generadores de santuarios. Crear una interfaz común para condiciones de apertura cuando una segunda familia la necesite.

## Etapa 6 Convertir las capas en biomas

**Objetivo:** que bajar cambie lo que el jugador ve, escucha y hace.

- [ ] 1. Mejorar primero huiros y naufragio como dos entornos diferentes.
- [ ] 2. Crear un primer bioma fantástico con formas, iluminación, fauna y recursos propios.
- [ ] 3. Añadir una ruta principal clara, un desvío opcional y un atajo de regreso por bioma.
- [ ] 4. Distribuir la fauna real existente de acuerdo con el recorrido y añadir descubrimientos al cuaderno del jugador.
- [ ] 5. Incorporar ambiente sonoro y una pista visual del siguiente lugar que se puede explorar.

**Criterio de cierre:** se distinguen los biomas sin depender de sus nombres en pantalla; cada uno aporta una prueba, un descubrimiento y una recompensa propios.

**Puntos de partida:** dividir gradualmente `WorldBuilder` en generadores por bioma y conservar definiciones editables. Revisar profundidad, límites de rescate y rendimiento antes de extender el fondo actual.

## Etapa 7 Añadir fauna fantástica y peligro legible

**Objetivo:** introducir tensión sin desplazar la exploración y los puzles.

- [ ] 1. Crear una criatura fantástica pacífica y una territorial para probar el lenguaje visual y de comportamiento.
- [ ] 2. Implementar aviso, persecución limitada, retirada y recuperación del comportamiento normal.
- [ ] 3. Ofrecer una ruta de evasión y comprobar que las criaturas no atraviesen paredes ni bloqueen permanentemente una prueba.
- [ ] 4. Añadir la ballesta cuando exista un comportamiento que justifique diferenciarla del arpón.
- [ ] 5. Ajustar el valor de capturas peligrosas frente al tiempo y recursos que exigen.

**Criterio de cierre:** el jugador puede reconocer el peligro antes del daño y elegir entre evitarlo o enfrentarlo. Las amenazas permiten completar y abandonar la expedición.

**Decisiones de esta etapa:** consecuencias de derrota, munición y papel exacto de cada arma. No cambiar el rescate actual de forma implícita.

## Etapa 8 Construir la base sobre la fosa

**Objetivo:** establecer un centro de operaciones que haga más cómodas las expediciones profundas.

- [ ] 1. Empezar con una plataforma comprable de ubicación fija y ampliaciones prediseñadas.
- [ ] 2. Integrar almacenamiento, preparación del equipo, amarre y punto de retorno.
- [ ] 3. Elegir e implementar un mecanismo de descenso: propuesta inicial, cable guía con anclajes activados por el jugador.
- [ ] 4. Permitir acceso rápido solo a destinos ya descubiertos y abiertos.
- [ ] 5. Guardar mejoras, almacenamiento y destinos, con una salida segura si no se puede usar un acceso.

**Criterio de cierre:** la base ahorra trayectos repetidos sin saltarse capas nuevas, puzles ni requisitos de equipo. Comprar un módulo produce una mejora utilizable.

**Dependencias:** guardado estable, lancha funcional, requisitos de capas y al menos dos capas completas.

## Etapa 9 Incorporar exploradores contratados

**Objetivo:** complementar los ingresos de las expediciones personales.

- [ ] 1. Crear un puesto de encargos con un explorador y pocos destinos ya descubiertos.
- [ ] 2. Mostrar coste, duración, riesgo y recompensas posibles antes de aceptar.
- [ ] 3. Registrar el encargo, su finalización y su recompensa de forma que cargar la partida no permita cobrar dos veces.
- [ ] 4. Mantener el acceso a capas nuevas y la resolución principal en manos del jugador.
- [ ] 5. Medir si los ingresos ayudan a preparar expediciones sin volver irrelevante pescar y explorar.

**Criterio de cierre:** contratar, esperar y recoger resultados funciona al guardar y cargar, con límites comprensibles y sin recompensa duplicada.

**Decisión de esta etapa:** reloj de juego o tiempo real, funcionamiento sin conexión y consecuencias del riesgo. La primera versión puede usar un tablón de encargos sin simular físicamente el recorrido completo del explorador.

## Etapa 10 Equilibrar y ampliar con una plantilla común

**Objetivo:** preparar una versión consistente que pueda crecer sin rehacer los sistemas.

- [ ] 1. Medir tiempo de expedición, ingresos, frecuencia de rescate y tiempo para conseguir cada mejora.
- [ ] 2. Ajustar precios y recompensas para que el dinero apoye el descubrimiento y no obligue a repetir capturas sin propósito.
- [ ] 3. Mejorar interfaz, señales de interacción, sonido, animaciones y opciones de sensibilidad y volumen.
- [ ] 4. Revisar rendimiento con fauna y biomas completos, y cargar por zonas si la escala lo necesita.
- [ ] 5. Añadir capas con una plantilla: identidad, fenómeno, fauna, recurso, prueba, recompensa, acceso y regreso.

**Criterio de cierre:** una partida nueva recorre el contenido disponible sin bloqueos; el progreso persiste; las herramientas tienen usos distintos; la dificultad y el rendimiento se comprueban jugando.

## Reglas para ejecutar el plan

Trabajar una etapa o una entrega acotada de ella por vez. Antes de editar, revisar las dependencias y preservar los cambios existentes. Después, compilar, comprobar las reglas afectadas y recorrer el resultado en el juego. Registrar lo terminado y lo pendiente en la documentación.

No hace falta multiplicar sistemas al principio: la construcción libre de bases, las simulaciones complejas de exploradores y grandes cantidades de criaturas pueden esperar a que la primera expedición y la segunda capa resulten entretenidas.

No se fijan fechas sin estimar el trabajo y comprobar el prototipo. El criterio para avanzar es cumplir la entrega jugable de la etapa anterior.

## Encargo inicial de referencia (implementado)

> Lee `Docs/CONTEXTO_JUEGO.md`, `Docs/PLAN_MEJORAS.md`, `README.md` y `Docs/MAPA.md`. Implementa únicamente la etapa 1: guardado y carga del progreso. Conserva el inicio con manos, la compra del tridente antes del arpón, la pesca, la tienda, los vehículos y los tres mecanismos de piedra. Incluye dinero, inventario, equipo, tesoros, estados de los puzles y embarcaciones. Añade Nueva partida y Continuar. Verifica cerrar y cargar, compras y tesoros sin duplicación, y recuperación ante un archivo de guardado inválido. Documenta lo implementado y sus límites.

Este encargo se implementó el 18 de octubre de 2026 y después se añadieron los accesos de la etapa 2 y la primera entrega de guía de la etapa 3. La comprobación manual pendiente y los límites están registrados en `Docs/PRUEBAS.md`; corresponde continuar el pulido y equilibrio de la etapa 3 antes de ampliar a la lancha.
