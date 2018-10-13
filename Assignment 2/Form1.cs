﻿using System;
using System.Drawing;
using System.Windows.Forms;

namespace INFOIBV {
    public partial class INFOIBV : Form {
        private Bitmap InputImage1, InputImage2, OutputImage;
        private Color[,] Image, Image2, ImageOut;

        public INFOIBV() {
            InitializeComponent();
        }

        private void LoadImage1Button_Click(object sender, EventArgs e) {
            if (openImageDialog.ShowDialog() == DialogResult.OK) {                  // Open File Dialog
                string file = openImageDialog.FileName;                             // Get the file name
                image1FileName.Text = file;                                         // Show file name
                if (InputImage1 != null) InputImage1.Dispose();                     // Reset image
                InputImage1 = new Bitmap(file);                                     // Create new Bitmap from file
                if (InputImage1.Size.Height <= 0 || InputImage1.Size.Width <= 0 ||
                    InputImage1.Size.Height > 512 || InputImage1.Size.Width > 512)  // Dimension check
                    MessageBox.Show("Error in image dimensions (have to be > 0 and <= 512)");
                else {
                    pictureBox1.Image = (Image)InputImage1;                         // Display input image
                    Image = new Color[InputImage1.Size.Width, InputImage1.Size.Height];
                    ImageOut = new Color[InputImage1.Size.Width, InputImage1.Size.Height];
                    // Copy input Bitmap to array            
                    for (int x = 0; x < InputImage1.Size.Width; x++) {
                        for (int y = 0; y < InputImage1.Size.Height; y++) {
                            Image[x, y] = InputImage1.GetPixel(x, y);
                        }
                    }
                }
            }
        }

        private void LoadImage2Button_Click(object sender, EventArgs e) {
            if (InputImage1 == null) {
                MessageBox.Show("Please select Image 1 first.");
                return;
            }
            if (openImageDialog.ShowDialog() == DialogResult.OK) {                  // Open File Dialog
                string file = openImageDialog.FileName;                             // Get the file name
                image2FileName.Text = file;                                         // Show file name
                if (InputImage2 != null) InputImage2.Dispose();                     // Reset image
                InputImage2 = new Bitmap(file);                                     // Create new Bitmap from file
                if (InputImage2.Size != InputImage1.Size) {                         // Check if dimensions are identical
                    MessageBox.Show(string.Format("Please select an image with the same dimensions as Image 1: {0} × {1}.", InputImage1.Size.Width, InputImage1.Size.Height));
                    return;
                }
                if (InputImage2.Size.Height <= 0 || InputImage2.Size.Width <= 0 ||
                    InputImage2.Size.Height > 512 || InputImage2.Size.Width > 512) {// Dimension check
                    MessageBox.Show("Error in image dimensions (have to be > 0 and <= 512)");
                    return;
                }
                pictureBox2.Image = (Image)InputImage2;                             // Display input image
                Image2 = new Color[InputImage2.Size.Width, InputImage2.Size.Height];
                for (int x = 0; x < InputImage2.Size.Width; x++) {
                    for (int y = 0; y < InputImage2.Size.Height; y++) {
                        Image2[x, y] = InputImage2.GetPixel(x, y);
                    }
                }
            }
        }

        private void applyButton_Click(object sender, EventArgs e) {
            if (InputImage1 == null) return;                                            // Get out if no input image
            if (OutputImage != null) OutputImage.Dispose();                             // Reset output image
            OutputImage = new Bitmap(InputImage1.Size.Width, InputImage1.Size.Height);  // Create new output image

            //==========================================================================================
            // (0) Negative
            //Negative();

            // (4) MIN/MAX
            //Min();
            Max();
            
            //==========================================================================================

            // Copy array to output Bitmap
            for (int x = 0; x < InputImage1.Size.Width; x++) {
                for (int y = 0; y < InputImage1.Size.Height; y++) {
                    OutputImage.SetPixel(x, y, ImageOut[x, y]);                // Set the pixel color at coordinate (x,y)
                }
            }

            pictureBoxOut.Image = (Image)OutputImage;                       // Display output image
        }

        private void saveButton_Click(object sender, EventArgs e) {
            if (OutputImage == null) return;                                // Get out if no output image
            if (saveImageDialog.ShowDialog() == DialogResult.OK)
                OutputImage.Save(saveImageDialog.FileName);                 // Save the output image
        }

        //==============================================================================================
        // Filter functions

        //private void template() {
        //    for (int x = 0; x < InputImage1.Size.Width; x++) {
        //        for (int y = 0; y < InputImage1.Size.Height; y++) {
        //            Color pixelColor = Image[x, y];
        //            Color updatedColor = Color.FromArgb(pixelColor.R, pixelColor.G, pixelColor.B);
        //            ImageOut[x, y] = updatedColor;
        //        }
        //    }
        //}

        private void Negative() {
            for (int x = 0; x < InputImage1.Size.Width; x++) {
                for (int y = 0; y < InputImage1.Size.Height; y++) {
                    Color pixelColor = Image[x, y];
                    Color updatedColor = Color.FromArgb(255 - pixelColor.R, 255 - pixelColor.G, 255 - pixelColor.B);
                    ImageOut[x, y] = updatedColor;
                }
            }
        }

        private void Min() {
            for (int x = 0; x < InputImage1.Size.Width; x++) {
                for (int y = 0; y < InputImage1.Size.Height; y++) {
                    Color pixel1Color = Image[x, y];
                    Color pixel2Color = Image2[x, y];
                    int min = Math.Min(pixel1Color.R, pixel2Color.R);
                    Color updatedColor = Color.FromArgb(min, min, min);
                    ImageOut[x, y] = updatedColor;
                }
            }
        }

        private void Max() {
            for (int x = 0; x < InputImage1.Size.Width; x++) {
                for (int y = 0; y < InputImage1.Size.Height; y++) {
                    Color pixel1Color = Image[x, y];
                    Color pixel2Color = Image2[x, y];
                    int max = Math.Max(pixel1Color.R, pixel2Color.R);
                    Color updatedColor = Color.FromArgb(max, max, max);
                    ImageOut[x, y] = updatedColor;
                }
            }
        }

        //==============================================================================================
    }
}