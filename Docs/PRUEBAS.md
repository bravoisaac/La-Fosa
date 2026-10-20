# Validación

## Entrega 3.2: descenso libre y peligro por aire (19 de octubre de 2026)

- [x] Compilación `Builds/FirstExpedition/La Fosa.exe` y validación del editor correctas: `LA_FOSA_BUILD_OK`, `LA_FOSA_TESTS_OK`. Log: `unity-descent-build.log`.
- [x] **458 comprobaciones de integración aprobadas**, proceso con salida 0. Evidencia: `runtime-descent-test.log` y `Builds/FirstExpedition/Verification/result.txt`. Se revisaron las expectativas anteriores de dos compuertas; el total no representa simplemente pruebas añadidas a la versión de 448.
- [x] Movimiento real del CharacterController por centro y laterales del primer descenso, ida/vuelta sin tesoro; ausencia de compuerta del naufragio y conservación de la compuerta cerrada del abismo, su cobertura, apertura, retorno y persistencia.
- [x] Consumo normal dentro del límite, ×8 fuera, ×12 acelerando, pausa sin consumo, recuperación de consumo normal al ascender y recarga de superficie. Aire agotado causa asfixia; rescate restaura vitales y conserva hallazgos. No hay daño directo de presión con aire disponible.
- [x] Acceso físico libre no concede tesoros ni desbloquea el segundo mecanismo. Se mantienen compra, piedras, palanca, cofre, mochila llena, orientación, equipo y persistencia. La guía ya no señala la escotilla retirada.
- [x] Reinicio real en dos procesos: **7 comprobaciones al escribir y 5 al cargar**, ambos con salida 0; progreso exacto, tesoro sin duplicación y mejoras aplicadas una vez. Logs: `runtime-descent-restart-write.log` y `runtime-descent-restart-read.log`. Resultados: `restart-write.txt` / `restart-read.txt` en `Verification`.
- [x] Registro de pruebas corregido: cada modo comienza como `RUNNING`, un error deja `FAILED` y solo completar la integración escribe `PASSED`. Reinicio y vista de agua usan sus propios archivos de resultado para no mezclar evidencias ni conservar aprobaciones antiguas tras un fallo.
- [ ] Revisión visual del HUD en ventana renderizada, recorrido con persona nueva y equilibrio. Ficha en `Docs/PRIMERA_EXPEDICION.md`; **la etapa 3 no está cerrada**.

La primera ejecución detectó una prueba de puzles que aún exigía la compuerta retirada; se corrigió y se repitió toda la integración. Una repetición también falló en el regreso vertical calculado completo dentro de un solo frame: la prueba ahora deja avanzar la simulación entre pasos del ascenso para que la fauna pueda moverse. Unity necesitó acceso fuera del entorno restringido a la licencia local. El reemplazo de la partida aislada de reinicio falló dentro de las restricciones y pasó al ejecutar fuera de ellas. Las pruebas usan `Verification/SaveTests-*` y `Verification/RestartSave`; no utilizan la partida personal. Las capturas de cámara verifican geometría, no el HUD ni la facilidad de aprendizaje.

## Etapa 3: primera entrega de guía (18 de octubre de 2026)

- Compilación aislada `Builds/FirstExpedition/La Fosa.exe`: correcta, `LA_FOSA_BUILD_OK`; validación del editor: `LA_FOSA_TESTS_OK`.
- **448 comprobaciones de integración aprobadas**, incluidas las 422 anteriores y 26 de primera expedición. Resultado final en `Builds/FirstExpedition/Verification/result.txt`; `errors.txt` vacío; proceso con salida 0. Log: `runtime-expedition-test.log`.
- Casos nuevos: pesca costera disponible, valor real y precio editable, reunir el coste en una salida, venta, compra/equipado, circuito listo, sello, cofre, mochila llena, regreso, escotilla de ascenso, prioridad por presión, exploración fuera del orden sugerido y continuidad tras guardar/cargar. Las pruebas manipulan posiciones/objetos y resuelven el circuito de forma programática; no equivalen a una persona aprendiendo a jugar.
- La primera ejecución pasó 447 comprobaciones. Después se ajustó el marcador de ascenso bajo losas, se añadió su comprobación y se repitió la integración completa: 448 correctas.
- La captura mediante `ScreenCapture` en las ejecuciones ocultas produjo imágenes negras, también sin `-batchmode`. **No se verificó visualmente el HUD.** Las capturas `guia-*.png` no se presentan como prueba visual.
- Pendientes: comprobar legibilidad del HUD y señales, recorrido humano de las etapas 1–3 y equilibrio de tiempo/precios/aire. Procedimiento en `Docs/PRIMERA_EXPEDICION.md`. No se da por cerrada la etapa 3.

## Base anterior y comprobaciones conservadas

Verificado con Unity **6000.3.26f1**, Windows x64 y renderizador integrado. Fauna: 17 de octubre de 2026. Ampliación de puzles de piedras: 18 de octubre de 2026.

- Compilación del ejecutable: correcta (`LA_FOSA_BUILD_OK`).
- Validación de reglas en el editor: correcta (`LA_FOSA_TESTS_OK`). Incluye pilas, rechazo atómico por capacidad, entradas inválidas y profundidad alcanzable. Los tres circuitos se comprueban con mezcla inicial sin resolver, cuatro giros equivalentes a la posición original, reinicio, solución conectada, rechazo de una ruta rota, equivalencia de rectas giradas 180 grados y bloqueo de cambios tras abrir el sello.
- **422 comprobaciones de integración en el ejecutable: correctas** (`LA_FOSA_SMOKE_OK · 422`, 18 de octubre de 2026). Incluyen las 355 comprobaciones anteriores, 34 de persistencia y 33 de accesos entre capas. Se conservan las 49 especies, sus 132 ejemplares, referencias de modelos/ítems, profundidad y posición sobre el fondo, captura única y venta exacta de cada especie, venta mixta y conservación de conchas/tesoros. Se mantienen inicio con manos, rechazo de captura lejana, captura cercana, bloqueo de herramientas, tridente como primera compra y requisito para arpón/red incluso con saldo. También se verifican oxígeno, puzles, tesoros, compras duplicadas, equipo, reposición de peces, navegación y rescate; la bombona se acumula con los tres tesoros.
- Validación de modelos en el editor: 49 IDs únicos, 49 mallas distintas por geometría, vértices finitos, precios positivos y prefabs con colisión y referencia de especie. Se detectó y corrigió una potencia fraccionaria de un seno negativo por redondeo en los extremos de las mallas.
- Las 49 vistas de prefabs se renderizaron desde el ejecutable y se revisaron juntas en `Docs/peces-catalogo.jpg`. El pejesapo se inspeccionó además a tamaño completo. Son modelos estilizados; la revisión visual no constituye una validación científica de anatomía.
- Sin errores ni excepciones de la aplicación en la ejecución final (`Builds/Windows/Verification/errors.txt` vacío).
- IA de peces: 13 comprobaciones adicionales cubren perfiles de velocidad, detección y huida costera, captura manual después de perseguir a 6 m/s, persecución marina de seis segundos a 12,15 m/s, fallo de alcance del tridente y éxito del arpón. También comprueban ocultación detrás de una pared, reacción al recuperar visión, pausa, desvío ante un obstáculo, regreso gradual y permanencia bajo la superficie. Las persecuciones usan pasos de simulación de 0,05 s con el movimiento real de la IA y consultas físicas; el ajuste de dificultad cerca de rincones requiere además juego manual.
- Puzles de piedras: se verificaron las 27 piezas y las tres palancas mediante el raycast de interacción del jugador. Comprobados reinicio local, conservación al regresar a playa, bloqueo del siguiente nivel, giro visible, iluminación, activación explícita, imposibilidad de reiniciar un sello abierto y cofre con mochila llena. Las pruebas resuelven los tableros con interacciones programadas; queda la prueba humana de dificultad y comodidad.
- Revisadas las capturas finales del segundo mural y del tercer mural resuelto. Se movió el indicador de corriente al encabezado para no superponerlo a la salida inferior del segundo tablero. Capturas: `03-nivel.png` a `05-nivel.png` y `puzle-1-resuelto.png` a `puzle-3-resuelto.png`.
- Capturas de cámara de la playa y los tres niveles revisadas; se corrigieron tamaño y oclusión de rótulos e iluminación del abismo. Se encuentran en `Builds/Windows/Verification/`.
- Cabaña y bote revisados en las capturas `07-cabana.png` y `08-embarcaciones.png`. El rótulo del almacén queda fijo en su fachada para evitar que se incruste al mirar de lado.
- Las pruebas de integración usan teletransporte y llamadas de interacción; no sustituyen el recorrido manual con teclado/ratón. Las capturas de cámara no incluyen el HUD IMGUI. La navegación completa, la presentación de la interfaz en distintas pantallas y el equilibrio de dificultad quedan para prueba manual.

Logs locales: `unity-build.log`, `runtime-test.log`. Resultado resumido: `Builds/Windows/Verification/result.txt`.

Para repetir la integración, compila y ejecuta `"Builds/Windows/La Fosa.exe" --smoke-test`. Este modo es opcional, guarda capturas y resultados en `Verification` junto al ejecutable y se cierra automáticamente. El juego normal no ejecuta las pruebas.

## Nota de ajuste visual: bruma submarina

El 18 de octubre de 2026 se ajustó la visibilidad para que el fondo también quede difuminado desde fuera del agua. Bajo el agua, la costa usa densidad 0,035 y el interior de la fosa usa 0,065–0,115 según profundidad, hasta 95 m. La transición espacial es gradual entre los radios 31 y 23 m de la fosa. Iluminación y color acompañan esa transición. La bruma del aire sigue en 0,0035. Los tres valores submarinos son editables en `Expedicion.asset`.

El shader de superficie utiliza un GrabPass nombrado y cinco muestras para suavizar el fondo visible a través del agua. Desde fuera utiliza el mismo tono en todo el mar, con opacidad 0,85 mar adentro; una franja de 20 m desde la orilla interpola desde opacidad 0,65. El radio de desenfoque pasa de 3,5 a 4,5 píxeles a 900p: un punto intermedio con más suavizado en la orilla y menos en el mar. Añade hasta 0,04 de opacidad en ángulos rasantes. La máscara radial de la fosa ya no oscurece la superficie vista desde arriba. Se conservan el tono regional y la opacidad 0,3 de la superficie vista desde abajo. Las manos, tierra y HUD no reciben un desenfoque global. Es un efecto visual sencillo, no una simulación volumétrica del trayecto de cada rayo por distintas aguas. La técnica corresponde al renderizador Built-in de [Unity GrabPass](https://docs.unity3d.com/6000.0/Documentation/Manual/writing-shader-grabpass.html).

La escena activa la niebla durante el arranque procedural. Se configuró la conservación explícita de variantes Exp2 en Graphics Settings para evitar su eliminación por la selección automática de escenas; véase [la documentación de Unity sobre Graphics Settings](https://docs.unity.com/en-us/engine/6000.0/manual/unity-editor/editor-settings-reference/comp-manager-group/class-graphics-settings).

En la validación de integración previa a esta corrección de superficie, el recorrido terminó con **422 comprobaciones de integración aprobadas**, código 0 y sin errores, con capturas de playa, santuarios y compuertas. Tras la corrección se recompiló sin errores y se ejecutó el diagnóstico visual con código 0 y `LA_FOSA_WATER_PREVIEW_OK`. En el ajuste intermedio se revisaron el panorama de playa y la orilla: el mar deja entrever el fondo y la orilla presenta mayor atenuación, con una transición menos marcada. Los registros conservan las densidades submarinas 0,035 fuera y 0,067 dentro a igual profundidad. No se repitió el recorrido de integración para este cambio visual. Las vistas previas al ajuste se conservaron en `Verification/AntesBruma`. No se añadieron aserciones de lógica nuevas para este ajuste visual; el equilibrio de visibilidad durante pesca y exploración queda para apreciación jugando.

Diagnóstico visual adicional: ejecutar con `--smoke-test --water-preview` genera `agua-panorama-playa.png`, `agua-orilla.png`, `agua-costa-desde-arriba.png`, `agua-fosa-desde-arriba.png`, `agua-costa-sumergido.png`, `agua-fosa-sumergido.png` y `agua-fosa-profunda.png` en `Verification`. Permite comparar la transparencia de la orilla con la turbidez uniforme mar adentro y comprobar que se conserva la atmósfera submarina. Este modo captura imágenes y no constituye una prueba humana de jugabilidad ni una medición de rendimiento.

## Etapa 1: guardado y carga

- Compilación final correcta (`LA_FOSA_BUILD_OK`); validación de reglas del editor correcta. El primer intento en el entorno restringido no obtuvo licencia; la validación y compilación completaron con acceso a la licencia local.
- 34 comprobaciones nuevas: serialización/restauración completa, playa segura, compras y cofres sin duplicados, conchas recogidas, confirmación de sobrescritura, reinicio completo, giros parciales, respaldo de la expedición nueva, archivo ausente/corrupto, recuperación sin contaminar el respaldo, versión futura, IDs/cantidades/giros/progresión inválidos, coordenadas no finitas y fallo de escritura.
- Reinicio real del ejecutable: `--smoke-test --save-restart-write` terminó con **7 comprobaciones aprobadas**; después `--smoke-test --save-restart-read` terminó con **5 aprobadas**. El segundo proceso recuperó exactamente el snapshot del primero, conservó el cofre reclamado y reaplicó el oxígeno de tesoro/bombona una sola vez.
- Los tres procesos terminaron con código 0. Logs: `runtime-test.log`, `runtime-restart-write.log`, `runtime-restart-read.log`. Resultados: `Verification/result.txt`, `restart-write.txt`, `restart-read.txt`; sin errores de aplicación.
- Las pruebas usan carpetas aisladas en `Verification`; no acceden al guardado personal. Funcionamiento, formato y límites: `Docs/GUARDADO.md`.

Pendiente de comprobar por una persona con teclado y ratón:

1. Crear una partida, capturar y vender dos peces, comprar el tridente y cerrar normalmente. Abrir y pulsar Continuar; comprobar saldo, equipo y regreso a la playa.
2. Girar piedras sin resolver el primer mural, regresar y cerrar. Continuar y comprobar giros; resolver, reclamar, cerrar y comprobar que el cofre no vuelve a entregar el tesoro.
3. Comprar y mover una embarcación, desembarcar, guardar y cargar; comprobar amarre, propiedad y posibilidad de volver a embarcar.
4. Volver al menú desde pausa, pulsar Nueva partida y Cancelar. Continuar debe conservar la expedición; repetir y confirmar debe reiniciar todo.
5. Revisar legibilidad de menús y mensajes a la resolución utilizada. Con una copia de prueba del guardado, dañar el principal y comprobar el aviso de recuperación; si ambos archivos fallan, Continuar debe quedar deshabilitado.

## Etapa 2: accesos entre capas

- Compilación Windows y validación del editor correctas. La ejecución final terminó con código 0, **422 comprobaciones aprobadas** y ningún error de aplicación.
- 33 comprobaciones nuevas: dos compuertas inicialmente cerradas, requisito visible, compra de equipo insuficiente para abrirlas, sello resuelto que todavía pide recoger la mejora, acceso después del tesoro, marcador de escotilla/santuario, fauna fuera de las losas y conservación del estado al cargar/reiniciar o descartar tesoros.
- Cobertura del corte de la fosa: **5.957 puntos** en la barrera del naufragio y **1.693 puntos** en la del abismo, con malla de muestreo de 0,5 m. También se comprueba en el editor que los bordes exteriores de las losas queden enterrados en el terreno.
- Pruebas físicas con `CharacterController.Move` en pasos de 0,15 m: el centro y los laterales bloquean el descenso cuando están cerrados; la abertura permite bajar y subir al abrirse. Se recorren el trayecto hasta cada santuario y el regreso a superficie por las compuertas abiertas, con colisiones activas. Las pruebas colocan inicialmente al personaje por teletransporte y no reproducen el tiempo real consumido en una expedición humana.
- Compatibilidad: el ejecutable de esta etapa cargó la partida aislada creada en la etapa 1 mediante `--save-restart-read`, con **5 comprobaciones aprobadas** y código 0. Log: `runtime-stage1-compatibility.log`. No se modificó la versión del formato de guardado.
- Capturas en `Verification/compuerta-naufragio-cerrada.png`, `compuerta-naufragio-abierta.png`, `compuerta-abismo-cerrada.png` y `compuerta-abismo-abierta.png`. Se revisaron las vistas cerrada/abiertas y se corrigió la superposición de señales de entrada/retorno: cada rótulo se muestra desde su lado de la losa. Revisada de nuevo la primera abertura después del ajuste.

Pendiente de recorrido humano:

1. Intentar bajar antes de resolver huiros; comprobar que la losa bloquea y comunica lo que falta.
2. Resolver el mural, accionar ABRIR SELLO y comprobar que aún pide recoger la Perla. Recogerla y seguir el marcador hasta la escotilla.
3. Bajar al naufragio, orientarse hacia el mural y regresar nadando por la misma escotilla. Valorar si señalización y reserva de aire resultan suficientes.
4. Repetir con la Brújula y el acceso al abismo. Cerrar/Continuar y comprobar que los pasos se conservan; iniciar una partida nueva y comprobar que vuelven a cerrarse.
5. Evaluar dificultad, tiempos y comodidad junto con las pruebas manuales de guardado. Esta comprobación sigue pendiente; las pruebas automáticas no la sustituyen.

## Recorrido manual

Además de estas comprobaciones generales, seguir el recorrido de compuertas descrito en la sección de etapa 2.

1. Iniciar, mirar con el ratón y caminar hasta las boyas. Comprobar salto en tierra y entrada/salida del agua.
2. Bucear con WASD, Espacio y Ctrl. Confirmar el cambio de niebla, profundidad y consumo de oxígeno.
3. Empezar con las manos, acercarse a un jurel y hacer clic. Comprobar que no se capturan peces lejanos. Vender dos jureles y comprar el tridente; comprobar que el arpón requiere primero esta compra.
4. Recoger una concha con E. Abrir Tab, descartar una pila y volver al juego. El oxígeno no debe bajar en pausa.
5. Llegar al primer santuario; girar una piedra con E, comprobar que gira y cambia la corriente visible. Unir las seis piedras desde ENTRADA hasta SELLO. El cofre debe seguir cerrado hasta accionar ABRIR SELLO.
6. Confirmar equipo de 64 m / 155 s y aire repuesto. Un cofre abierto no debe dar recompensas repetidas.
7. Resolver el mural de nueve piedras en el segundo nivel; probar REINICIAR y volver a conectar todas las piezas. Accionar la palanca, abrir el cofre y descender al tercer nivel.
8. Resolver las doce piedras del abismo, accionar la palanca y recoger el tesoro. Confirmar el mensaje de expedición completada. Comprobar que una mochila llena no elimina el tesoro ni vuelve a cerrar el sello.
9. Probar falta de oxígeno y exceso de presión: debe haber daño y rescate al agotar salud. R también debe rescatar.
10. Esc y pérdida de foco deben pausar; continuar debe restaurar ratón y movimiento.
11. Visitar la cabaña a la izquierda del inicio y apuntar al mostrador. E abre la tienda; comprobar que el saldo comienza en cero, los botones sin saldo están deshabilitados y Esc cierra.
12. Capturar peces, venderlos y comprar las siete opciones. Comprobar la legibilidad de la tienda en la pantalla utilizada, las compras marcadas como adquiridas y el desplazamiento de la lista si se amplía el catálogo.
13. Seleccionar manos/tridente/arpón/red con 1/2/3/4; comprobar la red bajo el agua y desde una embarcación, y verificar que los obstáculos impiden capturas a través de ellos.
14. Recoger bote y moto en ambos lados del muelle. Probar W/S, A/D, E, pausa y R; comprobar obstáculos, límite de costa y espacio libre al desembarcar.

## Límites conocidos de la base

- Una partida local con respaldo; carga desde la playa. Sin guardado en nube, ranuras múltiples ni migraciones automáticas de versiones/contenido.
- Paisaje, peces y arpón con mallas simples; sin audio ni animaciones de personaje.
- Fosa comprimida en profundidad y puzles de canales giratorios; compuertas funcionales con apertura inmediata, todavía sin biomas o túneles detallados.
- La UI está diseñada para 12 espacios; ampliar requiere paginación.
