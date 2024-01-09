using UnityEngine;

namespace PortalDefender.AavegotchiKit.Examples
{
    public class GotchiControllerExampleSceneSetup : MonoBehaviour
    {
        void Start()
        {
            Debug.Log("Setting up example scene...");

            //turn off gravity
            Physics2D.gravity = Vector2.zero;

            //check if the unity input system is set up

            //if this is running in the editor and was the first scene lanched then remove the back button
            if (Application.isEditor)
            {
                Destroy(GameObject.Find("BackButton"));
            }
        }
    }
}