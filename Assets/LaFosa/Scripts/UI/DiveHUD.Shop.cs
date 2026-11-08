using UnityEngine;

namespace LaFosa
{
    public sealed partial class DiveHUD
    {
        Vector2 shopScroll;
        bool showCrafting;
        void Shop()
        {
            var economy = session.Economy;
            var catalog = session.Config.shop;
            DimWorld(new Color(.015f, .03f, .035f, .86f));
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
            Text("Los minerales y las conchas se quedan en la mochila. En FABRICAR combinas minerales y dinero para mejorar tu equipo.", 204, 556, 280, 95, 16, Muted);
            Text("Pico submarino: $" + session.MiningPickPrice + "\nE junto a las vetas para extraer mineral.", 204, 660, 280, 50, 13, Mint);

            if (Button(showCrafting ? "TIENDA" : "TIENDA · ABIERTA", 550, 205, 410, 35)) { showCrafting = false; shopScroll = Vector2.zero; }
            if (Button(showCrafting ? "FABRICAR · ABIERTO" : "FABRICAR", 980, 205, 435, 35)) { showCrafting = true; shopScroll = Vector2.zero; }
            if (showCrafting)
            {
                DrawCrafting();
                Text(economy.Feedback, 180, 752, 1230, 42, 18, Mint);
                return;
            }
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
        void DrawCrafting()
        {
            var recipes = session.Config.recipes;
            shopScroll = GUI.BeginScrollView(new Rect(550, 250, 865, 475), shopScroll, new Rect(0, 0, 840, Mathf.Max(475, recipes.Length * 150)));
            for (int i = 0; i < recipes.Length; i++)
            {
                var recipe = recipes[i]; float y = i * 150;
                RectColor(0, y, 840, 140, new Color(.065f, .14f, .16f));
                Text(recipe.title + " · $" + recipe.price, 16, y + 8, 600, 28, 20, Color.white);
                Text(recipe.upgrade.unlockedDepth + " m · " + recipe.upgrade.unlockedOxygen + " u de aire", 16, y + 40, 600, 22, 15, Mint);
                string materials = "";
                foreach (var ingredient in recipe.ingredients)
                    materials += (materials.Length == 0 ? "" : " · ") + ingredient.item.displayName + " " + session.Diver.Inventory.Count(ingredient.item) + "/" + ingredient.amount;
                Text(materials, 16, y + 69, 600, 26, 15, Gold);
                string requirement = session.Economy.CraftRequirement(recipe);
                Text(requirement.Length == 0 ? "Materiales y dinero preparados" : requirement, 16, y + 105, 600, 24, 14, Muted);
                GUI.enabled = requirement.Length == 0;
                if (Button(session.HasUpgrade(recipe.upgrade) ? "FABRICADO" : "FABRICAR", 645, y + 45, 180, 48)) session.Economy.Craft(recipe);
                GUI.enabled = true;
            }
            GUI.EndScrollView();
        }
    }
}
