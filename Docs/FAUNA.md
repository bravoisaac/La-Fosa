# Fauna de La Fosa

Se incorporaron **49 modelos low poly**, uno por especie o grupo solicitado en «Peces de la costa de Chile según profundidad.docx». El documento se utilizó como listado de contenido, no como instrucciones de ejecución. No incluía imágenes. Palometa/vidriola comparten una ficha; pez víbora y pez hacha tienen modelos separados. Los grupos identificados como `spp.` o familias tienen un representante estilizado, sin inventar una especie concreta.

## Encontrar y editar los modelos

- **Prefabs:** `Assets/LaFosa/Models/Fish/`. Abre uno con doble clic en Unity o arrástralo a una escena.
- **Mallas independientes:** `Assets/LaFosa/Models/Fish/Meshes/`.
- **Materiales:** `Assets/LaFosa/Models/Fish/Materials/`.
- **Fichas de fauna:** `Assets/LaFosa/Data/FishSpecies/`.
- **Objetos vendibles:** subcarpeta `Items/`; el jurel conserva su objeto original en `Data/jurel.asset`.
- **Catálogo completo:** `Docs/catalogo-peces.tsv`, con nombre, taxón, franjas, precio y número de triángulos.
- **Vistas de los modelos reales:** `Builds/Windows/Verification/FishModels/`, producidas por las pruebas del ejecutable. La lámina conjunta está en `Docs/peces-catalogo.jpg`.

Cada prefab contiene malla, material, colisión y `FishAgent`. Las geometrías distinguen proporciones, aletas y rasgos: cuerpo plano y ojos superiores en lenguados; alas y cola en raya; cuerpos alargados en congrios; hocico en pejegallo; espada en pez espada; colmillos en pez víbora; fotóforos en peces de profundidad. Las mallas usan caras planas y colores por vértice. El shader aporta movimiento de cola y puntos luminosos; no necesitan texturas externas. No incluyen esqueleto ni animaciones de rig.

Los diseños, colores y tamaños son interpretaciones para el juego. Los ejemplares pequeños se agrandan para poder apuntarlos. No son modelos científicos a escala. El documento aportado define los nombres y rangos; se consultó también el [catálogo de peces de Subpesca](https://www.subpesca.cl/portal/sitio/Especies-Hidrobiologicas/Peces/) como referencia general. No se copiaron imágenes a las mallas.

## Distribución en la partida

La fosa del prototipo tiene 88 metros, por lo que las profundidades del documento se comprimen en seis franjas jugables. El dato original se conserva separado en cada ficha.

| Grupo | Rango del documento | Rango en el juego | Modelos | Individuos |
|---|---|---|---:|---:|
| Intermareal | 0–5 m | 1,2–4,5 m | 6 | 24 |
| Costa somera | 5–30 m | 5–19 m | 13 | 39 |
| Pelágicos | 0–200 m | 6–44 m | 9 | 27 |
| Plataforma | 50–400 m | 26–49 m | 7 | 14 |
| Talud | 300–1.500 m | 53–74 m | 8 | 16 |
| Batial y abisal | Más de 1.000 m | 79–85 m | 6 | 12 |
| **Total** | | | **49** | **132** |

La población se crea con la semilla de la expedición. Los peces patrullan cerca de su punto de aparición, huyen del buzo al verlo y regresan gradualmente cuando pierden la amenaza. Permanecen en su franja, buscan rodear obstáculos y reaparecen en su zona 25 segundos después de capturarlos.

## Huida y herramientas

| Comportamiento | Peces de orilla, franja 0 | Peces del mar, franjas 1–5 |
|---|---:|---:|
| Velocidad tranquila | 0,45 m/s | 1,1 m/s |
| Velocidad de huida | 1,8 m/s | 16 m/s |
| Distancia inicial de detección | 3,5 m | 12 m |
| Distancia para mantener la alerta visual | 6 m | 22 m |

El buzo nada a 6 m/s, acelera a 9 m/s y llega a 12,15 m/s acelerando con las aletas actuales. Por eso puede alcanzar los peces de la orilla con las manos y usar el tridente para facilitar esas capturas. En agua libre no alcanza por velocidad a los peces del mar: necesita aprovechar los 40 m del arpón. La velocidad es fija; los peces no aceleran artificialmente cuando compras aletas.

La detección requiere al buzo sumergido y sin un obstáculo sólido entre ambos. Los peces recuerdan la amenaza durante 1,5 segundos después de perderla y después regresan nadando a su zona. La pausa congela movimiento y temporizadores. La huida respeta terreno, obstáculos sólidos, superficie y límites de profundidad. El resultado cerca de rincones depende del espacio disponible: no existe una prohibición invisible que haga inmunes a los peces de mar frente a manos o tridente.

Los perfiles editables son `Data/PecesOrilla.asset` y `Data/PecesMar.asset`, referenciados por `Expedicion.asset`. `GameBootstrap` los asigna por `habitatBand` al crear el mundo. `FishAgent` gestiona patrulla, huida, regreso y reposición; `FishAgent.Navigation` comprueba visibilidad y rutas de movimiento. La IA no añade daño ni agresividad.

Apuntar a un pez cercano muestra su nombre, taxón y precio. La mochila separa las especies y muestra su valor unitario. La tienda vende toda la pesca sumando sus precios, conservando conchas y tesoros. Los peces intermareales valen $30: dos capturas permiten comprar el primer tridente de $60. Se mantiene **manos → tridente → arpón**, con la red disponible tras el tridente.

## Ampliar la fauna

1. Crea una ficha con **Create → La Fosa → Especie de pez** y un `ItemDefinition` marcado `isFish`, con precio y límite de pila.
2. Duplica un prefab como base o utiliza una malla nueva. Conecta el objeto y la ficha en `FishAgent`, y el prefab en la ficha.
3. Configura `population`, `playableDepth` y `habitatBand`. Las velocidades de movimiento y detección se configuran en los perfiles de comportamiento de `Expedicion`; el campo heredado `swimSpeed` no determina la velocidad de huida. Añade la ficha a `Expedicion.fishSpecies`.
4. Para hábitats fuera de las seis franjas actuales, amplía `WorldBuilder.Fish.cs` y el fondo.

`FishRecipes.cs` conserva las recetas originales y `FishMeshFactory.cs` construye las mallas. El menú **La Fosa → Fauna → Regenerar los 49 modelos originales** vuelve a escribir estos modelos y sus fichas: duplica los assets antes de personalizarlos si quieres conservar ambos. La preparación habitual conserva el catálogo existente, incluso al añadir especies. La comprobación de 49 modelos en `SmokeCheck.Fish` es específica de esta entrega; actualiza esa expectativa al ampliar el catálogo.
