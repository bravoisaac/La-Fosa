# Rutas entre capas

Estado vigente al 19 de octubre de 2026: el primer descenso, entre huiros y naufragio, es libre y no tiene losa ni escotilla. Superar la profundidad segura consume oxígeno ×8 (×12 al acelerar), sin daño directo de presión mientras quede aire. El HUD avisa y orienta el ascenso. Al agotar el aire se conserva el daño por asfixia y el rescate con inventario.

El abismo conserva una compuerta física a 64 m: una losa encajada en las paredes deja una escotilla central de 8 m. El panel se aparta al completar el mecanismo del naufragio y recoger su tesoro. Nadar hasta el naufragio sin equipo es posible, pero no permite saltarse el orden de sus puzles y recompensas.

| Capa | Entrada | Equipo mínimo (capacidad) | Prueba y recompensa | Regreso |
|---|---|---|---|---|
| Bosque de huiros | Boyas, entrada libre cerca de `(0, -2, 46)` | 24 m / 115 s; el equipo inicial lo cumple | Mural de 6 piedras, activar sello y recoger Perla: 64 m / 155 s | Ascender hacia las boyas; R permite rescate |
| Ecos del naufragio | Descenso libre por `(0, -32, 75)` y sus laterales | Para el mecanismo: 52 m / 155 s, sello de huiros abierto y Perla recogida | Mural de 9 piedras y Brújula: 100 m / 200 s | Ascenso libre hacia superficie; R permite rescate |
| Corazón del abismo | Escotilla a 64 m, centro `(0, -64, 75)` | 88 m / 200 s, sello del naufragio abierto y Brújula recogida | Mural de 12 piedras y Corazón: 110 m / 240 s | Ascender por la escotilla abierta y continuar hasta superficie; R sigue disponible |

Las cifras de aire son capacidad del equipo, no el aire restante exigido para cruzar. En el naufragio son requisitos de progreso del mecanismo, no una barrera al movimiento. La bombona de tienda sigue sumando 60 s. Su compra no abre sellos ni sustituye los tesoros. Un tablero conectado todavía requiere activar ABRIR SELLO; después hay que recoger el cofre para recibir la mejora y, en el naufragio, abrir el paso al abismo. Si la mochila está llena, el cofre y la solución esperan al jugador.

Hacia el naufragio, el marcador apunta al santuario; el regreso apunta directamente a superficie. Hacia el abismo, dirige primero a la escotilla y después al santuario. Su baliza y rótulos indican el estado del paso. E sobre la compuerta o chocar con ella explica la condición pendiente. Debajo de la losa hay una señal de superficie y cristales; al regresar desde el abismo abierto, la guía señala primero la escotilla superior.

## Persistencia y objetos

Las compuertas consultan los mecanismos y tesoros ya guardados. No añaden otro indicador de apertura que pudiera quedar desincronizado. Continuar las restaura inmediatamente, incluso durante la pausa del menú; Nueva partida las cierra. Se comprobó la carga de la partida de prueba creada durante la etapa 1 con su formato v1.

El requisito es **haber recuperado** el tesoro. Descartar su objeto no elimina las mejoras ni cierra la ruta. La tienda sigue vendiendo solamente peces. No se introduce una llave que el jugador pueda perder y que bloquee permanentemente el descenso.

## Configurar y ampliar

En cada `DiveZoneDefinition`, `hasPhysicalGate` controla si se construye una losa con escotilla; `entryPosition` define la entrada, `requiredSafeDepth` y `requiredOxygen` definen la preparación, `barrierWidth` cubre la sección de la fosa y `returnHint` explica la salida. La primera zona es de acceso libre; cada zona posterior mantiene el requisito del tesoro anterior para su mecanismo, incluso sin barrera física. `GameConfig.pressureOxygenMultiplier` configura el coste de aire fuera del límite seguro (8 actualmente).

`WorldBuilder.Gates` construye las losas y escotillas; `DiveGate` controla panel, señalización e interacción. El panel cambia de posición inmediatamente: la animación mecánica gradual queda para pulido posterior. Los colliders son cajas de doble cara con juntas solapadas, y el perímetro exterior debe quedar enterrado en el terreno. No basta con ampliar una puerta si se modifica el paisaje.

`ProjectSetup.Validate` comprueba la posición entre santuarios, equipo alcanzable y perímetros enterrados. `SmokeCheck.Gates` muestrea la superficie cerrada y usa el CharacterController real para intentar atravesar centro/laterales, cruzar una escotilla abierta, alcanzar el siguiente santuario rodeando el mural y regresar a la superficie. La posición de la fauna evita las losas desde su generación.

Antes de cambiar estas posiciones en una versión distribuida hay que revisar colisiones, visibilidad, aparición de fauna, tiempos de aire y compatibilidad con partidas. La incorporación actual conserva el retorno a la playa al cargar y las posiciones guardadas de las embarcaciones de superficie.

## Límites

Son accesos funcionales sobre la fosa actual; todavía no son biomas nuevos ni túneles detallados. Las pruebas de movimiento programado y las capturas no sustituyen un recorrido humano para comprobar orientación, dificultad y comodidad. La siguiente etapa del plan trabaja la primera expedición y sus indicaciones.
