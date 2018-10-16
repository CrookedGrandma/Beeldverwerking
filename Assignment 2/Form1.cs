﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
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
            ImageOut = new Color[InputImage1.Size.Width, InputImage1.Size.Height];

            //==========================================================================================

            // (1) Erosion/Dilation
            //Erosion(CircStructElem(5));
            //Dilation(CircStructElem(5));

            // (2) Opening/Closing
            //ImgOpening(CircStructElem(5));
            //ImgClosing(CircStructElem(5));

            // (3) Complement
            //Complement();

            // (4) MIN/MAX
            //Min();
            //Max();

            // (5) Value counting
            //ValueCount(true);

            // (6) Boundary trace
            //PaintList(Fourier(Boundary(), 50));
            PaintList(Boundary());

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

        private List<Coord> Fourier(List<Coord> boundary, int amount_of_descriptors = -1, int sample_density = 1, double stepsize = 1.0) {
            int N = boundary.Count / sample_density;
            if (amount_of_descriptors < 0) amount_of_descriptors = N;
            Complex etopowerix(double x) {
                return new Complex(Math.Cos(x), Math.Sin(x));
            }
            Complex Z(double k) {
                Complex accum = new Complex();
                for (int m = 0; m < N; m++) {
                    accum += new Complex(boundary[m].X, boundary[m].Y) * etopowerix((-2.0 * Math.PI * m * k) / (double)N);
                }
                return accum * new Complex(1 / (float)N, 0);
            }

            //calc Zs
            double max = 0;
            Complex newest = new Complex();
            List<Complex> Zs = new List<Complex>();
            for (int k = -(amount_of_descriptors / 2); k <= (amount_of_descriptors / 2); k++) {
                newest = Z(k);
                if (max < newest.R || newest.R < -max) {
                    max = Math.Abs(newest.R);
                }
                if (max < newest.I || newest.I < -max) {
                    max = Math.Abs(newest.I);
                }
                Zs.Add(newest);
            }

            //create graph
            int width = pictureBox2.Width;
            int height = pictureBox2.Height;
            Bitmap OutputImage2 = new Bitmap(width, height);
            for (int x = 0; x < width; x++) {
                for (int y = 0; y < height; y++) {
                    OutputImage2.SetPixel(x, y, Color.White);
                }
            }
            for (int z = 0; z < Zs.Count; z++) {
                int x = (int)((width / (double)(Zs.Count)) * (z + 0.5));
                int yr = (int)(((Zs[z].R) / max) * (double)((height / 2) - 1));
                int dirr = -Math.Sign(yr);
                for (int y = yr; y != 0; y += dirr) {
                    OutputImage2.SetPixel(x, y + height / 2, Color.Blue);
                }
                int yi = (int)(((Zs[z].I) / max) * (double)((height / 2) - 1));
                int diri = -Math.Sign(yi);
                for (int y = yi; y != 0; y += diri) {
                    Color temp = OutputImage2.GetPixel(x, y + height / 2);
                    temp = Color.FromArgb(255, 0, 255 - temp.R);
                    OutputImage2.SetPixel(x, y + height / 2, temp);
                }
                Color temp2 = OutputImage2.GetPixel(x, 0 / 2);
                temp2 = Color.FromArgb(255, 0, 255 - temp2.R);
                OutputImage2.SetPixel(x, height / 2, temp2);
            }
            pictureBox2.Image = (Image)OutputImage2;



            //create reconstruction
            List<Coord> retlist = new List<Coord>();
            for (double m = 0.0; m < N; m += stepsize) {
                Complex accum = new Complex();
                for (int k = -(amount_of_descriptors / 2); k <= (amount_of_descriptors / 2); k++) {
                    accum += Zs[k + (amount_of_descriptors / 2)] * etopowerix(2.0 * Math.PI * m * k / N);
                }
                retlist.Add(new Coord(ClampX((int)accum.R), ClampY((int)accum.I)));
            }
            return retlist;
        }



        private void Erosion(SEP[] structure) {
            if (IsBinary()) {
                MakeBlack();
                SEP[] mirror = Mirror(structure);
                for (int x = 0; x < InputImage1.Size.Width; x++) {
                    for (int y = 0; y < InputImage1.Size.Height; y++) {
                        foreach (SEP sep in mirror) {
                            int newX = x + sep.C.X;
                            int newY = y + sep.C.Y;
                            if (ClampX(newX) == newX && ClampY(newY) == newY) {
                                if (Image[newX, newY].R > 0) {
                                    ImageOut[x, y] = White();
                                    break;
                                }
                            }
                        }
                    }
                }
            }
            else {
                for (int x = 0; x < InputImage1.Size.Width; x++) {
                    for (int y = 0; y < InputImage1.Size.Height; y++) {
                        List<int> outs = new List<int>(structure.Length);
                        foreach (SEP sep in structure) {
                            int newX = x + sep.C.X;
                            int newY = y + sep.C.Y;
                            if (ClampX(newX) == newX && ClampY(newY) == newY) {
                                outs.Add(Image[newX, newY].R + sep.V);
                            }
                        }
                        int output = ClampCol(outs.Max());
                        ImageOut[x, y] = Color.FromArgb(output, output, output);
                    }
                }
            }
            RefreshImage();
            if (InputImage2 != null) {
                Min();
            }
        }

        private void Dilation(SEP[] structure) {
            if (IsBinary()) {
                MakeWhite();
                SEP[] mirror = Mirror(structure);
                for (int x = 0; x < InputImage1.Size.Width; x++) {
                    for (int y = 0; y < InputImage1.Size.Height; y++) {
                        foreach (SEP sep in mirror) {
                            int newX = x + sep.C.X;
                            int newY = y + sep.C.Y;
                            if (ClampX(newX) == newX && ClampY(newY) == newY) {
                                if (Image[newX, newY].R == 0) {
                                    ImageOut[x, y] = Black();
                                    break;
                                }
                            }
                        }
                    }
                }
            }
            else {
                for (int x = 0; x < InputImage1.Size.Width; x++) {
                    for (int y = 0; y < InputImage1.Size.Height; y++) {
                        List<int> outs = new List<int>(structure.Length);
                        foreach (SEP sep in structure) {
                            int newX = x + sep.C.X;
                            int newY = y + sep.C.Y;
                            if (ClampX(newX) == newX && ClampY(newY) == newY) {
                                outs.Add(Image[newX, newY].R - sep.V);
                            }
                        }
                        int output = ClampCol(outs.Min());
                        ImageOut[x, y] = Color.FromArgb(output, output, output);
                    }
                }
            }
            RefreshImage();
            if (InputImage2 != null) {
                Max();
            }
        }

        private void ImgOpening(SEP[] structure) {
            Erosion(structure);
            Dilation(structure);
        }

        private void ImgClosing(SEP[] structure) {
            Dilation(structure);
            Erosion(structure);
        }

        private void Complement() {
            for (int x = 0; x < InputImage1.Size.Width; x++) {
                for (int y = 0; y < InputImage1.Size.Height; y++) {
                    Color pixelColor = Image[x, y];
                    Color updatedColor = Color.FromArgb(255 - pixelColor.R, 255 - pixelColor.G, 255 - pixelColor.B);
                    ImageOut[x, y] = updatedColor;
                }
            }
            RefreshImage();
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
            RefreshImage();
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
            RefreshImage();
        }

        private int ValueCount(bool show) {
            List<int> values = new List<int>(256);
            for (int x = 0; x < InputImage1.Size.Width; x++) {
                for (int y = 0; y < InputImage1.Size.Height; y++) {
                    int pixelColor = Image[x, y].R;
                    if (!values.Contains(pixelColor)) values.Add(pixelColor);
                }
            }
            int count = values.Count;
            if (show) MessageBox.Show(count.ToString());
            return count;
        }

        private List<Coord> Boundary() {
            Coord current;
            Coord last = new Coord(0, 1);
            for (int y = 0; y < InputImage1.Size.Height; y++) {
                for (int x = 0; x < InputImage1.Size.Width; x++) {
                    if (Image[x, y].R == 0) {
                        current = new Coord(x, y);
                        List<Coord> history = new List<Coord>();
                        while (!(history.Count > 2 && (history[0] == last && history[1] == current))) { // Check end condition
                            history.Add(current);
                            int value = -1;
                            Coord next = last.Clone();
                            int counter = 0;
                            while (value != 0) {
                                counter++;
                                if (counter > 8) return history;
                                next = NextCoord(next, current);
                                if (ClampX(next.X) == next.X && ClampY(next.Y) == next.Y) value = Image[next.X, next.Y].R;
                            }
                            last = current.Clone();
                            current = next.Clone();
                        }
                        return history;
                    }
                    last = new Coord(x, y);
                }
                last = new Coord(0, y);
            }
            return new List<Coord>();
        }

        private void Threshold(int threshold) {
            for (int x = 0; x < InputImage1.Size.Width; x++) {
                for (int y = 0; y < InputImage1.Size.Height; y++) {
                    if (Image[x, y].R < threshold) ImageOut[x, y] = Black();
                    else ImageOut[x, y] = White();
                }
            }
            RefreshImage();
        }

        // Structuring element functions

        private SEP[] FourNeigh3x3zero() {
            return new SEP[5] { new SEP(0, -1, 0), new SEP(-1, 0, 0), new SEP(0, 0, 0), new SEP(1, 0, 0), new SEP(0, 1, 0) };
        }

        private SEP[] CircStructElem(int size) {
            if (size % 2 < 1) throw new Exception("Structuring element size not an odd number.");
            List<SEP> seps = new List<SEP>();
            float r = size / 2f;
            for (int x = -(int)r; x <= (int)r; x++) {
                for (int y = -(int)r; y <= (int)r; y++) {
                    if (x * x + y * y <= r * r) {
                        seps.Add(new SEP(x, y, 0));
                    }
                }
            }
            SEP[] sepsAR = seps.ToArray();
            return sepsAR;
        }

        // Helper functions

        private bool IsBinary() {
            return ValueCount(false) <= 2;
        }

        private int ClampX(int x) {
            if (x < 0) return 0;
            else if (x >= InputImage1.Width) return InputImage1.Width - 1;
            else return x;
        }

        private int ClampY(int y) {
            if (y < 0) return 0;
            else if (y >= InputImage1.Height) return InputImage1.Height - 1;
            else return y;
        }

        private int ClampCol(int c) {
            if (c < 0) return 0;
            else if (c > 255) return 255;
            else return c;
        }

        private Color White() {
            return Color.White;
        }

        private Color Black() {
            return Color.Black;
        }

        private void MakeBlack() {
            for (int x = 0; x < InputImage1.Size.Width; x++) {
                for (int y = 0; y < InputImage1.Size.Height; y++) {
                    ImageOut[x, y] = Black();
                }
            }
        }

        private void MakeWhite() {
            for (int x = 0; x < InputImage1.Size.Width; x++) {
                for (int y = 0; y < InputImage1.Size.Height; y++) {
                    ImageOut[x, y] = White();
                }
            }
        }

        private SEP[] Mirror(SEP[] structure) {
            SEP[] mirror = new SEP[structure.Length];
            for (int i = 0; i < structure.Length; i++) {
                mirror[i] = structure[i].Mirrored;
            }
            return mirror;
        }

        private Coord NextCoord(Coord current, Coord center) {
            Coord relative = current - center;
            int x = -relative.Y;
            int y = relative.X;
            if (relative.X == relative.Y) y = 0;
            if (relative.X == -relative.Y) x = 0;
            Coord vect = new Coord(x, y);
            return current + vect;
        }

        private void RefreshImage() {
            Image = (Color[,])ImageOut.Clone();
        }

        private void PaintList(List<Coord> list) {
            MakeWhite();
            foreach (Coord c in list) {
                ImageOut[c.X, c.Y] = Black();
            }
            RefreshImage();
        }

        // Structs

        /// <summary>
        /// A simple coordinate pair, containing an x and y value
        /// </summary>
        private class Coord {
            private int x, y;
            public Coord(int x, int y) {
                this.x = x;
                this.y = y;
            }
            public int X {
                get { return x; }
                set { x = value; }
            }
            public int Y {
                get { return y; }
                set { y = value; }
            }
            public Coord Clone() {
                return new Coord(x, y);
            }
            public static Coord operator +(Coord a, Coord b) {
                return new Coord(a.X + b.X, a.Y + b.Y);
            }
            public static Coord operator -(Coord a, Coord b) {
                return new Coord(a.X - b.X, a.Y - b.Y);
            }
            public static bool operator ==(Coord a, Coord b) {
                return a.X == b.X && a.Y == b.Y;
            }
            public static bool operator !=(Coord a, Coord b) {
                return !(a == b);
            }
            public override bool Equals(object obj) {
                return base.Equals(obj);
            }
            public override int GetHashCode() {
                return base.GetHashCode();
            }
            public override string ToString() {
                return "(" + x + ", " + y + ")";
            }
        }

        private class Complex {
            public Complex() {
                R = 0;
                I = 0;
            }
            public Complex(double r, double i) {
                this.R = r;
                this.I = i;
            }
            public double R { get; set; }
            public double I { get; set; }
            public override string ToString() {
                return R + " + " + I + "i";
            }
            public static Complex operator +(Complex a, Complex b) {
                return new Complex(a.R + b.R, a.I + b.I);
            }
            public static Complex operator -(Complex a, Complex b) {
                return new Complex(a.R - b.R, a.I - b.I);
            }
            public static Complex operator *(Complex a, Complex b) {
                return new Complex(a.R * b.R - a.I * b.I, a.R * b.I + a.I * b.R);
            }
        }

        /// <summary>
        /// A point in a structuring element, containing a coordinate and a value
        /// </summary>
        private struct SEP {
            public SEP(Coord c, int v) {
                this.C = c;
                this.V = v;
            }
            public SEP(int x, int y, int v) {
                this.C = new Coord(x, y);
                this.V = v;
            }
            public Coord C { get; set; }
            public int V { get; set; }
            public SEP Mirrored {
                get {
                    return new SEP(-C.X, -C.Y, V);
                }
            }
        }



        //==============================================================================================
    }
}
