using Cysharp.Threading.Tasks;
using PortalDefender.AavegotchiKit.GraphQL;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace PortalDefender.AavegotchiKit.Examples.StressTest
{
    public class GotchiStressTest : MonoBehaviour
    {
        [SerializeField]
        Gotchi gotchiPrefab;

        [SerializeField]
        BoxCollider2D spawnArea;

        [SerializeField]
        TMP_Text gotchiCountText_;

        [SerializeField]
        TMP_Text fpsCounter_;

        [SerializeField]
        Toggle spawningEnabledToggle_;

        [SerializeField]
        int startingGotchiId = 1;

        int currentGotchiId = 0;

        int spawnedGotchiCount = 0;

        int frames = 0;
        float time = 0.0f;

        //Gradually Load in gotchis
        private void Start()
        {
            currentGotchiId = startingGotchiId;
            StartCoroutine(LoadGotchis().ToCoroutine());
        }

        async UniTask LoadGotchis()
        {
            while (true)
            {
                bool shouldWait = await LoadGotchi();

                if (shouldWait)
                    await UniTask.Delay(1000);
            }
        }

        async UniTask<bool> LoadGotchi()
        {
            if (spawningEnabledToggle_.isOn == false)
                return true; 

            currentGotchiId++;
            Debug.Log("Loading Gotchi " + currentGotchiId);

            var gotchiData = await GraphManager.Instance.GetGotchiData(currentGotchiId.ToString());

            //spawn gotchi in a random position within bounds of spawnArea
            if (gotchiData != null 
                && gotchiData.status == 3 
                && gotchiData.name != "Default")
            {
                Debug.Log("Got Gotchi " + gotchiData.name);

                var spawnPosition = new Vector3(
                    Random.Range(spawnArea.bounds.min.x, spawnArea.bounds.max.x),
                    spawnArea.transform.position.y,
                    Random.Range(spawnArea.bounds.min.y, spawnArea.bounds.max.y));

                var gotchi = Instantiate(gotchiPrefab, spawnPosition, Quaternion.identity);
                gotchi.Init(gotchiData);
                gotchi.gameObject.name = $"{gotchiData.name} ({gotchiData.id})";
                gotchi.State.HandPose = GotchiHandPose.DOWN_OPEN;

                spawnedGotchiCount++;
                gotchiCountText_.text = $"Gotchis: {spawnedGotchiCount}";
                return true;
            }
            else
            {
                Debug.Log($"Gotchi {currentGotchiId} not found");
                return false;
            }
        }

        private void Update()
        {
            time += Time.unscaledDeltaTime;
            frames++;

            if (time >= 0.2f)
            {
                fpsCounter_.text = $"FPS: {(int)(frames / time)}";
                time = 0.0f;
                frames = 0;
            }
        }

    }
}