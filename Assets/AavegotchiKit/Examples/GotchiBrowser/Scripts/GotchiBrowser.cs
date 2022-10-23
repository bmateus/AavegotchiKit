using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Threading;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


namespace PortalDefender.AavegotchiKit
{
    public class GotchiBrowser : MonoBehaviour
    {
        [SerializeField]
        Gotchi gotchi;

        [SerializeField]
        Button nextButton;

        [SerializeField]
        Button prevButton;

        [SerializeField]
        Button turnButton;

        [SerializeField]
        TMP_InputField idField;

        [SerializeField]
        TMP_Text nameLabel;

        [SerializeField]
        TMP_Dropdown eyeExpressionDropdown;

        [SerializeField]
        TMP_Dropdown mouthExpressionDropdown;

        [SerializeField]
        TMP_Dropdown handposeDropdown;

        [SerializeField]
        int currentGotchi;

        int facing = 0;

        private void Start()
        {
            nextButton.onClick.AddListener(setNext);
            prevButton.onClick.AddListener(setPrev);
            turnButton.onClick.AddListener(turn);
            loadGotchi().Forget();

            idField.text = $"{currentGotchi}";
            idField.onEndEdit.AddListener((val) =>
            {
                if (int.TryParse(val, out currentGotchi))
                {
                    loadGotchi().Forget();
                }
            });

            handposeDropdown.AddOptions(new List<string> { "Down Closed", "Down Open", "Up" });
            handposeDropdown.value = (int)gotchi.State.HandPose;
            handposeDropdown.onValueChanged.AddListener(v => {
                gotchi.State.HandPose = (GotchiHandPose)v;
            });
            
            eyeExpressionDropdown.AddOptions(new List<string> { "None", "Happy", "Mad", "Sleeping" });
            eyeExpressionDropdown.value = (int)gotchi.State.EyeExpression;
            eyeExpressionDropdown.onValueChanged.AddListener(v => {
                gotchi.State.EyeExpression = (GotchiEyeExpression)v;
            });

            mouthExpressionDropdown.AddOptions(new List<string> { "Happy", "Neutral" });
            mouthExpressionDropdown.value = (int)gotchi.State.MouthExpression;
            mouthExpressionDropdown.onValueChanged.AddListener(v => {
                gotchi.State.MouthExpression = (GotchiMouthExpression)v;
            });
        }

        void setNext()
        {
            currentGotchi++;

            idField.text = $"{currentGotchi}";

            loadGotchi().Forget();
        }

        void setPrev()
        {
            currentGotchi--;
            if(currentGotchi < 0)
                currentGotchi = 0;

            idField.text = $"{currentGotchi}";

            loadGotchi().Forget();
        }



        void turn()
        {
            facing = (facing + 1) % 4;
            switch(facing)
            {
                case 0: gotchi.State.Facing = GotchiFacing.FRONT; break;
                case 1: gotchi.State.Facing = GotchiFacing.LEFT; break;
                case 2: gotchi.State.Facing = GotchiFacing.BACK; break;
                case 3: gotchi.State.Facing = GotchiFacing.RIGHT; break;
            }   
        }


        CancellationTokenSource cts;

        async UniTaskVoid loadGotchi()
        {
            if (cts != null)
            {
                cts.Cancel();
            }

            cts = new CancellationTokenSource();

            try
            {
                var gotchiData = await GraphManager.Instance.GetGotchi(currentGotchi.ToString(), cts.Token);
                if (gotchiData != null && gotchiData.collateral != null)
                {
                    gotchi.gameObject.SetActive(true);
                    gotchi.Init(gotchiData);
                    nameLabel.text = gotchiData.name;

                    handposeDropdown.value = (int)gotchi.State.HandPose;
                    eyeExpressionDropdown.value = (int)gotchi.State.EyeExpression;
                    mouthExpressionDropdown.value = (int)gotchi.State.MouthExpression;
                }
                else
                {
                    Debug.Log("Invalid Gotchi Id");
                    gotchi.gameObject.SetActive(false);
                }
            }
            catch (OperationCanceledException ocex)
            {
                Debug.LogException(ocex);
            }
            catch (Exception ex)
            {
                Debug.LogException(ex);
            }
        }

    }
}
