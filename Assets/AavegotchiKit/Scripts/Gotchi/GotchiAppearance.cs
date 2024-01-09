using System.Collections.Generic;
using UnityEngine;

namespace PortalDefender.AavegotchiKit
{
    /// <summary>
    /// Early implementation of gotchi appearance that uses an offline db to fetch sprites for the gotchi appearance.<br/>
    /// It attempts to mimic the behaviour of the AavegotchiDiamond SvgFacet/SvgViewsFacet to render an Aavegotchi's appearance
    /// </summary>
    /// 
    /// <remarks>
    /// This implementation was initially created because of issues with rendering SVGs in Unity;<br/>
    /// In the first iteration, the SVGs were rendered to PNGs and then imported into Unity as sprites<br/>
    /// After fixing the SVG rendering issues, this implementation changed so that it uses the SVGs directly.<br/>
    /// <br/>
    /// Some of the benefits of this approach are:<br/>
    /// - It doesn't require making web requests to fetch the SVGs<br/>
    /// - It doesn't require a Web3Provider in the scene<br/>
    /// - It allows for customization of wearables<br/>
    /// - Could possibly add support for custom wearables<br/>
    /// - Easy to modify layers, eyes, mouth, hand poses, etc.<br/>
    /// <br/>
    /// Some of the downsides of this approach are:<br/>
    /// - Rendering bugs: there are some special cases (sleeves?) that are tricky to handle<br/>
    /// - The DB needs to be updated whenever new wearables are added to the game.<br/>
    /// </remarks>
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

        [SerializeField]
        Material material_;

        GotchiBillboard billboard;

        class EquippedWearable
        {
            public Wearable data;
            public SpriteRenderer spriteRenderer;
            public GameObject gameObject;
            public SpriteRenderer sleevesSpriteRenderer;
            public GameObject sleevesGameObject;
        }

        Dictionary<GotchiEquipmentSlot, EquippedWearable> equippedWearables = new Dictionary<GotchiEquipmentSlot, EquippedWearable>();

        public void Awake()
        {
            billboard = GetComponentInChildren<GotchiBillboard>();

            body.sprite = null;
            hands.sprite = null;
            eyes.sprite = null;
            mouth.sprite = null;
            shadow.sprite = null;
            collateral.sprite = null;

        }

        public void Init(Gotchi gotchi)
        {
            this.gotchi = gotchi;

            this.gotchi.State.PropertyChanged -= State_PropertyChanged;

            if (billboard != null)
                billboard.PropertyChanged -= State_PropertyChanged;

            //Init wearables
            foreach (var wearable in equippedWearables.Values)
            {
                Destroy(wearable.gameObject);
                Destroy(wearable.sleevesGameObject);
            }

            equippedWearables.Clear();

            for (int i = 0; i < gotchi.Data.equippedWearables.Length; i++)
            {
                GotchiEquipmentSlot slot = (GotchiEquipmentSlot)i;

                //ignore BG
                if (slot == GotchiEquipmentSlot.BG)
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
                    wearableRenderer.material = material_;
                    wearableRenderer.sortingLayerName = "Characters";

                    equippedWearable.gameObject = wearableObj;
                    equippedWearable.spriteRenderer = wearableRenderer;

                    //check for sleeves
                    if (wearableData.HasSleeves)
                    {
                        var sleevesObj = new GameObject(wearableData.name + "_sleeves");
                        sleevesObj.transform.SetParent(floating, false);
                        var sleevesRenderer = sleevesObj.AddComponent<SpriteRenderer>();
                        sleevesRenderer.material = material_;
                        sleevesRenderer.sortingLayerName = "Characters";

                        equippedWearable.sleevesGameObject = sleevesObj;
                        equippedWearable.sleevesSpriteRenderer = sleevesRenderer;
                    }

                    equippedWearables.Add((GotchiEquipmentSlot)i, equippedWearable);

                    //force hands open
                    if (gotchi.State.HandPose == GotchiHandPose.DOWN_CLOSED
                        && slot == GotchiEquipmentSlot.BODY || slot == GotchiEquipmentSlot.HAND_LEFT || slot == GotchiEquipmentSlot.HAND_RIGHT)
                    {
                        gotchi.State.HandPose = GotchiHandPose.DOWN_OPEN;
                    }

                }
            }
            Refresh();

            this.gotchi.State.PropertyChanged += State_PropertyChanged;
            
            if ( billboard != null )
                billboard.PropertyChanged += State_PropertyChanged;
        }

        private void State_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            Refresh();
        }

        GotchiFacing Facing {
            get
            {
                var offset = billboard == null ? 0 : billboard.FacingOffset;
                return gotchi.State.Facing + offset;
            }
        }

        GotchiHandPose HandPose => gotchi.State.HandPose;
        GotchiEyeExpression EyeExpression => gotchi.State.EyeExpression;
        GotchiMouthExpression MouthExpression => gotchi.State.MouthExpression;


        void Refresh()
        {
            var collateralData = GotchiDataProvider.Instance.GetCollateral(gotchi.Data.collateral);

            if (collateralData == null)
                return;

            body.sprite = GotchiDataProvider.Instance.GetBodySprite(collateralData, Facing);
            hands.sprite = GotchiDataProvider.Instance.GetHandsSprite(collateralData, HandPose, Facing);
            eyes.sprite = GotchiDataProvider.Instance.GetSpecialEyesSprite(
                gotchi.Data.GetTraitValue(GotchiTrait.EyeShape),
                gotchi.Data.GetTraitValue(GotchiTrait.EyeColor),
                collateralData,
                gotchi.Data.hauntId,
                Facing,
                EyeExpression);
            collateral.sprite = collateralData.GetCollateralSprite(Facing);
            mouth.sprite = GotchiDataProvider.Instance.GetMouthSprite(collateralData, MouthExpression);
            shadow.sprite = GotchiDataProvider.Instance.GetShadowSprite(Facing);

            foreach (var entry in equippedWearables)
            {
                var slot = entry.Key;
                var wearable = entry.Value;

                //var PPU = 64.0f; //SVG pixels per unit should match what is in SvgLoader

                var sprite = wearable.data.GetSprite(HandPose, Facing);
                
                wearable.spriteRenderer.sprite = sprite;

                if (sprite == null)
                {
                    // Some sprites might be null if they are not supported for the current hand pose and facing
                    continue;
                }

                //var halfSpriteWidth = sprite.rect.width / 2f;
                //var halfSpriteHeight = sprite.rect.height / 2f;

                //var dimensions = wearable.data.GetDimensions(Facing); //offset from TOP LEFT
                //var offsetX = (-32f + dimensions.X + halfSpriteWidth) / PPU;
                //var offsetY = -(-32f + dimensions.Y + halfSpriteHeight) / PPU;

                //flip items in hands
                bool flipped = (Facing == GotchiFacing.BACK && slot == GotchiEquipmentSlot.HAND_LEFT)
                    || (Facing == GotchiFacing.FRONT && slot == GotchiEquipmentSlot.HAND_RIGHT);

                //if (flipped)
                //    offsetX = (-32f + halfSpriteWidth) / PPU;

                //wearable.spriteRenderer.transform.SetLocalPositionAndRotation(new Vector3(offsetX, offsetY, 0), Quaternion.identity);

                wearable.spriteRenderer.flipX = flipped;
                    
                if (wearable.data.HasSleeves)
                {
                    wearable.sleevesSpriteRenderer.sprite = wearable.data.GetSleeveSprite(HandPose, Facing);
                    //wearable.sleevesSpriteRenderer.transform.SetLocalPositionAndRotation(new Vector3(offsetX, offsetY, 0), Quaternion.identity);
                    wearable.sleevesSpriteRenderer.flipX = flipped;
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
            hands.enabled = (HandPose == GotchiHandPose.DOWN_CLOSED && Facing == GotchiFacing.FRONT)
                || (HandPose != GotchiHandPose.DOWN_CLOSED);
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
                        wearable.sleevesSpriteRenderer.enabled = wearable.data.HasSleeves && wearable.sleevesSpriteRenderer.sprite != null && Facing != GotchiFacing.BACK;
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