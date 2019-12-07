//using System.Windows.Controls;
using System.Windows.Media.Imaging;


namespace TileMan.Components
{
    public class ObjectDescription
    {
        public byte mId;
        public string mName;
        public CroppedBitmap mIcon;
//?        public int mType;
        public string mType;
        public string mDescription;


        public ObjectDescription( byte id, string name, CroppedBitmap icon, string type, string description )
        {
            mId = id;
            mName = name;
            mIcon = icon;
            mType = type;
            mDescription = description;
        }

    }
}
