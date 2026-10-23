using UnityEngine;

namespace LaFosa
{
    public sealed partial class DiveHUD : MonoBehaviour
    {
        public GameSession session;
        public HarpoonGun harpoon;
        static readonly Color Ink = new Color(.025f, .075f, .1f, .93f);
        static readonly Color Mint = new Color(.45f, .93f, .79f);
        static readonly Color Muted = new Color(.75f, .84f, .85f);
        static readonly Color Gold = new Color(1, .73f, .39f);
        GUIStyle style;
        bool showControls = true;
        bool confirmNewGame;
        void OnGUI()
        {
            if (session == null || session.Diver == null) return;
            var previousMatrix = GUI.matrix;
            float scale = Mathf.Min(Screen.width / 1600f, Screen.height / 900f);
            var offset = new Vector3((Screen.width - 1600 * scale) * .5f, (Screen.height - 900 * scale) * .5f, 0);
            GUI.matrix = Matrix4x4.TRS(offset, Quaternion.identity, new Vector3(scale, scale, 1));
            if (style == null) style = new GUIStyle(GUI.skin.label) { font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf"), wordWrap = true, richText = false };
            try { DrawHud(scale, offset); }
            finally { GUI.matrix = previousMatrix; }
        }
        void DrawHud(float scale, Vector3 offset)
        {
            var diver = session.Diver;
            Panel(30, 28, 270, 82);
            Text("L A  F O S A", 50, 38, 260, 36, 29, Color.white);
            Text("CALETA BRUMA   /   CHILE", 52, 78, 240, 22, 12, Mint);
            string zone = "COSTA DEL PACÍFICO";
            foreach (var entry in session.Config.zones) if (diver.Depth >= entry.startsAtDepth) zone = entry.displayName.ToUpperInvariant();
            Panel(1260, 28, 310, 82);
            Text(zone, 1282, 40, 270, 28, 16, Mint);
            Text(diver.Underwater ? "INMERSIÓN EN CURSO" : "SUPERFICIE · AIRE LIBRE", 1282, 76, 270, 20, 12, Muted);
            Text("N   ·   NE   ·   E   ·   SE   ·   S   ·   SO   ·   O   ·   NO", 550, 34, 500, 25, 12, Muted, TextAnchor.MiddleCenter);
            Text(Mathf.RoundToInt(diver.transform.eulerAngles.y).ToString("000") + "°", 740, 65, 120, 30, 21, Color.white, TextAnchor.MiddleCenter);

            var objective = session.ExpeditionObjective();
            float hintHeight = Mathf.Max(102, TextHeight(objective.Hint, 270, 16));
            float notebookBottom = 242 + hintHeight + 14;
            Panel(30, 137, 315, notebookBottom - 137);
            Text("CUADERNO · ZONAS " + session.DiscoveredZoneCount + "/" + session.Config.zones.Length, 50, 153, 280, 22, 12, Gold);
            Text(objective.Title, 50, 184, 275, 55, 20, Color.white);
            Text(objective.Hint, 50, 242, 270, hintHeight, 16, Muted);
            float pocketY = notebookBottom + 18;
            Panel(30, pocketY, 315, 94);
            Text("BOLSILLO   $" + session.Economy.Money, 50, pocketY + 12, 275, 30, 22, Gold);
            Text("PESCA PARA VENDER   $" + session.Economy.SaleValue + "\nTesoros: " + session.Treasures + " / " + session.Config.zones.Length, 50, pocketY + 48, 270, 40, 14, Muted);

            Panel(30, 720, 345, 150);
            Text("O₂", 50, 733, 50, 35, 25, Mint);
            Text(Mathf.CeilToInt(diver.Oxygen) + " / " + Mathf.CeilToInt(diver.MaxOxygen) + " s", 225, 742, 130, 25, 16, Color.white, TextAnchor.MiddleRight);
            bool needsAir = diver.Underwater && (diver.Oxygen < 30 || diver.Depth > diver.SafeDepth);
            Bar(50, 779, 305, 7, diver.Oxygen / diver.MaxOxygen, needsAir ? Gold : Mint);
            Text("SALUD  " + Mathf.CeilToInt(diver.Health) + "%", 50, 807, 160, 24, 13, Muted);
            Text("MOCHILA  " + diver.Inventory.Stacks.Count + "/" + diver.Inventory.capacity, 210, 807, 145, 24, 13, Muted, TextAnchor.MiddleRight);
            Text("TAB  inventario", 50, 839, 260, 20, 12, Mint);
            Panel(1240, 720, 330, 150);
            Text(diver.Depth.ToString("0.0") + " m", 1260, 730, 280, 48, 38, Color.white, TextAnchor.MiddleRight);
            Text("LÍMITE DEL EQUIPO   " + diver.SafeDepth + " m", 1260, 787, 280, 24, 13, diver.Depth > diver.SafeDepth ? Gold : Muted, TextAnchor.MiddleRight);
            Text(harpoon.ToolName + (harpoon.ReadyIn > 0 ? " · RECARGANDO" : " · LISTO"), 1260, 833, 280, 24, 13, Mint, TextAnchor.MiddleRight);
            if (!session.Blocked)
            {
                if (objective.Step != ExpeditionStep.Complete)
                {
                    Vector3 destination = objective.Destination;
                    Vector3 point = diver.View.WorldToViewportPoint(destination);
                    // Convert the camera's full-window viewport into the centered HUD canvas.
                    float projectedX = (point.x * Screen.width - offset.x) / scale;
                    float projectedY = ((1 - point.y) * Screen.height - offset.y) / scale;
                    bool behind = point.z <= 0;
                    Vector3 local = diver.View.transform.InverseTransformPoint(destination);
                    // Reserve the upper central band: never cover the notebook, reticle,
                    // fish/interaction panel (y=500) or air warning (y=620).
                    float markerX = behind ? (local.x < 0 ? 522 : 1078) : Mathf.Clamp(projectedX, 522, 1078);
                    float markerY = behind ? 330 : Mathf.Clamp(projectedY, 265, 350);
                    string direction = behind || point.x < 0 || point.x > 1
                        ? (local.x < 0 ? "← GIRA A LA IZQUIERDA\n" : "GIRA A LA DERECHA →\n")
                        : point.y > 1 ? "↑ SUBE LA MIRADA\n" : point.y < 0 ? "↓ BAJA LA MIRADA\n" : "";
                    RectColor(markerX - 22, markerY - 20, 44, 38, Ink);
                    Text("◇", markerX - 20, markerY - 20, 40, 36, 27, Gold, TextAnchor.MiddleCenter);
                    Panel(markerX - 152, markerY + 20, 304, 60);
                    Text(direction + objective.Marker + " · " + Mathf.RoundToInt(Vector3.Distance(diver.transform.position, destination)) + " m", markerX - 142, markerY + 24, 284, 52, 14, Gold, TextAnchor.MiddleCenter);
                }
                RectColor(798, 440, 4, 4, Color.white);
                if (diver.Riding != null)
                {
                    Panel(540, 720, 520, 50);
                    Text(diver.Riding.DisplayName + " · WASD conducir · E desembarcar", 550, 731, 500, 28, 16, Mint, TextAnchor.MiddleCenter);
                }
                if (diver.Target != null)
                {
                    Panel(510, 500, 580, 76);
                    Text("[ E ]   " + diver.Target.Prompt, 530, 510, 540, 56, 17, Color.white, TextAnchor.MiddleCenter);
                }
                if (needsAir)
                {
                    Panel(460, 620, 680, 74);
                    Text(diver.Depth > diver.SafeDepth ? "PROFUNDIDAD EXCESIVA · AIRE ×" + diver.PressureOxygenMultiplier.ToString("0.#") : "OXÍGENO BAJO · RECUPERA AIRE", 480, 630, 640, 26, 20, Gold, TextAnchor.MiddleCenter);
                    Text("ESPACIO para ascender · R para regresar con tus hallazgos", 480, 660, 640, 24, 15, Color.white, TextAnchor.MiddleCenter);
                }
                if (diver.Target == null && diver.ObservedFish != null && diver.ObservedFish.Available)
                {
                    var fish = diver.ObservedFish;
                    Panel(510, 500, 580, 94);
                    Text(fish.item.displayName + " · $" + fish.item.salePrice, 530, 508, 540, 28, 19, Color.white, TextAnchor.MiddleCenter);
                    Text(fish.species != null ? fish.species.scientificName : "CLIC para pescar", 530, 538, 540, 25, 13, Mint, TextAnchor.MiddleCenter);
                    float distance = Vector3.Distance(diver.View.transform.position, fish.transform.position);
                    Text(distance.ToString("0.0") + " m · " + (distance <= harpoon.EffectiveRange ? "CLIC para capturar" : "Acércate") + " · Alcance " + harpoon.EffectiveRange.ToString("0.#") + " m", 530, 566, 540, 22, 13, Gold, TextAnchor.MiddleCenter);
                }
                if (showControls)
                {
                    Panel(395, 810, 810, 65);
                    Text("1 MANOS   2 TRIDENTE   3 ARPÓN   4 RED", 410, 819, 780, 22, 14, Mint, TextAnchor.MiddleCenter);
                    Text("CLIC pescar   E interactuar   TAB mochila   R rescate", 410, 846, 780, 22, 14, Muted, TextAnchor.MiddleCenter);
                }
            }
            if (Time.unscaledTime < session.MessageUntil)
            {
                Panel(410, 112, 780, 60);
                Text(session.Message, 435, 123, 730, 42, 17, Mint, TextAnchor.MiddleCenter);
            }
            if (session.SaveFailed && !session.Intro)
                Text(session.SaveStatus, 410, 180, 780, 55, 16, Gold, TextAnchor.MiddleCenter);
            if (session.Intro) Intro();
            else if (session.ShopOpen) Shop();
            else if (session.InventoryOpen) Inventory();
            else if (session.Paused) Pause();
        }
        void Intro()
        {
            DimWorld(new Color(.015f, .04f, .065f, .65f));
            Panel(390, 165, 820, 650);
            Text("UNA EXPEDICIÓN AL PACÍFICO", 450, 208, 700, 25, 14, Mint);
            Text("Bajo la calma,\nun mundo por descubrir.", 450, 257, 700, 120, 42, Color.white);
            Text("Explora una caleta chilena y desciende por una fosa ficticia. Tres santuarios, tres tesoros y la oscuridad del océano te esperan.", 450, 398, 685, 65, 20, Muted);
            Text("01  Atrapa peces lentos en la orilla y reúne $" + session.TridentPrice + ".\n02  Vende en la cabaña y compra tu primer tridente.\n03  Sigue la guía al santuario y regresa con su tesoro.", 450, 482, 690, 100, 18, Color.white);
            if (confirmNewGame)
            {
                Text("Se reemplazará la partida guardada y su respaldo. ¿Empezar desde cero?", 450, 597, 690, 48, 18, Gold);
                if (Button("SÍ, NUEVA PARTIDA", 450, 660, 330, 50)) { if (session.NewGame(true)) confirmNewGame = false; }
                if (Button("CANCELAR", 810, 660, 330, 50)) confirmNewGame = false;
            }
            else
            {
                GUI.enabled = session.CanContinue;
                if (Button("CONTINUAR", 450, 621, 330, 55)) session.ContinueGame();
                GUI.enabled = true;
                if (Button("NUEVA PARTIDA", 810, 621, 330, 55))
                { if (session.HasSaveFiles) confirmNewGame = true; else session.NewGame(); }
            }
            Text(session.SaveStatus, 450, 736, 690, 60, 15, session.SaveFailed ? Gold : Muted);
        }
        void Inventory()
        {
            DimWorld(new Color(0, .025f, .045f, .75f));
            Panel(310, 145, 980, 610);
            Text("EQUIPO & HALLAZGOS", 350, 181, 600, 50, 32, Color.white);
            Text("$" + session.Economy.Money + " · Aletas " + (session.Economy.Has(GearKind.Fins) ? "equipadas" : "básicas") + " · Descartar elimina la pila sin pagar dinero.", 350, 240, 820, 30, 16, Muted);
            var inventory = session.Diver.Inventory;
            for (int i = 0; i < inventory.capacity; i++)
            {
                int col = i % 4, row = i / 4; float x = 350 + col * 225, y = 295 + row * 123;
                RectColor(x, y, 210, 108, new Color(.065f, .14f, .17f));
                if (i < inventory.Stacks.Count)
                {
                    var stack = inventory.Stacks[i];
                    RectColor(x, y, 4, 108, stack.item.color);
                    Text(stack.item.displayName, x + 14, y + 7, 180, 40, 15, Color.white);
                    if (stack.item.isFish) Text("$" + stack.item.salePrice + " / pez", x + 14, y + 45, 180, 20, 12, Gold);
                    Text("× " + stack.count, x + 14, y + 64, 60, 30, 17, Color.white);
                    if (Button("Descartar", x + 84, y + 65, 112, 29)) { inventory.RemoveAt(i); break; }
                }
                else Text("—", x, y + 32, 210, 35, 20, Muted, TextAnchor.MiddleCenter);
            }
            if (Button("VOLVER AL MAR  [TAB]", 920, 690, 310, 40)) session.ToggleInventory();
            Text("Equipo: " + session.Diver.SafeDepth + " m  /  " + session.Diver.MaxOxygen + " s de O₂", 350, 696, 520, 30, 17, Mint);
        }
        void Pause()
        {
            DimWorld(new Color(0, .025f, .045f, .75f));
            Panel(535, 210, 530, 470);
            Text("EN LA CALMA", 580, 246, 440, 50, 33, Color.white);
            Text("Expedición en pausa", 580, 304, 440, 30, 17, Muted);
            if (Button("CONTINUAR", 580, 370, 440, 52)) session.Resume();
            if (Button("VOLVER A LA PLAYA", 580, 438, 440, 52)) { session.Diver.Rescue("De regreso en Caleta Bruma."); session.Resume(); }
            if (Button(showControls ? "OCULTAR CONTROLES" : "MOSTRAR CONTROLES", 580, 506, 440, 52)) showControls = !showControls;
            if (Button("GUARDAR Y VOLVER AL MENÚ", 580, 574, 440, 42)) session.ReturnToMenu();
            Text(session.SaveStatus, 580, 626, 440, 44, 13, session.SaveFailed ? Gold : Muted);
        }
        void DimWorld(Color color)
        {
            // Menus still cover the whole window outside the centered 16:9 HUD canvas.
            var matrix = GUI.matrix;
            GUI.matrix = Matrix4x4.identity;
            RectColor(0, 0, Screen.width, Screen.height, color);
            GUI.matrix = matrix;
        }
        float TextHeight(string value, float width, int size)
        {
            style.fontSize = size;
            return style.CalcHeight(new GUIContent(value), width);
        }
        void Panel(float x, float y, float w, float h) { RectColor(x, y, w, h, Ink); RectColor(x, y, 3, h, new Color(.35f, .76f, .68f, .6f)); }
        void RectColor(float x, float y, float w, float h, Color color) { GUI.color = color; GUI.DrawTexture(new Rect(x, y, w, h), Texture2D.whiteTexture); GUI.color = Color.white; }
        void Bar(float x, float y, float w, float h, float value, Color color) { RectColor(x, y, w, h, new Color(.17f, .26f, .28f)); RectColor(x, y, w * Mathf.Clamp01(value), h, color); }
        void Text(string value, float x, float y, float w, float h, int size, Color color, TextAnchor anchor = TextAnchor.UpperLeft)
        {
            style.fontSize = size; style.normal.textColor = color; style.alignment = anchor; GUI.Label(new Rect(x, y, w, h), value, style);
        }
        bool Button(string label, float x, float y, float w, float h)
        {
            var rect = new Rect(x, y, w, h); bool hover = rect.Contains(Event.current.mousePosition);
            RectColor(x, y, w, h, hover ? new Color(.23f, .52f, .47f) : new Color(.12f, .32f, .31f));
            Text(label, x + 8, y, w - 16, h, 15, Color.white, TextAnchor.MiddleCenter);
            return GUI.Button(rect, GUIContent.none, GUIStyle.none);
        }
    }
}
