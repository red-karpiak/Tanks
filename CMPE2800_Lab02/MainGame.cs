/****************************************************************
 * File: Form1.cs                                               *
 * Author: Dillon Allan and Jason Karpiak                       *
 * Description: Main file for game event processing.            *
 ****************************************************************/
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Threading;
using System.Windows.Forms;

namespace CMPE2800_Lab02
{
    public partial class MainGame : Form
    {
        #region Members
        // modeless game UI form
        GameUI _modelessUI;

        // Main form y-offset in pixels
        const int _iMainFormYOffset = 100;

        // game level
        Level _lvGameLevel;

        // pause flag
        volatile bool _bGamePaused = false;

        // Random number generator, used for random ammo spawns
        Random _rng = new Random();

        // background thread for input processing, hit detection, and game updates.
        Thread _tBackgroundProcessing;

        // lock objects
        object _oInputLock = new object();
        object _oRenderLock = new object();

        // List of player data
        List<PlayerData> _lPlayerData;

        // List of abstract inputs
        List<AbstractInput> _lPlayerInputs;

        // List of moving shapes
        List<DynamicShape> _lDynShapes;

        // List of walls
        List<Wall> _lWalls;

        // List of Tank Spawn locations
        List<PointF> _lTankSpawns = new List<PointF>();

        // List of Ammo drops
        List<Ammo> _lAmmoDrops;

        // time between rendering a new ammo drop
        const int _iAmmoTimeout = 5000;

        // used for background thread sleeping (in ms)
        const int _iGameClock = 25;

        // colors 
        Color _cPlayer1Color = Color.Violet;
        Color _cPlayer2Color = Color.Coral;
        Color _cPlayer1MGColor = Color.Fuchsia;
        Color _cPlayer2MGColor = Color.Yellow;
        Color _cPlayer1RocketColor = Color.Aqua;
        Color _cPlayer2RocketColor = Color.DodgerBlue;
        #endregion

        #region Methods
        /// <summary>
        /// Main form instance constructor.
        /// </summary>
        public MainGame()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Run new game event on form load.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MainGame_Load(object sender, EventArgs e)
        {
            NewGame();
        }

        /// <summary>
        /// Renders one frame of gameplay.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void _timMain_Tick(object sender, EventArgs e)
        {
            // render new frame
            lock (_oRenderLock)
            {
                DoubleBufferedRender();
            }
        }

        /////////////////////////////////////////////////////////////////////////////
        // Player keyboard controls
        // NOTE: If one GamePad is connected, the keyboard controls are disabled
        // for player 1. If two GamePads are connected, both keyboard controls
        // are disabled.
        // General Game controls: P - pause game
        //                        Escape - quit game
        //                        N - new game
        //
        // Player 1 controls: Arrow keys - translational movement
        //                    Right-ctrl + Arrow key - rotate tank (can't translate)
        //                    Enter - fire
        //                    '/' key: switch weapon
        // Player 2 controls: WASD - translation movement
        //                    Left-shift + WASD key - rotate tank (can't translate)
        //                    Space - fire
        //                    Q key - switch weapon
        /////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// Checks key up states, and 
        /// applies input processing for the appropriate player.
        /// </summary>
        private void MainGame_KeyUp(object sender, KeyEventArgs e)
        {
            // send key up event to static input check
            AbstractInput.SetStaticInput(e);

            // don't do anything if the game is paused
            // (i.e. only static input is read from the keyboard while paused)
            if (_bGamePaused)
                return;

            // take a snapshot of the player inputs list
            List<AbstractInput> inputSnapshot;
            lock (_oInputLock)
            {
                inputSnapshot = new List<AbstractInput>(_lPlayerInputs);
            }

            // send key up event to each player input
            foreach (AbstractInput ab in inputSnapshot)
                ab.SetKeyUpInput(e);

            // update the player inputs list
            lock (_oInputLock)
            {
                _lPlayerInputs = new List<AbstractInput>(inputSnapshot);
            }
        }

        /// <summary>
        /// Checks key down states, and 
        /// applies input processing for the appropriate player.
        /// </summary>
        private void MainGame_KeyDown(object sender, KeyEventArgs e)
        {
            // don't do anything if the game is paused
            if (_bGamePaused)
                return;

            // take a snapshot of the player inputs list
            List<AbstractInput> inputSnapshot;
            lock (_oInputLock)
            {
                inputSnapshot = new List<AbstractInput>(_lPlayerInputs);
            }

            // send key down event to each player input
            foreach (AbstractInput ab in inputSnapshot)
                ab.SetKeyDownInput(e);

            // update the player inputs list
            lock (_oInputLock)
            {
                _lPlayerInputs = new List<AbstractInput>(inputSnapshot);
            }
        }
        #endregion
    }
}