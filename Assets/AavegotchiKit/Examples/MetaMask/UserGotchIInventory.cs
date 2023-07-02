using Cysharp.Threading.Tasks;
using UnityEngine;

namespace PortalDefender.AavegotchiKit.Examples.MetaMask
{
    public class UserGotchiInventory : MonoBehaviour
    {
        //Example of doing something once the scene loads or
        //when the user logs in
        
        void Start()
        {
            if (LoginManager.Instance.IsLoggedIn)
            {
                Populate().Forget();
            }
            else
            {
                LoginManager.LoggedIn += () => Populate().Forget();
            }
        }

        async UniTaskVoid Populate()
        {
            Debug.Log("Loading account for: " + LoginManager.Instance.SelectedAddress);
            var user = await GraphManager.Instance.GetUser(LoginManager.Instance.SelectedAddress);
            Debug.Log("Account has " + user.gotchisOwned.Length + " gotchis");
            foreach (var gotchi in user.gotchisOwned)
            {
                var gotchiData = await GraphManager.Instance.GetGotchi(gotchi.id.ToString());
                Debug.Log("Found gotchi:" + gotchiData.name);
            }   
        }


        //TODO: add example of listening for when this data changes


    }

}