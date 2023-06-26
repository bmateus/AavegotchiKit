using UnityEngine;

public class Showcase : MonoBehaviour
{
    public void LaunchDemo(string name)
    {
        if (Application.isPlaying)
        {
            UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(name);
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
