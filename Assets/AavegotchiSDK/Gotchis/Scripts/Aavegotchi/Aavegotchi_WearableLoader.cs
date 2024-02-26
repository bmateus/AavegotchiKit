using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.ResourceLocations;

namespace GotchiSDK
{
    public class Aavegotchi_WearableLoader : MonoBehaviour
    {
        [SerializeField]
        private Transform RootBone;

        [SerializeField]
        private SkinnedMeshRenderer MainBodyRenderer;

        [Header("Left Hand Sockets")]
        [SerializeField] private Transform LeftHand_Melee;
        [SerializeField] private Transform LeftHand_Shield;
        [SerializeField] private Transform LeftHand_Grenade;
        [SerializeField] private Transform LeftHand_Ranged;

        [Header("Right Hand Sockets")]
        [SerializeField] private Transform RightHand_Melee;
        [SerializeField] private Transform RightHand_Shield;
        [SerializeField] private Transform RightHand_Grenade;
        [SerializeField] private Transform RightHand_Ranged;

        [Header("Pet Socket")]
        [SerializeField] public Transform PetTransform;

        [Header("Face Mouth Socket")]
        [SerializeField] public Transform MouthRoot;
        [SerializeField] public Transform DefaultSmile;

        public Dictionary<EWearableSlot, Aavegotchi_Wearable> LoadedWearables = new Dictionary<EWearableSlot, Aavegotchi_Wearable>();

        private List<EWearableSlot> PendingLoads = new List<EWearableSlot>();
        private Action OnWearablesUpdatedCB = null;

        //--------------------------------------------------------------------------------------------------
        private void OnDestroy()
        {
            UnloadAllWearables(false);
        }

        //--------------------------------------------------------------------------------------------------
        public void UpdateWearables(Aavegotchi_Data data, IList<IResourceLocation> wearableAssetList, Action onWearablesUpdated)
        {
            OnWearablesUpdatedCB = onWearablesUpdated;
            UpdateSlot(data.Body_WearableID, wearableAssetList, EWearableSlot.Body);
            UpdateSlot(data.Face_WearableID, wearableAssetList, EWearableSlot.Face);
            UpdateSlot(data.Eyes_WearableID, wearableAssetList, EWearableSlot.Eyes);
            UpdateSlot(data.Head_WearableID, wearableAssetList, EWearableSlot.Head);
            UpdateSlot(data.HandLeft_WearableID, wearableAssetList, EWearableSlot.Hand_left);
            UpdateSlot(data.HandRight_WearableID, wearableAssetList, EWearableSlot.Hand_right);
            UpdateSlot(data.Pet_WearableID, wearableAssetList, EWearableSlot.Pet);

            if (PendingLoads.Count == 0)
            {
                onWearablesUpdated.Invoke();
            }
        }

        //--------------------------------------------------------------------------------------------------
        private void UpdateSlot(int wearableID, IList<IResourceLocation> wearableAssetList, EWearableSlot slotID)
        {
            if (wearableID == 0)
            {
                // If this slot should be empty, but we have a wearable loaded for it, unload it
                if (LoadedWearables.ContainsKey(slotID))
                {
                    UnloadWearable(LoadedWearables[slotID]);
                    LoadedWearables.Remove(slotID);
                }
            }
            else
            {
                if (LoadedWearables.ContainsKey(slotID) && LoadedWearables[slotID].WearableID != wearableID)
                {
                    // Unload the Addressable asset
                    UnloadWearable(LoadedWearables[slotID]);
                    LoadedWearables.Remove(slotID);
                }

                if (!LoadedWearables.ContainsKey(slotID))
                {
                    LoadWearable(wearableID, wearableAssetList, slotID);
                }
            }
        }

        //--------------------------------------------------------------------------------------------------
        public void LoadWearable(int wearableID, IList<IResourceLocation> wearableAssetList, EWearableSlot slot)
        {
            string addressablesKey = $"Wearable_Mesh_{wearableID}";

            IResourceLocation targetLocation = null;

            foreach (var resourceLocation in wearableAssetList)
            {
                if (resourceLocation.PrimaryKey.Equals(addressablesKey))
                {
                    targetLocation = resourceLocation;
                    break;
                }
            }

            if (targetLocation == null)
            {
                Debug.LogWarning($"Skipping loading of wearable with ID: {wearableID} because its resource was not found");
                return;
            }

            PendingLoads.Add(slot);

            try
            {
                Addressables.LoadAssetAsync<GameObject>(targetLocation).Completed += (task) =>
                {
                    if (task.Result != null)
                    {
                        if (LoadedWearables.ContainsKey(slot))
                        {
                            if (LoadedWearables[slot].WearableID != wearableID)
                            {
                                UnloadWearable(LoadedWearables[slot]);
                                LoadedWearables.Remove(slot);
                            }

                            return;
                        }

                        var instance = Instantiate(task.Result, transform);
                        var loadedWearable = instance.GetComponent<Aavegotchi_Wearable>();
                        if (loadedWearable != null)
                        {
                            FixLayer(instance.transform, gameObject.layer);
                            LoadedWearables[slot] = loadedWearable;
                            loadedWearable.EquippedSlot = slot;
                            loadedWearable.Initialize(RootBone, slot, MainBodyRenderer, () =>
                            {
                                if (loadedWearable.EquippedSlot == EWearableSlot.Hand_left && !loadedWearable.IsSkinnedWeapon)
                                {
                                    if (loadedWearable.WearableTypeHint == EWearableTypeHint.Hand_Shield)
                                    {
                                        loadedWearable.transform.SetParent(LeftHand_Shield, false);
                                    }
                                    else if (loadedWearable.WearableTypeHint == EWearableTypeHint.Hand_Grenade)
                                    {
                                        loadedWearable.transform.SetParent(LeftHand_Grenade, false);
                                    }
                                    else if (loadedWearable.WearableTypeHint == EWearableTypeHint.Hand_Ranged)
                                    {
                                        loadedWearable.transform.SetParent(LeftHand_Ranged, false);
                                    }
                                    else // if (loadedWearable.WearableTypeHint == EWearableTypeHint.Hand_Melee)
                                    {
                                        loadedWearable.transform.SetParent(LeftHand_Melee, false);
                                    }
                                }
                                else if (loadedWearable.EquippedSlot == EWearableSlot.Hand_right && !loadedWearable.IsSkinnedWeapon)
                                {
                                    if (loadedWearable.WearableTypeHint == EWearableTypeHint.Hand_Shield)
                                    {
                                        loadedWearable.transform.SetParent(RightHand_Shield, false);
                                    }
                                    else if (loadedWearable.WearableTypeHint == EWearableTypeHint.Hand_Grenade)
                                    {
                                        loadedWearable.transform.SetParent(RightHand_Grenade, false);
                                    }
                                    else if (loadedWearable.WearableTypeHint == EWearableTypeHint.Hand_Ranged)
                                    {
                                        loadedWearable.transform.SetParent(RightHand_Ranged, false);
                                    }
                                    else // if (loadedWearable.WearableTypeHint == EWearableTypeHint.Hand_Melee)
                                    {
                                        loadedWearable.transform.SetParent(RightHand_Melee, false);
                                    }
                                }
                                else if (loadedWearable.EquippedSlot == EWearableSlot.Pet && loadedWearable.WearableTypeHint == EWearableTypeHint.Pet)
                                {
                                    loadedWearable.transform.SetParent(PetTransform, false);
                                }
                                else if (loadedWearable.EquippedSlot == EWearableSlot.Face && loadedWearable.WearableTypeHint == EWearableTypeHint.Face_Mouth)
                                {
                                    loadedWearable.transform.SetParent(MouthRoot, false);
                                    DefaultSmile.gameObject.SetActive(false);
                                }

                                PendingLoads.Remove(slot);

                                if (PendingLoads.Count == 0)
                                {
                                    OnWearablesUpdatedCB.Invoke();
                                }
                            });
                        }
                        else
                        {
                            PendingLoads.Remove(slot);

                            if (PendingLoads.Count == 0)
                            {
                                OnWearablesUpdatedCB.Invoke();
                            }

                            Destroy(instance);
                            Debug.LogError($"Failed to get Aavegotchi_Wearable script when loading wearable with ID: {wearableID}");
                        }
                    }
                    else
                    {
                        PendingLoads.Remove(slot);

                        if (PendingLoads.Count == 0)
                        {
                            OnWearablesUpdatedCB.Invoke();
                        }

                        Debug.LogError($"Failed to load wearable with ID: {wearableID}");
                    }
                };
            }
            catch (InvalidKeyException)
            {
                Debug.LogWarning($"Failed to load wearble with ID: {wearableID}");

                PendingLoads.Remove(slot);

                if (PendingLoads.Count == 0)
                {
                    OnWearablesUpdatedCB.Invoke();
                }
            }
        }

        //--------------------------------------------------------------------------------------------------
        private void FixLayer(Transform root, int layer)
        {
            root.gameObject.layer = layer;

            foreach (Transform sub in root)
            {
                FixLayer(sub, layer);
            }
        }

        //--------------------------------------------------------------------------------------------------
        private void UnloadWearable(Aavegotchi_Wearable wearable, bool beingDestroyed = false)
        {
            if (!beingDestroyed)
            {
                if (wearable.EquippedSlot == EWearableSlot.Face && wearable.WearableTypeHint == EWearableTypeHint.Face_Mouth)
                {
                    DefaultSmile.gameObject.SetActive(true);
                }
            }
            Addressables.ReleaseInstance(wearable.gameObject);
            Destroy(wearable.gameObject);
        }

        //--------------------------------------------------------------------------------------------------
        private void UnloadAllWearables(bool beingDestroyed)
        {
            foreach (var loadedWearable in LoadedWearables.Values)
            {
                UnloadWearable(loadedWearable, beingDestroyed);
            }

            LoadedWearables.Clear();
        }
    }
}
