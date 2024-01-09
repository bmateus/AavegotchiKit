using UnityEngine;
using UnityEngine.SceneManagement;

public class Showcase : MonoBehaviour
{
    public void LaunchDemo(string name)
    {
        if (Application.isPlaying)
        {
            UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(name);
            //load the backbutton scene additionally
            UnityEngine.SceneManagement.SceneManager.LoadSceneAsync("BackButton", LoadSceneMode.Additive);
            
        }
    }

    public void LaunchGithub()
    {
        if (Application.isPlaying)
        {
            Application.OpenURL("https://github.com/bmateus/AavegotchiKit");
        }
    }
}
