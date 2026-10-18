using System.Collections;
using System.Linq;
using UnityEngine;

namespace LaFosa
{
    public sealed partial class SmokeCheck
    {
        IEnumerator TestStonePuzzle(GameSession session, DiverController diver, TreasureChest chest, int level)
        {
            var puzzle = chest.puzzle;
            var stones = puzzle.GetComponentsInChildren<RuneStone>().OrderBy(s => s.index).ToArray();
            var controls = puzzle.GetComponentsInChildren<StonePuzzleLever>();
            var lever = controls.First(c => !c.reset); var reset = controls.First(c => c.reset);
            Check(stones.Length == puzzle.Definition.Count && !puzzle.Ready && !puzzle.Solved, "Tablero mezclado y piedras completas " + level);
            lever.Interact(diver); Check(!puzzle.Solved, "Palanca rechaza circuito incompleto " + level);
            if (level == 0)
            {
                var locked = FindObjectsByType<StoneCircuitPuzzle>(FindObjectsSortMode.None).First(p => p.zone == session.Config.zones[1]);
                var lockedStone = locked.GetComponentInChildren<RuneStone>(); int before = locked.Turns(lockedStone.index);
                lockedStone.Interact(diver);
                foreach (var control in locked.GetComponentsInChildren<StonePuzzleLever>()) control.Interact(diver);
                Check(locked.Turns(lockedStone.index) == before && locked.Moves == 0 && !locked.Solved, "Piedras, palanca y reinicio del nivel siguiente bloqueados");
            }
            stones[0].Interact(diver); int savedTurn = puzzle.Turns(0);
            Vector3 previous = diver.transform.position;
            diver.Rescue("Prueba de persistencia del mecanismo");
            Check(puzzle.Turns(0) == savedTurn, "Regreso a playa conserva piedras " + level);
            Teleport(diver, previous); reset.Interact(diver);
            Check(puzzle.Moves == 0 && stones.All(s => puzzle.Turns(s.index) == puzzle.Definition.startingTurns[s.index]), "Reinicio local restaura mezcla " + level);

            // Exercise the same raycast target used by E, with a clear view of every stone.
            foreach (var stone in stones)
            {
                Teleport(diver, stone.transform.position + Vector3.back * 2.2f - Vector3.up * 1.55f);
                diver.LookAt(stone.transform.position);
                yield return null;
                Physics.SyncTransforms();
                Check(diver.Target == stone, "Piedra alcanzable mirando: " + level + "/" + stone.index);
                while (puzzle.Turns(stone.index) != 0) stone.Interact(diver);
            }
            Check(puzzle.Ready && !puzzle.Solved && puzzle.OpenConnections == 0 && puzzle.PoweredCount == stones.Length, "Circuito conectado espera palanca " + level);
            chest.Interact(diver); Check(!chest.Claimed, "Conexión sin palanca no entrega tesoro " + level);
            yield return new WaitForSeconds(.55f);
            Check(stones.All(s => Quaternion.Angle(s.rotor.localRotation, Quaternion.Euler(0, 0, -90 * puzzle.Turns(s.index))) < .1f), "Giro visible coincide con estado " + level);
            Check(stones.All(s => s.channels.All(r => r.sharedMaterial == s.flowing)), "Corriente ilumina todas las piedras " + level);
            Teleport(diver, lever.transform.position + Vector3.back * 1.15f - Vector3.up * .2f);
            diver.LookAt(lever.transform.position);
            yield return null;
            Check(diver.Target == lever, "Palanca accesible por interacción " + level + " (" + (diver.Target != null ? diver.Target.name : "sin objetivo") + ")");
            lever.Interact(diver); Check(puzzle.Solved, "Palanca abre sello " + level);
            if (level < session.Config.zones.Length - 1)
            {
                var gate = session.GetComponentsInChildren<DiveGate>().First(g => g.Puzzle == puzzle);
                Check(!gate.IsOpen && gate.MissingRequirement.Contains(puzzle.zone.treasure.displayName), "Sello resuelto pide recoger la mejora antes del descenso " + level);
            }
            int moves = puzzle.Moves;
            stones[0].Interact(diver); reset.Interact(diver); lever.Interact(diver);
            Check(puzzle.Solved && puzzle.Moves == moves, "Sello abierto no se reinicia ni duplica " + level);
            Check(stones.All(s => s.channels.All(r => r.sharedMaterial == s.complete)), "Piedras doradas al abrir sello " + level);
            int capacity = diver.Inventory.capacity; diver.Inventory.capacity = diver.Inventory.Stacks.Count;
            int rewards = session.Treasures;
            chest.Interact(diver);
            Check(!chest.Claimed && puzzle.Solved && session.Treasures == rewards, "Mochila llena conserva tesoro y solución " + level);
            diver.Inventory.capacity = capacity;
            Teleport(diver, puzzle.zone.stationPosition + new Vector3(0, 1.1f, -5.8f));
            diver.LookAt(puzzle.zone.stationPosition + new Vector3(0, 2.5f, .7f));
            yield return new WaitForSeconds(.8f);
            Capture(diver.View, "puzle-" + (level + 1) + "-resuelto.png");
        }
    }
}
