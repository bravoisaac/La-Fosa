using UnityEngine;

namespace LaFosa
{
    public sealed partial class WorldBuilder
    {
        void BuildCabin()
        {
            var cabin = new GameObject("Cabaña chilena · Tienda Don Lucho"); cabin.transform.SetParent(root); cabin.transform.position = new Vector3(-13, 3, -23);
            var timber = Mat("Madera de cabaña", new Color(.39f, .23f, .13f));
            var paleWood = Mat("Tablón cálido", new Color(.61f, .4f, .23f));
            var teal = Mat("Pintura de caleta", new Color(.14f, .38f, .4f));
            var cream = Mat("Marcos crema", new Color(.89f, .82f, .62f));
            var roofMat = Mat("Zinc rojo", new Color(.48f, .18f, .13f));
            var black = Mat("Pizarra", new Color(.055f, .1f, .1f));
            Shape("Piso y corredor", PrimitiveType.Cube, new Vector3(0, .1f, .6f), new Vector3(8.5f, .2f, 8), timber, true, cabin.transform);
            for (int board = 0; board < 14; board++)
            {
                float y = .35f + board * .24f;
                var material = board % 3 == 0 ? paleWood : timber;
                Shape("Tabla posterior", PrimitiveType.Cube, new Vector3(0, y, -3.2f), new Vector3(8, .225f, .18f), material, true, cabin.transform);
                foreach (float side in new[] { -4f, 4f })
                    Shape("Tabla lateral", PrimitiveType.Cube, new Vector3(side, y, 0), new Vector3(.18f, .225f, 6.4f), material, true, cabin.transform);
            }
            // Open doorway at the left and sales window on the right.
            Shape("Marco izquierdo", PrimitiveType.Cube, new Vector3(-3.8f, 1.9f, 3.2f), new Vector3(.3f, 3.4f, .25f), cream, true, cabin.transform);
            Shape("Marco puerta", PrimitiveType.Cube, new Vector3(-2, 1.9f, 3.2f), new Vector3(.25f, 3.4f, .25f), cream, true, cabin.transform);
            Shape("Dintel", PrimitiveType.Cube, new Vector3(0, 3.5f, 3.2f), new Vector3(8.4f, .32f, .3f), cream, true, cabin.transform);
            Shape("Muro bajo mostrador", PrimitiveType.Cube, new Vector3(1, .7f, 3.2f), new Vector3(5.7f, 1, .25f), teal, true, cabin.transform);
            Shape("Marco derecho", PrimitiveType.Cube, new Vector3(3.8f, 1.9f, 3.2f), new Vector3(.3f, 3.4f, .3f), cream, true, cabin.transform);
            var counter = Shape("Mostrador · interactuar con E", PrimitiveType.Cube, new Vector3(1, 1.25f, 3.5f), new Vector3(5.6f, .18f, 1.05f), paleWood, true, cabin.transform);
            counter.AddComponent<ShopCounter>();
            counter.GetComponent<BoxCollider>().size = new Vector3(1, 10, 1);
            counter.GetComponent<BoxCollider>().center = new Vector3(0, 4, 0);
            foreach (float side in new[] { -1f, 1f })
            {
                var roof = Shape("Techo a dos aguas", PrimitiveType.Cube, new Vector3(side * 2.1f, 4.45f, .3f), new Vector3(4.8f, .2f, 8.4f), roofMat, true, cabin.transform);
                roof.transform.localRotation = Quaternion.Euler(0, 0, side * -25);
                Shape("Pilar de corredor", PrimitiveType.Cylinder, new Vector3(side * 3.8f, 1.8f, 4.3f), new Vector3(.18f, 1.6f, .18f), cream, true, cabin.transform);
            }
            Shape("Cumbrera", PrimitiveType.Cube, new Vector3(0, 5.48f, .3f), new Vector3(.22f, .18f, 8.5f), roofMat, false, cabin.transform);
            Shape("Letrero de madera", PrimitiveType.Cube, new Vector3(.5f, 3.05f, 3.43f), new Vector3(5.5f, .65f, .1f), black, false, cabin.transform);
            var sign = Label("DON LUCHO · ALMACÉN DE MAR", new Vector3(-12.5f, 6.05f, -19.45f), .18f, new Color(.98f, .83f, .48f));
            sign.GetComponent<Billboard>().enabled = false;
            sign.transform.rotation = Quaternion.Euler(0, 180, 0);
            Label("[ E ]  VENDER PESCA / COMPRAR EQUIPO", new Vector3(-12, 4.7f, -18.8f), .09f, Color.white);
            Shape("Estante posterior", PrimitiveType.Cube, new Vector3(1, 1.5f, -2.5f), new Vector3(5, .16f, .75f), paleWood, true, cabin.transform);
            for (int i = 0; i < 4; i++)
            {
                Shape("Bombona de muestra", PrimitiveType.Cylinder, new Vector3(-.6f + i * .8f, 2.1f, -2.5f), new Vector3(.4f, .53f, .4f), teal, false, cabin.transform);
                Shape("Válvula", PrimitiveType.Cube, new Vector3(-.6f + i * .8f, 2.68f, -2.5f), new Vector3(.18f, .15f, .15f), cream, false, cabin.transform);
            }
            Shape("Don Lucho · chaqueta", PrimitiveType.Capsule, new Vector3(.8f, 1.1f, 2.4f), new Vector3(.65f, .8f, .55f), Mat("Chaqueta amarilla", new Color(.86f, .58f, .16f)), false, cabin.transform);
            Rock("Rostro de Don Lucho", new Vector3(.8f, 2.1f, 2.4f), new Vector3(.32f, .4f, .3f), Mat("Piel", new Color(.65f, .43f, .3f)), false, cabin.transform);
            Shape("Sombrero de pescador", PrimitiveType.Cylinder, new Vector3(.8f, 2.46f, 2.4f), new Vector3(.85f, .08f, .65f), cream, false, cabin.transform);
            Shape("Copa del sombrero", PrimitiveType.Cylinder, new Vector3(.8f, 2.59f, 2.4f), new Vector3(.55f, .12f, .46f), cream, false, cabin.transform);
            foreach (float eye in new[] { .69f, .91f }) Shape("Ojo", PrimitiveType.Sphere, new Vector3(eye, 2.17f, 2.67f), Vector3.one * .065f, black, false, cabin.transform);
            Shape("Caja de pesca", PrimitiveType.Cube, new Vector3(2.8f, .45f, 4), new Vector3(1.1f, .55f, .65f), teal, true, cabin.transform);
        }

        public void BuildVehicles(GameSession session)
        {
            session.Boat = BuildVehicle(session, GearKind.Boat, new Vector3(4.8f, config.waterLevel, 17));
            session.JetSki = BuildVehicle(session, GearKind.JetSki, new Vector3(13.2f, config.waterLevel, 17));
            Label("EMBARCACIONES\nRecoge aquí tus compras", new Vector3(9, 3.8f, 17), .12f, Color.white);
        }
        SurfaceVehicle BuildVehicle(GameSession session, GearKind kind, Vector3 dock)
        {
            bool boat = kind == GearKind.Boat;
            var obj = new GameObject(boat ? "Bote de caleta" : "Moto de agua"); obj.transform.SetParent(root); obj.transform.position = dock;
            var hull = Mat(boat ? "Casco azul" : "Casco naranja", boat ? new Color(.1f, .36f, .5f) : new Color(.95f, .38f, .09f));
            var white = Mat("Cubierta clara", new Color(.87f, .86f, .7f));
            var rubber = Mat("Asiento náutico", new Color(.07f, .1f, .13f));
            if (boat)
            {
                Rock("Casco facetado", new Vector3(0, -.1f, 0), new Vector3(1.35f, .65f, 2.8f), hull, false, obj.transform);
                Shape("Fondo interior", PrimitiveType.Cube, new Vector3(0, .22f, -.25f), new Vector3(1.8f, .14f, 3.7f), white, false, obj.transform);
                foreach (float side in new[] { -1f, 1f }) Shape("Borda", PrimitiveType.Cube, new Vector3(side, .48f, -.2f), new Vector3(.18f, .5f, 4), hull, false, obj.transform);
                foreach (float z in new[] { -.7f, 1f }) Shape("Banco", PrimitiveType.Cube, new Vector3(0, .65f, z), new Vector3(1.8f, .15f, .5f), white, false, obj.transform);
                Shape("Motor fuera de borda", PrimitiveType.Cube, new Vector3(0, .45f, -2.2f), new Vector3(.5f, .85f, .45f), rubber, false, obj.transform);
            }
            else
            {
                Rock("Casco deportivo", new Vector3(0, .1f, 0), new Vector3(.9f, .55f, 1.85f), hull, false, obj.transform);
                Shape("Sillín", PrimitiveType.Cube, new Vector3(0, .55f, -.5f), new Vector3(.6f, .25f, 1.4f), rubber, false, obj.transform);
                Rock("Carenado", new Vector3(0, .65f, .55f), new Vector3(.65f, .45f, .85f), white, false, obj.transform);
                Shape("Manillar", PrimitiveType.Cube, new Vector3(0, 1, .2f), new Vector3(1, .08f, .12f), rubber, false, obj.transform);
            }
            var vehicle = obj.AddComponent<SurfaceVehicle>(); vehicle.session = session; vehicle.kind = kind; vehicle.speed = boat ? 12 : 22;
            vehicle.hullSize = boat ? new Vector3(2.2f, .8f, 4.8f) : new Vector3(1.5f, 1, 3.4f);
            var collider = obj.AddComponent<BoxCollider>(); collider.center = Vector3.up * .25f; collider.size = vehicle.hullSize;
            obj.SetActive(false); return vehicle;
        }

        public GameObject[] BuildFishingTools(Transform camera)
        {
            var hands = new GameObject("Manos · equipo inicial"); hands.transform.SetParent(camera, false);
            var skin = Mat("Manos del buzo", new Color(.64f, .42f, .29f));
            var sleeve = Mat("Mangas del traje", new Color(.06f, .12f, .15f));
            foreach (float side in new[] { -1f, 1f })
            {
                Shape("Antebrazo", PrimitiveType.Cube, new Vector3(side * .24f, -.27f, .3f), new Vector3(.09f, .09f, .25f), sleeve, false, hands.transform);
                Shape("Palma", PrimitiveType.Cube, new Vector3(side * .22f, -.24f, .47f), new Vector3(.095f, .055f, .13f), skin, false, hands.transform);
                for (int finger = 0; finger < 4; finger++)
                    Shape("Dedo", PrimitiveType.Cube, new Vector3(side * .22f - .033f + finger * .022f, -.235f, .56f), new Vector3(.018f, .035f, .08f), skin, false, hands.transform);
                Shape("Pulgar", PrimitiveType.Cube, new Vector3(side * .16f, -.25f, .48f), new Vector3(.035f, .035f, .07f), skin, false, hands.transform);
            }
            var harpoon = BuildHarpoon(camera);
            var steel = Mat("Acero arpón", new Color(.23f, .29f, .32f));
            var rope = Mat("Cuerda de red", new Color(.7f, .74f, .53f));
            var net = new GameObject("Red de mano"); net.transform.SetParent(camera, false); net.transform.localPosition = new Vector3(.33f, -.25f, .7f);
            Shape("Mango de red", PrimitiveType.Cube, new Vector3(0, -.08f, -.1f), new Vector3(.045f, .05f, .7f), steel, false, net.transform);
            for (int i = 0; i <= 6; i++)
            {
                float offset = -.3f + i * .1f;
                Shape("Malla horizontal", PrimitiveType.Cube, new Vector3(0, offset + .3f, .25f), new Vector3(.65f, .012f, .012f), rope, false, net.transform);
                Shape("Malla vertical", PrimitiveType.Cube, new Vector3(offset, .3f, .25f), new Vector3(.012f, .65f, .012f), rope, false, net.transform);
            }
            var trident = new GameObject("Tridente"); trident.transform.SetParent(camera, false); trident.transform.localPosition = new Vector3(.3f, -.28f, .75f);
            Shape("Asta", PrimitiveType.Cube, Vector3.zero, new Vector3(.04f, .04f, 1.1f), steel, false, trident.transform);
            Shape("Horquilla", PrimitiveType.Cube, new Vector3(0, 0, .5f), new Vector3(.35f, .04f, .04f), steel, false, trident.transform);
            for (int i = -1; i <= 1; i++) Rock("Punta", new Vector3(i * .16f, 0, .69f), new Vector3(.035f, .035f, .22f), rope, false, trident.transform);
            harpoon.SetActive(false); net.SetActive(false); trident.SetActive(false);
            return new[] { hands, trident, harpoon, net };
        }
    }
}
