using System.Collections.Generic;
using System.Globalization;
using UnityEngine;

namespace LaFosa.Editor
{
    // Original low-poly designs. Dimensions/colors are art direction, not a biological measurement dataset.
    public static class FishRecipes
    {
        public static readonly string[] Bands = { "0–5 m · intermareal", "5–30 m · costa somera", "0–200 m · pelágicos", "50–400 m · plataforma", "300–1.500 m · talud", ">1.000 m · batial y abisal" };
        public static readonly Vector2[] Depths = { new Vector2(1.2f, 4.5f), new Vector2(5, 19), new Vector2(6, 44), new Vector2(26, 49), new Vector2(53, 74), new Vector2(79, 85) };
        // id|name|taxon|band|form|length|width|height|back|belly|accent|pattern|dorsals|dorsal height|eye|value
        const string Source = @"baunco|Baunco|Girella laevifrons|0|Reef|.8|.26|.4|546957|C2C5A1|283B31|Bands|1|.12|.036|30
borrachilla|Borrachilla|Scartichthys viridis|0|Blenny|.65|.18|.22|446F4A|9FAF65|293F29|Mottled|1|.11|.038|30
pejesapo|Pejesapo|Sicyases sanguineus|0|Clingfish|.7|.42|.2|8A3B31|C67754|492D2D|Mottled|1|.055|.034|30
trambollo|Trambollo|Labrisomus philippii|0|Blenny|.78|.21|.24|9A6146|CDA471|4A302D|Bands|1|.14|.044|30
chalaco|Chalaco|Auchenionchus spp.|0|Blenny|.72|.24|.26|75644B|C6B28D|3E3D33|Spots|1|.1|.045|30
tomoyo|Tomoyo|Helcogrammoides spp.|0|Blenny|.42|.12|.13|B76B4C|D7B38B|6D3B32|Bands|3|.07|.028|30
vieja_negra|Vieja negra|Graus nigra|1|Reef|1.35|.43|.61|252C32|626B70|151D27|Plain|1|.18|.052|65
pejeperro|Pejeperro|Semicossyphus darwini|1|Wrasse|1.25|.39|.55|8A3B34|D79069|242E37|Bands|1|.16|.05|70
rollizo|Rollizo|Pinguipes chilensis|1|Hake|1.05|.28|.32|8A714E|CCBE9E|3C5749|Stripe|1|.15|.047|45
bilagay|Bilagay|Cheilodactylus variegatus|1|Reef|1.0|.29|.52|BCB6A0|E1D9BE|4D3E36|Bands|1|.22|.042|45
jerguilla|Jerguilla|Aplodactylus punctatus|1|Reef|1.05|.33|.39|5A624B|ABA77B|252F2A|Spots|1|.13|.04|40
cabrilla|Cabrilla|Paralabrax humeralis|1|Reef|1.02|.36|.47|A17650|D9C29C|4A3430|Spots|2|.18|.05|50
sargo|Sargo|Anisotremus scapularis|1|Reef|.92|.3|.53|8D9A9B|E4DFCC|292F37|Bands|1|.19|.05|45
castaneta|Castañeta|Chromis crusma|1|Reef|.57|.17|.35|353E55|8B899B|222438|Plain|1|.13|.039|35
lenguado|Lenguado|Paralichthys adspersus|1|Flatfish|1.15|.7|.105|817255|BCB59A|443E32|Mottled|1|.05|.045|60
corvina|Corvina|Cilus gilberti|1|Hake|1.5|.4|.45|6A7C84|D4D9CE|405265|Stripe|2|.15|.05|60
pejerrey|Pejerrey de mar|Odontesthes regia|1|Pelagic|.66|.12|.14|71918C|DFE7D6|D7B85C|Stripe|2|.075|.028|30
cabinza|Cabinza|Isacia conceptionis|1|Reef|.74|.2|.3|889B97|D3DCCD|D0AF57|Stripe|1|.1|.034|35
robalo|Róbalo|Eleginops maclovinus|1|Hake|1.3|.34|.43|536B66|B5C6B1|354944|Plain|2|.15|.046|55
anchoveta|Anchoveta|Engraulis ringens|2|Pelagic|.48|.105|.13|3C797C|D4E8DC|A7C9C4|Stripe|1|.055|.025|20
sardina_comun|Sardina común|Strangomera bentincki|2|Pelagic|.57|.125|.185|527A80|DFE4CF|6A9293|Plain|1|.075|.028|25
sardina_espanola|Sardina española|Sardinops sagax|2|Pelagic|.72|.15|.22|457880|DEE5D0|274C61|Spots|1|.09|.034|30
jurel|Jurel|Trachurus murphyi|2|Pelagic|.9|.2|.28|427A82|CDDBCE|C8B75E|Stripe|2|.1|.04|30
caballa|Caballa|Scomber japonicus|2|Pelagic|.95|.21|.3|34778D|D2E1D8|203D5B|Waves|2|.13|.037|40
sierra|Sierra|Thyrsites atun|2|Pelagic|1.65|.22|.25|446F80|C4D8D4|2B4C5B|Stripe|2|.18|.047|65
cojinoba|Cojinoba del norte|Seriolella violacea|2|Reef|1.2|.35|.55|5A698F|B8C5D3|343D61|Plain|2|.16|.048|60
palometa|Palometa o vidriola|Seriola lalandi|2|Pelagic|1.7|.4|.55|467B80|D7E0CB|E7C451|Stripe|2|.22|.056|80
pez_espada|Pez espada|Xiphias gladius|2|Swordfish|2.4|.44|.58|304D68|AFBFBE|253C56|Plain|2|.55|.06|120
merluza_comun|Merluza común|Merluccius gayi|3|Hake|1.25|.3|.34|728690|C8D1C9|455C6A|Plain|2|.14|.056|65
congrio_colorado|Congrio colorado|Genypterus chilensis|3|Eel|1.8|.29|.35|9A473D|D8966A|50342D|Mottled|1|.12|.044|75
congrio_negro|Congrio negro|Genypterus maculatus|3|Eel|1.9|.31|.32|333E3C|939A7B|121E24|Spots|1|.105|.046|75
pejegallo|Pejegallo|Callorhinchus callorynchus|3|Chimera|1.6|.39|.43|919CA2|DADBCB|414D58|Spots|2|.39|.064|85
raya_volantin|Raya volantín|Zearaja chilensis|3|Ray|1.7|1.5|.22|907460|C7B8A1|473E35|Spots|0|0|.044|90
lenguado_ojos_grandes|Lenguado de ojos grandes|Hippoglossina macrops|3|Flatfish|.95|.53|.09|A28B60|D5CAAB|514737|Spots|1|.04|.061|75
tollo|Tollo|Mustelus spp.|3|Shark|2.0|.42|.45|647E85|D7DBCD|425B66|Plain|2|.38|.046|95
congrio_dorado|Congrio dorado|Genypterus blacodes|4|Eel|2.05|.33|.37|B78E58|E5CB94|745937|Mottled|1|.13|.052|100
merluza_austral|Merluza austral|Merluccius australis|4|Hake|1.65|.37|.43|586E7B|BCC9C6|344B5F|Plain|2|.19|.066|105
merluza_cola|Merluza de cola|Macruronus magellanicus|4|Grenadier|1.7|.27|.34|7199A2|D0DFD5|3B6878|Stripe|1|.13|.056|100
merluza_tres_aletas|Merluza de tres aletas|Micromesistius australis|4|Hake|1.4|.3|.38|617F93|C2D7DA|405E76|Plain|3|.16|.06|100
besugo|Besugo|Epigonus crassicaudus|4|Reef|.85|.28|.43|8A6B73|C7B7AE|4D4657|Plain|2|.16|.074|110
alfonsino|Alfonsino|Beryx splendens|4|Reef|1.03|.32|.6|C84D44|E6A389|842E38|Plain|1|.23|.095|125
orange_roughy|Orange roughy|Hoplostethus atlanticus|4|Reef|1.12|.39|.65|D87241|E6B27C|984834|Mottled|1|.24|.08|130
granadero_coelorinchus|Granadero o cola de rata|Coelorinchus spp.|4|Grenadier|1.6|.35|.42|8F9184|C8C9AF|555F5A|Plain|2|.19|.079|115
bacalao_profundidad|Bacalao de profundidad|Dissostichus eleginoides|5|Hake|2.35|.66|.68|535D66|ADB4AD|303841|Mottled|2|.25|.083|170
granadero_coryphaenoides|Granadero abisal|Coryphaenoides spp.|5|Grenadier|1.95|.43|.53|485D72|99AFC1|293C55|Plain|2|.23|.095|145
pez_linterna|Pez linterna|Myctophidae (mictófidos)|5|Lantern|.65|.16|.22|30465B|91AABD|63E5CB|Photophores|2|.085|.058|140
pez_vibora|Pez víbora|Chauliodus spp.|5|Viper|1.25|.21|.28|384A60|9CACB7|8EE6DA|Photophores|1|.27|.075|165
pez_hacha|Pez hacha|Argyropelecus spp.|5|Hatchet|.62|.12|.45|6C91A8|BDDFDF|6CF1D9|Photophores|1|.1|.055|155
pez_caracol|Pez caracol|Liparidae|5|Snailfish|1.08|.4|.41|D9B7BC|F0DED5|AD899F|Plain|1|.06|.034|180";

        public static List<FishSpeciesDefinition> Create()
        {
            var result = new List<FishSpeciesDefinition>();
            foreach (string line in Source.Split('\n'))
            {
                var p = line.Trim().Split('|');
                var species = ScriptableObject.CreateInstance<FishSpeciesDefinition>();
                species.id = p[0]; species.displayName = p[1]; species.scientificName = p[2];
                species.habitatBand = int.Parse(p[3]); species.referenceDepth = Bands[species.habitatBand]; species.playableDepth = Depths[species.habitatBand];
                species.form = (FishForm)System.Enum.Parse(typeof(FishForm), p[4]);
                species.length = Float(p[5]); species.width = Float(p[6]); species.height = Float(p[7]);
                species.backColor = ColorOf(p[8]); species.bellyColor = ColorOf(p[9]); species.accentColor = ColorOf(p[10]);
                species.pattern = (FishPattern)System.Enum.Parse(typeof(FishPattern), p[11]);
                species.dorsalCount = int.Parse(p[12]); species.dorsalHeight = Float(p[13]); species.eyeSize = Float(p[14]);
                species.swimSpeed = species.habitatBand == 0 ? .2f : species.form == FishForm.Pelagic ? .45f : .27f;
                species.population = species.habitatBand == 0 ? 4 : species.habitatBand < 3 ? 3 : 2;
                species.item = ScriptableObject.CreateInstance<ItemDefinition>();
                species.item.id = species.id; species.item.displayName = species.displayName; species.item.description = species.scientificName + " · " + species.referenceDepth;
                species.item.isFish = true; species.item.salePrice = int.Parse(p[15]); species.item.color = new Color(species.backColor.r, species.backColor.g, species.backColor.b, 1); species.item.maxStack = 10;
                result.Add(species);
            }
            return result;
        }
        static float Float(string value) => float.Parse(value, CultureInfo.InvariantCulture);
        static Color ColorOf(string hex) { ColorUtility.TryParseHtmlString("#" + hex, out var color); color.a = 0; return color; }
    }
}
