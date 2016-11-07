/********************************************************************
 * File: AbstractInput.cs                                           *
 * Author: Dillon Allan and Jared Karpiak                           *
 * Description: Class used for input abstraction in-game.           *
 ********************************************************************/
////////////////////////////////////////////////////
// Rules for control devices:
// 1) P1 and P2 both use keyboards, 
//    if no controllers are connected.
// 2) If one controller is connected,
//    P1's control device is changed
//    from the keyboard to the controller.
//    (PlayerIndex.One)
// 3) If two controllers are connected,
//    P2's control device is changed from
//    the keyboard to the second controller.
//    (PlayerIndex.Two)
////////////////////////////////////////////////////

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System.Windows.Forms;

namespace CMPE2800_Lab02
{
    public class AbstractInput
    {
        #region Fields
        // pause game flag
        public static bool Pause { get; private set; }

        // quit game flag
        public static bool Quit { get; private set; }

        // new game flag
        public static bool NewGame { get; private set; }

        // input device (keyboard by default)
        private InputDevice Device = InputDevice.Keyboard;

        // used for tracking previous button presses
        private GamePadState _gpsPrevGPadState;

        // get-only player index number
        public PlayerNumber Player { get; }

        // input directions
        public bool Up { get; private set; }
        public bool Down { get; private set; }
        public bool Left { get; private set; }
        public bool Right { get; private set; }

        // rotate tank
        public bool Rotate { get; private set; }

        // fire tank cannon
        public bool Fire { get; private set; }

        // switch weapon
        public bool SwitchWeapon { get; private set; }
        #endregion

        #region Methods
        /// <summary>
        /// Sets input device.
        /// </summary>
        /// <param name="Device">
        /// Add control type.
        /// </param>
        public AbstractInput(PlayerNumber Player)
        {
            this.Player = Player;
        }

        /// <summary>
        /// Checks if a gamepad is connected
        /// for the given player,
        /// and updates Device field,
        /// if necessary.
        /// </summary>
        public void CheckGamePadConnection()
        {
            GamePadState gps = new GamePadState();

            // check player number
            switch (Player)
            {
                case PlayerNumber.One:
                    // get current state of player 1 gamepad
                    gps = GamePad.GetState(PlayerIndex.One);
                    break;

                case PlayerNumber.Two:
                    // get current state of player 2 gamepad
                    gps = GamePad.GetState(PlayerIndex.Two);
                    break;
            }

            // set initial gamepad state
            if (_gpsPrevGPadState == null)
                _gpsPrevGPadState = gps;

            // proceed if the gamepad has been plugged in
            if (gps.IsConnected)
            {
                // change device to reflect the new gamepad,
                // if necessary
                if (Device != InputDevice.GamePad)
                    Device = InputDevice.GamePad;

                // send gamepad state for processing
                SetGamePadInput(gps);
            }
            // (do nothing if gamepad isn't connected
            // -- keyboard input handled by main form keyUp & keyDown events)
        }

        /// <summary>
        /// Sets input fields based 
        /// on the polled gamepad state.
        /// </summary>
        /// <param name="gps">
        /// Gamepad state object.
        /// </param>
        private void SetGamePadInput(GamePadState gps)
        {
            // set translation members

            // Up
            if (gps.ThumbSticks.Left.Y > 0)
                Up = true;
            else
                Up = false;

            // Down
            if (gps.ThumbSticks.Left.Y < 0)
                Down = true;
            else
                Down = false;

            // Left
            if (gps.ThumbSticks.Left.X < 0)
                Left = true;
            else
                Left = false;

            // Right
            if (gps.ThumbSticks.Left.X > 0)
                Right = true;
            else
                Right = false;

            // set rotation member on pressing the A button
            if (gps.Buttons.A == Microsoft.Xna.Framework.Input.ButtonState.Pressed)
                Rotate = true;
            else
                Rotate = false;

            // check input for fire cannon 
            // (user can pull trigger halfway to fire)
            if (gps.Triggers.Right > 0.5)
                Fire = true;
            else
                Fire = false;

            // check for switch weapon
            if (_gpsPrevGPadState.Buttons.B == Microsoft.Xna.Framework.Input.ButtonState.Pressed &&
                gps.Buttons.B == Microsoft.Xna.Framework.Input.ButtonState.Released)
                SwitchWeapon = true;
            else
                SwitchWeapon = false;

            // check for pause game
            if (_gpsPrevGPadState.Buttons.Start == Microsoft.Xna.Framework.Input.ButtonState.Pressed &&
                gps.Buttons.Start == Microsoft.Xna.Framework.Input.ButtonState.Released)
                Pause = !Pause;

            // check for quit game
            if (gps.Buttons.Y == Microsoft.Xna.Framework.Input.ButtonState.Pressed)
                Quit = true;

            // check for new game
            if (gps.Buttons.Back == Microsoft.Xna.Framework.Input.ButtonState.Pressed)
                NewGame = true;

            // save as the previous gamepad state
            _gpsPrevGPadState = gps;
        }

        /// <summary>
        /// Checks keyUp event data and sets static game controls.
        /// </summary>
        /// <param name="key"></param>
        public static void SetStaticInput(KeyEventArgs key)
        {
            // check general game input (pause, quit, new game)
            switch (key.KeyCode)
            {
                // set pause member
                case System.Windows.Forms.Keys.P:
                    Pause = !Pause;
                    break;

                // set quit game member
                case System.Windows.Forms.Keys.Escape:
                    Quit = true;
                    break;

                // set new game member
                case System.Windows.Forms.Keys.N:
                    NewGame = true;
                    break;
            }
        }

        /// <summary>
        /// Sets abstract input values based on 
        /// data from KeyDown events.
        /// </summary>
        /// <param name="key">
        /// KeyDown event.
        /// </param>
        public void SetKeyDownInput(KeyEventArgs key)
        {
            // check keys by player number
            switch (Player)
            {
                // player 1 controls
                case PlayerNumber.One:
                    switch (key.KeyCode)
                    {
                        // set rotation member
                        case System.Windows.Forms.Keys.ControlKey:
                            Rotate = true;
                            break;

                        // set translation members
                        case System.Windows.Forms.Keys.Up:
                            Up = true;
                            break;
                        case System.Windows.Forms.Keys.Down:
                            Down = true;
                            break;
                        case System.Windows.Forms.Keys.Left:
                            Left = true;
                            break;
                        case System.Windows.Forms.Keys.Right:
                            Right = true;
                            break;

                        // set fire member
                        case System.Windows.Forms.Keys.Enter:
                            Fire = true;
                            break;

                        // set weapon switch member
                        case System.Windows.Forms.Keys.Oem2:
                            SwitchWeapon = true;
                            break;
                    }
                    break;

                // player 2 controls
                case PlayerNumber.Two:
                    switch (key.KeyCode)
                    {
                        // set rotation member
                        case System.Windows.Forms.Keys.ShiftKey:
                            Rotate = true;
                            break;

                        // set translation members 
                        case System.Windows.Forms.Keys.W:
                            Up = true;
                            break;
                        case System.Windows.Forms.Keys.S:
                            Down = true;
                            break;
                        case System.Windows.Forms.Keys.A:
                            Left = true;
                            break;
                        case System.Windows.Forms.Keys.D:
                            Right = true;
                            break;

                        // set fire member
                        case System.Windows.Forms.Keys.Space:
                            Fire = true;
                            break;

                        // set weapon switch member
                        case System.Windows.Forms.Keys.Q:
                            if (!SwitchWeapon)
                                SwitchWeapon = true;
                            break;
                    }
                    break;
            }
        }

        /// <summary>
        /// Resets abstract input values based on 
        /// keyUp event data.
        /// </summary>
        /// <param name="key">
        /// KeyUp event.
        /// </param>
        public void SetKeyUpInput(KeyEventArgs key)
        {
            // check keys by player number
            switch (Player)
            {
                // player 1 controls
                case PlayerNumber.One:
                    switch (key.KeyCode)
                    {
                        // reset rotation member
                        case System.Windows.Forms.Keys.ControlKey:
                            Rotate = false;
                            break;

                        // reset translation members
                        case System.Windows.Forms.Keys.Up:
                            Up = false;
                            break;
                        case System.Windows.Forms.Keys.Down:
                            Down = false;
                            break;
                        case System.Windows.Forms.Keys.Left:
                            Left = false;
                            break;
                        case System.Windows.Forms.Keys.Right:
                            Right = false;
                            break;

                        // reset fire member
                        case System.Windows.Forms.Keys.Enter:
                            Fire = false;
                            break;
                    }
                    break;

                // player 2 controls
                case PlayerNumber.Two:
                    switch (key.KeyCode)
                    {
                        // reset rotation member
                        case System.Windows.Forms.Keys.ShiftKey:
                            Rotate = false;
                            break;

                        // reset translation members 
                        case System.Windows.Forms.Keys.W:
                            Up = false;
                            break;
                        case System.Windows.Forms.Keys.S:
                            Down = false;
                            break;
                        case System.Windows.Forms.Keys.A:
                            Left = false;
                            break;
                        case System.Windows.Forms.Keys.D:
                            Right = false;
                            break;

                        // reset fire member
                        case System.Windows.Forms.Keys.Space:
                            Fire = false;
                            break;

                        // reset weapon switch member
                        case System.Windows.Forms.Keys.Q:
                            SwitchWeapon = false;
                            break;
                    }
                    break;
            }
        }

        /// <summary>
        /// Callback function for reseting 
        /// the SwitchWeapon property.
        /// </summary>
        public void ResetSwitchWeapon()
        {
            SwitchWeapon = false;
        }
        #endregion
    }
}

