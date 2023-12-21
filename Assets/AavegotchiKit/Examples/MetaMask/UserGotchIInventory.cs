using Aavegotchi.AavegotchiDiamond.Service;
using Cysharp.Threading.Tasks;
using Nethereum.Unity.Rpc;
using Nethereum.Web3;
using PortalDefender.AavegotchiKit.GraphQL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using TMPro;
using UnityEngine;

namespace PortalDefender.AavegotchiKit.Examples.MetaMask
{
    public class UserGotchiInventory : MonoBehaviour
    {
        //Example of doing something once the scene loads or
        //when the user logs in

        [SerializeField]
        Transform contentRoot_; //the root of the scroll view contents

        [SerializeField]
        OwnedGotchiEntryUI ownedGotchEntryUiPrefab_; //the prefab to use for each displayed gotchi

        [SerializeField]
        TMP_Text blockNumberLabel_; //label to display the current block number

        [SerializeField]
        string selectedAddress_; //can set an arbitrary address to test
        
        List<OwnedGotchiEntryUI> ownedGotchiEntries_ = new List<OwnedGotchiEntryUI>();


        IWeb3 web3_;
        async UniTask<IWeb3> GetWeb3()
        {
            if (web3_ == null)
            {
                //IWeb3 web3 = await LoginManager.Instance.GetWeb3Async();
                web3_ = new Web3(new UnityWebRequestRpcTaskClient(new Uri(Constants.DefaultPolygonRPC)));
            }
            return web3_;
        }


        void Start()
        {
            blockNumberLabel_.text = "";

            if (LoginManager.Instance.IsLoggedIn)
            {
                selectedAddress_ = LoginManager.Instance.SelectedAddress;
                Populate().ContinueWith(CheckBlockNumber).Forget();
            }
            else
            {
                LoginManager.LoggedIn += () =>
                {
                    selectedAddress_ = LoginManager.Instance.SelectedAddress;
                    Populate().ContinueWith(CheckBlockNumber).Forget();
                };
            }

            StartCoroutine(CheckBlockNumberLoop().ToCoroutine());
        }

        async UniTask Populate()
        {
            foreach (var gotchi in ownedGotchiEntries_)
            {
                Destroy(gotchi.gameObject);
            }

            Debug.Log("Loading account for: " + selectedAddress_);
            var user = await GraphManager.Instance.GetUserAccount(selectedAddress_);
            Debug.Log("Account has " + user.gotchisOwned.Length + " gotchis");
            foreach (var ownedGotchi in user.gotchisOwned)
            {
                var gotchiData = await GraphManager.Instance.GetGotchiData(ownedGotchi.id.ToString());
                Debug.Log("Found gotchi:" + gotchiData);

                var ui = Instantiate(ownedGotchEntryUiPrefab_, contentRoot_);
                ui.Init(gotchiData);                
                ownedGotchiEntries_.Add(ui);
            }   
        }

        //coroutine to check block number and update the UI
        async UniTask CheckBlockNumber()
        {
            var web3 = await GetWeb3();
            var blockNumber = await web3.Eth.Blocks.GetBlockNumber.SendRequestAsync();
            //Debug.Log("Current block number: " + blockNumber.ToString());
            
            //display it on the UI
            blockNumberLabel_.text = blockNumber.ToString();

            //last interacted is UTC epoch time in milliseconds
            var currentTime = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();

            //check each gotchi to see if its time to pet it!
            foreach (var ownedGotchiEntry in ownedGotchiEntries_)
            {
                ownedGotchiEntry.UpdatePetStatus(currentTime);
            }
        }

        async UniTask CheckBlockNumberLoop()
        {
            while (true)
            {
                await CheckBlockNumber();
                await UniTask.Delay(5000);
            }
        }


        public void PetGotchis()
        {
            //turn on a spinner to prevent further interactions until this is complete
            _PetGotchis().Forget(); 
        }

        async UniTaskVoid _PetGotchis()
        {
            try
            {
                //get all the gotchis that have been selected for petting
                var tokens = ownedGotchiEntries_.Where(x => x.IsSelected).Select(x => new BigInteger(x.Data.id)).ToList();

                if (tokens.Count == 0)
                {
                    Debug.Log("No gotchis selected for petting");
                    return;
                }   

                IWeb3 web3 = await LoginManager.Instance.GetWeb3Async();
                //need a web3 that we can sign with
                var diamondService = new AavegotchiDiamondService(web3, Constants.AavegotchiDiamondAddress);
                var transactionId = await diamondService.InteractRequestAsync(tokens);
                Debug.Log("Petting result: " + transactionId);

                //TODO: show a toast with the transaction info
                
                //if everything is ok, we can refresh the UI                
                //TODO: after we get the result, the graph might take a bit to catch up
                
                Populate().Forget();
                
            }
            catch (Exception e)
            {
                Debug.LogError(e);
            }
            finally
            {
                //turn off spinner
            }
        }


        //TODO: add example of listening for when this data changes
        
        
        
        [Sirenix.OdinInspector.Button]
        void TestPopulate()
        {
            Populate().ContinueWith(CheckBlockNumber).Forget();
        }

    }

}