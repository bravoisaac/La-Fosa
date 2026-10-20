using UnityEngine;

namespace LaFosa
{
    public sealed partial class DiveHUD
    {
        Vector2 shopScroll;
        void Shop()
        {
            var economy = session.Economy;
            var catalog = session.Config.shop;
            RectColor(0, 0, 1600, 900, new Color(.015f, .03f, .035f, .86f));
            Panel(140, 100, 1320, 710);
            Text("DON LUCHO", 180, 130, 600, 50, 36, Gold);
            Text("ALMACÉN DE MAR  /  CALETA BRUMA", 182, 190, 600, 26, 13, Mint);
            Text("TU DINERO   $" + economy.Money, 900, 143, 300, 40, 23, Color.white, TextAnchor.MiddleRight);
            if (Button("SALIR  [ESC]", 1250, 140, 165, 43)) session.CloseShop();

            RectColor(180, 250, 330, 475, new Color(.07f, .16f, .18f));
            Text("TU PESCA DE HOY", 204, 275, 280, 28, 16, Mint);
            Text(economy.FishCount + " peces", 204, 320, 280, 55, 34, Color.white);
            Text("Precio según especie · moneda del juego", 204, 390, 280, 35, 14, Muted);
            Text("RECIBES   $" + economy.SaleValue, 204, 435, 280, 40, 24, Gold);
            GUI.enabled = economy.FishCount > 0;
            if (Button("VENDER TODOS LOS PECES", 204, 493, 282, 48)) economy.SellFish();
            GUI.enabled = true;
            Text("Las conchas y los tesoros se quedan en tu mochila. El equipo comprado no ocupa espacios.", 204, 566, 280, 75, 16, Muted);
            Text("1 manos · 2 tridente · 3 arpón · 4 red\nPrimera herramienta: tridente ($" + session.TridentPrice + ").", 204, 660, 280, 50, 13, Mint);

            Text("EQUÍPATE PARA LA PRÓXIMA SALIDA", 550, 216, 820, 28, 13, Muted);
            var offers = catalog.offers ?? new ShopOffer[0];
            shopScroll = GUI.BeginScrollView(new Rect(550, 250, 865, 475), shopScroll, new Rect(0, 0, 840, Mathf.Max(475, offers.Length * 67)));
            for (int i = 0; i < offers.Length; i++)
            {
                var offer = offers[i]; if (offer == null) continue;
                float y = i * 67;
                bool owned = economy.Has(offer.gear), unlocked = economy.MeetsRequirement(offer.gear);
                bool canBuy = !owned && unlocked && economy.Money >= offer.price;
                RectColor(0, y, 840, 61, i % 2 == 0 ? new Color(.065f, .14f, .16f) : new Color(.045f, .105f, .13f));
                Text(offer.title, 16, y + 5, 600, 25, 18, owned ? Mint : Color.white);
                Text(offer.description, 16, y + 33, 615, 26, 13, Muted);
                GUI.enabled = canBuy;
                if (Button(owned ? "COMPRADO" : !unlocked ? "TRIDENTE PRIMERO" : "$" + offer.price + (canBuy ? "  COMPRAR" : "  SIN SALDO"), 645, y + 12, 180, 38)) economy.Buy(offer);
                GUI.enabled = true;
            }
            GUI.EndScrollView();
            Text(economy.Feedback, 180, 752, 1230, 42, 18, Mint);
        }
    }
}
