# Mecanismos de piedras

Los santuarios usan circuitos de canales tallados, en lugar de repetir una secuencia escrita. No hace falta colocar objetos del inventario ni comprar una herramienta para resolverlos.

1. Busca el cristal **ENTRADA**, conectado al borde del mural.
2. Apunta a las piedras y pulsa **E**: cada pulsación gira la pieza 90° en sentido horario.
3. Haz coincidir los canales entre piedras vecinas. La luz verde muestra hasta dónde llega la corriente desde la entrada; los canales de cobre aún no la reciben.
4. Lleva la corriente hasta el cristal **SELLO**, atravesando **todas** las piedras, sin conexiones que terminen contra una pared o salgan del borde por otro lugar.
5. Cuando el mural indique «CORRIENTE COMPLETA», acciona la palanca **ABRIR SELLO**. El cerrojo gira y las piedras se vuelven doradas. Recoge el tesoro con E.

El cofre cerrado ofrece una pista. La piedra **REINICIAR** devuelve únicamente ese tablero a su mezcla inicial. Un error no borra los demás giros. R devuelve al jugador a la playa y conserva el estado del mecanismo durante la sesión. Si la mochila está llena, el sello permanece abierto y puedes regresar por el tesoro. Los santuarios posteriores no se pueden manipular hasta recuperar el tesoro anterior.

| Santuario | Mecanismo | Tablero | Desafío |
|---|---|---|---|
| Bosque de huiros | El cauce de los huiros | 3 × 2, seis piedras | Aprender cómo se unen los canales rectos y los codos |
| Ecos del naufragio | El nudo del naufragio | 3 × 3, nueve piedras | Seguir un recorrido que vuelve hacia el centro |
| Corazón del abismo | Las venas del abismo | 4 × 3, doce piedras | Unir un recorrido largo que recorre las columnas |

## Crear más mecanismos

Las fichas están en `Assets/LaFosa/Data/StonePuzzles/`. Crea una con **Create → La Fosa → Puzle de piedras** y asígnala a `DiveZoneDefinition.stonePuzzle`.

- `columns` y `rows`: entre 2 y 6; la geometría del mural se adapta a las dimensiones. Para tableros mayores, amplía también la plataforma y comprueba el alcance del jugador.
- `route`: lista de todos los índices en orden de recorrido. Los índices van por filas de izquierda a derecha, empezando arriba en 0. Cada piedra debe aparecer una sola vez y ser vecina ortogonal de la anterior.
- `inletDirection` y `outletDirection`: 0 arriba, 1 derecha, 2 abajo, 3 izquierda. Deben salir del tablero en la primera y última piedra de la ruta.
- `startingTurns`: una cantidad de cuartos de vuelta, de 0 a 3, por piedra. La mezcla debe empezar sin resolver.
- `title` y `clue`: nombre del mecanismo y pista que ofrece el cofre.

La ruta genera las formas de los canales, pero **no se muestra como solución en el juego**. La validación utiliza conectividad real: acepta cualquier recorrido que ilumine todas las piedras, una entrada con una salida y ningún extremo suelto. Girar una pieza recta 180° conserva su conexión.

`StoneCircuitPuzzle` separa los datos y la comprobación lógica del arte. `WorldBuilder.Puzzles.cs` genera los discos, surcos, cristales, palanca y rótulos. `RuneStone` anima cada giro; `StonePuzzleView` muestra el progreso y anima el cierre. Se pueden sustituir los modelos manteniendo estos componentes.

Se conserva el progreso solo durante la partida actual, como el resto del prototipo.
