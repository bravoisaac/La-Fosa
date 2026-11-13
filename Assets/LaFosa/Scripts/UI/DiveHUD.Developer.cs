using UnityEngine;

namespace LaFosa
{
    public sealed partial class DiveHUD
    {
        void DeveloperBadge()
        {
            Panel(550, 180, 500, 55);
            Text((session.DeveloperMode ? "DEVELOP" : "JUGADOR DE PRUEBA") + " · SIN GUARDAR", 565, 186, 470, 22, 16, Gold, TextAnchor.MiddleCenter);
            Text("F1 herramientas · F2 cambiar modo", 565, 211, 470, 18, 12, Muted, TextAnchor.MiddleCenter);
        }
        void DeveloperPanel()
        {
            DimWorld(new Color(.01f, .025f, .04f, .9f));
            Panel(320, 100, 960, 720);
            Text("PRUEBAS DE DESARROLLO", 355, 125, 880, 40, 29, Gold);
            Text("Tu progreso normal está protegido. Cambia a Jugador para probar las reglas con este equipo y posición. Al terminar, se descartan los cambios y vuelves a la playa.", 355, 177, 870, 64, 17, Muted);
            if (Button(session.DeveloperMode ? "PROBAR COMO JUGADOR [F2]" : "VOLVER A DEVELOP [F2]", 355, 252, 430, 48)) session.ToggleDeveloperMode();
            if (Button("CERRAR PANEL [F1 / ESC]", 805, 252, 430, 48)) session.ToggleDeveloperPanel();
            GUI.enabled = session.DeveloperMode;
            if (Button(session.DeveloperInfiniteAir ? "AIRE Y SALUD FIJOS: SÍ" : "AIRE Y SALUD FIJOS: NO", 355, 320, 430, 44)) session.ToggleDeveloperAir();
            if (Button(session.DeveloperFastMove ? "VELOCIDAD ×3: SÍ" : "VELOCIDAD ×3: NO", 805, 320, 430, 44)) session.ToggleDeveloperSpeed();
            if (Button("RECARGAR AIRE Y SALUD", 355, 378, 430, 44)) session.Diver.DeveloperRefill();
            if (Button("AÑADIR $1000 DE PRUEBA", 805, 378, 430, 44)) session.DeveloperAddMoney();
            Text("Inicio y minería reinician la prueba. Equipo conserva el progreso.", 355, 431, 880, 20, 12, Muted);
            if (Button("INICIO SIN EQUIPO", 355, 454, 280, 36)) session.DeveloperPrepareScenario(DeveloperScenario.FirstExpedition);
            if (Button("PRIMERA MINERÍA", 655, 454, 280, 36)) session.DeveloperPrepareScenario(DeveloperScenario.FirstMining);
            if (Button("EQUIPO PARA 650 m", 955, 454, 280, 36)) session.DeveloperEquip();
            Text("IR A UNA ZONA · conserva colisiones y recetas", 355, 497, 880, 24, 15, Mint);
            float[] depths = { 0, 24, 52, 87, 150, 280, 420, 560, 620 };
            for (int i = 0; i < depths.Length; i++)
            {
                float x = 355 + (i % 3) * 300, y = 531 + (i / 3) * 49;
                if (Button(depths[i] == 0 ? "PLAYA" : depths[i] + " m", x, y, 280, 40)) session.DeveloperTravel(depths[i]);
            }
            if (Button("+10 DE CADA MINERAL · PRUEBA", 355, 685, 430, 34)) session.DeveloperMaterials();
            GUI.enabled = true;
            Text("Jugador: reglas normales, sin guardar.", 805, 687, 430, 25, 14, Muted);
            if (Button("TERMINAR PRUEBAS · RECUPERAR MI PARTIDA", 355, 735, 880, 48)) session.EndDeveloperSession();
        }
    }
}
