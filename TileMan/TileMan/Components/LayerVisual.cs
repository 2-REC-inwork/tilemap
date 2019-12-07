using System;
using System.Windows;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;


namespace TileMan.Components
{
    public class LayerVisual : Layer
    {
        public double mSizeFactorX;
        public double mSizeFactorY;
        public double mSpeedX;
        public double mSpeedY;

        public LayerVisual() : base()
        {
            mSizeFactorX = 1.0;
            mSizeFactorY = 1.0;
            mSpeedX = 0.0;
            mSpeedY = 0.0;
        }

        public override void Reset( string name, int type, int nbTilesH, int nbTilesV, int tileWidth, int tileHeight )
        {
            base.Reset( name, type, nbTilesH, nbTilesV, tileWidth, tileHeight );

            mSizeFactorX = 1.0;
            mSizeFactorY = 1.0;
            mSpeedX = 0.0;
            mSpeedY = 0.0;
        }

        public void SetSizeFactors( double sizeFactorX, double sizeFactorY )
        {
            mSizeFactorX = sizeFactorX;
            mSizeFactorY = sizeFactorY;
//?...
        }

        public void SetMovingSpeeds( double speedX, double speedY )
        {
            mSpeedX = speedX;
            mSpeedY = speedY;
//?...
        }


        public override int SetTile( double  x, double y )
        {
            return base.SetTile( ( x / mSizeFactorX ), ( y / mSizeFactorY ) );
        }

        public override int UnsetTile( double x, double y )
        {
            return base.UnsetTile( ( x / mSizeFactorX ), ( y / mSizeFactorY ) );
        }

    }
}
