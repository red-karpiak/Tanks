/****************************************************************
 * File: GameOver.cs                                            *
 * Author: Dillon Allan and Jared Karpiak                       *
 * Description: Modal dialog for handling game over events.     *
 ****************************************************************/
using System;
using System.Windows.Forms;

namespace CMPE2800_Lab02
{
    public partial class GameOver : Form
    {
        #region Members
        // delegate for handling game over event
        public delegate void delVoidGameOver(PlayerNumber player);
        #endregion

        #region Methods
        /// <summary>
        /// Instance constructor. Centres form to screen.
        /// </summary>
        public GameOver()
        {
            InitializeComponent();

            // center form to screen
            StartPosition = FormStartPosition.CenterScreen;

            DialogResult = DialogResult.None;
        }

        /// <summary>
        /// Click event triggers new game event.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void _btnNewGame_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.OK;
        }

        /// <summary>
        /// Click event triggers application exit event.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void _btnQuit_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
        }

        /// <summary>
        /// Callback function for setting player victory text.
        /// </summary>
        /// <param name="player">Winner's player number.</param>
        public void CBSetPlayerVictory(PlayerNumber player)
        {
            int playerNum = 0;

            // get number from player
            switch (player)
            {
                case PlayerNumber.One:
                    playerNum = 1;
                    break;
                case PlayerNumber.Two:
                    playerNum = 2;
                    break;
            }

            // set victory text
            _labWinner.Text = $"Player {playerNum} wins";
        }
        #endregion
    }
}
