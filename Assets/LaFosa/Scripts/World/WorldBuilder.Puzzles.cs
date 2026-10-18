using System.Collections.Generic;
using UnityEngine;

namespace LaFosa
{
    public sealed partial class WorldBuilder
    {
        const float StoneSpacing = .96f;
        static Vector3 StonePosition(StonePuzzleDefinition d, int index) => new Vector3((index % d.columns - (d.columns - 1) / 2f) * StoneSpacing, 2.15f + (d.rows - 1 - index / d.columns) * StoneSpacing, .8f);
        void BuildStonePuzzle(StoneCircuitPuzzle puzzle)
        {
            var d = puzzle.Definition; var station = puzzle.transform;
            float width = d.columns * StoneSpacing + 1.35f, height = d.rows * StoneSpacing + .4f;
            var basalt = Mat("Piedra tallada", new Color(.22f, .29f, .32f));
            var edge = Mat("Marco de piedra", new Color(.11f, .17f, .2f));
            var bronze = Mat("Canal sin corriente", new Color(.65f, .43f, .2f), true);
            var flowing = Mat("Corriente de piedra", new Color(.13f, .94f, .77f), true);
            var gold = Mat("Sello despertado", new Color(1, .76f, .24f), true);
            float centerY = 2.15f + (d.rows - 1) * StoneSpacing / 2;
            Shape("Monolito de canales", PrimitiveType.Cube, new Vector3(0, centerY, 1.12f), new Vector3(width, height, .55f), edge, true, station);
            foreach (float side in new[] { -1f, 1f })
            {
                Shape("Pilar del mecanismo", PrimitiveType.Cube, new Vector3(side * (width / 2 + .2f), centerY - .65f, 1.25f), new Vector3(.32f, height + 1.3f, .75f), basalt, true, station);
                Rock("Corona del pilar", new Vector3(side * (width / 2 + .2f), centerY + height / 2 + .1f, 1.25f), new Vector3(.28f, .25f, .35f), basalt, false, station);
            }
            for (int i = 0; i < d.Count; i++)
            {
                var tile = new GameObject("Piedra giratoria " + (i + 1)); tile.transform.SetParent(station, false); tile.transform.localPosition = StonePosition(d, i);
                var collider = tile.AddComponent<BoxCollider>(); collider.size = new Vector3(.88f, .88f, .28f);
                var rotor = new GameObject("Disco tallado"); rotor.transform.SetParent(tile.transform, false);
                var disc = Shape("Piedra", PrimitiveType.Cylinder, Vector3.zero, new Vector3(.88f, .115f, .88f), basalt, false, rotor.transform); disc.transform.localRotation = Quaternion.Euler(90, 0, 0);
                var renderers = new List<Renderer>();
                renderers.Add(Shape("Centro del canal", PrimitiveType.Sphere, new Vector3(0, 0, -.145f), new Vector3(.2f, .2f, .055f), bronze, false, rotor.transform).GetComponent<Renderer>());
                for (int side = 0; side < 4; side++)
                {
                    if ((puzzle.BaseMask(i) & (1 << side)) == 0) continue;
                    Vector3 direction = ChannelDirection(side);
                    var groove = Shape("Canal tallado", PrimitiveType.Cube, direction * .235f + Vector3.back * .147f, new Vector3(.12f, .47f, .035f), bronze, false, rotor.transform);
                    groove.transform.localRotation = Quaternion.Euler(0, 0, -90 * side); renderers.Add(groove.GetComponent<Renderer>());
                    // A dark border makes the geometry readable independently of its colour.
                    var recess = Shape("Borde del surco", PrimitiveType.Cube, direction * .235f + Vector3.back * .126f, new Vector3(.19f, .48f, .02f), edge, false, rotor.transform);
                    recess.transform.localRotation = groove.transform.localRotation;
                }
                var stone = tile.AddComponent<RuneStone>(); stone.puzzle = puzzle; stone.index = i; stone.rotor = rotor.transform; stone.channels = renderers.ToArray();
                stone.dormant = bronze; stone.flowing = flowing; stone.complete = gold;
                rotor.transform.localRotation = Quaternion.Euler(0, 0, -90 * puzzle.Turns(i));
            }
            Endpoint(station, d, d.Start, d.inletDirection, "ENTRADA", flowing);
            Endpoint(station, d, d.End, d.outletDirection, "SELLO", gold);
            float top = centerY + height / 2;
            PuzzleLabel(d.title, new Vector3(0, top + .9f, .65f), .17f, puzzle.zone.accent, station);
            PuzzleLabel("UNE TODOS LOS CANALES  ·  GIRA LAS PIEDRAS CON E", new Vector3(0, top + .58f, .65f), .1f, Color.white, station);
            var status = PuzzleLabel("", new Vector3(0, top + .26f, .65f), .12f, Color.white, station);
            var leverBase = Shape("Palanca del sello", PrimitiveType.Cube, new Vector3(1.65f, .65f, -.65f), new Vector3(.7f, 1.3f, .65f), basalt, true, station);
            var control = leverBase.AddComponent<StonePuzzleLever>(); control.puzzle = puzzle;
            var handle = new GameObject("Manivela"); handle.transform.SetParent(station, false); handle.transform.localPosition = new Vector3(1.65f, 1.25f, -.65f);
            Shape("Brazo de palanca", PrimitiveType.Cylinder, Vector3.up * .25f, new Vector3(.09f, .3f, .09f), bronze, false, handle.transform);
            Shape("Agarre", PrimitiveType.Cube, Vector3.up * .55f, new Vector3(.48f, .12f, .13f), gold, false, handle.transform);
            PuzzleLabel("ABRIR SELLO", new Vector3(1.65f, .85f, -1.02f), .12f, new Color(1, .8f, .4f), station);
            var reset = Shape("Piedra de reinicio", PrimitiveType.Cube, new Vector3(-1.65f, .45f, -.65f), new Vector3(.7f, .9f, .65f), basalt, true, station);
            var resetControl = reset.AddComponent<StonePuzzleLever>(); resetControl.puzzle = puzzle; resetControl.reset = true;
            PuzzleLabel("REINICIAR", new Vector3(-1.65f, .7f, -1.02f), .11f, Color.white, station);
            var seal = new GameObject("Cerrojo del tesoro"); seal.transform.SetParent(station, false); seal.transform.localPosition = new Vector3(0, .7f, -.61f);
            var sealFace = Shape("Disco del cerrojo", PrimitiveType.Cylinder, Vector3.zero, new Vector3(.42f, .06f, .42f), bronze, false, seal.transform); sealFace.transform.localRotation = Quaternion.Euler(90, 0, 0);
            Shape("Barra del sello", PrimitiveType.Cube, Vector3.back * .07f, new Vector3(1.18f, .1f, .12f), bronze, false, seal.transform);
            var view = station.gameObject.AddComponent<StonePuzzleView>(); view.puzzle = puzzle; view.status = status; view.seal = seal.transform; view.lever = handle.transform;
            view.sealFace = sealFace.GetComponent<Renderer>(); view.sleeping = bronze; view.ready = flowing; view.opened = gold;
        }
        static Vector3 ChannelDirection(int side) => side == 0 ? Vector3.up : side == 1 ? Vector3.right : side == 2 ? Vector3.down : Vector3.left;
        void Endpoint(Transform station, StonePuzzleDefinition d, int index, int side, string title, Material material)
        {
            Vector3 direction = ChannelDirection(side), pos = StonePosition(d, index);
            var pipe = Shape(title + " · conexión", PrimitiveType.Cube, pos + direction * .61f + Vector3.back * .15f, new Vector3(.12f, .39f, .045f), material, false, station);
            pipe.transform.localRotation = Quaternion.Euler(0, 0, -90 * side);
            Rock(title + " · cristal", pos + direction * .8f + Vector3.back * .12f, new Vector3(.13f, .13f, .1f), material, false, station);
            Vector3 labelOffset = side == 2 ? new Vector3(.4f, -.82f, -.18f) : direction * 1.15f + Vector3.back * .18f;
            PuzzleLabel(title, pos + labelOffset, .1f, material.color, station);
        }
        TextMesh PuzzleLabel(string text, Vector3 localPosition, float size, Color color, Transform parent)
        {
            var obj = Label(text, parent.TransformPoint(localPosition), size, color);
            obj.transform.SetParent(parent, true);
            // Fixed to the mural, not a billboard: labels cannot drift through adjacent stones.
            var billboard = obj.GetComponent<Billboard>(); billboard.enabled = false; Object.Destroy(billboard);
            obj.transform.localRotation = Quaternion.identity;
            return obj.GetComponent<TextMesh>();
        }
    }
}
