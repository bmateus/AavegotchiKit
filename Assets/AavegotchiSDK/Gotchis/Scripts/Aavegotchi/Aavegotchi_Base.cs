using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceLocations;

namespace GotchiSDK
{
    public class Aavegotchi_Base : MonoBehaviour
    {
        [SerializeField] private Aavegotchi_Data CurrentData;

        [SerializeField] private bool Refresh = false;

        [Header("Unity Object Controllers")]
        [SerializeField] private Aavegotchi_Eyes Eyes;

        [SerializeField] public Aavegotchi_Collaterals Collaterals;

        [SerializeField] public Aavegotchi_WearableLoader Wearable_Loader;

        [SerializeField] private Animator GotchiAnimator;

        [SerializeField] private bool isRandomSeedActive = false;

        IList<IResourceLocation> WearableAssetList = null;

        private const string RandomSeed = "RandomSeed";
        private const string MeleeHandsFlag = "MeleeHands";
        private const string GrenadeHandsFlag = "GrenadeHands";
        private const string ShieldHandsFlag = "ShieldHands";
        private const string RangedHandsFlag = "RangedHands";

        private void Start()
        {
            if (WearableAssetList == null)
            {
                Addressables.InitializeAsync().Completed += (task) =>
                {
                    Addressables.LoadResourceLocationsAsync("Wearable").Completed += (task) =>
                    {
                        if (task.Status == AsyncOperationStatus.Succeeded)
                        {
                            if (task.Result != null)
                            {
                                WearableAssetList = task.Result;
                                Refresh = true;
                            }
                        }
                    };
                };
            }
        }


        //--------------------------------------------------------------------------------------------------
        void Update()
        {
            if (isRandomSeedActive)
            {
                GotchiAnimator.SetFloat(RandomSeed, UnityEngine.Random.Range(0, 1001) / 1000.0f);

                foreach (var (key, wearable) in Wearable_Loader.LoadedWearables)
                {
                    if (wearable.CharacterAnimator != null)
                    {
                        wearable.CharacterAnimator.SetFloat(RandomSeed, UnityEngine.Random.Range(0, 1001) / 1000.0f);
                    }
                }
            }

            if (Refresh)
            {
                Refresh = false;
                UpdateForData(CurrentData);
            }

        }

        //--------------------------------------------------------------------------------------------------
        private IResourceLocation GetWearableResource(string key)
        {
            foreach (var location in WearableAssetList)
            {
                if (location.PrimaryKey.Contains(key))
                {
                    return location;
                }
            }

            return null;
        }

        //--------------------------------------------------------------------------------------------------
        public void UpdateForData(Aavegotchi_Data newData, Action OnDataCompleted = null)
        {
            CurrentData = new Aavegotchi_Data(newData);
            if (WearableAssetList == null)
            {
                return;
            }

            Collaterals.UpdateConfiguration(CurrentData.CollateralType);
            Eyes.UpdateConfiguration(CurrentData.EyeShape, CurrentData.CollateralType, CurrentData.EyeColor, Collaterals);

            bool pending = true;
            Wearable_Loader.UpdateWearables(newData, WearableAssetList, () =>
            {
                pending = false;
                gameObject.SetActive(true);
                UpdateWearableLinks();
                OnDataCompleted?.Invoke();
            });

            if (pending)
            {
                gameObject.SetActive(false);
            }
            else
            {
                gameObject.SetActive(true);
                OnDataCompleted?.Invoke();
            }
        }

        //--------------------------------------------------------------------------------------------------
        public Color GetOutlineColor()
        {
            return Collaterals.GetData(CurrentData.CollateralType).PrimaryColor;
        }

        //--------------------------------------------------------------------------------------------------
        // Certain aspects change based on the wearables we were for example, some wearables disable the
        // Collateral rendering. This function updates all of this
        //--------------------------------------------------------------------------------------------------
        private void UpdateWearableLinks()
        {
            bool collateralEnabled = true;
            bool eyesEnabled = true;

            foreach (var (key, value) in Wearable_Loader.LoadedWearables)
            {
                if (value.DisableCollateral)
                {
                    collateralEnabled = false;
                }

                if (value.DisableEyes)
                {
                    eyesEnabled = false;
                }
            }

            Eyes.gameObject.SetActive(eyesEnabled);
            Collaterals.gameObject.SetActive(collateralEnabled);

            UpdateAnimatorFlag(MeleeHandsFlag, EWearableTypeHint.Hand_Melee);
            UpdateAnimatorFlag(GrenadeHandsFlag, EWearableTypeHint.Hand_Grenade);
            UpdateAnimatorFlag(ShieldHandsFlag, EWearableTypeHint.Hand_Shield);
            UpdateAnimatorFlag(RangedHandsFlag, EWearableTypeHint.Hand_Ranged);

            // Shoulders logic for weapons like energy gun
            if (Wearable_Loader.LoadedWearables.ContainsKey(EWearableSlot.Body))
            {
                var leftSleeveDisabled = Wearable_Loader.LoadedWearables.ContainsKey(EWearableSlot.Hand_left) && Wearable_Loader.LoadedWearables[EWearableSlot.Hand_left].DisableShoulder;
                var rightSleeveDisabled = Wearable_Loader.LoadedWearables.ContainsKey(EWearableSlot.Hand_right) && Wearable_Loader.LoadedWearables[EWearableSlot.Hand_right].DisableShoulder;

                Wearable_Loader.LoadedWearables[EWearableSlot.Body].UpdateSleeves(leftSleeveDisabled, rightSleeveDisabled);
            }
        }

        //--------------------------------------------------------------------------------------------------
        // 0 = both hands
        // 1 = favor left hand
        // 2 = favor right hand
        //--------------------------------------------------------------------------------------------------
        private void UpdateAnimatorFlag(string animatorKey, EWearableTypeHint hintToVerifty)
        {
            int flagValue = 0;
            var leftHandWearabe = Wearable_Loader.LoadedWearables.ContainsKey(EWearableSlot.Hand_left) ? Wearable_Loader.LoadedWearables[EWearableSlot.Hand_left] : null;
            var rightHandWearable = Wearable_Loader.LoadedWearables.ContainsKey(EWearableSlot.Hand_right) ? Wearable_Loader.LoadedWearables[EWearableSlot.Hand_right] : null;

            if (leftHandWearabe != null && leftHandWearabe.WearableTypeHint == hintToVerifty && (rightHandWearable == null || rightHandWearable.WearableTypeHint != hintToVerifty))
            {
                flagValue = 1;
            }
            else if (rightHandWearable != null && rightHandWearable.WearableTypeHint == hintToVerifty && (leftHandWearabe == null || leftHandWearabe.WearableTypeHint != hintToVerifty))
            {
                flagValue = 2;
            }
            else if (rightHandWearable != null && leftHandWearabe == null)
            {
                flagValue = 1;
            }
            else if (leftHandWearabe != null && rightHandWearable == null)
            {
                flagValue = 2;
            }

            GotchiAnimator.SetInteger(animatorKey, flagValue);
        }
    }
}