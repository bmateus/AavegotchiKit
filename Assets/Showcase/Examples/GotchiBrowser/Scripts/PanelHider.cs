using TMPro;
using UnityEngine;

namespace PortalDefender.AavegotchiKit.Examples
{    
    //opens and closes a panel
    public class PanelHider : MonoBehaviour
    {
        RectTransform rectTransform;

        float currentX = 0;
        float targetX = 0;

        float totalTransitionTime = 0.33f;
        float currTransitionTime = 0;

        [SerializeField]
        TMP_Text handleText;

        bool isOpen = false;

        private void Awake()
        {
            rectTransform = GetComponent<RectTransform>();
            currentX = rectTransform.anchoredPosition.x;
        }

        public void Toggle()
        {
            if (isOpen)
                Close();
            else
                Open();
        }

        public void Open()
        {
            isOpen = true;
            targetX = 0;
            currTransitionTime = totalTransitionTime;
            handleText.text = "<<";
        }

        public void Close()
        {
            isOpen = false;
            targetX = -rectTransform.rect.width;
            currTransitionTime = totalTransitionTime;
            handleText.text = ">>";
        }

        public void Update()
        {
            //smoothly move the anchored position of the panel to the target position
            if (currTransitionTime > 0)
            {
                currTransitionTime = Mathf.Max(currTransitionTime - Time.deltaTime, 0.0f);
                currentX = Mathf.Lerp(currentX, targetX, 1.0f - currTransitionTime / totalTransitionTime);
                rectTransform.anchoredPosition = new Vector2(currentX, rectTransform.anchoredPosition.y);
            }
        }


    }
}
