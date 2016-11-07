/****************************************************************
 * File: Utilities.cs                                           *
 * Author: Dillon Allan and Jared Karpiak                       *
 * Description: Helper functions used by Form1.cs.              *
 ****************************************************************/
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading;
using System.Windows.Forms;

namespace CMPE2800_Lab02
{
    public partial class MainGame : Form
    {
        #region Startup
        /// <summary>
        /// Initializes main form members.
        /// </summary>
        private void InitializeMembers(string XMLValue)
        {
            // initialize the level
            _lvGameLevel = new Level(XMLValue);

            // reset pause flag
            _bGamePaused = false;

            // initialize players
            PlayerData p1 = new PlayerData(PlayerNumber.One);
            PlayerData p2 = new PlayerData(PlayerNumber.Two);

            // initialize abstract inputs
            AbstractInput a1 = new AbstractInput(PlayerNumber.One);
            AbstractInput a2 = new AbstractInput(PlayerNumber.Two);

            // tank 1 starts at a random spawn point
            PointF startP1 = _lvGameLevel._respawnPoints[_rng.Next(0, _lvGameLevel._respawnPoints.Count)];

            // tank 2 starts as far from player 1 as possible
            PointF startP2 = GetFurthestSpawn(startP1);

            // initialize the tanks
            Tank t1 = new Tank(startP1, _cPlayer1Color, PlayerNumber.One);
            Tank t2 = new Tank(startP2, _cPlayer2Color, PlayerNumber.Two);

            // add inputs to input list
            _lPlayerInputs = new List<AbstractInput> { a1, a2 };

            // add player data to data list
            _lPlayerData = new List<PlayerData> { p1, p2 };

            // add tank shapes to dynamic shape list
            _lDynShapes = new List<DynamicShape> { t1, t2 };

            // make wall list a copy of the level's walls
            _lWalls = new List<Wall>(_lvGameLevel.Walls);

            // make ammo drops a copy of the level's ammo drops
            _lAmmoDrops = new List<Ammo>(_lvGameLevel._ammoDrops);

            // start background input processing thread 
            _tBackgroundProcessing = new Thread(TBackground);
        }

        /// <summary>
        /// Centres the main game window, and positions the UI
        /// above it.
        /// </summary>
        /// <param name="UI">Game UI window.</param>
        private void SetUpWindows(GameUI UI)
        {
            // set form width and height to the dimensions of the background image
            ClientSize = new Size(_lvGameLevel.LevelBackground.Width, _lvGameLevel.LevelBackground.Height);

            // Center main form to screen
            CenterToScreen();

            // move main form down a little
            Location = new Point(Location.X, Location.Y + _iMainFormYOffset);

            // set the UI width to the main game window width
            UI.Width = Width;

            // position UI above the game window
            UI.StartPosition = FormStartPosition.Manual;
            UI.Location = new Point(Location.X, Location.Y - UI.Height);
        }
        #endregion

        #region Main Thread
        /// <summary>
        /// Renders all game shapes to a back buffer
        /// before replacing the front buffer. 
        /// </summary>
        private void DoubleBufferedRender()
        {
            using (BufferedGraphicsContext bgc = new BufferedGraphicsContext())
            {
                using (BufferedGraphics bg = bgc.Allocate(CreateGraphics(), ClientRectangle))
                {
                    // draw level background in the top left corner
                    _lvGameLevel.Render(bg.Graphics);

                    // render moving shapes
                    _lDynShapes.ForEach(ds => ds.Render(bg.Graphics));

                    // render walls
                    _lWalls.ForEach(w => w.Render(bg.Graphics));

                    // render active ammo drops
                    _lAmmoDrops.Where(a => a.IsAlive).ToList()
                        .ForEach(a => a.Render(bg.Graphics));

                    // flip back buffer to front
                    bg.Render();
                }
            }
        }
        #endregion

        #region Background Thread
        /// <summary>
        /// Poll for keyboard and gamepad state
        /// </summary>
        private void TBackground()
        {
            // loop forever
            while (true)
            {
                // update UI
                UpdateUI();

                // check for game over condition
                if (PlayerData.PlayerVictory)
                {
                    // pause game
                    _bGamePaused = true;

                    // trigger game over event
                    GameOver(_lPlayerData.Find(p => p.Score == PlayerData.ScoreToWin).Player);
                }

                // poll for new input, and apply the input to game elements
                InputProcessing();

                // don't do anything further if the game is paused
                if (_bGamePaused)
                    continue;

                // perform hit detection
                HitDetection();

                // check if a new ammo drop is needed
                CheckAmmoDrops();

                // slow background thread according to clock value
                // (25ms to match tick rate of _timMain)
                Thread.Sleep(_iGameClock);
            }
        }

        /// <summary>
        /// Update modeless UI form with current player data.
        /// </summary>
        private void UpdateUI()
        {
            // send current player stats to the UI 
            PlayerData p1 = _lPlayerData.Find(p => p.Player == PlayerNumber.One);
            PlayerData p2 = _lPlayerData.Find(p => p.Player == PlayerNumber.Two);

            // issue callback function to update UI
            _modelessUI.Invoke(new GameUI.delVoidUIStats(_modelessUI.CBUpdateUI), p1.HP, p2.HP,
                p1.Lives, p2.Lives, p1.HeavyAmmo, p2.HeavyAmmo, p1.Score, p2.Score, _bGamePaused);
        }

        /// <summary>
        /// Monitors the list of ammo drops. 
        /// If none are active, a random one is spawned after 
        /// the timeout has cleared.
        /// </summary>
        private void CheckAmmoDrops()
        {
            // take a snapshot of the list of ammo drops
            List<Ammo> ammoDropsSnapshot;
            lock (_oRenderLock)
                ammoDropsSnapshot = new List<Ammo>(_lAmmoDrops);

            // if the ammo timer isn't running, start it
            if (!Ammo._stopwatch.IsRunning)
                Ammo._stopwatch.Start();

            // if there are no active ammo drops, and the timer has timed out,
            // set a random ammo drop's render flag
            if (!ammoDropsSnapshot.Any(a => a.IsAlive) &&
                Ammo._stopwatch.ElapsedMilliseconds > _iAmmoTimeout)
            {
                Random r = new Random();
                ammoDropsSnapshot[r.Next(0, _lAmmoDrops.Count)].IsAlive = true;

                // reset the timer
                Ammo._stopwatch.Reset();

                // update the list of ammo drops
                lock (_oRenderLock)
                    _lAmmoDrops = new List<Ammo>(ammoDropsSnapshot);
            }
        }

        /// <summary>
        /// Checks input devices states, and applies new input.
        /// </summary>
        private void InputProcessing()
        {
            // take a snapshot of the input list
            List<AbstractInput> inputSnapshot;
            lock (_oInputLock)
            {
                inputSnapshot = new List<AbstractInput>(_lPlayerInputs);
            }

            // process new input from each player
            foreach (AbstractInput ab in inputSnapshot)
            {
                // set ab state according to input device state,
                // (checking for game pad connections first)
                ab.CheckGamePadConnection();

                // update game using ab's state
                ApplyInput(ab);
            }

            // update original input list
            lock (_oInputLock)
            {
                _lPlayerInputs = new List<AbstractInput>(inputSnapshot);
            }
        }
        #endregion

        #region Collision Detection
        /// <summary>
        /// Performs hit detection checks on all game shapes.
        /// </summary>
        private void HitDetection()
        {
            // take snapshots of game collections
            List<DynamicShape> dynShapeSnapshot;
            List<Wall> wallListSnapshot;
            List<PlayerData> playerListSnapshot;
            List<Ammo> ammoDropsSnapshot;
            lock (_oRenderLock)
            {
                dynShapeSnapshot = new List<DynamicShape>(_lDynShapes);
                wallListSnapshot = new List<Wall>(_lWalls);
                playerListSnapshot = new List<PlayerData>(_lPlayerData);
                ammoDropsSnapshot = new List<Ammo>(_lAmmoDrops);
            }

            // perform hit detection testing: 
            lock (_oRenderLock)
            {
                using (Graphics gr = CreateGraphics())
                {
                    // 1) moving shapes crossing the level bounds
                    CheckBounds(dynShapeSnapshot, gr);

                    // 2) two moving shapes hitting each other
                    ProcessDynamicHits(dynShapeSnapshot, playerListSnapshot, gr);

                    // 3) moving shapes hitting walls
                    ProcessWallHits(dynShapeSnapshot, wallListSnapshot, gr);

                    // 4) tanks hitting ammo spawns
                    ProcessAmmoSpawnHits(dynShapeSnapshot, _lAmmoDrops, playerListSnapshot, gr);
                }
            }

            // remove dead shapes, and tick moving shapes
            UpdateGameElements(dynShapeSnapshot, wallListSnapshot);

            // update original list of shapes, walls, and the list of player data
            lock (_oRenderLock)
            {
                _lDynShapes = new List<DynamicShape>(dynShapeSnapshot);
                _lWalls = new List<Wall>(wallListSnapshot);
                _lPlayerData = new List<PlayerData>(playerListSnapshot);
                _lAmmoDrops = new List<Ammo>(ammoDropsSnapshot);
            }
        }

        /// <summary>
        /// Checks every moving shape for being out of bounds. 
        /// Handles out-of-bounds conditions according to the type of moving shape.
        /// </summary>
        /// <param name="movingShapes">Collection of moving shapes.</param>
        /// <param name="gr">Screen graphics object.</param>
        private void CheckBounds(List<DynamicShape> movingShapes, Graphics gr)
        {
            // check every moving shape
            foreach (DynamicShape dn in movingShapes)
            {
                // check if dn is out of bounds
                if (dn.IsOutOfBounds(_lvGameLevel, gr))
                {
                    // if dn is a tank, set the blocked flag
                    if (dn is Tank)
                        (dn as Tank).IsBlocked = true;

                    // if dn is Gunfire, mark for death
                    if (dn is Gunfire)
                        (dn as Gunfire).IsMarkedForDeath = true;
                }
            }
        }

        /// <summary>
        /// Compares every moving shape to each other, and to every non-moving shape.
        /// If they are close enough to be colliding, a check is performed. 
        /// If a collision if found, it is handled based on what shapes collided with each other.
        /// </summary>
        /// <param name="movingShapes">Collection of moving shapes.</param>
        /// <param name="gr">Screen graphics.</param>
        private void ProcessDynamicHits(List<DynamicShape> movingShapes, List<PlayerData> players, Graphics gr)
        {
            // go over every moving shape
            foreach (DynamicShape ds1 in movingShapes)
            {
                // compare s1 to all other dyn shapes
                foreach (DynamicShape ds2 in movingShapes)
                {
                    // ignore self
                    if (ReferenceEquals(ds1, ds2))
                        continue;

                    // move on if no collision is found
                    if (!ds1.IsColliding(ds2, gr))
                        continue;

                    // collision found -- determine the type of collision occured between s1 and s2

                    // 1) s1 and s2 are tanks -- set collision flag for both
                    if (ds1 is Tank && ds2 is Tank)
                        (ds1 as Tank).IsBlocked = true;

                    // 2) s1 is a bullet and s2 is a tank
                    else if (ds1 is Gunfire && ds2 is Tank)
                    {
                        // mark bullet for death
                        ds1.IsMarkedForDeath = true;

                        // get shooter
                        PlayerData shooter = /*null*/players.Find(p => p.Player == ds1.Player);

                        // get victim
                        PlayerData victim = players.Find(p => p.Player == ds2.Player);

                        // take damage if the player is still alive
                        if (victim.IsAlive)
                            victim.TakeDamage((ds1 as Gunfire).Gun, shooter);

                        // check if the player died after taking damage
                        if (!victim.IsAlive)
                        {
                            // signal for tank removal
                            ds2.IsMarkedForDeath = true;

                            // reset player HP for new life
                            victim.Respawn();
                        }
                    }

                    // 3) s1 is a tank, and s2 is a bullet -- reverse case of 2)
                    //    (do nothing, because doing something would mean 2) and 3)
                    //     would be processed for the same frame, 
                    //     resulting in a tank losing 2x health

                    // 4) s1 is a bullet, and s2 is a bullet
                    else if (ds1 is Gunfire && ds2 is Gunfire)
                    {
                        // cancel both shots out
                        // i.e. players can defend themselves by shooting incoming bullets
                        ds1.IsMarkedForDeath = ds2.IsMarkedForDeath = true;
                    }
                }
            }
        }

        /// <summary>
        /// Compares every moving shape to every wall,
        /// reacting to collisions if necessary.
        /// </summary>
        /// <param name="MovingShapes">Collection of dynamic shapes.</param>
        /// <param name="LevelBlocks">Collection of wall tiles.</param>
        /// <param name="gr">Screen graphics object.</param>
        private void ProcessWallHits(List<DynamicShape> MovingShapes, List<Wall> LevelBlocks, Graphics gr)
        {
            // check each moving shape
            foreach (DynamicShape ds in MovingShapes)
            {
                // compare s1 to all walls
                foreach (Wall wall in LevelBlocks)
                {
                    // move on if no collision is found
                    if (!ds.IsColliding(wall, gr))
                        continue;

                    // collision found -- determine the type of collision occured between ds and wall

                    // 1) s1 is Tank
                    if (ds is Tank)
                    {
                        // set blocked flag on s1
                        (ds as Tank).IsBlocked = true;
                    }

                    // 2) s1 is Gunfire
                    if (ds is Gunfire)
                    {
                        // mark gunfire for removal
                        ds.IsMarkedForDeath = true;

                        // check for heavy gunfire
                        if ((ds as Gunfire).Gun == GunType.Rocket)
                        {
                            // mark wall for removal
                            if (wall._wallType == WallType.Weak)
                                wall.IsMarkedForDeath = true;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Checks for collisions between tanks and ammo drops.
        /// Replenish's the player's ammo if their tank hits an ammo drop.
        /// </summary>
        /// <param name="movingShapes">Collection of dynamic shapes.</param>
        /// <param name="ammoDrops">Collection of ammo drops.</param>
        /// <param name="gr">Screen graphics object.</param>
        private void ProcessAmmoSpawnHits(List<DynamicShape> movingShapes, List<Ammo> ammoDrops, List<PlayerData> players, Graphics gr)
        {
            // check each tank
            foreach (DynamicShape ds in movingShapes.Where(ds => ds is Tank))
            {
                // compare ds to all active ammo drops
                foreach (Ammo ammoDrop in _lAmmoDrops.Where(a => a.IsAlive))
                {
                    // move on if no collision is found
                    if (!ds.IsColliding(ammoDrop, gr))
                        continue;

                    // get the tank's player number
                    PlayerNumber playerNum = (ds as Tank).Player;

                    // replenish the player's ammo
                    players.Find((pd) => pd.Player == playerNum).GetMaxAmmo();

                    // reset ammoDrop's render flag 
                    ammoDrop.IsAlive = false;

                    // start Ammo class timeout timer
                    Ammo._stopwatch.Restart();
                }
            }
        }
        #endregion

        #region Input Processing
        /// <summary>
        /// Checks input field values, and 
        /// updates Tank properties accordingly.
        /// </summary>
        /// <param name="input">
        /// Player input.
        /// </param>
        private void ApplyInput(AbstractInput input)
        {
            // player tank and data locals
            Tank tank;
            PlayerData thisPlayer;

            // copy moving shape and player data collections
            List<DynamicShape> movingShapesSnapshot;
            List<PlayerData> playerListSnapshot;
            lock (_oRenderLock)
            {
                movingShapesSnapshot = new List<DynamicShape>(_lDynShapes);
                playerListSnapshot = new List<PlayerData>(_lPlayerData);
            }

            // get tank and player data
            tank = movingShapesSnapshot.Find(dn => dn is Tank && dn.Player == input.Player) as Tank;
            thisPlayer = playerListSnapshot.Find(p => p.Player == input.Player);

            // check rotation
            SetRotInc(input, tank);

            // check translation
            SetTranslation(input, tank);

            // Check weapon select
            SetSwitchWeapon(input, thisPlayer);

            // Check fire cannon
            SetFireWeapon(input, thisPlayer, tank, movingShapesSnapshot);

            // update moving shape list
            lock (_oRenderLock)
            {
                _lDynShapes = new List<DynamicShape>(movingShapesSnapshot);
            }

            // Check for pause
            if (AbstractInput.Pause)
            {
                // set pause flag
                _bGamePaused = true;
            }

            // pause flag is false -- unpause the game
            else
            {
                _bGamePaused = false;
            }

            // check quit game
            if (AbstractInput.Quit)
            {
                // shutdown the game
                Application.Exit();

                // kill thread to prevent it from
                // accessing the graphics as it's being disposed of
                _tBackgroundProcessing.Abort();
            }

            // check new game
            if (AbstractInput.NewGame)
            {
                Application.Restart();
                _tBackgroundProcessing.Abort();
            }
        }

        /// <summary>
        /// Sets Tank rotation increment given current input state.
        /// </summary>
        /// <param name="input">Player input.</param>
        /// <param name="tank">Player tank.</param>
        private void SetRotInc(AbstractInput input, Tank tank)
        {
            // check rotation
            if (input.Rotate)
            {
                // stop the tank
                // (player can't rotate AND translate at once)
                tank.XSpeed = tank.YSpeed = tank.RotIncrement = 0;

                // rotate clockwise
                if (input.Rotate && (input.Right || input.Down))
                {
                    tank.RotIncrement = Tank.RotInc;
                }

                // rotate counterclockwise
                else if (input.Rotate && (input.Left || input.Up))
                {
                    tank.RotIncrement = -Tank.RotInc;
                }
            }

            // not rotating, so must be translating
            else
            {
                // not rotating, reset rotation increment
                tank.RotIncrement = 0;
            }
        }

        /// <summary>
        /// Sets Tank X and Y speeds based on current input.
        /// </summary>
        /// <param name="input">
        /// Player input.
        /// </param>
        /// <param name="tank">
        /// Player tank.
        /// </param>
        private void SetTranslation(AbstractInput input, Tank tank)
        {
            // can't translate during rotations
            if (input.Rotate)
                return;

            // check up AND down
            if (input.Up && input.Down)
            {
                // trigger motor interlock
                tank.YSpeed = 0;
            }

            // check up OR down
            else
            {
                // moving straight up
                if (input.Up && !(input.Left || input.Right))
                    tank.YSpeed = -Tank.Speed;

                // moving straight down
                else if (input.Down && !(input.Left || input.Right))
                    tank.YSpeed = Tank.Speed;

                // moving up diagonally
                else if (input.Up && (input.Left || input.Right))
                {
                    double angle = 45 * Math.PI / 180;
                    tank.YSpeed = -(float)Math.Sqrt(Math.Pow(Tank.Speed, 2) -
                        Math.Pow(Tank.Speed * Math.Sin(angle), 2));
                }

                // moving down diagonally
                else if (input.Down && (input.Left || input.Right))
                {
                    double angle = 45 * Math.PI / 180;
                    tank.YSpeed = (float)Math.Sqrt(Math.Pow(Tank.Speed, 2) -
                        Math.Pow(Tank.Speed * Math.Sin(angle), 2));
                }

                // no vertical movement input
                else
                    tank.YSpeed = 0;
            }

            // Check left AND right 
            if (input.Right && input.Left)
            {
                // trigger motor interlock
                tank.XSpeed = 0;
            }

            // Check left OR right
            else
            {
                // moving straight right
                if (input.Right && !(input.Up || input.Down))
                    tank.XSpeed = Tank.Speed;

                // moving straight left
                else if (input.Left && !(input.Up || input.Down))
                    tank.XSpeed = -Tank.Speed;

                // moving right diagonally
                else if (input.Right && (input.Up || input.Down))
                {
                    double angle = 45 * Math.PI / 180;
                    tank.XSpeed = (float)Math.Sqrt(Math.Pow(Tank.Speed, 2) -
                        Math.Pow(Tank.Speed * Math.Cos(angle), 2));
                }

                // moving left diagonally
                else if (input.Left && (input.Up || input.Down))
                {
                    double angle = 45 * Math.PI / 180;
                    tank.XSpeed = -(float)Math.Sqrt(Math.Pow(Tank.Speed, 2) -
                        Math.Pow(Tank.Speed * Math.Cos(angle), 2));
                }

                // no horizontal movement input
                else
                    tank.XSpeed = 0;
            }
        }

        /// <summary>
        /// Checks current input state, and switches weapon if necessary.
        /// </summary>
        /// <param name="input">Player input.</param>
        /// <param name="thisPlayer">Player data.</param>
        private void SetSwitchWeapon(AbstractInput input, PlayerData thisPlayer)
        {
            // Check weapon select
            if (input.SwitchWeapon)
            {
                // switch to the next weapon
                thisPlayer.SwitchWeapon();

                // issue callback to highlight weapon icon in game UI
                _modelessUI.Invoke(new GameUI.delVoidSwitchWeapon(_modelessUI.CBSwitchWeapon),
                    input.Player, thisPlayer.CurrentWeapon);

                // signal the abstract input to reset the SwitchWeapon flag
                input.ResetSwitchWeapon();
            }
        }

        /// <summary>
        /// Checks for fire input, and fires the player's gun
        /// </summary>
        /// <param name="input">Player input.</param>
        /// <param name="thisPlayer">Player data.</param>
        /// <param name="tank">Player tank.</param>
        /// <param name="movingShapes">Collection of moving shapes.</param>
        private void SetFireWeapon(AbstractInput input, PlayerData thisPlayer,
            Tank tank, List<DynamicShape> movingShapes)
        {
            if (input.Fire)
            {
                // check if not reloading
                if (thisPlayer.CheckReload())
                {
                    // get gunshot color (white by default)
                    Color shotColor = Color.White;
                    switch (thisPlayer.Player)
                    {
                        case PlayerNumber.One:
                            switch (thisPlayer.CurrentWeapon)
                            {
                                case GunType.MachineGun:
                                    shotColor = _cPlayer1MGColor;
                                    break;
                                case GunType.Rocket:
                                    shotColor = _cPlayer1RocketColor;
                                    break;
                            }
                            break;
                        case PlayerNumber.Two:
                            switch (thisPlayer.CurrentWeapon)
                            {
                                case GunType.MachineGun:
                                    shotColor = _cPlayer2MGColor;
                                    break;
                                case GunType.Rocket:
                                    shotColor = _cPlayer2RocketColor;
                                    break;
                            }
                            break;
                    }

                    // get a new gunshot
                    Gunfire newShot = new Gunfire(tank.Position, tank.Rotation,
                        thisPlayer.CurrentWeapon, shotColor, thisPlayer.Player);

                    // start reloading timer
                    thisPlayer.StartReloading();

                    //add new gunshot it to the list of moving shapes
                    movingShapes.Add(newShot);
                }
            }
        }
        #endregion

        #region Game Events
        /// <summary>
        /// Issues a modal dialog asking the user to select a level.
        /// Sets up the game based on level selection.
        /// </summary>
        private void NewGame()
        {
            // set pause flag
            _bGamePaused = true;

            // disable main thread during setup
            _timMain.Enabled = false;

            // run new game modal dialog
            NewGame _modalNewGame = new NewGame();

            DialogResult result = _modalNewGame.ShowDialog();

            // on OK, load the selected level
            if (result == DialogResult.OK)
            {
                // level = selected level resource
                string xmlValue = _modalNewGame.XMLValue;

                // perform new game setup on all class members
                InitializeMembers(xmlValue);

                // Initialize the UI form, and position it relative to the main form
                _modelessUI = new GameUI();
                SetUpWindows(_modelessUI);
                _modelessUI.Show();

                // start input processing thread in the background
                _tBackgroundProcessing.IsBackground = true;
                _tBackgroundProcessing.Start();

                // enable the timer
                _timMain.Enabled = true;
            }

            // cancel game
            else
            {
                Application.Exit();
                _tBackgroundProcessing?.Abort();
            }
        }

        /// <summary>
        /// Triggers game over modal dialog, 
        /// with a victory message for the winner.
        /// </summary>
        /// <param name="winner">Index of the winning player.</param>
        private void GameOver(PlayerNumber winner)
        {
            // issue a modal dialog announcing the winner
            GameOver _modalGameOver = new GameOver();

            // instantiate the modal dialog's handle
            // (needed for setting winner label.)
            IntPtr handle = _modalGameOver.Handle;

            // send winner to the game over modal dialog
            _modalGameOver.Invoke(new GameOver.delVoidGameOver(_modalGameOver.CBSetPlayerVictory), winner);

            // get result
            DialogResult result = _modalGameOver.ShowDialog();

            // check if user wants to play again
            if (result == DialogResult.OK)
            {
                // set up new game
                Application.Restart();
                _tBackgroundProcessing.Abort();
            }

            // quit game
            else
            {
                Application.Exit();
                _tBackgroundProcessing.Abort();
            }
        }
        #endregion

        #region Helpers
        /// <summary>
        /// Method for finding the furthest spawn point from a given location.
        /// </summary>
        /// <param name="StartingLoc">
        /// Location to calculate the distance from.
        /// </param>
        /// <returns>
        /// Returns a PointF struct indicating the 
        /// farthest spawn point from the given location.
        /// </returns>
        private PointF GetFurthestSpawn(PointF StartingLoc)
        {
            //variables to hold distances and the return spawnPoint
            int _iDistance = 0;
            int _iFurthestDistance = 0;
            PointF _spFurthest = StartingLoc;

            //iterate through list
            foreach (PointF sp in _lvGameLevel._respawnPoints)
            {
                //if the spawn is not the current location
                if (sp != StartingLoc)
                {
                    //find the distance between the this spawn and the Spawn Point we entered using Pythagoras' Theorem
                    _iDistance = (int)Math.Sqrt(Math.Pow(StartingLoc.X - sp.X, 2) +
                                        Math.Pow(StartingLoc.Y - sp.Y, 2));

                    //compare the distances set and add the new one if it is further
                    if (_iDistance > _iFurthestDistance)
                    {
                        _iFurthestDistance = _iDistance;
                        _spFurthest = sp;
                    }
                }
            }
            //return the furthest spawn point
            return _spFurthest;
        }

        /// <summary>
        /// Resets player data IsAlive flag, and creates a new tank
        /// at the spawn point furthest from the opponent.
        /// </summary>
        private void IssueRespawn(List<DynamicShape> movingShapes, Tank playerTank, PointF opponentTankLoc)
        {
            // add new tank for the playerTank's player, in a location far away from the shooter
            movingShapes.Add(new Tank(GetFurthestSpawn(opponentTankLoc),
           playerTank.Color, playerTank.Player));
        }

        /// <summary>
        /// Removes all shapes marked for death, 
        /// and ticks dynamic shapes that aren't blocked.
        /// </summary>
        /// <param name="movingShapes">
        /// Collection of moving shapes.
        /// </param>
        /// <param name="walls">
        /// Collection of walls.
        /// </param>
        private void UpdateGameElements(List<DynamicShape> movingShapes, List<Wall> walls)
        {
            // check for a dead tank 
            if (movingShapes.Any(ds => ds is Tank && ds.IsMarkedForDeath))
            {
                // issue respawn for the player, somewhere far from where they died
                Tank deadTank = movingShapes.Find(ds => ds is Tank && ds.IsMarkedForDeath) as Tank;
                IssueRespawn(movingShapes, deadTank, deadTank.Position);

            }

            // remove all moving shapes marked for death
            movingShapes.RemoveAll((DynamicShape s) => s.IsMarkedForDeath);

            // remove all walls marked for death
            walls.RemoveAll(w => w.IsMarkedForDeath);

            // update dynamic shape positions
            movingShapes.ForEach(ds =>
            {
                // don't let blocked tanks be ticked
                if (ds is Tank && (ds as Tank).IsBlocked)
                {
                    // reset collision flag
                    (ds as Tank).IsBlocked = false;
                    return;
                }

                // safe to update shape
                ds.Tick();
            });
        }
        #endregion
    }
}