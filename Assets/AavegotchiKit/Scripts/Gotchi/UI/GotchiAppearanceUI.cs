using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace PortalDefender.AavegotchiKit
{
    using Image = Unity.VectorGraphics.SVGImage;

    public class GotchiAppearanceUI : MonoBehaviour, IGotchiAppearance
    {
        Gotchi gotchi;

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

        [SerializeField]
        Material material_;

        class EquippedWearableUI
        {
            public Wearable data;
            public Image image;
            public GameObject gameObject;
            public Image sleevesImage;
            public GameObject sleevesGameObject;
        }

        Dictionary<GotchiEquipmentSlot, EquippedWearableUI> equippedWearables = 
            new Dictionary<GotchiEquipmentSlot, EquippedWearableUI>();

        Dictionary<Image, int> sortOrder = new Dictionary<Image, int>();

        private void Start()
        {
        }

        public void Init(Gotchi gotchi)
        {
            this.gotchi = gotchi;

            this.gotchi.State.PropertyChanged -= State_PropertyChanged;

            //Init wearables
            foreach (var wearable in equippedWearables.Values)
            {
                Destroy(wearable.gameObject);
                Destroy(wearable.sleevesGameObject);
            }

            equippedWearables.Clear();
            sortOrder.Clear();

            for (int i = 0; i < gotchi.Data.equippedWearables.Length; i++)
            {
                GotchiEquipmentSlot slot = (GotchiEquipmentSlot)i;

                //ignore BG
                if (slot == GotchiEquipmentSlot.BG)
                    continue;


                if (gotchi.Data.equippedWearables[i] != 0)
                {
                    var equippedWearable = new EquippedWearableUI();

                    var wearableData = GotchiDataProvider.Instance.GetWearable(gotchi.Data.equippedWearables[i]);

                    equippedWearable.data = wearableData;

                    var wearableObj = new GameObject(wearableData.name);
                    wearableObj.transform.SetParent(
                        (GotchiEquipmentSlot)i == GotchiEquipmentSlot.PET ? transform : floating, false);
                    var wearableRenderer = wearableObj.AddComponent<Image>();
                    wearableRenderer.material = material_;

                    equippedWearable.gameObject = wearableObj;
                    equippedWearable.image = wearableRenderer;

                    if ((GotchiEquipmentSlot)i == GotchiEquipmentSlot.BODY)
                    {
                        var sleevesObj = new GameObject(wearableData.name + "-sleeves");
                        sleevesObj.transform.SetParent(floating, false);
                        var sleevesRenderer = sleevesObj.AddComponent<Image>();
                        sleevesRenderer.material = material_;

                        equippedWearable.sleevesGameObject = sleevesObj;
                        equippedWearable.sleevesImage = sleevesRenderer;
                    }

                    equippedWearables.Add((GotchiEquipmentSlot)i, equippedWearable);

                    //force hands open
                    if (gotchi.State.HandPose == GotchiHandPose.DOWN_CLOSED 
                        && slot == GotchiEquipmentSlot.BODY 
                        || slot == GotchiEquipmentSlot.HAND_LEFT 
                        || slot == GotchiEquipmentSlot.HAND_RIGHT)
                    {
                        gotchi.State.HandPose = GotchiHandPose.DOWN_OPEN;
                    }
                }
            }

            Refresh();

            this.gotchi.State.PropertyChanged += State_PropertyChanged;
        }

        private void State_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            //Debug.Log($"Property Changed! {e.PropertyName}");
            Refresh();
        }

        GotchiFacing Facing => gotchi.State.Facing;
        GotchiHandPose HandPose => gotchi.State.HandPose;
        GotchiEyeExpression EyeExpression => gotchi.State.EyeExpression;
        GotchiMouthExpression MouthExpression => gotchi.State.MouthExpression;

        void Refresh()
        {
            var collateralData = GotchiDataProvider.Instance.GetCollateral(gotchi.Data.collateral);

            body.sprite = GotchiDataProvider.Instance.GetBodySprite(collateralData, Facing);
            hands.sprite = GotchiDataProvider.Instance.GetHandsSprite(collateralData, HandPose, Facing);
            if (HandPose == GotchiHandPose.UP 
                && (Facing == GotchiFacing.LEFT || Facing == GotchiFacing.RIGHT)
                && !equippedWearables.ContainsKey(GotchiEquipmentSlot.BODY))
            {
                hands.transform.localScale = new Vector3(1, -1, 1);
                //hands.transform.localPosition = new Vector3(0, -12.0f, 0);
            }
            else
            {
                hands.transform.localScale = Vector3.one;
                //hands.transform.localPosition = Vector3.zero;
            }


            eyes.sprite = GotchiDataProvider.Instance.GetSpecialEyesSprite(
                gotchi.Data.GetTraitValue(GotchiTrait.EyeShape),
                gotchi.Data.GetTraitValue(GotchiTrait.EyeColor),
                collateralData, 
                Facing,
                EyeExpression);
            collateral.sprite = collateralData.GetCollateralSprite(Facing);
            mouth.sprite = GotchiDataProvider.Instance.GetMouthSprite(collateralData, MouthExpression);
            shadow.sprite = GotchiDataProvider.Instance.GetShadowSprite(Facing);

            foreach (var entry in equippedWearables)
            {
                var slot = entry.Key;
                var wearable = entry.Value;

                var sprite = wearable.data.GetSprite(HandPose, Facing);
                wearable.image.sprite = sprite;
                
                //var dimensions = wearable.data.GetDimensions(Facing);
                var scaleFactor = 1.0f; // 100.0f / 64.0f;

                if (sprite != null)
                {
                    //resize the image to match the sprite's dimensions
                    wearable.image.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal,
                        sprite.rect.width * scaleFactor);
                    wearable.image.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical,
                        sprite.rect.height * scaleFactor);
                    //wearable.image.rectTransform.pivot = new Vector2(0.0f, 1.0f); //TOP LEFT
                    //wearable.image.rectTransform.anchorMin = new Vector2(0.0f, 1.0f);
                    //wearable.image.rectTransform.anchorMax = new Vector2(0.0f, 1.0f);
                    //wearable.image.rectTransform.anchoredPosition = new Vector3(
                    //    dimensions.X * scaleFactor, dimensions.Y * -scaleFactor, 0);

                    var flipX =
                        (Facing == GotchiFacing.BACK && slot == GotchiEquipmentSlot.HAND_LEFT)
                        || (Facing == GotchiFacing.FRONT && slot == GotchiEquipmentSlot.HAND_RIGHT);

                    wearable.gameObject.transform.rotation = Quaternion.Euler(new Vector3(0, flipX ? 180 : 0, 0));

                    if (flipX)
                    {
                        //wearable.image.rectTransform.anchorMin = new Vector2(1.0f, 1.0f);
                        //wearable.image.rectTransform.anchorMax = new Vector2(1.0f, 1.0f);
                        //wearable.image.rectTransform.anchoredPosition = new Vector3(
                        //    dimensions.X * -scaleFactor, dimensions.Y * -scaleFactor, 0);
                    }
                }
                
                if (slot == GotchiEquipmentSlot.BODY)
                {
                    var sleeveSprite = wearable.data.GetSleeveSprite(HandPose, Facing);
                    wearable.sleevesImage.sprite = sleeveSprite;

                    if (sleeveSprite != null)
                    {
                        wearable.sleevesImage.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, 
                            sleeveSprite.rect.width * scaleFactor);
                        wearable.sleevesImage.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, 
                            sleeveSprite.rect.height * scaleFactor);
                        //wearable.sleevesImage.rectTransform.pivot = new Vector2(0.0f, 1.0f); //TOP LEFT
                        //wearable.sleevesImage.rectTransform.anchorMin = new Vector2(0.0f, 1.0f);
                        //wearable.sleevesImage.rectTransform.anchorMax = new Vector2(0.0f, 1.0f);
                        //wearable.sleevesImage.rectTransform.anchoredPosition = new Vector3(
                        //    dimensions.X * scaleFactor, dimensions.Y * -scaleFactor, 0);
                    }
                    else
                    {
                        wearable.sleevesImage.sprite = null;
                    }
                }
            }

            UpdateBaseVisibility();
            UpdateWearableVisibility();
            UpdateBaseSorting();
            UpdateWearableSorting();
            ApplySorting();
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

                if (slot == GotchiEquipmentSlot.BODY )
                {
                    if (wearable.sleevesImage != null)
                    {
                        wearable.sleevesImage.enabled = wearable.data.HasSleeves 
                            && wearable.sleevesImage.sprite != null 
                            &&  Facing != GotchiFacing.BACK;
                    }
                }
                
                if (wearable.image.sprite == null)
                {
                    wearable.image.enabled = false;
                }
                else
                {
                    wearable.image.enabled = true;
                }

                if (slot == GotchiEquipmentSlot.HAND_LEFT)
                {
                    wearable.image.enabled = Facing != GotchiFacing.LEFT 
                        && HandPose == GotchiHandPose.DOWN_OPEN;
                }

                if (slot == GotchiEquipmentSlot.HAND_RIGHT)
                {
                    wearable.image.enabled = Facing != GotchiFacing.RIGHT 
                        && HandPose == GotchiHandPose.DOWN_OPEN;
                }
            }
        }

        void UpdateBaseSorting()
        {
            switch (Facing)
            {
                case GotchiFacing.FRONT:
                    sortOrder[body] = 1;
                    sortOrder[eyes] = 2;
                    sortOrder[collateral] = 2;
                    sortOrder[mouth] = 2;
                    sortOrder[hands] = 4;
                    //sortOrder[shadow] = 1;
                    break;
                case GotchiFacing.LEFT:
                    sortOrder[body] = 1;
                    sortOrder[eyes] = 2;
                    sortOrder[collateral] = 2;
                    sortOrder[mouth] = 2;
                    sortOrder[hands] = 8;
                    //sortOrder[shadow] = 1;
                    break;
                case GotchiFacing.RIGHT:
                    sortOrder[body] = 1;
                    sortOrder[eyes] = 2;
                    sortOrder[collateral] = 2;
                    sortOrder[mouth] = 2;
                    sortOrder[hands] = 8;
                    //sortOrder[shadow] = 1;
                    break;
                case GotchiFacing.BACK:
                    sortOrder[body] = 3;
                    sortOrder[hands] = 2;
                    //sortOrder[shadow] = 1;
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
                sortOrder[wearable.image] = wearableSorting[slot][(int)Facing];
                if (wearable.sleevesImage != null)
                {
                    sortOrder[wearable.sleevesImage] = sleevesSorting[(int)Facing];
                }
            }
        }

        void ApplySorting()
        {
            var sorted = sortOrder.OrderBy(x => x.Value).Select(x => x.Key);

            int idx = 0;
            foreach(var img in sorted)
            {
                img.transform.SetSiblingIndex(idx++);
            }
        }

    }
}
