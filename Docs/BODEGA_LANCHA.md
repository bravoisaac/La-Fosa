# Bodega de la lancha · 4.2b

Implementada el 13 de noviembre de 2026. Siguiente fase: almacenamiento/preparación de suministros en la base (8b); consultar primero `PLAN_MEJORAS.md`.

## Funcionamiento y decisiones

- Comprar la lancha en el astillero, subir con E y abrir **B**. Cerrar con **B o Esc**. No exige estar amarrada; abrir pausa conducción, aire y producción como los demás menús.
- Bodega de **24 espacios**, con el límite de pila propio de cada objeto. Panel con mochila/bodega y desplazamiento independiente, ocupación visible y botones de una unidad o pila completa.
- Solo admite peces y los minerales del catálogo. Conchas/objetos antiguos permanecen en mochila. La moto no tiene bodega; no se permite acceso remoto desde playa o base.
- Una transferencia completa ambos inventarios antes de notificar cambios. Si no cabe toda la cantidad solicitada, no mueve nada. Aun sin espacios vacíos se pueden completar pilas existentes.
- Vender y fabricar siguen usando la mochila: retirar primero. No cambiar precios ni costes. Rescatar al buzo conserva la carga y deja la lancha en su posición.
- La bodega es un inventario independiente asociado a la única lancha comprable, creado en GameSession. No añade una segunda embarcación ni modifica la geometría.

## Mapa mínimo para retomar

| Archivo | Responsabilidad |
|---|---|
| `Scripts/Inventory/Inventory.cs` | `CanReceive` / `TryTransferTo`: capacidad y transferencia atómica reutilizable |
| `Scripts/Core/GameSession.Cargo.cs` | Capacidad, inventario, propiedad/pasajero, panel y reglas de transferencia |
| `Scripts/UI/DiveHUD.Cargo.cs` | Dos listas con scroll y acciones de una unidad/pila |
| `Scripts/Core/GameSession.cs` | Inicialización, tecla B, pausa, exclusión de Tab, Escape |
| `Scripts/Core/SaveStore.cs` | Campo opcional `SaveData.boatCargo` |
| `Scripts/Core/GameSession.Persistence.cs` | Captura/restauración y suscripción de cambios; Nueva partida vacía |
| `Scripts/Core/GameSession.SaveValidation.cs` | IDs, tipo permitido, cantidades, 24 espacios y propiedad |
| `Scripts/Core/GameSession.Developer.cs` | Cierre de panel al cambiar modo; snapshot existente restaura también bodega |
| `Editor/CargoChecks.cs` | 39 comprobaciones, llamada desde `DeveloperModeChecks` |

Rutas relativas a `Assets/LaFosa/`. El catálogo y la capacidad de la mochila no cambiaron. La base futura puede reutilizar la transferencia; necesita su propio acceso, inventario y persistencia.

## Persistencia y evidencia

Formato v2 conservado: `boatCargo` contiene `{id,count}`; ausente/null significa bodega vacía (v1/v2 anteriores). Cargar siempre sustituye toda la bodega. Rechaza carga no vacía sin propiedad, objetos no admitidos, cantidades fuera de pila y más de 24 espacios. Nueva partida y escenarios iniciales Develop reinician carga; terminar Develop restaura la copia anterior. Cada transferencia marca guardado al final del frame, con ambos inventarios ya actualizados.

`unity-cargo-build.log`: 39 comprobaciones de acceso, transferencias, capacidad parcial/llena, notificaciones coherentes, datos inválidos, guardado/menú/Continuar, compatibilidad v1/v2, rescate, venta, Nueva partida y aislamiento de principal/respaldo Develop. Pasan también suites existentes y compilación Windows, salida 0. Archivos de prueba temporales; no se abrió el juego ni Play Mode.

Compilar/verificar: Unity `6000.3.26f1`, `-batchmode -nographics -quit -projectPath "<raíz del proyecto>" -executeMethod LaFosa.Editor.ProjectSetup.BuildWindows -logFile "<ruta del log>"`. Requiere acceso a la licencia local. Entrega: `Builds/Windows/La Fosa.exe`.

## Prueba humana pendiente

1. Comprar lancha, subir y pulsar B; comprobar lectura de nombres/cantidades y scroll hasta el espacio 24 en la resolución habitual.
2. Guardar/retirar pesca y minerales por unidad y pila. Probar mochila/bodega llenas y pila con un único hueco: no debe moverse una fracción si se pidió una pila que no cabe.
3. Cerrar con B y Esc, navegar, amarrar, desembarcar y volver. Tab no debe superponer otro panel mientras está abierta la bodega.
4. Guardar/menú/Continuar y cerrar/abrir el juego: misma carga, lancha y amarre. Continuar aparece en playa; volver a la lancha para acceder.
5. En Develop, mover recursos, reiniciar escenario y terminar pruebas: recuperar la bodega original. Rescatar al buzo debe conservar carga sin permitir transferir remotamente.

La validación de editor no acredita legibilidad, control con teclado/ratón ni balance humano. Recuperar lancha abandonada (4.4) y suministros en lancha/base (4.3/8b) siguen pendientes.
