
namespace TileMan.Components
{
    public class Level
    {
        public int mNbTilesH;
        public int mNbTilesV;
        public int mTileWidth;
        public int mTileHeight;

        public Background mBackground;

        public Layer mMainLayer;
        public Layer mCollisionLayer;
        public LayerObjects mObjectsLayer;
        public LayerObjects mHotspotsLayer;

//        public int mNbVisualLayers;
        public System.Collections.Generic.List<LayerVisual> mVisualLayers;

        public bool mCollisionSameAsMain;


        public Level()
        {
            mVisualLayers = new System.Collections.Generic.List<LayerVisual>();
//            mNbVisualLayers = 0;

            mBackground = new Background();

            mMainLayer = new Layer();
            mCollisionLayer = new Layer();
            mObjectsLayer = new LayerObjects();
            mHotspotsLayer = new LayerObjects();

// !!!! ???? TODO: should be here or in "Reset" ? ???? !!!!
            mCollisionSameAsMain = true;

            Reset();
        }

        public void Reset()
        {
            mVisualLayers.Clear();
//            mNbVisualLayers = 0;

            mNbTilesH = Settings.mNbTilesH;
            mNbTilesV = Settings.mNbTilesV;
            mTileWidth = Settings.mTileWidth;
            mTileHeight = Settings.mTileHeight;

            mBackground.Reset();

            mMainLayer.Reset( "main", Layer.TYPE_MAIN, mNbTilesH, mNbTilesV, mTileWidth, mTileHeight );
            mCollisionLayer.Reset( "collision", Layer.TYPE_COLLISION, mNbTilesH, mNbTilesV, mTileWidth, mTileHeight );
            mObjectsLayer.Reset( "objects", Layer.TYPE_OBJECTS, mNbTilesH, mNbTilesV, mTileWidth, mTileHeight );
            mHotspotsLayer.Reset( "hotspots", Layer.TYPE_HOTSPOTS, mNbTilesH, mNbTilesV, mTileWidth, mTileHeight );

        }

/*
        public void Load(...)
        {
        }
*/


    }
}
