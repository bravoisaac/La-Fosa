# Validación

Verificado con Unity **6000.3.26f1**, Windows x64 y renderizador integrado. Fauna: 17 de octubre de 2026. Ampliación de puzles de piedras: 18 de octubre de 2026.

- Compilación del ejecutable: correcta (`LA_FOSA_BUILD_OK`).
- Validación de reglas en el editor: correcta (`LA_FOSA_TESTS_OK`). Incluye pilas, rechazo atómico por capacidad, entradas inválidas y profundidad alcanzable. Los tres circuitos se comprueban con mezcla inicial sin resolver, cuatro giros equivalentes a la posición original, reinicio, solución conectada, rechazo de una ruta rota, equivalencia de rectas giradas 180 grados y bloqueo de cambios tras abrir el sello.
- **355 comprobaciones de integración en el ejecutable: correctas** (`LA_FOSA_SMOKE_OK · 355`). Incluyen las 49 especies, sus 132 ejemplares, referencias de modelos/ítems, profundidad y posición sobre el fondo, captura única y venta exacta de cada especie, venta mixta y conservación de conchas/tesoros. Se mantienen inicio con manos, rechazo de captura lejana, captura cercana, bloqueo de herramientas, tridente como primera compra y requisito para arpón/red incluso con saldo. También se verifican oxígeno, puzles, tesoros, compras duplicadas, equipo, reposición de peces, navegación y rescate; la bombona se acumula con los tres tesoros.
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

## Recorrido manual

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

- Progreso solamente en memoria; reiniciar borra la sesión.
- Paisaje, peces y arpón con mallas simples; sin audio ni animaciones de personaje.
- Fosa comprimida en profundidad y puzles de secuencia como ejemplo extensible.
- La UI está diseñada para 12 espacios; ampliar requiere paginación.
