/****************************************************************************
 * File: Tank.cs                                                            *
 * Author: Dillon Allan and Jared Karpiak                                   *
 * Description: Class derived from DynamicShape.                            *
 *              Used for tanks rendered in-game.                            *
 ****************************************************************************/
using System.Drawing;
using System.Drawing.Drawing2D;
using System;

namespace CMPE2800_Lab02
{
    class Tank : DynamicShape
    {
        #region Fields
        // set size of tank model (90% of tilesize for maneuverability)
        public const int TankSize = Tilesize * 7 / 10;

        // rectangle describing the tank's body
        private static RectangleF _rfTankBody = new RectangleF(-1 / 2f * TankSize, -2 / 6f * TankSize,
                4 / 5f * TankSize, 2 / 3f * TankSize);

        // rectangle describing the tip of the tank barrel extending past the tank body
        private static RectangleF _rfTankBarrel = new RectangleF(3 / 10f * TankSize, -1 / 12f * TankSize,
                1 / 5f * TankSize, 1 / 6f * TankSize);

        // set tank speed 
        public const float Speed = 2.5f;

        // set rate of rotation
        public const float RotInc = 3f;

        // collision flag
        public bool IsBlocked = false;

        // list of eight directions for positioning the tank upon spawn event
        private static Direction[] _aDirections =
        {
            Direction.North, Direction.South, Direction.East, Direction.West,
            Direction.NorthEast, Direction.SouthEast, Direction.SouthWest, Direction.NorthWest
        };

        // used for getting random spawn directions
        private static Random _rng = new Random();
        #endregion

        #region Methods
        /// <summary>
        /// Instance constructor sets tank color and initial direction, and 
        /// leverages base constructor to set position.
        /// </summary>
        /// <param name="position">
        /// Screen position of Tank.
        /// </param>
        /// <param name="initialDirection">
        /// Initial direction of the tank is facing at game start.
        /// </param>
        /// <param name="color">
        /// Tank color.
        /// </param>
        public Tank(PointF position, Color color, PlayerNumber player)
            : base(position, color, player)
        {
            // set tank model
            _model = new GraphicsPath();

            // tank body
            _model.AddRectangle(_rfTankBody);

            // tip of tank barrel that protrudes past tank body
            _model.AddRectangle(_rfTankBarrel);

            // give the tank a random initial direction
            switch (_aDirections[_rng.Next(0, _aDirections.Length)])
            {
                case Direction.North:
                    Rotation = 270;
                    break;
                case Direction.South:
                    Rotation = 90;
                    break;
                case Direction.East:
                    Rotation = 0;
                    break;
                case Direction.West:
                    Rotation = 180;
                    break;
                case Direction.NorthEast:
                    Rotation = 315;
                    break;
                case Direction.SouthEast:
                    Rotation = 45;
                    break;
                case Direction.SouthWest:
                    Rotation = 135;
                    break;
                case Direction.NorthWest:
                    Rotation = 225;
                    break;
            }
        }

        /// <summary>
        /// Alternative Tank constructor for making a copy of a pre-existing tank.
        /// </summary>
        /// <param name="tank">
        /// Tank to copy.
        /// </param>
        public Tank(Tank tank) : base(tank.Position, tank.Color, tank.Player)
        {
            // set members
            Rotation = tank.Rotation;
            RotIncrement = tank.RotIncrement;
            XSpeed = tank.XSpeed;
            YSpeed = tank.YSpeed;

            // set tank model
            _model = new GraphicsPath();

            _model.AddPath(tank._model, false);
        }
        #endregion
    }
}
