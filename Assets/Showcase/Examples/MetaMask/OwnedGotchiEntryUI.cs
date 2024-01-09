using System;
using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace PortalDefender.AavegotchiKit.Examples.MetaMask
{
    [RequireComponent(typeof(Gotchi))]
    public class OwnedGotchiEntryUI : MonoBehaviour
    {
        [SerializeField]
        TMP_Text nameLabel_;

        [SerializeField]
        Image bg_;

        [SerializeField]
        GameObject petMeNotification_;

        [SerializeField]
        Button petMeButton_;

        Gotchi gotchi_;
        public Gotchi Gotchi => gotchi_;

        GotchiData data_;
        public GotchiData Data => data_;

        public bool IsSelected { get; set; }

        public event Action<GotchiData> PetMeClicked;

        private void Awake()
        {
            gotchi_ = GetComponent<Gotchi>();
            bg_.color = Color.grey;
        }

        public void Init(GotchiData data)
        {
            data_ = data;
            gotchi_.Init(data);
            gotchi_.State.HandPose = GotchiHandPose.DOWN_OPEN;            
            nameLabel_.text = data_.name + " (" + data_.id + ")";
            petMeButton_.onClick.AddListener(() => { 
                IsSelected = !IsSelected;
                bg_.color = IsSelected ? Color.green : Color.grey;
            });
        }

        public void UpdatePetStatus(long timestamp)
        {
            var lastInteracted = BigInteger.Parse(Data.lastInteracted);
            var nextInteraction = lastInteracted + (12 * 60 * 60 * 1000);

            if (timestamp > nextInteraction)
            {
                Debug.Log("Time to pet gotchi! " + Data.id);
                petMeNotification_.SetActive(true);
            }
            else
            {
                petMeNotification_.SetActive(false);
            }
        }

    }
}
