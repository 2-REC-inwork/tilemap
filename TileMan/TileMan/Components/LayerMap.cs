using System;
using System.Windows.Controls.Primitives;
using System.Collections.Generic;


namespace TileMan.Components
{
    class LayerMap
    {
        public Layer mLayer;
        public UniformGrid mGrid;
        public List<LayerMap> mAssociatedLayerMaps;


        public LayerMap( Layer layer, UniformGrid grid )
        {
            mLayer = layer;
            mGrid = grid;
            mAssociatedLayerMaps = new System.Collections.Generic.List<LayerMap>();
        }

        public void Reset()
        {
            mAssociatedLayerMaps.Clear();
        }

        public void AddAssociatedLayerMap( LayerMap layerMap )
        {
            List<LayerMap> otherAssociatedMaps = layerMap.mAssociatedLayerMaps;

            // add other layer to dependencies
            mAssociatedLayerMaps.Add( layerMap );

            // for each dependent layer of the other layer
            for ( int i = 0; i < otherAssociatedMaps.Count; ++i )
            {
                // add dependent layer to dependencies
                mAssociatedLayerMaps.Add( otherAssociatedMaps[ i ] );

                // add this layer to dependent dependencies
                otherAssociatedMaps[ i ].mAssociatedLayerMaps.Add( this );
            }

            // add this layer to other layer's dependencies
            otherAssociatedMaps.Add( this );
        }

        public void RemoveAssociatedLayerMaps()
        {
            // for each dependent layer
            for ( int i = 0; i < mAssociatedLayerMaps.Count; ++i )
            {
                // remove this layer from their dependencies
                mAssociatedLayerMaps[ i ].mAssociatedLayerMaps.Remove( this );
            }

            // remove all layers from dependencies
            mAssociatedLayerMaps.Clear();
        }
    }
}
