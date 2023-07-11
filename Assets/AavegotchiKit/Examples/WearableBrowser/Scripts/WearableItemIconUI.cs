using Unity.VectorGraphics;
using UnityEngine;

namespace PortalDefender.AavegotchiKit.WearableBrowser
{
    public class WearableItemIconUI : MonoBehaviour
    {
        [SerializeField]
        SVGImage main;

        [SerializeField]
        SVGImage sleeves;

        RectTransform rectTransform;

        private void Awake()
        {
            rectTransform = GetComponent<RectTransform>();
        }

        public void Init(Wearable wearableData)
        {
            var mainSprite = wearableData.GetSprite(GotchiHandPose.DOWN_OPEN, GotchiFacing.FRONT);
            main.sprite = mainSprite;

            //need to explicitly offset to the correct position
            //since it's not handled by the SVG Renderer
            //in the src data, the offset given is from the TOP LEFT of a 64x64 box
            var offset = wearableData.GetOffset(GotchiFacing.FRONT);
            //scale it down to the size of this box
            var scaleFactorX = rectTransform.rect.width / 64.0f;
            var scaleFactorY = rectTransform.rect.height / 64.0f;

            var mainRectTransform = main.rectTransform;
            mainRectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, mainSprite.rect.width * scaleFactorX);
            mainRectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, mainSprite.rect.height * scaleFactorY);
            mainRectTransform.pivot = new Vector2(0.0f, 1.0f); //TOP LEFT
            mainRectTransform.anchorMin = new Vector2(0.0f, 1.0f);
            mainRectTransform.anchorMax = new Vector2(0.0f, 1.0f);
            mainRectTransform.anchoredPosition = new Vector3(offset.x * scaleFactorX, offset.y * -scaleFactorY, 0);

            if (wearableData.HasSleeves)
            {
                var sleevesSprite = wearableData.GetSleeveSprite(GotchiHandPose.DOWN_OPEN, GotchiFacing.FRONT);
                if (sleevesSprite != null)
                {
                    sleeves.sprite = sleevesSprite;
                    sleeves.gameObject.SetActive(true);
                    var sleevesRectTransform = sleeves.rectTransform;
                    sleevesRectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, sleevesSprite.rect.width * scaleFactorX);
                    sleevesRectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, sleevesSprite.rect.height * scaleFactorY);
                    sleevesRectTransform.pivot = new Vector2(0.0f, 1.0f); //TOP LEFT
                    sleevesRectTransform.anchorMin = new Vector2(0.0f, 1.0f);
                    sleevesRectTransform.anchorMax = new Vector2(0.0f, 1.0f);
                    sleevesRectTransform.anchoredPosition = new Vector3(offset.x * scaleFactorX, offset.y * -scaleFactorY, 0);
                }
                else
                {
                    Debug.LogError($"Sleeves sprite not found for {wearableData.name} ({wearableData.id})");
                }
            }
            else
            {
                sleeves.sprite = null;
                sleeves.gameObject.SetActive(false);
            }
        }
    }
}