using System.Linq;
using UnityEngine;

namespace LaFosa
{
    public sealed partial class DiveHUD
    {
        Vector2 shopScroll;
        bool showCrafting;
        ShopKind lastShop = (ShopKind)(-1);
        string Materials(CraftIngredient[] ingredients)
        {
            if (ingredients == null || ingredients.Length == 0) return "Sin materiales · compra inicial";
            return string.Join(" · ", ingredients.Select(i => i.item.displayName + " " + session.Diver.Inventory.Count(i.item) + "/" + i.amount));
        }
        void Shop()
        {
            var economy = session.Economy; var shop = session.CurrentShop;
            if (lastShop != shop) { lastShop = shop; shopScroll = Vector2.zero; showCrafting = shop == ShopKind.Construction; }
            DimWorld(new Color(.015f, .03f, .035f, .86f)); Panel(140, 100, 1320, 710);
            Text(CityShops.Name(shop), 180, 130, 720, 50, 30, Gold);
            Text("CALETA BRUMA  /  COMERCIO Y EXPEDICIONES", 182, 190, 650, 26, 13, Mint);
            Text("TU DINERO   $" + economy.Money, 915, 143, 280, 40, 23, Color.white, TextAnchor.MiddleRight);
            if (Button("SALIR  [ESC]", 1250, 140, 165, 43)) session.CloseShop();
            if (shop == ShopKind.Transit) { DrawTransit(); return; }
            RectColor(180, 250, 330, 475, new Color(.07f, .16f, .18f));
            if (shop == ShopKind.Fishing)
            {
                Text("VENTA DE PESCA", 204, 275, 280, 28, 18, Mint);
                Text(economy.FishCount + " peces", 204, 320, 280, 55, 34, Color.white);
                Text("RECIBES   $" + economy.SaleValue, 204, 410, 280, 40, 24, Gold);
                GUI.enabled = economy.FishCount > 0;
                if (Button("VENDER TODOS LOS PECES", 204, 475, 282, 48)) economy.SellFish(); GUI.enabled = true;
                Text("Aquí compras herramientas de pesca y vendes capturas manuales o automáticas. Los minerales se conservan.", 204, 550, 280, 120, 17, Muted);
            }
            else if (shop == ShopKind.Mining)
            {
                Text("VENTA DE MINERALES", 204, 270, 280, 28, 18, Mint);
                for (int i = 0; i < session.Config.minerals.Length; i++)
                {
                    var item = session.Config.minerals[i]; int amount = session.Diver.Inventory.Count(item); float y = 310 + i * 105;
                    Text(item.displayName + " · " + amount + " u · $" + item.salePrice + "/u", 204, y, 280, 28, 16, Color.white);
                    GUI.enabled = amount > 0;
                    if (Button("VENDER 1", 204, y + 34, 130, 38)) economy.SellMineral(item, 1);
                    if (Button("TODO $" + (long)amount * item.salePrice, 342, y + 34, 142, 38)) economy.SellMineral(item, amount);
                    GUI.enabled = true;
                }
                Text("Reserva materiales para equipo y obras antes de vender. Pico: E frente a una veta bajo el agua.", 204, 640, 280, 75, 15, Muted);
            }
            else
            {
                Text(shop == ShopKind.Construction ? "CRECE TU CALETA" : "PREPARA EL DESCENSO", 204, 275, 280, 50, 21, Mint);
                Text(shop == ShopKind.Construction ? "Obras: " + session.InfrastructureLevel + "/5\n\nMuelle → redes automáticas → base → descensor.\n\nCada obra pide dinero y materiales. Se construye al comprar y se guarda." : "Compra accesorios o fabrica trajes con minerales y dinero.\n\nEl equipo amplía la profundidad segura. El paso por la fosa siempre está abierto.", 204, 350, 280, 300, 19, Color.white);
                Text("Pesca: Don Lucho\nMinería: La Veta\nBuceo: Marea\nObras: Astillero Bruma", 204, 625, 280, 90, 15, Muted);
            }
            bool tabs = shop == ShopKind.Diving || shop == ShopKind.Construction;
            if (tabs)
            {
                if (Button(shop == ShopKind.Construction ? "EMBARCACIONES" : "ACCESORIOS", 550, 205, 410, 35)) { showCrafting = false; shopScroll = Vector2.zero; }
                if (Button(shop == ShopKind.Construction ? "OBRAS DEL MUELLE" : "FABRICAR TRAJES", 980, 205, 435, 35)) { showCrafting = true; shopScroll = Vector2.zero; }
            }
            if (tabs && showCrafting) { if (shop == ShopKind.Construction) DrawConstruction(); else DrawCrafting(); }
            else DrawOffers();
            Text(economy.Feedback, 180, 752, 1230, 42, 18, Mint);
        }
        void DrawOffers()
        {
            var offers = session.Config.shop.offers.Where(o => o.shop == session.CurrentShop).ToArray();
            shopScroll = GUI.BeginScrollView(new Rect(550, 250, 865, 475), shopScroll, new Rect(0, 0, 840, Mathf.Max(475, offers.Length * 140)));
            for (int i = 0; i < offers.Length; i++)
            {
                var offer = offers[i]; float y = i * 140; string requirement = session.Economy.PurchaseRequirement(offer);
                RectColor(0, y, 840, 132, new Color(.065f, .14f, .16f));
                Text(offer.title + " · $" + offer.price, 16, y + 6, 600, 28, 20, Color.white);
                Text(offer.description, 16, y + 37, 600, 38, 14, Muted);
                Text(Materials(offer.ingredients), 16, y + 80, 610, 25, 15, Gold);
                Text(requirement.Length == 0 ? "Dinero y materiales preparados" : requirement, 16, y + 106, 610, 24, 13, Mint);
                GUI.enabled = requirement.Length == 0;
                if (Button(session.Economy.Has(offer.gear) ? "COMPRADO" : "COMPRAR", 645, y + 40, 180, 44)) session.Economy.Buy(offer);
                GUI.enabled = true;
            }
            GUI.EndScrollView();
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
                Text(Materials(recipe.ingredients), 16, y + 69, 600, 26, 15, Gold);
                string requirement = session.Economy.CraftRequirement(recipe);
                Text(requirement.Length == 0 ? "Materiales y dinero preparados" : requirement, 16, y + 105, 600, 24, 14, Muted);
                GUI.enabled = requirement.Length == 0;
                if (Button(session.HasUpgrade(recipe.upgrade) ? "FABRICADO" : "FABRICAR", 645, y + 45, 180, 48)) session.Economy.Craft(recipe);
                GUI.enabled = true;
            }
            GUI.EndScrollView();
        }
        void DrawConstruction()
        {
            var offers = session.Config.infrastructure;
            shopScroll = GUI.BeginScrollView(new Rect(550, 250, 865, 475), shopScroll, new Rect(0, 0, 840, offers.Length * 168));
            for (int i = 0; i < offers.Length; i++)
            {
                var offer = offers[i]; float y = i * 168; string requirement = session.Economy.ConstructionRequirement(i);
                RectColor(0, y, 840, 158, new Color(.065f, .14f, .16f));
                Text(offer.title + " · $" + offer.price, 16, y + 8, 605, 28, 20, Color.white);
                Text(offer.description, 16, y + 42, 605, 46, 15, Muted);
                Text(Materials(offer.ingredients), 16, y + 92, 605, 28, 15, Gold);
                Text(requirement.Length == 0 ? "Lista para construir" : requirement, 16, y + 125, 605, 24, 14, Mint);
                GUI.enabled = requirement.Length == 0;
                if (Button(i < session.InfrastructureLevel ? "CONSTRUIDO" : "CONSTRUIR", 645, y + 48, 180, 48)) session.Economy.Construct(i);
                GUI.enabled = true;
            }
            GUI.EndScrollView();
        }
        void DrawTransit()
        {
            Text("Viajes gratuitos · inventario y aire se conservan", 185, 250, 1100, 35, 22, Mint);
            if (Button("VOLVER A LA CIUDAD", 185, 310, 540, 50)) session.Travel(-1);
            if (Button("IR A LA BASE DE LA FOSA", 755, 310, 650, 50)) session.Travel(0);
            Text("DESCENSOR · requiere obra 5 y usar el terminal de la base. Máxima profundidad explorada: " + session.ExploredDepth.ToString("0") + " m", 185, 395, 1220, 45, 17, Muted);
            for (int i = 0; i < GameSession.DescentStops.Length; i++)
            {
                float depth = GameSession.DescentStops[i]; float x = 185 + (i % 2) * 610, y = 460 + (i / 2) * 65;
                string reason = session.TravelRequirement(depth); GUI.enabled = reason.Length == 0;
                if (Button("BAJAR A " + depth + " m" + (reason.Length == 0 ? "" : " · " + reason), x, y, 590, 50)) session.Travel(depth);
                GUI.enabled = true;
            }
            Text("Baja con reserva de aire para subir. El descensor no recupera aire ni salud.", 185, 752, 1220, 40, 17, Mint);
        }
    }
}