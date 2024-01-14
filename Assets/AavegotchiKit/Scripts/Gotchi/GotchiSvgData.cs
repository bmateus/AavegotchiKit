namespace PortalDefender.AavegotchiKit
{
    [System.Serializable]
    public class GotchiSvgData
    {
        public string front;
        public string left;
        public string right;
        public string back;     
        
       public string GetFacing(GotchiFacing facing)
        {
            switch (facing)
            {
                case GotchiFacing.FRONT:
                    return front;
                case GotchiFacing.LEFT:
                    return left;
                case GotchiFacing.RIGHT:
                    return right;
                case GotchiFacing.BACK:
                    return back;
                default:
                    return null;
            }
        }   
    }    
}
