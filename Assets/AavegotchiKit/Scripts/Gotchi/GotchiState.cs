using System.ComponentModel;

namespace PortalDefender.AavegotchiKit
{
    public class GotchiState : INotifyPropertyChanged
    {
        GotchiFacing _facing;
        public GotchiFacing Facing {
            get { return _facing; }
            set { 
                _facing = value;
                RaisePropertyChanged("Facing");
            }
        }

        GotchiHandPose _handPose;
        public GotchiHandPose HandPose {
            get { return _handPose; }
            set {
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
