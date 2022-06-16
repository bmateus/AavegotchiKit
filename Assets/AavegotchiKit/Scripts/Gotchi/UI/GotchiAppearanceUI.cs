using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace PortalDefender.AavegotchiKit
{
    public class GotchiAppearanceUI : MonoBehaviour
    {
        GotchiFacing facing = GotchiFacing.FRONT;
        GotchiHandPose handPose = GotchiHandPose.DOWN_CLOSED;
        GotchiMouthExpression mouthExpression = GotchiMouthExpression.HAPPY;
        GotchiEyeExpression eyeExpression = GotchiEyeExpression.NONE;

        [SerializeField]
        Image body;

        [SerializeField]
        Image hands;

        [SerializeField]
        Image eyes;
        
        [SerializeField]
        Image collateral;
        
        [SerializeField]
        Image mouth;
        
        [SerializeField]
        Image shadow;

        [SerializeField]
        Transform floating;

        class EquippedWearableUI
        {
            public Wearable data;
            public Image image;
            public GameObject gameObject;
            public Image sleevesImage;
            public GameObject sleevesGameObject;
        }

        GotchiData data;

        Collateral collateralData;

        Dictionary<GotchiEquipmentSlot, EquippedWearableUI> equippedWearables = new Dictionary<GotchiEquipmentSlot, EquippedWearableUI>();

        MaterialPropertyBlock materialProperties;

        private void Start()
        {
            materialProperties = new MaterialPropertyBlock();
        }

        public void Init(GotchiData data)
        {
            this.data = data;

            collateralData = AavegotchiData.Instance.GetCollateral(data.collateral);

            //should it have open hands or close hands?
            handPose = GotchiHandPose.DOWN_CLOSED;
            if (data.equippedWearables[(int)GotchiEquipmentSlot.BODY] != 0 
                || data.equippedWearables[(int)GotchiEquipmentSlot.HAND_LEFT] != 0 
                || data.equippedWearables[(int)GotchiEquipmentSlot.HAND_RIGHT] != 0)
            {
                handPose = GotchiHandPose.DOWN_OPEN;
            }

            //Init wearables
            foreach (var wearable in equippedWearables.Values)
            {
                Destroy(wearable.gameObject);
                Destroy(wearable.sleevesGameObject);
            }
            equippedWearables.Clear();

            for (int i=0; i<data.equippedWearables.Length; i++)
            {
                //ignore BG
                if ((GotchiEquipmentSlot)i == GotchiEquipmentSlot.BG)
                    continue;

                if (data.equippedWearables[i] != 0)
                {
                    var equippedWearable = new EquippedWearableUI();
                    
                    var wearableData = AavegotchiData.Instance.GetWearable(data.equippedWearables[i]);
                    WearablePose pose = wearableData.poses[0];
                    if (handPose == GotchiHandPose.UP && wearableData.poses.Count > 1)
                    {
                        pose = wearableData.poses[1];
                    }

                    equippedWearable.data = wearableData;

                    var wearableObj = new GameObject(wearableData.name);
                    wearableObj.transform.SetParent(
                        (GotchiEquipmentSlot)i == GotchiEquipmentSlot.PET ? transform : floating, false);
                    var wearableRenderer = wearableObj.AddComponent<Image>();
                    //wearableRenderer.sortingLayerName = "Characters"; //ui is sorted in hierarchy order
                    
                    equippedWearable.gameObject = wearableObj;
                    equippedWearable.image = wearableRenderer;

                    //check for sleeves
                    if (pose.sleeves[0] != null)
                    {
                        var sleevesObj = new GameObject(wearableData.name+"_sleeves");
                        sleevesObj.transform.SetParent(floating, false);
                        var sleevesRenderer = sleevesObj.AddComponent<Image>();   
                        //sleevesRenderer.sortingLayerName = "Characters";

                        equippedWearable.sleevesGameObject = sleevesObj;
                        equippedWearable.sleevesImage = sleevesRenderer;
                    }

                    equippedWearables.Add((GotchiEquipmentSlot)i, equippedWearable);
                }
            }

            UpdateBaseColors();

            Refresh();
        }


        public void SetFacing(GotchiFacing facing)
        {
            this.facing = facing;
            Refresh();
        }

        void Refresh()
        {
            body.sprite = AavegotchiData.Instance.GetBodySprite(facing);
            hands.sprite = AavegotchiData.Instance.GetHandsSprite(handPose, facing);
            eyes.sprite = AavegotchiData.Instance.GetSpecialEyesSprite(
                data.GetTraitValue(GotchiTrait.EyeShape),
                collateralData, 
                facing,
                eyeExpression);
            collateral.sprite = collateralData.GetCollateralSprite(facing);
            mouth.sprite = AavegotchiData.Instance.GetMouthSprite(mouthExpression);
            shadow.sprite = AavegotchiData.Instance.GetShadowSprite(facing);

            foreach (var entry in equippedWearables)
            {
                var slot = entry.Key;
                var wearable = entry.Value;
                WearablePose pose = wearable.data.poses[0];
                if (handPose == GotchiHandPose.UP && wearable.data.poses.Count > 1)
                {
                    pose = wearable.data.poses[1];
                }
                wearable.image.sprite = pose.sprites[(int)facing];
                
                var flipX = 
                    (facing == GotchiFacing.BACK && slot == GotchiEquipmentSlot.HAND_LEFT)
                    || (facing == GotchiFacing.FRONT && slot == GotchiEquipmentSlot.HAND_RIGHT);

                if (flipX)
                    wearable.gameObject.transform.rotation = Quaternion.Euler(new Vector3(0, 180, 0));

                if (wearable.sleevesImage != null)
                {
                    wearable.sleevesImage.sprite = pose.sleeves[(int)facing];
                }
            }

            UpdateBaseVisibility();
            UpdateWearableVisibility();
            UpdateBaseSorting();
            UpdateWearableSorting();
        }


        void UpdateBaseColors()
        {
            Vector4[] colors = new Vector4[16];

            var c = collateralData.PrimaryColor;
            colors[1] = new Vector4(c.r, c.g, c.b, 0);
            
            c = collateralData.SecondaryColor;
            colors[2] = new Vector4(c.r, c.g, c.b, 0);
            
            c = collateralData.CheekColor;
            colors[3] = new Vector4(c.r, c.g, c.b, 0);

            int eyeColorTrait = data.GetTraitValue(GotchiTrait.EyeColor);
            c = AavegotchiData.Instance.GetEyeColor(eyeColorTrait, collateralData);
            colors[4] = new Vector4(c.r, c.g, c.b, 0);

            c = Color.white;
            colors[15] = new Vector4(c.r, c.g, c.b, 0);

            //body.GetPropertyBlock(materialProperties);
            //materialProperties.SetVectorArray("colorTable", colors);
            //body.SetPropertyBlock(materialProperties);

            //hands.GetPropertyBlock(materialProperties);
            //materialProperties.SetVectorArray("colorTable", colors);
            //hands.SetPropertyBlock(materialProperties);

            //eyes.GetPropertyBlock(materialProperties);
            //materialProperties.SetVectorArray("colorTable", colors);
            //eyes.SetPropertyBlock(materialProperties);

            //mouth.GetPropertyBlock(materialProperties);
            //materialProperties.SetVectorArray("colorTable", colors);
            //mouth.SetPropertyBlock(materialProperties);

        }

        void UpdateBaseVisibility()
        {
            collateral.enabled = facing != GotchiFacing.BACK;
            eyes.enabled = facing != GotchiFacing.BACK;
            mouth.enabled = facing == GotchiFacing.FRONT;
        }

        void UpdateWearableVisibility()
        {
            foreach (var entry in equippedWearables)
            {
                var slot = entry.Key;
                var wearable = entry.Value;

                if (slot == GotchiEquipmentSlot.BODY )
                {
                    if (wearable.sleevesImage != null)
                    {
                        wearable.sleevesImage.enabled = facing != GotchiFacing.BACK;
                    }
                }
                
                if (slot == GotchiEquipmentSlot.HAND_LEFT)
                {
                    wearable.image.enabled = facing != GotchiFacing.LEFT;
                }

                if (slot == GotchiEquipmentSlot.HAND_RIGHT)
                {
                    wearable.image.enabled = facing != GotchiFacing.RIGHT;
                }
            }
        }

        void UpdateBaseSorting()
        {
            //switch(facing)
            //{
            //    case AavegotchiData.Facing.FRONT:
            //        body.sortingOrder = 1;
            //        eyes.sortingOrder = 2;
            //        collateral.sortingOrder = 2;
            //        mouth.sortingOrder = 2;
            //        hands.sortingOrder = 4;
            //        shadow.sortingOrder = 1;
            //        break;
            //    case AavegotchiData.Facing.LEFT:
            //        body.sortingOrder = 1;
            //        eyes.sortingOrder = 2;
            //        collateral.sortingOrder = 2;
            //        mouth.sortingOrder = 2;
            //        hands.sortingOrder = 8;
            //        shadow.sortingOrder = 1;
            //        break;
            //    case AavegotchiData.Facing.RIGHT:
            //        body.sortingOrder = 1;
            //        eyes.sortingOrder = 2;
            //        collateral.sortingOrder = 2;
            //        mouth.sortingOrder = 2;
            //        hands.sortingOrder = 8;
            //        shadow.sortingOrder = 1;
            //        break;
            //    case AavegotchiData.Facing.BACK:
            //        body.sortingOrder = 3;
            //        hands.sortingOrder = 2;
            //        shadow.sortingOrder = 1;
            //        break;
            //}
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
            //foreach (var entry in equippedWearables)
            //{
            //    var slot = entry.Key;
            //    var wearable = entry.Value;
            //    wearable.image.sortingOrder = wearableSorting[slot][(int)facing];
            //    if ( wearable.sleevesImage != null )
            //    {
            //        wearable.sleevesImage.sortingOrder = sleevesSorting[(int)facing];
            //    }
            //}
        }
    }
}
