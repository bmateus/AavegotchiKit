using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace PortalDefender.AavegotchiKit
{
    public class GotchiAppearance : MonoBehaviour, IGotchiAppearance
    {
        Gotchi gotchi;

        [SerializeField]
        SpriteRenderer body;

        [SerializeField]
        SpriteRenderer hands;

        [SerializeField]
        SpriteRenderer eyes;

        [SerializeField]
        SpriteRenderer collateral;

        [SerializeField]
        SpriteRenderer mouth;

        [SerializeField]
        SpriteRenderer shadow;

        [SerializeField]
        Transform floating;

        class EquippedWearable
        {
            public Wearable data;
            public SpriteRenderer spriteRenderer;
            public GameObject gameObject;
            public SpriteRenderer sleevesSpriteRenderer;
            public GameObject sleevesGameObject;
        }

        Dictionary<GotchiEquipmentSlot, EquippedWearable> equippedWearables = new Dictionary<GotchiEquipmentSlot, EquippedWearable>();

        public void Init(Gotchi gotchi)
        {
            this.gotchi = gotchi;
            this.gotchi.State.PropertyChanged += State_PropertyChanged; ;


            //Init wearables
            foreach (var wearable in equippedWearables.Values)
            {
                Destroy(wearable.gameObject);
                Destroy(wearable.sleevesGameObject);
            }

            equippedWearables.Clear();

            for (int i = 0; i < gotchi.Data.equippedWearables.Length; i++)
            {
                //ignore BG
                if ((GotchiEquipmentSlot)i == GotchiEquipmentSlot.BG)
                    continue;

                if (gotchi.Data.equippedWearables[i] != 0)
                {
                    var equippedWearable = new EquippedWearable();

                    var wearableData = GotchiDataProvider.Instance.GetWearable(gotchi.Data.equippedWearables[i]);

                    equippedWearable.data = wearableData;

                    var wearableObj = new GameObject(wearableData.name);
                    wearableObj.transform.SetParent(
                        (GotchiEquipmentSlot)i == GotchiEquipmentSlot.PET ? transform : floating, false);
                    var wearableRenderer = wearableObj.AddComponent<SpriteRenderer>();
                    wearableRenderer.sortingLayerName = "Characters";

                    equippedWearable.gameObject = wearableObj;
                    equippedWearable.spriteRenderer = wearableRenderer;

                    //check for sleeves
                    if (wearableData.HasSleeves)
                    {
                        var sleevesObj = new GameObject(wearableData.name + "_sleeves");
                        sleevesObj.transform.SetParent(floating, false);
                        var sleevesRenderer = sleevesObj.AddComponent<SpriteRenderer>();
                        sleevesRenderer.sortingLayerName = "Characters";

                        equippedWearable.sleevesGameObject = sleevesObj;
                        equippedWearable.sleevesSpriteRenderer = sleevesRenderer;
                    }

                    equippedWearables.Add((GotchiEquipmentSlot)i, equippedWearable);
                }
            }

            Refresh();
        }

        private void State_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            Refresh();
        }

        GotchiFacing Facing => gotchi.State.Facing;
        GotchiHandPose HandPose => gotchi.State.HandPose;
        GotchiEyeExpression EyeExpression => gotchi.State.EyeExpression;
        GotchiMouthExpression MouthExpression => gotchi.State.MouthExpression;



        void Refresh()
        {
            body.sprite = GotchiDataProvider.Instance.GetBodySprite(gotchi.Collateral, Facing);
            hands.sprite = GotchiDataProvider.Instance.GetHandsSprite(gotchi.Collateral, HandPose, Facing);
            eyes.sprite = GotchiDataProvider.Instance.GetSpecialEyesSprite(
                gotchi.Data.GetTraitValue(GotchiTrait.EyeShape),
                gotchi.Data.GetTraitValue(GotchiTrait.EyeColor),
                gotchi.Collateral,
                Facing,
                EyeExpression);
            collateral.sprite = gotchi.Collateral.GetCollateralSprite(Facing);
            mouth.sprite = GotchiDataProvider.Instance.GetMouthSprite(gotchi.Collateral, MouthExpression);
            shadow.sprite = GotchiDataProvider.Instance.GetShadowSprite(Facing);

            foreach (var entry in equippedWearables)
            {
                var slot = entry.Key;
                var wearable = entry.Value;

                var PPU = 75.0f; //SVG pixels per unit should match what is in SvgLoader

                var sprite = wearable.data.GetSprite(HandPose, Facing);
                var halfSpriteWidth = sprite.rect.width / 2f;
                var halfSpriteHeight = sprite.rect.height / 2f;

                wearable.spriteRenderer.sprite = sprite;

                var offset = wearable.data.GetOffset(Facing); //offset from TOP LEFT
                var offsetX = (-32f + offset.x + halfSpriteWidth) / PPU;
                var offsetY = -(-32f + offset.y + halfSpriteHeight) / PPU;

                //flip items in hands
                bool flipped = (Facing == GotchiFacing.BACK && slot == GotchiEquipmentSlot.HAND_LEFT)
                    || (Facing == GotchiFacing.FRONT && slot == GotchiEquipmentSlot.HAND_RIGHT);

                if (flipped)
                    offsetX = (-32f + halfSpriteWidth) / PPU;

                wearable.spriteRenderer.transform.SetLocalPositionAndRotation(new Vector3(offsetX, offsetY, 0), Quaternion.identity);

                wearable.spriteRenderer.flipX = flipped;
                    
                if (wearable.data.HasSleeves)
                {
                    //wearable.sleevesSpriteRenderer.sprite = wearable.data.GetSleeveSprite(HandPose, Facing);
                    //should hide sleeves when hands down & closed?
                }
            }

            UpdateBaseVisibility();
            UpdateWearableVisibility();
            UpdateBaseSorting();
            UpdateWearableSorting();
        }

        void UpdateBaseVisibility()
        {
            collateral.enabled = Facing != GotchiFacing.BACK;
            eyes.enabled = Facing != GotchiFacing.BACK;
            mouth.enabled = Facing == GotchiFacing.FRONT;
        }

        void UpdateWearableVisibility()
        {
            foreach (var entry in equippedWearables)
            {
                var slot = entry.Key;
                var wearable = entry.Value;

                if (slot == GotchiEquipmentSlot.BODY)
                {
                    if (wearable.sleevesSpriteRenderer != null)
                    {
                        wearable.sleevesSpriteRenderer.enabled = wearable.data.HasSleeves && Facing != GotchiFacing.BACK;
                    }
                }

                if (slot == GotchiEquipmentSlot.HAND_LEFT)
                {
                    wearable.spriteRenderer.enabled = Facing != GotchiFacing.LEFT && HandPose == GotchiHandPose.DOWN_OPEN;
                }

                if (slot == GotchiEquipmentSlot.HAND_RIGHT)
                {
                    wearable.spriteRenderer.enabled = Facing != GotchiFacing.RIGHT && HandPose == GotchiHandPose.DOWN_OPEN;
                }
            }
        }

        void UpdateBaseSorting()
        {
            switch (Facing)
            {
                case GotchiFacing.FRONT:
                    body.sortingOrder = 1;
                    eyes.sortingOrder = 2;
                    collateral.sortingOrder = 2;
                    mouth.sortingOrder = 2;
                    hands.sortingOrder = 4;
                    shadow.sortingOrder = 1;
                    break;
                case GotchiFacing.LEFT:
                    body.sortingOrder = 1;
                    eyes.sortingOrder = 2;
                    collateral.sortingOrder = 2;
                    mouth.sortingOrder = 2;
                    hands.sortingOrder = 8;
                    shadow.sortingOrder = 1;
                    break;
                case GotchiFacing.RIGHT:
                    body.sortingOrder = 1;
                    eyes.sortingOrder = 2;
                    collateral.sortingOrder = 2;
                    mouth.sortingOrder = 2;
                    hands.sortingOrder = 8;
                    shadow.sortingOrder = 1;
                    break;
                case GotchiFacing.BACK:
                    body.sortingOrder = 3;
                    hands.sortingOrder = 2;
                    shadow.sortingOrder = 1;
                    break;
            }
        }


        static Dictionary<GotchiEquipmentSlot, int[]> wearableSorting = new Dictionary<GotchiEquipmentSlot, int[]>()
        {
            { GotchiEquipmentSlot.BODY, new int[] { 3, 3, 3, 4 } },
            { GotchiEquipmentSlot.FACE, new int[] { 5, 4, 4, 5 } },
            { GotchiEquipmentSlot.EYES, new int[] { 6, 5, 5, 6 } },
            { GotchiEquipmentSlot.HEAD, new int[] { 7, 6, 6, 7 } },
            { GotchiEquipmentSlot.PET, new int[] { 10, 10, 10, 8 } },
            { GotchiEquipmentSlot.HAND_LEFT, new int[] { 9, 1, 7, 1 } },
            { GotchiEquipmentSlot.HAND_RIGHT, new int[] { 9, 7, 1, 1 } },
        };

        static int[] sleevesSorting = new int[] { 8, 9, 9, 1 };

        void UpdateWearableSorting()
        {
            foreach (var entry in equippedWearables)
            {
                var slot = entry.Key;
                var wearable = entry.Value;
                wearable.spriteRenderer.sortingOrder = wearableSorting[slot][(int)Facing];
                if (wearable.sleevesSpriteRenderer != null)
                {
                    wearable.sleevesSpriteRenderer.sortingOrder = sleevesSorting[(int)Facing];
                }
            }
        }

    }
}