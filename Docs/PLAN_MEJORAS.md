# La Fosa plan de mejora por etapas

El objetivo es transformar el prototipo actual en un juego de expediciones por una fosa misteriosa. Este plan desarrolla la visión de `Docs/CONTEXTO_JUEGO.md`. **Es un plan de trabajo: las funciones futuras que describe todavía no están implementadas.**

El orden de construcción no es idéntico al orden en que el jugador comprará el equipo. Primero hay que conseguir una expedición completa que se guarde, se entienda y permita avanzar; después se amplía el mundo.

## Etapa 0 Consolidar la dirección

**Estado: documentación preparada.**

Usar el contexto como referencia para conservar Unity, primera persona, low poly, caleta chilena e inicio con manos. Separar los elementos existentes de los objetivos futuros y de las propuestas abiertas.

**Resultado:** una visión común y este orden de desarrollo. La preparación de este plan no cambia el funcionamiento del juego.

## Etapa 1 Guardar y recuperar el progreso

**Objetivo:** que una expedición no se pierda al cerrar el juego.

1. Definir identificadores estables para objetos, zonas, puzles y vehículos.
2. Guardar dinero, inventario, equipo, tesoros, giros de piedras, zonas descubiertas y propiedad de embarcaciones.
3. Añadir Nueva partida y Continuar, con protección frente a sobrescribir una partida por accidente.
4. Guardar al volver a un lugar seguro, comprar equipo o recuperar un tesoro. Preparar versión del formato y copia de respaldo.

**Criterio de cierre:** cerrar y volver a abrir restaura una partida coherente, sin duplicar recompensas ni compras. Un archivo ausente o dañado recibe un tratamiento claro.

**Puntos de partida:** `GameSession`, `Inventory`, `Economy`, `StoneCircuitPuzzle`, `TreasureChest` y `SurfaceVehicle`.

## Etapa 2 Organizar los requisitos de cada descenso

**Objetivo:** que el jugador sepa qué necesita para avanzar.

1. Definir por capa su entrada, equipo necesario, prueba principal, recompensa y salida segura.
2. Separar la compra de equipo de la apertura de una ruta: tener mejor oxígeno no debe completar un puzle.
3. Introducir una compuerta real entre las primeras capas, conectada al mecanismo de piedra.
4. Mostrar el requisito que falta cuando el jugador intenta atravesar un paso bloqueado.
5. Mantener un regreso viable a la superficie y evitar que un objeto obligatorio pueda venderse o perderse sin recuperación.

**Criterio de cierre:** el siguiente paso está bloqueado antes de cumplir sus requisitos, se abre al resolver la prueba y permanece abierto al cargar la partida. No puede rodearse por un hueco accidental del escenario.

**Puntos de partida:** `DiveZoneDefinition`, `GameSession.IsZoneUnlocked`, `WorldBuilder`, puzles y cofres. La fosa actual usa principalmente daño por presión y orden de tesoros; hacen falta barreras de recorrido.

## Etapa 3 Completar la primera expedición

**Objetivo:** pulir una experiencia pequeña de principio a fin antes de añadir sistemas grandes.

1. Guiar al jugador desde la cabaña hasta una zona segura de pesca manual.
2. Explicar mirar, nadar, respirar, capturar, vender y comprar mediante acciones cortas.
3. Mantener manos → tridente → arpón y hacer visible la utilidad de cada compra.
4. Integrar el primer puzle, su compuerta y una recompensa que permita preparar la siguiente salida.
5. Ajustar capacidad, distancia de captura, oxígeno y precios para reducir viajes repetitivos sin decisiones interesantes.

**Criterio de cierre:** una persona que no conozca el proyecto puede empezar sin equipo, capturar, vender, comprar el tridente, resolver el primer mecanismo, regresar y continuar su partida. Se comprueba jugando, además de con pruebas automáticas.

**Primer hito jugable:** al terminar las etapas 1–3 existe una expedición completa y persistente. Este es el primer objetivo de implementación recomendado.

## Etapa 4 Dar una función de expedición a la lancha

**Objetivo:** que la compra de la embarcación cambie la forma de explorar.

1. Reutilizar el bote conducible como primera lancha, con un nombre consistente en tienda y juego.
2. Añadir amarre cerca de la entrada de la fosa y una zona de almacenamiento.
3. Permitir preparar la salida y reponer aire en un lugar definido de la embarcación.
4. Guardar posición, propiedad y carga, y asegurar el regreso si el jugador abandona la lancha.

**Criterio de cierre:** comprar la lancha reduce desplazamientos desde la playa y permite transportar recursos sin duplicaciones. El jugador puede embarcar, bucear y volver a ella.

**Decisión de esta etapa:** qué suministros repone gratis y cuáles requieren recursos. La moto de agua queda como opción secundaria.

## Etapa 5 Diversificar los puzles y las pruebas

**Objetivo:** ofrecer nuevos problemas que comprender, además de tableros mayores.

1. Conservar y pulir la familia de canales de piedra.
2. Añadir una segunda familia, por ejemplo piedras que desvían corrientes para abrir un paso.
3. Añadir después una prueba basada en luz, pesos o comportamiento de fauna; elegir una y terminarla antes de iniciar la siguiente.
4. Diseñar una presentación sencilla de cada regla, seguida de una variación que combine lo aprendido.
5. Conectar cada solución a un cambio visible del mundo y guardar ese cambio.

**Criterio de cierre:** hay al menos dos familias con reglas distintas, pistas comprensibles, reinicio o recuperación y una consecuencia útil en la exploración.

**Puntos de partida:** `Interactable`, `StonePuzzleDefinition`, `StoneCircuitPuzzle` y los generadores de santuarios. Crear una interfaz común para condiciones de apertura cuando una segunda familia la necesite.

## Etapa 6 Convertir las capas en biomas

**Objetivo:** que bajar cambie lo que el jugador ve, escucha y hace.

1. Mejorar primero huiros y naufragio como dos entornos diferentes.
2. Crear un primer bioma fantástico con formas, iluminación, fauna y recursos propios.
3. Añadir una ruta principal clara, un desvío opcional y un atajo de regreso por bioma.
4. Distribuir la fauna real existente de acuerdo con el recorrido y añadir descubrimientos al cuaderno del jugador.
5. Incorporar ambiente sonoro y una pista visual del siguiente lugar que se puede explorar.

**Criterio de cierre:** se distinguen los biomas sin depender de sus nombres en pantalla; cada uno aporta una prueba, un descubrimiento y una recompensa propios.

**Puntos de partida:** dividir gradualmente `WorldBuilder` en generadores por bioma y conservar definiciones editables. Revisar profundidad, límites de rescate y rendimiento antes de extender el fondo actual.

## Etapa 7 Añadir fauna fantástica y peligro legible

**Objetivo:** introducir tensión sin desplazar la exploración y los puzles.

1. Crear una criatura fantástica pacífica y una territorial para probar el lenguaje visual y de comportamiento.
2. Implementar aviso, persecución limitada, retirada y recuperación del comportamiento normal.
3. Ofrecer una ruta de evasión y comprobar que las criaturas no atraviesen paredes ni bloqueen permanentemente una prueba.
4. Añadir la ballesta cuando exista un comportamiento que justifique diferenciarla del arpón.
5. Ajustar el valor de capturas peligrosas frente al tiempo y recursos que exigen.

**Criterio de cierre:** el jugador puede reconocer el peligro antes del daño y elegir entre evitarlo o enfrentarlo. Las amenazas permiten completar y abandonar la expedición.

**Decisiones de esta etapa:** consecuencias de derrota, munición y papel exacto de cada arma. No cambiar el rescate actual de forma implícita.

## Etapa 8 Construir la base sobre la fosa

**Objetivo:** establecer un centro de operaciones que haga más cómodas las expediciones profundas.

1. Empezar con una plataforma comprable de ubicación fija y ampliaciones prediseñadas.
2. Integrar almacenamiento, preparación del equipo, amarre y punto de retorno.
3. Elegir e implementar un mecanismo de descenso: propuesta inicial, cable guía con anclajes activados por el jugador.
4. Permitir acceso rápido solo a destinos ya descubiertos y abiertos.
5. Guardar mejoras, almacenamiento y destinos, con una salida segura si no se puede usar un acceso.

**Criterio de cierre:** la base ahorra trayectos repetidos sin saltarse capas nuevas, puzles ni requisitos de equipo. Comprar un módulo produce una mejora utilizable.

**Dependencias:** guardado estable, lancha funcional, requisitos de capas y al menos dos capas completas.

## Etapa 9 Incorporar exploradores contratados

**Objetivo:** complementar los ingresos de las expediciones personales.

1. Crear un puesto de encargos con un explorador y pocos destinos ya descubiertos.
2. Mostrar coste, duración, riesgo y recompensas posibles antes de aceptar.
3. Registrar el encargo, su finalización y su recompensa de forma que cargar la partida no permita cobrar dos veces.
4. Mantener el acceso a capas nuevas y la resolución principal en manos del jugador.
5. Medir si los ingresos ayudan a preparar expediciones sin volver irrelevante pescar y explorar.

**Criterio de cierre:** contratar, esperar y recoger resultados funciona al guardar y cargar, con límites comprensibles y sin recompensa duplicada.

**Decisión de esta etapa:** reloj de juego o tiempo real, funcionamiento sin conexión y consecuencias del riesgo. La primera versión puede usar un tablón de encargos sin simular físicamente el recorrido completo del explorador.

## Etapa 10 Equilibrar y ampliar con una plantilla común

**Objetivo:** preparar una versión consistente que pueda crecer sin rehacer los sistemas.

1. Medir tiempo de expedición, ingresos, frecuencia de rescate y tiempo para conseguir cada mejora.
2. Ajustar precios y recompensas para que el dinero apoye el descubrimiento y no obligue a repetir capturas sin propósito.
3. Mejorar interfaz, señales de interacción, sonido, animaciones y opciones de sensibilidad y volumen.
4. Revisar rendimiento con fauna y biomas completos, y cargar por zonas si la escala lo necesita.
5. Añadir capas con una plantilla: identidad, fenómeno, fauna, recurso, prueba, recompensa, acceso y regreso.

**Criterio de cierre:** una partida nueva recorre el contenido disponible sin bloqueos; el progreso persiste; las herramientas tienen usos distintos; la dificultad y el rendimiento se comprueban jugando.

## Reglas para ejecutar el plan

Trabajar una etapa o una entrega acotada de ella por vez. Antes de editar, revisar las dependencias y preservar los cambios existentes. Después, compilar, comprobar las reglas afectadas y recorrer el resultado en el juego. Registrar lo terminado y lo pendiente en la documentación.

No hace falta multiplicar sistemas al principio: la construcción libre de bases, las simulaciones complejas de exploradores y grandes cantidades de criaturas pueden esperar a que la primera expedición y la segunda capa resulten entretenidas.

No se fijan fechas sin estimar el trabajo y comprobar el prototipo. El criterio para avanzar es cumplir la entrega jugable de la etapa anterior.

## Encargo inicial sugerido para Codex

> Lee `Docs/CONTEXTO_JUEGO.md`, `Docs/PLAN_MEJORAS.md`, `README.md` y `Docs/MAPA.md`. Implementa únicamente la etapa 1: guardado y carga del progreso. Conserva el inicio con manos, la compra del tridente antes del arpón, la pesca, la tienda, los vehículos y los tres mecanismos de piedra. Incluye dinero, inventario, equipo, tesoros, estados de los puzles y embarcaciones. Añade Nueva partida y Continuar. Verifica cerrar y cargar, compras y tesoros sin duplicación, y recuperación ante un archivo de guardado inválido. Documenta lo implementado y sus límites.

Este encargo es una propuesta para la siguiente tarea; no se ha ejecutado como parte de la preparación del plan.
