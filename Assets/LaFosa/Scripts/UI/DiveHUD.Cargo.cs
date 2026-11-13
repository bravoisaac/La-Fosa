using UnityEngine;

namespace LaFosa
{
    public sealed partial class DiveHUD
    {
        Vector2 backpackScroll, cargoScroll;
        void Cargo()
        {
            DimWorld(new Color(0, .025f, .045f, .85f));
            Panel(220, 175, 1160, 640);
            Text("BODEGA DE LA LANCHA", 250, 195, 900, 44, 30, Color.white);
            Text("Solo pesca y minerales · Cada pila respeta su límite · La navegación está en pausa.", 250, 248, 1090, 30, 16, Muted);
            CargoColumn(session.Diver.Inventory, true, 250, ref backpackScroll);
            CargoColumn(session.BoatCargo, false, 825, ref cargoScroll);
            Text("Retira los recursos a la mochila para vender o fabricar en la ciudad.", 250, 745, 690, 48, 16, Mint);
            if (Button("CERRAR  [B / ESC]", 1020, 749, 320, 44)) session.CloseCargo();
        }
        void CargoColumn(Inventory inventory, bool deposit, float x, ref Vector2 scroll)
        {
            Text((deposit ? "MOCHILA" : "BODEGA") + "  " + inventory.Stacks.Count + " / " + inventory.capacity + " espacios", x, 292, 510, 34, 21, Mint);
            var viewport = new Rect(x, 340, 515, 385);
            scroll = GUI.BeginScrollView(viewport, scroll, new Rect(0, 0, 490, Mathf.Max(385, inventory.Stacks.Count * 76)));
            try
            {
                if (inventory.Stacks.Count == 0) Text("Sin carga", 15, 18, 450, 40, 18, Muted);
                for (int i = 0; i < inventory.Stacks.Count; i++)
                {
                    var stack = inventory.Stacks[i]; float y = i * 76;
                    RectColor(0, y, 490, 69, new Color(.065f, .14f, .17f));
                    RectColor(0, y, 3, 69, stack.item.color);
                    Text(stack.item.displayName, 12, y + 6, 240, 38, 16, Color.white);
                    Text(stack.count + " / " + Mathf.Max(1, stack.item.maxStack) + " por pila", 12, y + 44, 240, 22, 13, Muted);
                    if (!session.IsCargoItem(stack.item))
                    { Text("Solo en mochila", 266, y + 24, 214, 32, 14, Muted); continue; }
                    GUI.enabled = session.CanTransferCargo(deposit, stack.item, 1);
                    bool one = Button((deposit ? "Guardar" : "Retirar") + " 1", 262, y + 9, 108, 48);
                    GUI.enabled = session.CanTransferCargo(deposit, stack.item, stack.count);
                    bool pile = Button("Pila (" + stack.count + ")", 377, y + 9, 103, 48);
                    GUI.enabled = true;
                    if (one || pile)
                    { session.TransferCargo(deposit, stack.item, one ? 1 : stack.count); break; }
                }
            }
            finally { GUI.enabled = true; GUI.EndScrollView(); }
        }
    }
}
