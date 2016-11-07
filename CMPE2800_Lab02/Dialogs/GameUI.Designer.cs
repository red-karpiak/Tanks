namespace CMPE2800_Lab02
{
    partial class GameUI
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this._labP1 = new System.Windows.Forms.Label();
            this._labP2 = new System.Windows.Forms.Label();
            this._labHP1 = new System.Windows.Forms.Label();
            this._labHP2 = new System.Windows.Forms.Label();
            this._labScoreDisplay = new System.Windows.Forms.Label();
            this._labScore = new System.Windows.Forms.Label();
            this._pbxRocket2 = new System.Windows.Forms.PictureBox();
            this._pbxMG2 = new System.Windows.Forms.PictureBox();
            this._pbxMG1 = new System.Windows.Forms.PictureBox();
            this._pbxRocket1 = new System.Windows.Forms.PictureBox();
            this._labLives1 = new System.Windows.Forms.Label();
            this._labLives2 = new System.Windows.Forms.Label();
            this._labHAmmo1 = new System.Windows.Forms.Label();
            this._labHAmmo2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this._pbxRocket2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this._pbxMG2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this._pbxMG1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this._pbxRocket1)).BeginInit();
            this.SuspendLayout();
            // 
            // _labP1
            // 
            this._labP1.AutoSize = true;
            this._labP1.Dock = System.Windows.Forms.DockStyle.Left;
            this._labP1.Font = new System.Drawing.Font("Courier New", 14.25F, System.Drawing.FontStyle.Underline, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this._labP1.Location = new System.Drawing.Point(0, 0);
            this._labP1.Name = "_labP1";
            this._labP1.Size = new System.Drawing.Size(98, 21);
            this._labP1.TabIndex = 0;
            this._labP1.Text = "Player 1";
            // 
            // _labP2
            // 
            this._labP2.AutoSize = true;
            this._labP2.Dock = System.Windows.Forms.DockStyle.Right;
            this._labP2.Font = new System.Drawing.Font("Courier New", 14.25F, System.Drawing.FontStyle.Underline, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this._labP2.Location = new System.Drawing.Point(686, 0);
            this._labP2.Name = "_labP2";
            this._labP2.Size = new System.Drawing.Size(98, 21);
            this._labP2.TabIndex = 1;
            this._labP2.Text = "Player 2";
            this._labP2.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // _labHP1
            // 
            this._labHP1.AutoSize = true;
            this._labHP1.Font = new System.Drawing.Font("Courier New", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this._labHP1.Location = new System.Drawing.Point(1, 33);
            this._labHP1.Name = "_labHP1";
            this._labHP1.Size = new System.Drawing.Size(88, 18);
            this._labHP1.TabIndex = 2;
            this._labHP1.Text = "HP : 100";
            // 
            // _labHP2
            // 
            this._labHP2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this._labHP2.AutoSize = true;
            this._labHP2.Font = new System.Drawing.Font("Courier New", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this._labHP2.Location = new System.Drawing.Point(696, 33);
            this._labHP2.Name = "_labHP2";
            this._labHP2.Size = new System.Drawing.Size(88, 18);
            this._labHP2.TabIndex = 3;
            this._labHP2.Text = "100 : HP";
            // 
            // _labScoreDisplay
            // 
            this._labScoreDisplay.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this._labScoreDisplay.AutoSize = true;
            this._labScoreDisplay.Font = new System.Drawing.Font("Courier New", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this._labScoreDisplay.Location = new System.Drawing.Point(360, 25);
            this._labScoreDisplay.Name = "_labScoreDisplay";
            this._labScoreDisplay.Size = new System.Drawing.Size(65, 21);
            this._labScoreDisplay.TabIndex = 4;
            this._labScoreDisplay.Text = "0 | 0";
            // 
            // _labScore
            // 
            this._labScore.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this._labScore.AutoSize = true;
            this._labScore.Font = new System.Drawing.Font("Courier New", 14.25F, System.Drawing.FontStyle.Underline, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this._labScore.Location = new System.Drawing.Point(360, 0);
            this._labScore.Name = "_labScore";
            this._labScore.Size = new System.Drawing.Size(65, 21);
            this._labScore.TabIndex = 5;
            this._labScore.Text = "Score";
            // 
            // _pbxRocket2
            // 
            this._pbxRocket2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this._pbxRocket2.Image = global::CMPE2800_Lab02.Properties.Resources.rockets;
            this._pbxRocket2.Location = new System.Drawing.Point(488, 59);
            this._pbxRocket2.Name = "_pbxRocket2";
            this._pbxRocket2.Size = new System.Drawing.Size(100, 50);
            this._pbxRocket2.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this._pbxRocket2.TabIndex = 9;
            this._pbxRocket2.TabStop = false;
            // 
            // _pbxMG2
            // 
            this._pbxMG2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this._pbxMG2.BackColor = System.Drawing.SystemColors.ActiveCaption;
            this._pbxMG2.Image = global::CMPE2800_Lab02.Properties.Resources.shell;
            this._pbxMG2.Location = new System.Drawing.Point(488, 21);
            this._pbxMG2.Name = "_pbxMG2";
            this._pbxMG2.Size = new System.Drawing.Size(100, 30);
            this._pbxMG2.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this._pbxMG2.TabIndex = 8;
            this._pbxMG2.TabStop = false;
            // 
            // _pbxMG1
            // 
            this._pbxMG1.BackColor = System.Drawing.SystemColors.ActiveCaption;
            this._pbxMG1.Image = global::CMPE2800_Lab02.Properties.Resources.shell;
            this._pbxMG1.Location = new System.Drawing.Point(196, 21);
            this._pbxMG1.Name = "_pbxMG1";
            this._pbxMG1.Size = new System.Drawing.Size(100, 30);
            this._pbxMG1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this._pbxMG1.TabIndex = 7;
            this._pbxMG1.TabStop = false;
            // 
            // _pbxRocket1
            // 
            this._pbxRocket1.Image = global::CMPE2800_Lab02.Properties.Resources.rockets;
            this._pbxRocket1.Location = new System.Drawing.Point(196, 59);
            this._pbxRocket1.Name = "_pbxRocket1";
            this._pbxRocket1.Size = new System.Drawing.Size(100, 50);
            this._pbxRocket1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this._pbxRocket1.TabIndex = 6;
            this._pbxRocket1.TabStop = false;
            // 
            // _labLives1
            // 
            this._labLives1.AutoSize = true;
            this._labLives1.Font = new System.Drawing.Font("Courier New", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this._labLives1.Location = new System.Drawing.Point(1, 65);
            this._labLives1.Name = "_labLives1";
            this._labLives1.Size = new System.Drawing.Size(98, 18);
            this._labLives1.TabIndex = 10;
            this._labLives1.Text = "Lives : 3";
            // 
            // _labLives2
            // 
            this._labLives2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this._labLives2.AutoSize = true;
            this._labLives2.Font = new System.Drawing.Font("Courier New", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this._labLives2.Location = new System.Drawing.Point(686, 64);
            this._labLives2.Name = "_labLives2";
            this._labLives2.Size = new System.Drawing.Size(98, 18);
            this._labLives2.TabIndex = 11;
            this._labLives2.Text = "3 : Lives";
            // 
            // _labHAmmo1
            // 
            this._labHAmmo1.AutoSize = true;
            this._labHAmmo1.Font = new System.Drawing.Font("Courier New", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this._labHAmmo1.Location = new System.Drawing.Point(1, 97);
            this._labHAmmo1.Name = "_labHAmmo1";
            this._labHAmmo1.Size = new System.Drawing.Size(148, 18);
            this._labHAmmo1.TabIndex = 12;
            this._labHAmmo1.Text = "Heavy Ammo : 5";
            // 
            // _labHAmmo2
            // 
            this._labHAmmo2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this._labHAmmo2.AutoSize = true;
            this._labHAmmo2.Font = new System.Drawing.Font("Courier New", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this._labHAmmo2.Location = new System.Drawing.Point(636, 95);
            this._labHAmmo2.Name = "_labHAmmo2";
            this._labHAmmo2.Size = new System.Drawing.Size(148, 18);
            this._labHAmmo2.TabIndex = 13;
            this._labHAmmo2.Text = "5 : Heavy Ammo";
            // 
            // label1
            // 
            this.label1.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Courier New", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(336, 60);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(120, 48);
            this.label1.TabIndex = 14;
            this.label1.Text = "P   - Pause\r\nEsc - Quit\r\nN   - New Game";
            // 
            // GameUI
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(784, 115);
            this.ControlBox = false;
            this.Controls.Add(this.label1);
            this.Controls.Add(this._labHAmmo2);
            this.Controls.Add(this._labHAmmo1);
            this.Controls.Add(this._labLives2);
            this.Controls.Add(this._labLives1);
            this.Controls.Add(this._pbxRocket2);
            this.Controls.Add(this._pbxMG2);
            this.Controls.Add(this._pbxMG1);
            this.Controls.Add(this._pbxRocket1);
            this.Controls.Add(this._labScore);
            this.Controls.Add(this._labScoreDisplay);
            this.Controls.Add(this._labHP2);
            this.Controls.Add(this._labHP1);
            this.Controls.Add(this._labP2);
            this.Controls.Add(this._labP1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Name = "GameUI";
            this.Text = "First to 3 points wins!";
            ((System.ComponentModel.ISupportInitialize)(this._pbxRocket2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this._pbxMG2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this._pbxMG1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this._pbxRocket1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label _labP1;
        private System.Windows.Forms.Label _labP2;
        private System.Windows.Forms.Label _labHP1;
        private System.Windows.Forms.Label _labHP2;
        private System.Windows.Forms.Label _labScoreDisplay;
        private System.Windows.Forms.Label _labScore;
        private System.Windows.Forms.PictureBox _pbxRocket1;
        private System.Windows.Forms.PictureBox _pbxMG1;
        private System.Windows.Forms.PictureBox _pbxMG2;
        private System.Windows.Forms.PictureBox _pbxRocket2;
        private System.Windows.Forms.Label _labLives1;
        private System.Windows.Forms.Label _labLives2;
        private System.Windows.Forms.Label _labHAmmo1;
        private System.Windows.Forms.Label _labHAmmo2;
        private System.Windows.Forms.Label label1;
    }
}