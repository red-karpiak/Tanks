/****************************************************************************
 * File: Ammo.cs                                                            *
 * Author: Dillon Allan and Jared Karpiak                                   *
 * Description: Used for creating ammo drops for tanks for collect in-game. *
 ****************************************************************************/
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Diagnostics;

namespace CMPE2800_Lab02
{
    class Ammo : Shape
    {
        // the stopwatch for counting the time between ammo spawns
        public static Stopwatch _stopwatch;

        //ammo image
        private Bitmap _bm;
        
        //public property to determine if an ammo object is either showing or has a stopwatch started
        public bool IsAlive { get; set; }
        
        //Ammo Constructor
        public Ammo(Point sp) : base(sp)
        {
            //create the new stopwatch object
            _stopwatch = new Stopwatch();

            //creat the model. Positioning the image in the center of the tile
            _model = new GraphicsPath();
            _model.AddRectangle(new RectangleF(Position.X + Tilesize / 4, Position.Y + Tilesize / 4,
                Tilesize / 2, Tilesize / 2));

            //set the bitmap image to the missiles
            _bm = Properties.Resources.ammoDrop;

            //all ammo object start with a stopped stopwatch not showing.
            IsAlive = false;
        }
        /// <summary>
        /// Get the graphics path for use with hit detection
        /// </summary>
        /// <returns>
        /// the GraphicsPath of the ammo object
        /// </returns>
        public override GraphicsPath GetPath()
        {
            return _model.Clone() as GraphicsPath;
        }
        /// <summary>
        /// Render the Ammo drop. It needs to be a quarter of the size of tile and 
        /// placed in the center of the tile
        /// </summary>
        /// <param name="gr">
        /// The main graphics object of game
        /// </param>
        public override void Render(Graphics gr)
        {
            // get the model's bounds
            RectangleF modRect = _model.GetBounds();

            //render an ammo icon
            gr.DrawImage(_bm, modRect.X, modRect.Y, modRect.Width, modRect.Height);
        }
    }
}
