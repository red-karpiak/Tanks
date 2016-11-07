/********************************************************************************
 * File: GunFire.cs                                                             *
 * Author: Dillon Allan and Jared Karpiak                                       *
 * Description: Class derived from Shape. Used for gunshots rendered in-game.   *
 ********************************************************************************/
using System;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace CMPE2800_Lab02
{
    class Gunfire : DynamicShape
    {
        #region Members
        // size of machine gun bullets and rockets
        private const int _iMachineGunSize = Tilesize * (1 / 10);
        private const int _iRocketSize = Tilesize * (1 / 5);

        // speed of machine gun bullets and rockets
        private const float _iMachineGunSpeed = Tank.Speed * 5.0f;
        private const float _iRocketSpeed = Tank.Speed * 3.75f;

        // gunfire offset distance 
        // (ensures the bullet doesn't spawn over top of the tank)
        private const int _iGunfireOffset = Tilesize / 5;

        // models for machine gun bullets and rockets
        private static readonly RectangleF _rfMachineGunModel = 
            new RectangleF((3 / 10f * Tank.TankSize) + _iGunfireOffset, -1 / 12f * Tank.TankSize,
                1 / 6f * Tank.TankSize, 1 / 6f * Tank.TankSize);
        private static readonly RectangleF _rfRocketModel = 
            new RectangleF((3 / 10f * Tank.TankSize) + _iGunfireOffset, -1 / 12f * Tank.TankSize,
                1 / 4f * Tank.TankSize, 1 / 6f * Tank.TankSize);

        // type of gun fired
        public GunType Gun { get; private set; }
        #endregion

        #region Methods
        /// <summary>
        /// Default instance constructor for new gunfire.
        /// Sets initial position and gun type.
        /// </summary>
        /// <param name="position">
        /// Initial location of the gun blast.
        /// </param>
        /// <param name="rotation">
        /// Angle of gun fire.
        /// </param>
        /// <param name="gunType">
        /// Type of gun fired.
        /// </param>
        /// <param name="color">
        /// Color of gunfire shape.
        /// </param>
        /// <param name="player">
        /// Player index.
        /// </param>
        public Gunfire(PointF position, float rotation, GunType gunType, Color color, PlayerNumber player)
            : base(position, color, player)
        {
            // set angle of gun fire
            Rotation = rotation;

            // set gunfire initial position
            Position = new PointF(position.X, position.Y);

            // gunfire speed magnitude i.e. the hypotenuse
            float speed = 0;

            // initialize gunfire model
            _model = new GraphicsPath();

            // set gunfire properties according to gun type
            Gun = gunType;
            switch (Gun)
            {
                case GunType.MachineGun:
                    // draw a little ellipse for the mg bullet
                    _model.AddEllipse(_rfMachineGunModel);
                    speed = _iMachineGunSpeed;
                    break;

                case GunType.Rocket:
                    // draw a little rectangle for the rocket
                    _model.AddRectangle(_rfRocketModel);
                    speed = _iRocketSpeed;
                    break;
            }

            // set angle in radians
            double angle = Math.PI / 180 * rotation;

            // use trigonometry to set the x and y speeds
            XSpeed = speed * (float)Math.Cos(angle);
            YSpeed = speed * (float)Math.Sin(angle);
        }

        /// <summary>
        /// Alternate constructor used for creating 
        /// a copy of a gunfire object for hit testing.
        /// </summary>
        /// <param name="gunfire">
        /// The gunfire object to copy.
        /// </param>
        public Gunfire(Gunfire gunfire) : base(gunfire.Position, gunfire.Color, gunfire.Player)
        {
            // copy all properties to the new gunfire object
            Rotation = gunfire.Rotation;
            RotIncrement = gunfire.RotIncrement;
            XSpeed = gunfire.XSpeed;
            YSpeed = gunfire.YSpeed;
            _model = gunfire._model;
        }
        #endregion
    }
}
