﻿/*
 * Tijmen van ter Beek - 5961564
 * Kasper Nooteboom    - 5845866
 */

using System;
using System.Drawing;
using System.Windows.Forms;

namespace INFOIBV {
    public partial class INFOIBV : Form {
        private Bitmap InputImage;
        private Bitmap OutputImage;

        public INFOIBV() {
            InitializeComponent();
        }

        private void LoadImageButton_Click(object sender, EventArgs e) {
            if (openImageDialog.ShowDialog() == DialogResult.OK)             // Open File Dialog
             {
                string file = openImageDialog.FileName;                     // Get the file name
                imageFileName.Text = file;                                  // Show file name
                if (InputImage != null) InputImage.Dispose();               // Reset image
                InputImage = new Bitmap(file);                              // Create new Bitmap from file
                if (InputImage.Size.Height <= 0 || InputImage.Size.Width <= 0 ||
                    InputImage.Size.Height > 512 || InputImage.Size.Width > 512) // Dimension check
                    MessageBox.Show("Error in image dimensions (have to be > 0 and <= 512)");
                else
                    pictureBox1.Image = (Image)InputImage;                 // Display input image
            }
        }

        private void applyButton_Click(object sender, EventArgs e) {
            if (InputImage == null) return;                                 // Get out if no input image
            if (OutputImage != null) OutputImage.Dispose();                 // Reset output image
            OutputImage = new Bitmap(InputImage.Size.Width, InputImage.Size.Height); // Create new output image
            Color[,] image = new Color[InputImage.Size.Width, InputImage.Size.Height]; // Create array to speed-up operations (Bitmap functions are very slow)

            // Copy input Bitmap to array            
            for (int x = 0; x < InputImage.Size.Width; x++) {
                for (int y = 0; y < InputImage.Size.Height; y++) {
                    image[x, y] = InputImage.GetPixel(x, y);                // Set pixel color in array at (x,y)
                }
            }

            //==========================================================================================
            // (0) Negative
            //Negative(image);

            // (1) Grayscale
            //Grayscale(image);

            // (2) Contrast
            //Contrast(image);

            // (3/4) Gaussian / Linear Filter
            //Linear(image, GaussianKernel(5, 1));

            // (5) Median Filter
            //Median(image, 5);

            // (6) Edges
            //Edges(image);

            // (7) Thresholding
            //Threshold(image, 55);

            // Visualisation of Laplacian used in Edge Sharpening
            //Laplacian(image);

            // Bonus: Edge Sharpening
            //SharpenEdges(image);

            // Bonus: Histogram Equalisation
            //FlattenHist(image);
            //==========================================================================================

            // Copy array to output Bitmap
            for (int x = 0; x < InputImage.Size.Width; x++) {
                for (int y = 0; y < InputImage.Size.Height; y++) {
                    OutputImage.SetPixel(x, y, image[x, y]);               // Set the pixel color at coordinate (x,y)
                }
            }

            pictureBox2.Image = (Image)OutputImage;                         // Display output image
        }

        private void saveButton_Click(object sender, EventArgs e) {
            if (OutputImage == null) return;                                // Get out if no output image
            if (saveImageDialog.ShowDialog() == DialogResult.OK)
                OutputImage.Save(saveImageDialog.FileName);                 // Save the output image
        }

        //==============================================================================================
        // Filter functions

        private void Negative(Color[,] image) {
            for (int x = 0; x < InputImage.Size.Width; x++) {
                for (int y = 0; y < InputImage.Size.Height; y++) {
                    Color pixelColor = image[x, y];                         // Get the pixel color at coordinate (x,y)
                    Color updatedColor = Color.FromArgb(255 - pixelColor.R, 255 - pixelColor.G, 255 - pixelColor.B); // Negative image
                    image[x, y] = updatedColor;                             // Set the new pixel color at coordinate (x,y)
                }
            }
        }

        private void Grayscale(Color[,] image) {
            for (int x = 0; x < InputImage.Size.Width; x++) {
                for (int y = 0; y < InputImage.Size.Height; y++) {
                    Color pixelColor = image[x, y];
                    int gray = (pixelColor.R + pixelColor.G + pixelColor.B) / 3;
                    Color updatedColor = Color.FromArgb(gray, gray, gray);
                    image[x, y] = updatedColor;
                }
            }
        }

        private void Contrast(Color[,] image) {
            int loR = 256, hiR = 0, loG = 256, hiG = 0, loB = 256, hiB = 0;
            for (int x = 0; x < InputImage.Size.Width; x++) {
                for (int y = 0; y < InputImage.Size.Height; y++) {
                    Color pixelColor = image[x, y];
                    if (pixelColor.R < loR) loR = pixelColor.R;
                    if (pixelColor.R > hiR) hiR = pixelColor.R;
                    if (pixelColor.G < loG) loG = pixelColor.G;
                    if (pixelColor.G > hiG) hiG = pixelColor.G;
                    if (pixelColor.B < loB) loB = pixelColor.B;
                    if (pixelColor.B > hiB) hiB = pixelColor.B;
                }
            }
            double rMult = 255.0 / (hiR - loR);
            double gMult = 255.0 / (hiG - loG);
            double bMult = 255.0 / (hiB - loB);
            for (int x = 0; x < InputImage.Size.Width; x++) {
                for (int y = 0; y < InputImage.Size.Height; y++) {
                    Color pixelColor = image[x, y];
                    int r, g, b;
                    if (hiR - loR > 0) r = (int)((pixelColor.R - loR) * rMult);
                    else r = pixelColor.R;
                    if (hiG - loG > 0) g = (int)((pixelColor.G - loG) * gMult);
                    else g = pixelColor.G;
                    if (hiB - loB > 0) b = (int)((pixelColor.B - loB) * bMult);
                    else b = pixelColor.B;
                    Color updatedColor = Color.FromArgb(r, g, b);
                    image[x, y] = updatedColor;
                }
            }
        }

        private void Linear(Color[,] image, float[,] matrix) {
            int size = matrix.GetLength(0);
            int radius = size / 2;
            int width = InputImage.Size.Width;
            int height = InputImage.Size.Height;
            int xrange = width - 1;
            int yrange = height - 1;
            Color[,] database = (Color[,])image.Clone();
            for (int x = 0; x < width; x++) {
                for (int y = 0; y < height; y++) {
                    float output = 0;
                    for (int u = x - radius; u <= x + radius; u++) {
                        for (int v = y - radius; v <= y + radius; v++) {
                            int eu = Math.Abs(-Math.Abs(u - xrange) + xrange);
                            int ev = Math.Abs(-Math.Abs(v - yrange) + yrange); //mirror at edges
                            output += database[eu, ev].R * matrix[u - x + radius, v - y + radius];
                        }
                    }
                    int op = (int)output;
                    image[x, y] = Color.FromArgb(op, op, op);
                }
            }
        }

        private void Edges(Color[,] image) {

            float[,] matrix = Sobel1();
            float[,] altmatrix = Sobel2();
            int size = matrix.GetLength(0);
            int radius = size / 2;
            int width = InputImage.Size.Width;
            int height = InputImage.Size.Height;
            int[,] tempOutput = new int[width, height];
            int xrange = width - 1;
            int yrange = height - 1;
            Color[,] database = (Color[,])image.Clone();
            for (int x = 0; x < width; x++) {
                for (int y = 0; y < height; y++) {
                    float output1 = 0;
                    float output2 = 0;
                    for (int u = x - radius; u <= x + radius; u++) {
                        for (int v = y - radius; v <= y + radius; v++) {
                            int eu = Math.Abs(-Math.Abs(u - xrange) + xrange);
                            int ev = Math.Abs(-Math.Abs(v - yrange) + yrange); //mirror at edges
                            output1 += database[eu, ev].R * matrix[u - x + radius, v - y + radius];
                            output2 += database[eu, ev].R * altmatrix[u - x + radius, v - y + radius];
                        }
                    }
                    int op = (int)Math.Sqrt(output1 * output1 + output2 * output2);
                    if (op > 255) { op = 255; }
                    image[x, y] = Color.FromArgb(op, op, op);
                }
            }
        }

        private void FlattenHist(Color[,] image) {
            int width = InputImage.Size.Width;
            int height = InputImage.Size.Height;
            int[] hist = CalcHistCapital(image);
            for (int x = 0; x < InputImage.Size.Width; x++) {
                for (int y = 0; y < InputImage.Size.Height; y++) {
                    int pixelColor = image[x, y].R;
                    int newColor = (int)(hist[pixelColor] * 255 / ((float)width * height));
                    image[x, y] = Color.FromArgb(newColor, newColor, newColor);
                }
            }
        }

        private void Laplacian(Color[,] image) {

            float[,] matrix = LaplacianKernel();
            int size = matrix.GetLength(0);
            int radius = size / 2;
            int width = InputImage.Size.Width;
            int height = InputImage.Size.Height;
            int[,] tempOutput = new int[width, height];
            int xrange = width - 1;
            int yrange = height - 1;
            int minimum = 0;
            int maximum = 0;
            Color[,] database = (Color[,])image.Clone();
            for (int x = 0; x < width; x++) {
                for (int y = 0; y < height; y++) {
                    float output = 0;
                    for (int u = x - radius; u <= x + radius; u++) {
                        for (int v = y - radius; v <= y + radius; v++) {
                            int eu = Math.Abs(-Math.Abs(u - xrange) + xrange);
                            int ev = Math.Abs(-Math.Abs(v - yrange) + yrange); //mirror at edges
                            output += database[eu, ev].R * matrix[u - x + radius, v - y + radius];
                        }
                    }
                    int op = (int)output;
                    tempOutput[x, y] = op;
                    if (op < minimum) {
                        minimum = op;
                    }
                    if (op > maximum) {
                        maximum = op;
                    }
                }
            }
            for (int x = 0; x < width; x++) {
                for (int y = 0; y < height; y++) {
                    int op = tempOutput[x, y];
                    float multiply;
                    if (minimum == 0 && maximum == 0) multiply = 1;
                    else multiply = Math.Min(-127 / (float)minimum, 128 / (float)maximum);
                    op = 127 + (int)(op * multiply);
                    image[x, y] = Color.FromArgb(op, op, op);
                }
            }
            Contrast(image);
        }

        private void SharpenEdges(Color[,] image, int weight = 1) {

            float[,] matrix = LaplacianKernel();
            int size = matrix.GetLength(0);
            int radius = size / 2;
            int width = InputImage.Size.Width;
            int height = InputImage.Size.Height;
            int[,] tempOutput = new int[width, height];
            int xrange = width - 1;
            int yrange = height - 1;
            int minimum = 0;
            int maximum = 0;
            Color[,] database = (Color[,])image.Clone();
            for (int x = 0; x < width; x++) {
                for (int y = 0; y < height; y++) {
                    float output = 0;
                    for (int u = x - radius; u <= x + radius; u++) {
                        for (int v = y - radius; v <= y + radius; v++) {
                            int eu = Math.Abs(-Math.Abs(u - xrange) + xrange);
                            int ev = Math.Abs(-Math.Abs(v - yrange) + yrange); //mirror at edges
                            output += database[eu, ev].R * matrix[u - x + radius, v - y + radius];
                        }
                    }
                    int op = (int)output;
                    tempOutput[x, y] = op;
                    if (op < minimum) {
                        minimum = op;
                    }
                    if (op > maximum) {
                        maximum = op;
                    }
                }
            }
            for (int x = 0; x < width; x++) {
                for (int y = 0; y < height; y++) {
                    int op = tempOutput[x, y];
                    op = image[x, y].R - weight * op;
                    if (op > 255) {
                        op = 255;
                    }
                    if (op < 0) {
                        op = 0;
                    }
                    image[x, y] = Color.FromArgb(op, op, op);
                }
            }
        }

        private void Median(Color[,] image, int size) {
            int radius = size / 2;
            int width = InputImage.Size.Width;
            int height = InputImage.Size.Height;
            int xrange = width - 1;
            int yrange = height - 1;
            Color[,] database = (Color[,])image.Clone();
            for (int x = 0; x < width; x++) {
                for (int y = 0; y < height; y++) {
                    int[] output = new int[size * size];
                    for (int u = x - radius; u <= x + radius; u++) {
                        for (int v = y - radius; v <= y + radius; v++) {
                            int eu = Math.Abs(-Math.Abs(u - xrange) + xrange);
                            int ev = Math.Abs(-Math.Abs(v - yrange) + yrange); //mirror at edges
                            output[u - x + radius + (v - y + radius) * size] = image[eu, ev].R;
                        }
                    }
                    Array.Sort(output);
                    int op = output[output.Length / 2];
                    image[x, y] = Color.FromArgb(op, op, op);
                }
            }
        }

        private void Threshold(Color[,] image, int threshold) {
            for (int x = 0; x < InputImage.Size.Width; x++) {
                for (int y = 0; y < InputImage.Size.Height; y++) {
                    Color pixelColor = image[x, y];
                    if (pixelColor.R < threshold) image[x, y] = Color.FromArgb(0, 0, 0);
                    else image[x, y] = Color.FromArgb(255, 255, 255);
                }
            }
        }

        // Kernel functions

        private float[,] UniformKernel(int size) {
            if (size % 2 < 1) throw new Exception("Kernel size not an odd number.");
            float[,] kernel = new float[size, size];
            for (int x = 0; x < size; x++) {
                for (int y = 0; y < size; y++) {
                    kernel[x, y] = 1 / (float)(size * size);
                }
            }
            return kernel;
        }

        private float[,] GaussianKernel(int size, float sigma) {
            if (size % 2 < 1) throw new Exception("Kernel size not an odd number.");
            float[,] kernel = new float[size, size];
            float sum = 0;
            for (int x = 0; x < size; x++) {
                for (int y = 0; y < size; y++) {
                    float dist = (float)Math.Sqrt((x - (size / 2)) * (x - (size / 2)) + (y - (size / 2)) * (y - (size / 2)));
                    float val = NormalPDF(sigma, dist);
                    kernel[x, y] = val;
                    sum += val;
                }
            }
            for (int x = 0; x < size; x++) {
                for (int y = 0; y < size; y++) {
                    kernel[x, y] /= sum;
                }
            }
            return kernel;
        }

        private float[,] LaplacianKernel() {
            return new float[3, 3] { { 0, 1, 0 }, { 1, -4, 1 }, { 0, 1, 0 } };
        }

        private float[,] Sobel1() {
            return new float[3, 3] { { 1, 1, 1 }, { 0, 0, 0 }, { -1, -1, -1 } };
        }

        private float[,] Sobel2() {
            return new float[3, 3] { { 1, 0, -1 }, { 1, 0, -1 }, { 1, 0, -1 } };
        }

        // Other supportive functions

        private float NormalPDF(float sigma, float x) {
            return (float)(1 / (sigma * Math.Sqrt(2 * Math.PI)) * Math.Exp(-(x * x) / (2 * sigma * sigma)));
        }

        private int[] CalcHist(Color[,] image) {
            int[] hist = new int[256];
            for (int x = 0; x < InputImage.Size.Width; x++) {
                for (int y = 0; y < InputImage.Size.Height; y++) {
                    int pixelColor = image[x, y].R;
                    hist[pixelColor]++;
                }
            }
            return hist;
        }

        private int[] CalcHistCapital(Color[,] image) {
            int[] hist = CalcHist(image);
            int[] newHist = new int[256];
            int total = 0;
            for (int i = 0; i < 256; i++) {
                total += hist[i];
                newHist[i] = total;
            }
            return newHist;
        }

        //==============================================================================================

    }
}
