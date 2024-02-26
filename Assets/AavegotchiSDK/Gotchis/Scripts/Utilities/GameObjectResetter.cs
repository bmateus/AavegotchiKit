using System;
using System.Collections.Generic;
using UnityEngine;

namespace GotchiSDK
{

    //--------------------------------------------------------------------------------------------------
    // stores data to be used to reset the object
    //--------------------------------------------------------------------------------------------------
    [Serializable]
    public class GameObjectResetData
    {
        public GameObject GameObject;
        public bool ResetTransform = true;
        public Vector3 ResetPosition;
        public Vector3 ResetScale;
        public Quaternion ResetRotation;
        public bool ShouldResetActive = true;
        public bool ResetActive;
    }

    //--------------------------------------------------------------------------------------------------
    // This class keeps a set object objects and ensures they are reset to values when
    // the game object gets disabled
    // This is particularly useful for animations
    //--------------------------------------------------------------------------------------------------
    public class GameObjectResetter : MonoBehaviour
    {
        [SerializeField] public List<GameObjectResetData> ResetData = new List<GameObjectResetData>();

        //--------------------------------------------------------------------------------------------------
        // Expose this to editor via custom editor to fetch current values and cache them to
        // prevent the need for manually setting everything
        //--------------------------------------------------------------------------------------------------
        public void CacheValues()
        {
            foreach (var resetData in ResetData)
            {
                if (resetData.ResetTransform)
                {
                    resetData.ResetPosition = resetData.GameObject.transform.localPosition;
                    resetData.ResetRotation = resetData.GameObject.transform.localRotation;
                    resetData.ResetScale = resetData.GameObject.transform.localScale;
                }

                if (resetData.ShouldResetActive)
                {
                    resetData.ResetActive = resetData.GameObject.activeSelf;
                }
            }
        }

        //--------------------------------------------------------------------------------------------------
        // When the game object is disable, reset its value so its fresh when next enabled
        //--------------------------------------------------------------------------------------------------
        public void OnDisable()
        {
            foreach (var resetData in ResetData)
            {
                if (resetData.ResetTransform)
                {
                    resetData.GameObject.transform.SetLocalPositionAndRotation(resetData.ResetPosition, resetData.ResetRotation);
                    resetData.GameObject.transform.localScale = resetData.ResetScale;
                }

                if (resetData.ShouldResetActive)
                {
                    resetData.GameObject.SetActive(resetData.ResetActive);
                }
            }
        }
    }
}
