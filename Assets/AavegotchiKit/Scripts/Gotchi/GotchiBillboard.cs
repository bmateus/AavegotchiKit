using System.ComponentModel;
using UnityEngine;

namespace PortalDefender.AavegotchiKit
{
    /// <summary>
    /// Faces the camera and changes direction based on camera movement
    /// </summary>
    public class GotchiBillboard : MonoBehaviour, INotifyPropertyChanged
    {
        Camera cam;

        int facingOffset_ = 0;
        public int FacingOffset
        {
            get { return facingOffset_; }
            set
            {
                if (facingOffset_ == value)
                    return;

                facingOffset_ = value;
                RaisePropertyChanged("FacingOffset");
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        void RaisePropertyChanged(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }


        void Start()
        {
            cam = Camera.main;
        }

        void Update()
        {
            //turn x axis to face camera and set facing direction depending on the angle
            if (cam != null)
            {
                //set facing offset depending on forward angle of the camera
                if (cam.transform.localEulerAngles.y > 45 && cam.transform.localEulerAngles.y < 135)
                {
                    FacingOffset = 2;
                }
                else if (cam.transform.localEulerAngles.y > 135 && cam.transform.localEulerAngles.y < 225)
                {
                    FacingOffset = 3;
                }
                else if (cam.transform.localEulerAngles.y > 225 && cam.transform.localEulerAngles.y < 315)
                {
                    FacingOffset = 1;
                }
                else
                {
                    FacingOffset = 0;
                }

                Vector3 targetForward = Vector3.RotateTowards(transform.forward, cam.transform.forward, 0.1f, 0.0f);
                targetForward.y = 0;
                transform.rotation = Quaternion.LookRotation(targetForward, Vector3.up);                
            }
        }
    }
}