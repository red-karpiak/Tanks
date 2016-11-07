/********************************************************************************
 * File: NewGame.cs                                                             *
 * Author: Dillon Allan and Jared Karpiak                                       *
 * Description: Modal dialog used for having the user select which map to load. *
 ********************************************************************************/
using System;
using System.Windows.Forms;

namespace CMPE2800_Lab02
{
    public partial class NewGame : Form
    {
        #region Members
        // delegate for setting up a new game
        public delegate string delStringNewGame();

        // file path for the level background image
        public string XMLValue { get; private set; }
        #endregion

        #region Methods
        /// <summary>
        /// Instance constructor. Centers the form to the screen.
        /// </summary>
        public NewGame()
        {
            InitializeComponent();

            // center the form to the screen
            StartPosition = FormStartPosition.CenterScreen;
        }

        /// <summary>
        /// Sets the path member to the value of the desired XML file,
        /// and sets the dialog result to "OK."
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void _btnLoad_Click(object sender, EventArgs e)
        {
            // set the xml file path based on which radio btn is checked
            if (_rbCity.Checked)
                XMLValue = Properties.Resources.CityLevel;
            else if (_rbDesert.Checked)
                XMLValue = Properties.Resources.DesertLevel;
            else if (_rbPlains.Checked)
                XMLValue = Properties.Resources.GrassLevel;

            DialogResult = DialogResult.OK;
        }

        /// <summary>
        /// Sets dialog result to "cancel."
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void _btnCancel_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
        }
        #endregion

        /// <summary>
        /// Sets focus on the Load button.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void NewGame_Load(object sender, EventArgs e)
        {
            // set focus on the load button
            _btnLoad.Select();
        }
    }
}
