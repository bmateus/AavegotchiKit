using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace PortalDefender.AavegotchiKit
{
    public class GotchiAppearanceUI : MonoBehaviour
    {
        AavegotchiData.Facing facing = AavegotchiData.Facing.FRONT;
        AavegotchiData.HandPose handPose = AavegotchiData.HandPose.DOWN_CLOSED;
        AavegotchiData.MouthExpression mouthExpression = AavegotchiData.MouthExpression.HAPPY;
        AavegotchiData.EyeExpression eyeExpression = AavegotchiData.EyeExpression.NONE;

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

        Dictionary<GotchiData.Slot, EquippedWearableUI> equippedWearables = new Dictionary<GotchiData.Slot, EquippedWearableUI>();

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
            handPose = AavegotchiData.HandPose.DOWN_CLOSED;
            if (data.equippedWearables[(int)GotchiData.Slot.BODY] != 0 
                || data.equippedWearables[(int)GotchiData.Slot.HAND_LEFT] != 0 
                || data.equippedWearables[(int)GotchiData.Slot.HAND_RIGHT] != 0)
            {
                handPose = AavegotchiData.HandPose.DOWN_OPEN;
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
                if ((GotchiData.Slot)i == GotchiData.Slot.BG)
                    continue;

                if (data.equippedWearables[i] != 0)
                {
                    var equippedWearable = new EquippedWearableUI();
                    
                    var wearableData = AavegotchiData.Instance.GetWearable(data.equippedWearables[i]);
                    WearablePose pose = wearableData.poses[0];
                    if (handPose == AavegotchiData.HandPose.UP && wearableData.poses.Count > 1)
                    {
                        pose = wearableData.poses[1];
                    }

                    equippedWearable.data = wearableData;

                    var wearableObj = new GameObject(wearableData.name);
                    wearableObj.transform.SetParent(
                        (GotchiData.Slot)i == GotchiData.Slot.PET ? transform : floating, false);
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

                    equippedWearables.Add((GotchiData.Slot)i, equippedWearable);
                }
            }

            UpdateBaseColors();

            Refresh();
        }


        public void SetFacing(AavegotchiData.Facing facing)
        {
            this.facing = facing;
            Refresh();
        }

        void Refresh()
        {
            body.sprite = AavegotchiData.Instance.GetBodySprite(facing);
            hands.sprite = AavegotchiData.Instance.GetHandsSprite(handPose, facing);
            eyes.sprite = AavegotchiData.Instance.GetSpecialEyesSprite(
                data.GetTraitValue(GotchiData.Trait.EyeShape),
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
                if (handPose == AavegotchiData.HandPose.UP && wearable.data.poses.Count > 1)
                {
                    pose = wearable.data.poses[1];
                }
                wearable.image.sprite = pose.sprites[(int)facing];
                
                var flipX = 
                    (facing == AavegotchiData.Facing.BACK && slot == GotchiData.Slot.HAND_LEFT)
                    || (facing == AavegotchiData.Facing.FRONT && slot == GotchiData.Slot.HAND_RIGHT);

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

            int eyeColorTrait = data.GetTraitValue(GotchiData.Trait.EyeColor);
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
            collateral.enabled = facing != AavegotchiData.Facing.BACK;
            eyes.enabled = facing != AavegotchiData.Facing.BACK;
            mouth.enabled = facing == AavegotchiData.Facing.FRONT;
        }

        void UpdateWearableVisibility()
        {
            foreach (var entry in equippedWearables)
            {
                var slot = entry.Key;
                var wearable = entry.Value;

                if (slot == GotchiData.Slot.BODY )
                {
                    if (wearable.sleevesImage != null)
                    {
                        wearable.sleevesImage.enabled = facing != AavegotchiData.Facing.BACK;
                    }
                }
                
                if (slot == GotchiData.Slot.HAND_LEFT)
                {
                    wearable.image.enabled = facing != AavegotchiData.Facing.LEFT;
                }

                if (slot == GotchiData.Slot.HAND_RIGHT)
                {
                    wearable.image.enabled = facing != AavegotchiData.Facing.RIGHT;
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


        static Dictionary<GotchiData.Slot, int[]> wearableSorting = new Dictionary<GotchiData.Slot, int[]>()
        {
            { GotchiData.Slot.BODY, new int[] { 3, 3, 3, 4 } },
            { GotchiData.Slot.FACE, new int[] { 5, 4, 4, 5 } },
            { GotchiData.Slot.EYES, new int[] { 6, 5, 5, 6 } },
            { GotchiData.Slot.HEAD, new int[] { 7, 6, 6, 7 } },
            { GotchiData.Slot.PET, new int[] { 10, 10, 10, 8 } },
            { GotchiData.Slot.HAND_LEFT, new int[] { 9, 1, 7, 1 } },
            { GotchiData.Slot.HAND_RIGHT, new int[] { 9, 7, 1, 1 } },
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
