# La Fosa contexto de diseño para Codex

Este es el contexto de diseño de **La Fosa**. Explica la experiencia que se quiere construir y las decisiones ya tomadas. El estado técnico actual está en `README.md`; el orden de desarrollo está en `Docs/PLAN_MEJORAS.md`. Una función descrita como objetivo no debe darse por implementada.

## Concepto

La Fosa es un juego de exploración submarina, aventura y puzles en primera persona, desarrollado en Unity con estética 3D low poly. Comienza en una caleta chilena y gira alrededor de una enorme fosa marina misteriosa.

La inspiración de Made in Abyss es la curiosidad por descender, el asombro ante entornos desconocidos y el peligro creciente. El mundo, los personajes, la historia, las criaturas y los mecanismos de La Fosa serán propios.

Cerca de la superficie, el jugador reconoce una costa chilena y peces reales. Conforme desciende, aparecen biomas extraños, fauna fantástica, ruinas y fenómenos que sugieren que la fosa oculta algo mayor. La pregunta que debe impulsar la partida es: **«¿Qué habrá un poco más abajo?»**

## Objetivo y prioridades

El objetivo principal es descender, descubrir los secretos de la fosa y prepararse para continuar. El destino final y la explicación del origen de la fosa están pendientes de definir.

Las prioridades de diseño son:

1. **Exploración y descubrimiento:** cada capa debe ofrecer algo que el jugador quiera conocer.
2. **Puzles y pruebas del entorno:** observar, comprender y actuar permite abrir nuevos caminos.
3. **Preparación y progreso:** las herramientas y la infraestructura hacen posibles nuevas expediciones.
4. **Economía al servicio de la exploración:** pescar y recuperar hallazgos financia el siguiente descenso.
5. **Peligro creciente:** la profundidad exige más cuidado; combatir es una opción, junto a evitar amenazas y buscar otras rutas.

No todas las recompensas tienen que ser dinero. Una ruta descubierta, una pista, un atajo o un registro de fauna también deben hacer que una expedición valga la pena.

## Decisiones confirmadas

| Aspecto | Dirección del proyecto |
|---|---|
| Motor y presentación | Unity, 3D low poly, primera persona |
| Punto de partida | Caleta chilena con una cabaña que funciona como tienda |
| Inicio del jugador | Sin armas; captura peces con las manos |
| Primera herramienta | Tridente; después se accede al arpón |
| Actividad central | Explorar la fosa y superar puzles o pruebas para avanzar |
| Economía | Vender pesca y hallazgos para comprar herramientas e infraestructura |
| Herramienta futura | Ballesta adaptada al uso submarino; su posición exacta en la progresión está pendiente |
| Infraestructura | Pasar de inmersiones desde la costa a una lancha y, después, una base sobre la fosa |
| Apoyo económico futuro | Contratar exploradores para enviar expediciones |
| Evolución del entorno | De fauna costera reconocible a biomas extraños y fauna fantástica, con más amenazas al bajar |

## Ciclo de una expedición

**Preparar → descender → explorar y resolver pruebas → obtener capturas y hallazgos → regresar → vender y mejorar → descender más.**

Antes de salir, el jugador decide qué llevar y qué objetivo perseguir. Bajo el agua administra aire, capacidad de carga y exposición al peligro. Al regresar puede invertir en herramientas, equipo, transporte o infraestructura.

La siguiente meta debe ser comprensible: por ejemplo, una entrada bloqueada que requiere una herramienta, una corriente que se puede desviar mediante piedras o una zona cuya presión exige mejor equipo. El jugador debe poder entender qué le falta y cómo conseguirlo.

## Estructura de la fosa

La ruta principal de descenso es fácil de reconocer. A su alrededor hay cuevas, desvíos, secretos y lugares opcionales. Una ruta legible no significa acceso libre al fondo: la preparación y las pruebas delimitan hasta dónde se puede avanzar.

Cada bioma debe tener identidad visual y sonora, fauna, recursos, un fenómeno ambiental y una prueba principal. La profundidad aumenta la extrañeza y el peligro, pero debe haber espacios de calma y referencias que permitan orientarse.

Los tres santuarios actuales son una base para desarrollar estas capas. Los 88 metros del prototipo son una escala jugable provisional, no el tamaño definitivo de la fosa.

**Propuesta inicial de evolución de los biomas:**

| Capa | Sensación | Prueba posible | Papel de la fauna |
|---|---|---|---|
| Costa y bosque de huiros | Familiar, luminosa, segura para aprender | Unir canales de piedra y descubrir el primer paso | Peces costeros y primeras capturas manuales |
| Naufragio y grietas | Intriga, corrientes y menor visibilidad | Desviar una corriente o equilibrar un mecanismo | Fauna real junto a las primeras especies extrañas |
| Jardines bioluminiscentes | Asombro y desorientación | Orientar luz o interpretar señales vivas | Criaturas fantásticas, algunas territoriales |
| Capas profundas | Oscuridad, presión y tensión | Combinar herramientas, observación y mecanismos | Depredadores y rutas que conviene evitar |

Los nombres, el número final de capas y sus profundidades todavía pueden cambiar.

## Puzles y pruebas

Los puzles deben formar parte del lugar: mecanismos de piedra, cauces, ruinas, corrientes, iluminación y comportamiento de criaturas. Resolverlos debe producir un cambio visible, como abrir una compuerta, detener una corriente o habilitar un atajo.

Los mosaicos de piedras giratorias ya construidos se conservan como una familia de puzles. Las futuras capas deben incorporar otras reglas para que avanzar no consista únicamente en repetir tableros más grandes.

Una prueba debe comunicar qué se puede manipular, mostrar la consecuencia de la acción y permitir entender un error. La dificultad debe venir de comprender el entorno, no de adivinar controles. El equipo puede permitir llegar a una prueba o interactuar con ella; comprarlo no sustituye resolverla.

## Pesca y equipo

Se conserva la progresión **manos → tridente → arpón**. La ballesta submarina será otra herramienta posterior, con una función que justifique utilizarla. Las redes ofrecen otra forma de capturar fauna.

Los peces de la orilla huyen lentamente y se pueden alcanzar con las manos; el tridente facilita la pesca cercana. Los peces del mar huyen más rápido que el buzo con aletas, por lo que el arpón permite capturarlos desde fuera del alcance cercano. Esta diferencia de IA ya está implementada. Como propuesta futura, la ballesta podría ofrecer mayor precisión a cambio de una recarga más lenta.

La fauna real del catálogo de 49 modelos sigue formando parte del mundo. La fauna fantástica se añadirá gradualmente, sin convertir automáticamente a todos los peces reales en enemigos. La ubicación y el valor de cada especie podrán revisarse para encajar en los nuevos biomas.

Las mejoras de aire, iluminación, movilidad, carga y resistencia ambiental deben abrir posibilidades claras. Los precios y tiempos actuales son valores del prototipo, no un equilibrio definitivo.

## Lancha y base sobre la fosa

La lancha debe ser un avance útil: facilitar el transporte, permitir organizar expediciones y acercar el punto de salida a la fosa. El bote actual aporta conducción, pero todavía no tiene todas estas funciones.

La base flotante será el centro de operaciones sobre la entrada de la fosa, con preparación del equipo, almacenamiento y apoyo a los descensos. Debe reducir viajes repetitivos a zonas ya dominadas.

**Propuesta por validar:** un cable guía y un sistema de descenso conectado a anclajes que el jugador active personalmente. Solo permitirían viajar a lugares ya alcanzados y desbloqueados. No se ha decidido todavía si habrá ascensor, cápsula, puntos de retorno u otro mecanismo.

La cabaña de la costa conserva su papel como tienda y punto de regreso. La moto de agua existente puede mantenerse como transporte opcional; no es un paso obligatorio de la progresión principal.

## Exploradores contratados

Los exploradores aportarán apoyo económico mediante encargos a zonas conocidas. Como propuesta inicial, cada encargo tendrá coste, duración, preparación y posibles resultados visibles antes de contratarlo.

El jugador seguirá siendo quien descubre nuevas capas y supera las pruebas principales. Los exploradores no deben abrir automáticamente la siguiente zona ni reemplazar la exploración personal.

La duración en tiempo de juego o tiempo real, el funcionamiento sin conexión y las consecuencias de una expedición fallida están pendientes. Conviene empezar con un sistema pequeño de encargos, antes de construir simulaciones completas de personajes autónomos.

## Fauna agresiva

La fauna podrá ser pacífica, huidiza, territorial o depredadora. La profundidad aumenta la presencia de amenazas, sin eliminar la diversidad de comportamientos.

Una criatura peligrosa debe mostrar señales que el jugador pueda aprender: postura, sonido, distancia de advertencia o reacción a la luz. Observarla, alejarse o usar una ruta alternativa debe ser viable. Las condiciones de derrota y pérdida de recursos requieren una decisión de diseño antes de implementarse.

## Qué existe y qué falta

**Ya existe en el prototipo:** movimiento y buceo en primera persona; oxígeno y presión; inventario; cabaña y tienda; venta de peces; manos, tridente, arpón y red; mejoras de aire y aletas; bote y moto de agua conducibles; 49 modelos de fauna y 132 individuos; tres mosaicos de 6, 9 y 12 piedras; cofres y mejoras de equipo.

**Todavía no está implementado:** guardado persistente; base flotante; exploradores; ballesta; fauna fantástica agresiva; capas con fenómenos ambientales propios; puertas físicas que vinculen los puzles al avance; viajes rápidos desbloqueables; historia desarrollada y sonido. El rescate actual conserva los objetos y el progreso solo dura la sesión.

## Criterios para trabajar con Codex

- Leer este contexto, el plan y el mapa de archivos antes de ampliar una mecánica relacionada.
- Construir sobre el proyecto actual y conservar las funciones que ya sirven al diseño.
- Distinguir decisiones confirmadas de propuestas. No presentar una propuesta como una petición ya aprobada.
- Trabajar por etapas jugables, con una condición observable para dar cada una por terminada.
- Mantener configurables biomas, especies, equipo, precios, requisitos y puzles.
- Comprobar que se puede empezar sin armas, obtener recursos, mejorar el equipo y avanzar sin bloquear la partida.
- Priorizar una expedición completa y entretenida antes de multiplicar el número de capas, criaturas o sistemas.

## Decisiones que se resolverán al llegar a su etapa

Consecuencias del rescate o derrota; modelo de compra y reposición de oxígeno; papel de munición y mantenimiento; habilidades concretas de la ballesta; número y profundidad de las capas; mecanismo de descenso desde la base; reglas de los exploradores; historia y final. No es necesario decidirlo todo para empezar a mejorar la primera expedición.
