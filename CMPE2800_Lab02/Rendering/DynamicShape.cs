/********************************************************************
 * File: DynamicShape.cs                                            *
 * Author: Dillon Allan and Jared Karpiak                           *
 * Description: Class derived from Shape.                           *
 *              Used for all moving shapes rendered in-game.        *
 ********************************************************************/
using System.Drawing;
using System.Drawing.Drawing2D;

namespace CMPE2800_Lab02
{
    class DynamicShape : Shape
    {
        #region Members
        // horizontal speed
        public float XSpeed = 0,

            // vertical speed
            YSpeed = 0,

            // rotation (in degrees)
            Rotation = 0,

            // rotation increment (in degrees)
            RotIncrement = 0;     

        // shape color
        public Color Color { get; private set; }

        // player number (all moving shapes belong to a certain player
        public PlayerNumber Player { get; private set; }
        #endregion

        #region Methods
        /// <summary>
        /// Default instance constructor.
        /// Sets initial position and color of the DynamicShape.
        /// </summary>
        /// <param name="position">
        /// Screen position of the shape.
        /// </param>
        /// <param name="color">
        /// Shape color.
        /// </param>
        /// <param name="player">
        /// Player index.
        /// </param>
        public DynamicShape(PointF position,  Color color, PlayerNumber player) : base(position)
        {
            Color = color;
            Player = player;
        }

        /// <summary>
        /// Applies SRT-ordered transformations on the dynamic shape.
        /// </summary>
        /// <returns>
        /// Returns a fully transformed clone of the dynamic shape's model.
        /// </returns>
        public override GraphicsPath GetPath()
        {
            GraphicsPath gp = _model.Clone() as GraphicsPath;
            Matrix mat = new Matrix();

            // in order: rotate, then translate
            mat.Rotate(Rotation);

            // set dynamic shape screen position 
            mat.Translate(Position.X, Position.Y, MatrixOrder.Append);

            // apply transformations to the GraphicsPath object
            gp.Transform(mat);

            return gp;
        }

        /// <summary>
        /// Fills dynamic shape's region in screen space, based on 
        /// its model.
        /// </summary>
        /// <param name="gr">
        /// Screen graphics object.
        /// </param>
        public override void Render(Graphics gr)
        {
            gr.FillRegion(new SolidBrush(Color), new Region(GetPath()));
        }

        /// <summary>
        /// Updates shape position and roration to current speed and rotation values.
        /// </summary>
        /// <param name="size">Client rectangle size.</param>
        public void Tick(/*Size size*/)
        {
            // update rotation
            // (reset rotation to 0 if at 360 degrees,
            //  to avoid an overflow error in the event that
            //  the rotation member gets too big)
            if (Rotation + RotIncrement == 360)
                Rotation = 0;
            Rotation += RotIncrement;

            // update position
            Position = new PointF(Position.X + XSpeed, Position.Y + YSpeed);
        }

        /// <summary>
        /// Makes a copy of the invoking DynamicShape's region,
        /// and checks if it is intersecting with the given Shape's region,
        /// with respect to the given Graphics object.
        /// </summary>
        /// <param name="shape">
        /// Shape being compared against.
        /// </param>
        /// <param name="gr">
        /// Screen graphics.
        /// </param>
        /// <returns>
        /// True for a confirmed intersection, 
        /// false for an empty intersection.
        /// </returns>
        public bool IsIntersecting(Shape shape, Graphics gr)
        {
            // create a copy of the invoker based on its derived type
            DynamicShape thisCopy = null;

            if (this is Tank)
                thisCopy = new Tank(this as Tank);
            else if (this is Gunfire)
                thisCopy = new Gunfire(this as Gunfire);

            // tick the copy if not null
            thisCopy?.Tick();

            // get the copy's region
            Region rCopy = new Region(thisCopy.GetPath());

            // intersect the copy with the provided shape
            rCopy.Intersect(shape.GetPath());

            // if it's empty, not colliding
            if (rCopy.IsEmpty(gr))
                return false;

            // not empty, a collision has occured
            return true;
        }

        /// <summary>
        /// Checks if two valid shapes have collided.
        /// </summary>
        /// <param name="shape">The other shape.</param>
        /// <param name="gr">Screen graphics object.</param>
        /// <returns>
        /// True for a collision, false for no collision.
        /// </returns>
        public bool IsColliding(Shape shape, Graphics gr)
        {
            // ignore Tanks that are already blocked or not alive
            if (this is Tank && (this as Tank).IsBlocked)
                return false;

            // ignore shapes that are already marked for death
            if (IsMarkedForDeath || shape.IsMarkedForDeath)
                return false;

            // check distance to determine if the invoker and shape are capable of colliding
            if (!IsWithinTileSize(shape))
                return false;

            // test ticked position for a collision with shape
            if (!IsIntersecting(shape, gr))
            {
                // not colliding, move along
                return false;
            }

            // collision confirmed
            return true;
        }

        /// <summary>
        /// Checks if the dynamic shape is out of bounds.
        /// </summary>
        /// <param name="level">The level to check bounds.</param>
        /// <param name="gr">Screen graphics object.</param>
        /// <returns>
        /// True if the shape is out of bounds, and
        /// false if the shape is within bounds.
        /// </returns>
        public bool IsOutOfBounds(Level level, Graphics gr)
        {
            // make a copy of the invoker, based on derived type
            DynamicShape thisCopy = null;

            if (this is Tank)
                thisCopy = new Tank(this as Tank);
            else if (this is Gunfire)
                thisCopy = new Gunfire(this as Gunfire);

            // tick the copy if not null
            thisCopy?.Tick();

            // get the level bounds
            RectangleF bounds = level.GetPath().GetBounds();

            // check if the copy is within 1 tilesize from the bounds
            if (thisCopy.Position.X - Tilesize >= bounds.Left &&
                thisCopy.Position.X + Tilesize < bounds.Right &&
                thisCopy.Position.Y - Tilesize >= bounds.Top &&
                thisCopy.Position.Y + Tilesize < bounds.Bottom)
                return false;

            // copy is within a tilesize from the edge -- compare regions

            // get the copy's region
            Region rCopy = new Region(thisCopy.GetPath());

            // exclude the copy from the level's graphics path outline
            rCopy.Exclude(level.GetPath());

            // if it's empty, not out of bounds
            if (rCopy.IsEmpty(gr))
                return false;

            // not empty, some of the copy is out of bounds
            return true;
        }
        #endregion
    }
}
