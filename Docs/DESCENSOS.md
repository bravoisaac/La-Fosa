# Rutas entre capas

La etapa 2 añade dos compuertas físicas. Ya no basta con nadar más profundo alrededor de los santuarios: unas losas de piedra encajadas en las paredes de la fosa dejan una única escotilla central de 8 m. El panel se aparta al completar el mecanismo anterior y recoger el tesoro que prepara el equipo para la siguiente capa.

| Capa | Entrada | Equipo mínimo (capacidad) | Prueba y recompensa | Regreso |
|---|---|---|---|---|
| Bosque de huiros | Boyas, entrada libre cerca de `(0, -2, 46)` | 24 m / 115 s; el equipo inicial lo cumple | Mural de 6 piedras, activar sello y recoger Perla: 64 m / 155 s | Ascender hacia las boyas; R permite rescate |
| Ecos del naufragio | Escotilla a 32 m, centro `(0, -32, 75)` | 52 m / 155 s, sello de huiros abierto y Perla recogida | Mural de 9 piedras y Brújula: 100 m / 200 s | Escotilla iluminada superior; continúa hasta la superficie |
| Corazón del abismo | Escotilla a 64 m, centro `(0, -64, 75)` | 88 m / 200 s, sello del naufragio abierto y Brújula recogida | Mural de 12 piedras y Corazón: 110 m / 240 s | Ascender por las dos escotillas abiertas; R sigue disponible |

Las cifras de aire son capacidad del equipo, no el aire restante exigido para cruzar. La bombona de tienda sigue sumando 60 s. Su compra no abre sellos ni sustituye los tesoros. Un tablero conectado todavía requiere activar ABRIR SELLO; después hay que recoger el cofre para recibir la mejora y abrir el paso siguiente. Si la mochila está llena, el cofre y la solución esperan al jugador.

El marcador del HUD dirige a la escotilla antes de señalar el santuario situado debajo. La baliza y los rótulos indican el estado del paso. E sobre la compuerta explica la condición pendiente, y chocar con ella también muestra ese mensaje, con un intervalo para evitar repetirlo cada frame. Debajo de cada losa hay una señal de superficie y cristales que marcan el ascenso.

## Persistencia y objetos

Las compuertas consultan los mecanismos y tesoros ya guardados. No añaden otro indicador de apertura que pudiera quedar desincronizado. Continuar las restaura inmediatamente, incluso durante la pausa del menú; Nueva partida las cierra. Se comprobó la carga de la partida de prueba creada durante la etapa 1 con su formato v1.

El requisito es **haber recuperado** el tesoro. Descartar su objeto no elimina las mejoras ni cierra la ruta. La tienda sigue vendiendo solamente peces. No se introduce una llave que el jugador pueda perder y que bloquee permanentemente el descenso.

## Configurar y ampliar

En cada `DiveZoneDefinition`, `entryPosition` define la entrada, `requiredSafeDepth` y `requiredOxygen` definen la preparación, `barrierWidth` cubre la sección de la fosa y `returnHint` explica la salida. La primera zona es de acceso libre; cada zona posterior se conecta al mecanismo y tesoro de la anterior.

`WorldBuilder.Gates` construye las losas y escotillas; `DiveGate` controla panel, señalización e interacción. El panel cambia de posición inmediatamente: la animación mecánica gradual queda para pulido posterior. Los colliders son cajas de doble cara con juntas solapadas, y el perímetro exterior debe quedar enterrado en el terreno. No basta con ampliar una puerta si se modifica el paisaje.

`ProjectSetup.Validate` comprueba la posición entre santuarios, equipo alcanzable y perímetros enterrados. `SmokeCheck.Gates` muestrea la superficie cerrada y usa el CharacterController real para intentar atravesar centro/laterales, cruzar una escotilla abierta, alcanzar el siguiente santuario rodeando el mural y regresar a la superficie. La posición de la fauna evita las losas desde su generación.

Antes de cambiar estas posiciones en una versión distribuida hay que revisar colisiones, visibilidad, aparición de fauna, tiempos de aire y compatibilidad con partidas. La incorporación actual conserva el retorno a la playa al cargar y las posiciones guardadas de las embarcaciones de superficie.

## Límites

Son accesos funcionales sobre la fosa actual; todavía no son biomas nuevos ni túneles detallados. Las pruebas de movimiento programado y las capturas no sustituyen un recorrido humano para comprobar orientación, dificultad y comodidad. La siguiente etapa del plan trabaja la primera expedición y sus indicaciones.
