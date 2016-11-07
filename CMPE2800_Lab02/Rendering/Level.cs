/****************************************************************************
 * File: Level.cs                                                           *
 * Author: Dillon Allan and Jared Karpiak                                   *
 * Description: Used for processing xml files and generating the game map.  *
 ****************************************************************************/
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Drawing.Drawing2D;
using System.Xml;

namespace CMPE2800_Lab02
{
    class Level : Shape
    {
        //Lists of walls, tank spawns, and ammo spawns for game mechanics
        public List<Wall> Walls { get; private set; } = new List<Wall>();
        public List<PointF> _respawnPoints = new List<PointF>();
        public List<Ammo> _ammoDrops = new List<Ammo>();
        //background image
        Bitmap _backgroundImage;
        /// <summary>
        /// A List of spawn Locations and what type of spawn they are. Also copy this for rendering purposes
        /// </summary>
        public Bitmap LevelBackground { get { return _backgroundImage; } } //use as backdrop

        
        /// <summary>
        /// Level Constructor. Call the ParseLevel member function to get the lvl data from the xml file
        /// and make set the bounds for use with hit detection so the tanks don't run off the map
        /// </summary>
        /// <param name="file"></param>
        /// <param name="topLeftX"></param>
        /// <param name="topLeftY"></param>
        public Level(string file, int topLeftX = 0, int topLeftY = 0) : base(new PointF(topLeftX, topLeftY))
        {
            // Get level properties from the XML file
            ParseLevel(file);

            // create the model from the pixel bounds of the bg image
            _model = new GraphicsPath();
            RectangleF levelRect = new RectangleF(new PointF(0, 0), LevelBackground.Size);
            _model.AddRectangle(levelRect);
        }

        /// <summary>
        /// Get the graphics path for use with hit detection
        /// </summary>
        /// <returns>
        /// the GraphicsPath of the Level
        /// </returns>

        public override GraphicsPath GetPath()
        {
            return _model.Clone() as GraphicsPath;
        }
        /// <summary>
        /// Render the background image of the level
        /// </summary>
        /// <param name="gr">
        /// The main graphics object of game
        /// </param>
        public override void Render(Graphics gr)
        {
            gr.DrawImage(_backgroundImage, _model.GetBounds());
        }

        #region Parsers
        /// <summary>
        /// ParseLevel() takes in the Level xml file, extracts the data from it, and makes it useable for rendering
        /// </summary>
        /// <param name="fileName">
        /// The file path of the xml file that contains all the lvl information
        /// </param>
        private void ParseLevel(string fileName)
        {
            //variable to hold the raw sml string
            string XMLRaw;
            //get the xml data
            using (StringReader SR = new StringReader(fileName))
            {
                XMLRaw = SR.ReadToEnd();
            }
            //start an XMLparser
            using (XmlReader XR = XmlReader.Create(new StringReader(XMLRaw)))
            {
                //a string to hold the type of level we are using (e.g. desert, grass, etc.)
                string levelType = null;
                // while we have a line to read read
                while (XR.Read())
                {
                    //check each node
                    switch (XR.NodeType)
                    {
                        //if the node is an element read it
                        case XmlNodeType.Element:
                            //check the name of the element
                            switch (XR.Name)
                            {
                                //set the type of level, this will determine the background
                                case "LEVELTYPE":
                                    levelType = XR.ReadElementContentAsString();
                                    //depending on the level type, assign the appropriate background image
                                    switch (levelType)
                                    {
                                        case "DESERT":
                                            _backgroundImage = new Bitmap(Properties.Resources.DirtTerrain);
                                            break;
                                        case "CITY":
                                            _backgroundImage = new Bitmap(Properties.Resources.CityTerrain);
                                            break;
                                        case "GRASS":
                                            _backgroundImage = new Bitmap(Properties.Resources.GrassTerrain);
                                            break;
                                    }
                                    break;
                                //set the walls we will use in our level
                                case "WALL":
                                    //take the wall element and put it into its own xml parser
                                    ParseWall(XR.ReadSubtree());
                                    break;
                                //set the spawns in our level
                                case "SPAWN":
                                    //take the spawn element and put it into its own xml parser
                                    ParseSpawns(XR.ReadSubtree());
                                    break;
                            }
                            break;
                    }
                }
            }
        }
        /// <summary>
        /// This function takes an XmlReader the was created from a WALL element in the main file,
        /// It then creates a new wall and adds it to the List<Wall>
        /// </summary>
        /// <param name="wallReader">
        /// The XML Reader object created from a wall subtree in the ParseLevel() method
        /// </param>
        private void ParseWall(XmlReader wallReader)
        {
            //a point to hold the location from where we add the wall
            Point wallLocation = new Point();

            //a Walltype to hold the type, set the default to hard
            WallType wt = WallType.Hard;

            //a Wallshape to hold the shape, default is vertical line
            WallShape ws = WallShape.l;

            //a string to hold text data parsed from the file
            string tempString = "";

            //width and height variables
            int width = 0, height = 0;

            //read to the end
            while (wallReader.Read())
            {
                //check the node type
                switch (wallReader.NodeType)
                {
                    ///*******************************************************
                    ///Here we parse the WALL subelements into their own parts
                    ///*******************************************************
                    case XmlNodeType.Element:
                        //check the tag name and perform the necessary action on it
                        switch (wallReader.Name)
                        {
                            //get the starting X location of the wall
                            case "X":
                                wallLocation.X = wallReader.ReadElementContentAsInt();
                                break;
                            //get the starting Y location of the wall
                            case "Y":
                                wallLocation.Y = wallReader.ReadElementContentAsInt();
                                break;
                            //Get the wall type as a string, and then assign the appropriate type based on
                            //the string
                            case "TYPE":
                                tempString = wallReader.ReadElementContentAsString();
                                if (tempString == "WEAK")
                                    wt = WallType.Weak;
                                break;
                            //Get the shape type. Call the SetWallShape function. It returns the appropriate enum WallShape
                            case "SHAPE":
                                tempString = wallReader.ReadElementContentAsString();
                                ws = SetWallShape(tempString);
                                break;
                            //Get the width of the wall
                            case "WIDTH":
                                width = wallReader.ReadElementContentAsInt() * Tilesize;
                                break;
                            //Get the height of the wall
                            case "HEIGHT":
                                height = wallReader.ReadElementContentAsInt() * Tilesize;
                                //add the walls once we reach the end
                                AddWalls(wallLocation, wt, ws, width, height);
                                break;
                        }
                        break;
                }
            }
        }

        /// <summary>
        /// Takes an XmlReader the was created from a SPAWNS element in the main file,
        /// It cycles through all of the subelements and then either creates an ammo object and adds
        /// it to the ammo spawn list, or it sets adds a PointF position to the respawn list
        /// </summary>
        /// <param name="spawnReader"></param>
        private void ParseSpawns(XmlReader spawnReader)
        {
            //a point to hold the location where we will spawn
            Point spawnLocation = new Point();

            //A Bitmap to hold the rockets image
            Bitmap bm = null;
            //a string to hold text data parsed from the file
            string tempString = "";

            //read to the end
            while (spawnReader.Read())
            {
                //check the node type
                switch (spawnReader.NodeType)
                {
                    ///*******************************************************
                    ///Here we parse the SPAWN subelements into their own parts
                    ///*******************************************************
                    case XmlNodeType.Element:
                        switch (spawnReader.Name)
                        {
                            //get the spawn X position
                            case "X":
                                spawnLocation.X = spawnReader.ReadElementContentAsInt() * Tilesize;
                                break;
                            //get the spawn Y position
                            case "Y":
                                spawnLocation.Y = spawnReader.ReadElementContentAsInt() * Tilesize;
                                break;
                            //Get the spawn type as a string, then assign it to the appropriate list (Ammo or Tank)
                            case "TYPE":
                                tempString = spawnReader.ReadElementContentAsString();
                                if (tempString == "Ammo")
                                {
                                    //set the bitmap to the missiles and add a new ammo object to the List
                                    bm = Properties.Resources.ammoDrop;
                                    _ammoDrops.Add(new Ammo(spawnLocation));
                                }
                                else
                                {
                                    //add tankspawn
                                    _respawnPoints.Add(spawnLocation);
                                }

                                break;
                        }
                        break;

                }
            }
        }
        /// <summary>
        /// This funtion takes in a string and returns the appropriate WallShape
        /// </summary>
        /// <param name="s"></param>
        /// <returns>
        /// The WallShape
        /// </returns>
        private WallShape SetWallShape(string s)
        {
            //return the WallShape associated with the input string
            switch (s)
            {
                //an "o" shaped wall
                case "o":
                    return WallShape.o;
                //horizontal line shaped wall
                case "hl":
                    return WallShape.hl;
                //vertical line shaped wall
                default:
                    return WallShape.l;
            }
        }
        /// <summary>
        /// This method adds the walls to the list of walls
        /// </summary>
        /// <param name="wallLocation"> location to draw the wall </param>
        /// <param name="wt"> The type of wall, Hard or Weak </param>
        /// <param name="ws"> The Shape of the wall </param>
        /// <param name="width"> The width of the wall </param>
        /// <param name="height"> The height of the wall </param>
        private void AddWalls(Point wallLocation, WallType wt, WallShape ws, int width, int height)
        {
            //create a temporary point to hold the location we are adding to
            Point tempPoint = new Point(wallLocation.X, wallLocation.Y);
            
            switch (ws)
            {
                //add an 'o' shaped wall
                case WallShape.o:
                    //copy of the origin point for drawing
                    Point drawPoint = tempPoint;
                    //nested for loop because the shape is a equilateral
                    for (int i = 0; i < height / Tilesize; ++i)
                    {
                        //move the draw point down
                        drawPoint.Y = tempPoint.Y + i;
                        //cycle through the horizontal plane
                        for (int j = 0; j < width / Tilesize; ++j)
                        {
                            //move the draw point across the screen
                            drawPoint.X = tempPoint.X + j;
                            //add the wall at the draw point
                            Walls.Add(new Wall(new Point(drawPoint.X * Tilesize, drawPoint.Y * Tilesize), wt));
                        }
                        //reset the drawpoint to the original x position
                        drawPoint.X = tempPoint.X;
                    }
                    break;
                //add a vertical line shaped wall
                case WallShape.l:
                    //single for loop because we are drawing straight down. no change in X
                    for (int i = 0; i < height / Tilesize; ++i)
                        Walls.Add(new Wall
                            (new Point(tempPoint.X * Tilesize, (tempPoint.Y + i) * Tilesize), wt));
                    break;
                //add a horizontal line shaped wall 
                case WallShape.hl:
                    //single for loop because we are drawing straight across. no change in Y
                    for (int i = 0; i < width / Tilesize; ++i)
                        Walls.Add(new Wall
                            (new Point((tempPoint.X + i) * Tilesize, tempPoint.Y * Tilesize), wt));
                    break;

            }
        }
        #endregion
    }
}