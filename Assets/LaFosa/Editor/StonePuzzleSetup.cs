using System.IO;
using UnityEditor;
using UnityEngine;

namespace LaFosa.Editor
{
    public static class StonePuzzleSetup
    {
        public static void Ensure(GameConfig config)
        {
            const string folder = "Assets/LaFosa/Data/StonePuzzles";
            Directory.CreateDirectory(folder); AssetDatabase.Refresh();
            int[][] routes = { new[] { 0, 1, 2, 5, 4, 3 }, new[] { 0, 1, 2, 5, 8, 7, 4, 3, 6 }, new[] { 0, 4, 8, 9, 5, 1, 2, 6, 10, 11, 7, 3 } };
            string[] titles = { "EL CAUCE DE LOS HUIROS", "EL NUDO DEL NAUFRAGIO", "LAS VENAS DEL ABISMO" };
            for (int i = 0; i < config.zones.Length; i++)
            {
                var zone = config.zones[i]; if (zone.stonePuzzle != null) { zone.stonePuzzle.Validate(); continue; }
                int variant = Mathf.Min(i, 2);
                string path = folder + "/" + zone.name + ".asset";
                var definition = AssetDatabase.LoadAssetAtPath<StonePuzzleDefinition>(path);
                if (definition == null)
                {
                    definition = ScriptableObject.CreateInstance<StonePuzzleDefinition>(); definition.name = zone.name;
                    definition.title = titles[variant]; definition.columns = variant == 2 ? 4 : 3; definition.rows = variant == 0 ? 2 : 3;
                    definition.route = routes[variant]; definition.inletDirection = 3; definition.outletDirection = variant == 0 ? 3 : variant == 1 ? 2 : 1;
                    definition.startingTurns = new int[definition.Count];
                    for (int j = 0; j < definition.Count; j++) definition.startingTurns[j] = (j * 2 + variant) % 3 + 1;
                    definition.clue = variant == 0 ? "Conecta ENTRADA con SELLO usando las seis piedras. Los canales deben tocarse. Gira con E y después acciona la palanca." : variant == 1 ? "Todas las piedras deben recibir corriente. Ningún canal puede terminar contra piedra. Sigue la luz desde ENTRADA hasta SELLO." : "Despierta las doce piedras con un único cauce continuo. Une todos los extremos antes de accionar el sello.";
                    AssetDatabase.CreateAsset(definition, path);
                }
                zone.stonePuzzle = definition; EditorUtility.SetDirty(zone);
            }
        }
        public static void Validate(GameObject host, DiveZoneDefinition zone)
        {
            var puzzle = host.AddComponent<StoneCircuitPuzzle>(); puzzle.Initialize(zone);
            try
            {
                Check(!puzzle.Ready && !puzzle.Solved && !puzzle.Activate(), "No abre antes de resolver");
                Check(!puzzle.Rotate(-1) && !puzzle.Rotate(zone.stonePuzzle.Count), "Índices inválidos");
                int start = puzzle.Mask(0); for (int i = 0; i < 4; i++) puzzle.Rotate(0);
                Check(puzzle.Mask(0) == start, "Cuatro giros restauran una piedra");
                puzzle.Rotate(0); puzzle.ResetBoard();
                for (int i = 0; i < zone.stonePuzzle.Count; i++) Check(puzzle.Turns(i) == zone.stonePuzzle.startingTurns[i], "Reinicio restaura mezcla");
                for (int i = 0; i < zone.stonePuzzle.Count; i++) while (puzzle.Turns(i) != 0) puzzle.Rotate(i);
                Check(puzzle.Ready && !puzzle.Solved && puzzle.PoweredCount == zone.stonePuzzle.Count && puzzle.OpenConnections == 0, "Ruta completa espera palanca");
                for (int i = 0; i < zone.stonePuzzle.Count; i++)
                    if (puzzle.BaseMask(i) == 5 || puzzle.BaseMask(i) == 10)
                    {
                        puzzle.Rotate(i); puzzle.Rotate(i);
                        Check(puzzle.Ready, "Una piedra recta girada 180 grados sigue conectada");
                        puzzle.Rotate(i); puzzle.Rotate(i); break;
                    }
                puzzle.Rotate(zone.stonePuzzle.Start);
                Check(!puzzle.Ready && !puzzle.Activate(), "Ruta rota no abre el sello");
                for (int i = 0; i < 3; i++) puzzle.Rotate(zone.stonePuzzle.Start);
                Check(puzzle.Activate() && puzzle.Solved, "Palanca abre el circuito correcto");
                Check(!puzzle.Rotate(0) && !puzzle.ResetBoard() && !puzzle.Activate(), "Sello abierto permanece resuelto");
            }
            finally { Object.DestroyImmediate(puzzle); }
        }
        static void Check(bool condition, string message) { if (!condition) throw new System.Exception("STONE_PUZZLE_FAILED: " + message); }
    }
}
