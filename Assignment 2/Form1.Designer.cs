﻿namespace INFOIBV
{
    partial class INFOIBV
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
            this.LoadImage1Button = new System.Windows.Forms.Button();
            this.openImageDialog = new System.Windows.Forms.OpenFileDialog();
            this.image1FileName = new System.Windows.Forms.TextBox();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.applyButton = new System.Windows.Forms.Button();
            this.saveImageDialog = new System.Windows.Forms.SaveFileDialog();
            this.saveButton = new System.Windows.Forms.Button();
            this.pictureBoxOut = new System.Windows.Forms.PictureBox();
            this.pictureBox2 = new System.Windows.Forms.PictureBox();
            this.image2FileName = new System.Windows.Forms.TextBox();
            this.LoadImage2Button = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxOut)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).BeginInit();
            this.SuspendLayout();
            // 
            // LoadImage1Button
            // 
            this.LoadImage1Button.Location = new System.Drawing.Point(12, 12);
            this.LoadImage1Button.Name = "LoadImage1Button";
            this.LoadImage1Button.Size = new System.Drawing.Size(98, 23);
            this.LoadImage1Button.TabIndex = 0;
            this.LoadImage1Button.Text = "Load image 1";
            this.LoadImage1Button.UseVisualStyleBackColor = true;
            this.LoadImage1Button.Click += new System.EventHandler(this.LoadImage1Button_Click);
            // 
            // openImageDialog
            // 
            this.openImageDialog.Filter = "Bitmap files (*.bmp;*.gif;*.jpg;*.png;*.tiff;*.jpeg)|*.bmp;*.gif;*.jpg;*.png;*.ti" +
    "ff;*.jpeg";
            this.openImageDialog.InitialDirectory = "..\\..\\images";
            // 
            // image1FileName
            // 
            this.image1FileName.Location = new System.Drawing.Point(116, 14);
            this.image1FileName.Name = "image1FileName";
            this.image1FileName.ReadOnly = true;
            this.image1FileName.Size = new System.Drawing.Size(316, 20);
            this.image1FileName.TabIndex = 1;
            // 
            // pictureBox1
            // 
            this.pictureBox1.Location = new System.Drawing.Point(13, 45);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(512, 512);
            this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage;
            this.pictureBox1.TabIndex = 2;
            this.pictureBox1.TabStop = false;
            // 
            // applyButton
            // 
            this.applyButton.Location = new System.Drawing.Point(1049, 12);
            this.applyButton.Name = "applyButton";
            this.applyButton.Size = new System.Drawing.Size(103, 23);
            this.applyButton.TabIndex = 3;
            this.applyButton.Text = "Apply";
            this.applyButton.UseVisualStyleBackColor = true;
            this.applyButton.Click += new System.EventHandler(this.applyButton_Click);
            // 
            // saveImageDialog
            // 
            this.saveImageDialog.Filter = "Bitmap file (*.bmp)|*.bmp";
            this.saveImageDialog.InitialDirectory = "..\\..\\images";
            // 
            // saveButton
            // 
            this.saveButton.Location = new System.Drawing.Point(1466, 11);
            this.saveButton.Name = "saveButton";
            this.saveButton.Size = new System.Drawing.Size(95, 23);
            this.saveButton.TabIndex = 4;
            this.saveButton.Text = "Save as BMP...";
            this.saveButton.UseVisualStyleBackColor = true;
            this.saveButton.Click += new System.EventHandler(this.saveButton_Click);
            // 
            // pictureBoxOut
            // 
            this.pictureBoxOut.Location = new System.Drawing.Point(1049, 45);
            this.pictureBoxOut.Name = "pictureBoxOut";
            this.pictureBoxOut.Size = new System.Drawing.Size(512, 512);
            this.pictureBoxOut.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage;
            this.pictureBoxOut.TabIndex = 5;
            this.pictureBoxOut.TabStop = false;
            // 
            // pictureBox2
            // 
            this.pictureBox2.Location = new System.Drawing.Point(531, 45);
            this.pictureBox2.Name = "pictureBox2";
            this.pictureBox2.Size = new System.Drawing.Size(512, 512);
            this.pictureBox2.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage;
            this.pictureBox2.TabIndex = 7;
            this.pictureBox2.TabStop = false;
            // 
            // image2FileName
            // 
            this.image2FileName.Location = new System.Drawing.Point(635, 14);
            this.image2FileName.Name = "image2FileName";
            this.image2FileName.ReadOnly = true;
            this.image2FileName.Size = new System.Drawing.Size(316, 20);
            this.image2FileName.TabIndex = 9;
            // 
            // LoadImage2Button
            // 
            this.LoadImage2Button.Location = new System.Drawing.Point(531, 12);
            this.LoadImage2Button.Name = "LoadImage2Button";
            this.LoadImage2Button.Size = new System.Drawing.Size(98, 23);
            this.LoadImage2Button.TabIndex = 8;
            this.LoadImage2Button.Text = "Load image 2";
            this.LoadImage2Button.UseVisualStyleBackColor = true;
            this.LoadImage2Button.Click += new System.EventHandler(this.LoadImage2Button_Click);
            // 
            // INFOIBV
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1629, 576);
            this.Controls.Add(this.image2FileName);
            this.Controls.Add(this.LoadImage2Button);
            this.Controls.Add(this.pictureBox2);
            this.Controls.Add(this.pictureBoxOut);
            this.Controls.Add(this.saveButton);
            this.Controls.Add(this.applyButton);
            this.Controls.Add(this.pictureBox1);
            this.Controls.Add(this.image1FileName);
            this.Controls.Add(this.LoadImage1Button);
            this.Location = new System.Drawing.Point(10, 10);
            this.Name = "INFOIBV";
            this.ShowIcon = false;
            this.Text = "INFOIBV";
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxOut)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button LoadImage1Button;
        private System.Windows.Forms.OpenFileDialog openImageDialog;
        private System.Windows.Forms.TextBox image1FileName;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.Button applyButton;
        private System.Windows.Forms.SaveFileDialog saveImageDialog;
        private System.Windows.Forms.Button saveButton;
        private System.Windows.Forms.PictureBox pictureBoxOut;
        private System.Windows.Forms.PictureBox pictureBox2;
        private System.Windows.Forms.TextBox image2FileName;
        private System.Windows.Forms.Button LoadImage2Button;
    }
}

