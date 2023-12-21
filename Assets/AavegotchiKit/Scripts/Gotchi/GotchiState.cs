using System.ComponentModel;

namespace PortalDefender.AavegotchiKit
{
    [System.Serializable]
    public class GotchiState : INotifyPropertyChanged
    {
        GotchiFacing _facing;
        public GotchiFacing Facing {
            get { return _facing; }
            set {
                if (_facing == value)
                    return;
                
                _facing = value;
                RaisePropertyChanged("Facing");
            }
        }

        GotchiHandPose _handPose;
        public GotchiHandPose HandPose {
            get { return _handPose; }
            set {
                if (_handPose == value)
                    return;

                _handPose = value;
                RaisePropertyChanged("HandPose");
            } 
        }

        GotchiMouthExpression _mouthExpression;
        public GotchiMouthExpression MouthExpression
        {
            get { return _mouthExpression; }
            set
            {
                if (_mouthExpression == value)
                    return;

                _mouthExpression = value;
                RaisePropertyChanged("MouthExpression");
            }
        }

        GotchiEyeExpression _eyeExpression;
        public GotchiEyeExpression EyeExpression
        {
            get { return _eyeExpression; }
            set
            {
                if (_eyeExpression == value)
                    return;

                _eyeExpression = value;
                RaisePropertyChanged("EyeExpression");
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        void RaisePropertyChanged(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        public GotchiState()
        {
            _facing = GotchiFacing.FRONT;
            _handPose = GotchiHandPose.DOWN_CLOSED;
            _mouthExpression = GotchiMouthExpression.HAPPY;
            _eyeExpression = GotchiEyeExpression.NONE;
        }

    }
}
