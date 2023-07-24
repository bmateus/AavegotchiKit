using Cysharp.Threading.Tasks;
using UnityEditor;
using UnityEngine;

namespace PortalDefender.AavegotchiKit
{
    [CreateAssetMenu(fileName = "Wearable", menuName = "Aavegotchi/Wearable")]
    public class WearableScriptableObject : ScriptableObject
    {
        public Wearable data = new Wearable();

#if UNITY_EDITOR

        [ContextMenu("Refresh Data")]
        public async UniTask Refresh()
        {
            Debug.Log($"Refreshing Data for wearable id: {data.id}");
            WearableFetcher fetcher = new WearableFetcher();
            var w = await fetcher.GetWearable(data.id);
            if (w != null)
            {
                data = w;
                EditorUtility.SetDirty(this);

            }
        }

#endif

    }
}