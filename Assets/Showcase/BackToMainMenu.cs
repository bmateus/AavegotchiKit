using UnityEngine;
using UnityEngine.SceneManagement;

public class BackToMainMenu : MonoBehaviour
{
    public void ReturnToMainMenu()
    {
        SceneManager.LoadSceneAsync(0);
    }
}
