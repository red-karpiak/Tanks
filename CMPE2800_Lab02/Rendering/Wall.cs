/****************************************************************************
 * File: Wall.cs                                                            *
 * Author: Dillon Allan and Jared Karpiak                                   *
 * Description: Used for generating wall shapes in-game.                    *
 ****************************************************************************/
using System.Drawing;
using System.Drawing.Drawing2D;

namespace CMPE2800_Lab02
{
    class Wall : Shape
    {
        //Wall Color
        Color _wallColor;
        //use the WallType to know what image to render for the wall color
        public WallType _wallType { get; private set; }

        //wall styling to differentiate between hard and weak walls (Bricks and steel)
        HatchStyle _hatch;

        //leverage the base class constructor except for the WallType
        public Wall(Point position, WallType wt) : base(position)
        {
            //wall type (hard or weak)
            _wallType = wt;
            //the graphics model
            _model = new GraphicsPath();
            _model.AddRectangle(new RectangleF(Position.X, Position.Y, Tilesize, Tilesize));

            //make sure it appears. Once the wall gets destroyed (if able) this will be marked true
            IsMarkedForDeath = false;
            //create a hard (steel) wall
            if (_wallType == WallType.Hard)
            {
                _wallColor = Color.LightSteelBlue;
                _hatch = HatchStyle.Percent70;
            }
            //draw a weak (brick) wall
            else
            {
                _wallColor = Color.DarkRed;
                _hatch = HatchStyle.HorizontalBrick;
            }
        }
        /// <summary>
        /// Renders the wall wall, which is just a square hatch brush
        /// it will be bricks if is a weak wall, or a steel if is a hard wall (see constructor)
        /// </summary>
        /// <param name="gr">
        /// The main graphics object of the game
        /// </param>
        public override void Render(Graphics gr)
        {
            gr.FillRegion(new HatchBrush(_hatch, Color.Black, _wallColor), new Region(GetPath()));
        }
        /// <summary>
        /// Basic function to return the graphics path of the wall for use with hit detection
        /// </summary>
        /// <returns>
        /// A GraphicsPath of the Wall
        /// </returns>
        public override GraphicsPath GetPath()
        {
            return _model.Clone() as GraphicsPath;
        }

    }
}