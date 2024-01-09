using TMPro;
using UnityEngine;

public class PageController : MonoBehaviour
{
    [SerializeField]
    RectTransform[] pages;

    [SerializeField]
    GameObject nextButton;

    [SerializeField]
    GameObject previousButton;

    [SerializeField]
    TMP_Text notesText;

    [SerializeField]
    string[] notes;

    [SerializeField]
    float MOVE_TIME = 0.3f;

    int currentPage;

    float xTarget = 0;
    float moveSecs = 0;


    private void Start()
    {
        previousButton.SetActive(false);
        nextButton.SetActive(pages.Length > 0);

        notesText.text = notes[0];
    }

    void setPage(int idx)
    {
        currentPage = idx;
        previousButton.SetActive(idx > 0);
        nextButton.SetActive(idx < pages.Length - 1);

        //animate the x position of the pages; set the targetX offset
        xTarget = -idx * 1080;
        moveSecs = MOVE_TIME;

        notesText.text = notes[idx];

    }

    public void NextPage() 
    {   
        if (currentPage < pages.Length - 1)
            setPage(currentPage + 1);        
    }

    public void PreviousPage()
    {
        if (currentPage > 0)
            setPage(currentPage - 1);
    }

    private void Update()
    {
        //animate the x position of the pages
        if (moveSecs > 0)
        {
            moveSecs -= Time.deltaTime;            
            if (moveSecs < 0)
                moveSecs = 0;

            float t = 1 - (moveSecs / MOVE_TIME);
            float x = Mathf.Lerp(pages[0].anchoredPosition.x, xTarget, t);
            for (int i = 0; i < pages.Length; i++)
            {
                pages[i].anchoredPosition = new Vector2(x + (i * 1080), pages[i].anchoredPosition.y);
            }
           
        }
    }


}
