/********************************************************************
 * File: Shape.cs                                                   *
 * Author: Dillon Allan and Jared Karpiak                           *
 * Description: Base class used for all shapes rendered in-game.    *
 ********************************************************************/
using System;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace CMPE2800_Lab02
{
    public abstract class Shape
    {
        #region Fields
        // set to increase drawing size
        public const int Tilesize = 50;

        // shape outline
        protected GraphicsPath _model;
        #endregion

        #region Properties
        // removal flag
        public bool IsMarkedForDeath { get; set; }

        // Screen position
        public PointF Position { get; protected set; }
        #endregion

        #region Methods
        /// <summary>
        /// Default instance constructor.
        /// Sets initial position of the Shape.
        /// </summary>
        /// <param name="position">
        /// Screen position of the shape.
        /// </param>
        public Shape(PointF position)
        {
            Position = position;
        }

        /// <summary>
        /// Abstract method that returns a shape's GraphicPath object. 
        /// </summary>
        /// <returns>GraphicPath object.</returns>
        public abstract GraphicsPath GetPath();

        /// <summary>
        /// Abstract method for rendering Shapes. 
        /// </summary>
        /// <param name="gr">Graphics buffer.</param>
        public abstract void Render(Graphics gr);

        /// <summary>
        /// Checks distance to determine if invoking shape 
        /// and arg shape are within one tile size of each other.
        /// </summary>
        /// <param name="shape">Shape to compare position against.</param>
        /// <returns>
        /// True for within one tilesize, 
        /// and false for not within one tilesize.
        /// </returns>
        public bool IsWithinTileSize(Shape shape)
        {
            if ((Math.Abs(Position.X - shape.Position.X) > Tilesize * 2) ||
                (Math.Abs(Position.Y - shape.Position.Y) > Tilesize * 2))
            {
                // not within one tilesize
                return false;
            }

            // within one tilesize
            return true;
        }
        #endregion
    }
}