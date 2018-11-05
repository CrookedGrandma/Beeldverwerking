using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace INFOIBV {
    public partial class INFOIBV : Form {
        private Bitmap InputImage, OutputImage;
        private Color[,] Image, ImageOut;

        public INFOIBV() {
            InitializeComponent();
        }

        private void LoadImageButton_Click(object sender, EventArgs e) {
            if (openImageDialog.ShowDialog() == DialogResult.OK) {                  // Open File Dialog
                string file = openImageDialog.FileName;                             // Get the file name
                imageFileName.Text = file;                                          // Show file name
                if (InputImage != null) InputImage.Dispose();                       // Reset image
                InputImage = new Bitmap(file);                                      // Create new Bitmap from file
                if (InputImage.Size.Height <= 0 || InputImage.Size.Width <= 0 ||
                    InputImage.Size.Height > 512 || InputImage.Size.Width > 512)    // Dimension check
                    MessageBox.Show("Error in image dimensions (have to be > 0 and <= 512)");
                else {
                    pictureBox1.Image = (Image)InputImage;                          // Display input image
                    LoadImage();
                }
                
            }
        }

        private void LoadImage() {
            Image = new Color[InputImage.Size.Width, InputImage.Size.Height];
            // Copy input Bitmap to array            
            for (int x = 0; x < InputImage.Size.Width; x++) {
                for (int y = 0; y < InputImage.Size.Height; y++) {
                    Image[x, y] = InputImage.GetPixel(x, y);
                }
            }
        }

        private void applyButton_Click(object sender, EventArgs e) {
            LoadImage();
            if (InputImage == null) return;                                             // Get out if no input image
            if (OutputImage != null) OutputImage.Dispose();                             // Reset output image
            OutputImage = new Bitmap(InputImage.Size.Width, InputImage.Size.Height);    // Create new output image
            ImageOut = (Color[,])Image.Clone();

            // Copy input Bitmap to array            
            for (int x = 0; x < InputImage.Size.Width; x++) {
                for (int y = 0; y < InputImage.Size.Height; y++) {
                    Image[x, y] = InputImage.GetPixel(x, y);                            // Set pixel color in array at (x,y)
                }
            }

            //==========================================================================================
            // TODO: include here your own code
            

            //==========================================================================================

            // Copy array to output Bitmap
            for (int x = 0; x < InputImage.Size.Width; x++) {
                for (int y = 0; y < InputImage.Size.Height; y++) {
                    OutputImage.SetPixel(x, y, ImageOut[x, y]);             // Set the pixel color at coordinate (x,y)
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

        private void Negative() {
            for (int x = 0; x < InputImage.Size.Width; x++) {
                for (int y = 0; y < InputImage.Size.Height; y++) {
                    Color pixelColor = Image[x, y];
                    Color updatedColor = Color.FromArgb(255 - pixelColor.R, 255 - pixelColor.G, 255 - pixelColor.B);
                    ImageOut[x, y] = updatedColor;
                }
            }
            RefreshImage();
        }

        private void Grayscale() {
            for (int x = 0; x < InputImage.Size.Width; x++) {
                for (int y = 0; y < InputImage.Size.Height; y++) {
                    Color pixelColor = Image[x, y];
                    int gray = (pixelColor.R + pixelColor.G + pixelColor.B) / 3;
                    Color updatedColor = Color.FromArgb(gray, gray, gray);
                    ImageOut[x, y] = updatedColor;
                }
            }
            RefreshImage();
        }

        private void Contrast() {
            int loR = 256, hiR = 0, loG = 256, hiG = 0, loB = 256, hiB = 0;
            for (int x = 0; x < InputImage.Size.Width; x++) {
                for (int y = 0; y < InputImage.Size.Height; y++) {
                    Color pixelColor = Image[x, y];
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
                    Color pixelColor = Image[x, y];
                    int r, g, b;
                    if (hiR - loR > 0) r = (int)((pixelColor.R - loR) * rMult);
                    else r = pixelColor.R;
                    if (hiG - loG > 0) g = (int)((pixelColor.G - loG) * gMult);
                    else g = pixelColor.G;
                    if (hiB - loB > 0) b = (int)((pixelColor.B - loB) * bMult);
                    else b = pixelColor.B;
                    Color updatedColor = Color.FromArgb(r, g, b);
                    ImageOut[x, y] = updatedColor;
                }
            }
        }

        private void Linear(float[,] matrix) {
            int size = matrix.GetLength(0);
            int radius = size / 2;
            int width = InputImage.Size.Width;
            int height = InputImage.Size.Height;
            int xrange = width - 1;
            int yrange = height - 1;
            Color[,] database = (Color[,])Image.Clone();
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
                    ImageOut[x, y] = Color.FromArgb(op, op, op);
                }
            }
        }

        private void Edges() {

            float[,] matrix = PrewittHorizontal();
            float[,] altmatrix = PrewittVertical();
            int size = matrix.GetLength(0);
            int radius = size / 2;
            int width = InputImage.Size.Width;
            int height = InputImage.Size.Height;
            int[,] tempOutput = new int[width, height];
            int xrange = width - 1;
            int yrange = height - 1;
            for (int x = 0; x < width; x++) {
                for (int y = 0; y < height; y++) {
                    float output1 = 0;
                    float output2 = 0;
                    for (int u = x - radius; u <= x + radius; u++) {
                        for (int v = y - radius; v <= y + radius; v++) {
                            int eu = Math.Abs(-Math.Abs(u - xrange) + xrange);
                            int ev = Math.Abs(-Math.Abs(v - yrange) + yrange); //mirror at edges
                            output1 += Image[eu, ev].R * matrix[u - x + radius, v - y + radius];
                            output2 += Image[eu, ev].R * altmatrix[u - x + radius, v - y + radius];
                        }
                    }
                    int op = (int)Math.Sqrt(output1 * output1 + output2 * output2);
                    if (op > 255) { op = 255; }
                    ImageOut[x, y] = Color.FromArgb(op, op, op);
                }
            }
        }

        private void Laplacian() {
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
            for (int x = 0; x < width; x++) {
                for (int y = 0; y < height; y++) {
                    float output = 0;
                    for (int u = x - radius; u <= x + radius; u++) {
                        for (int v = y - radius; v <= y + radius; v++) {
                            int eu = Math.Abs(-Math.Abs(u - xrange) + xrange);
                            int ev = Math.Abs(-Math.Abs(v - yrange) + yrange); //mirror at edges
                            output += Image[eu, ev].R * matrix[u - x + radius, v - y + radius];
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
                    ImageOut[x, y] = Color.FromArgb(op, op, op);
                }
            }
            Contrast();
        }

        private void SharpenEdges(int weight = 1) {

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
            for (int x = 0; x < width; x++) {
                for (int y = 0; y < height; y++) {
                    float output = 0;
                    for (int u = x - radius; u <= x + radius; u++) {
                        for (int v = y - radius; v <= y + radius; v++) {
                            int eu = Math.Abs(-Math.Abs(u - xrange) + xrange);
                            int ev = Math.Abs(-Math.Abs(v - yrange) + yrange); //mirror at edges
                            output += Image[eu, ev].R * matrix[u - x + radius, v - y + radius];
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
                    op = Image[x, y].R - weight * op;
                    if (op > 255) {
                        op = 255;
                    }
                    if (op < 0) {
                        op = 0;
                    }
                    ImageOut[x, y] = Color.FromArgb(op, op, op);
                }
            }
        }

        private void Median(int size) {
            int radius = size / 2;
            int width = InputImage.Size.Width;
            int height = InputImage.Size.Height;
            int xrange = width - 1;
            int yrange = height - 1;
            for (int x = 0; x < width; x++) {
                for (int y = 0; y < height; y++) {
                    int[] output = new int[size * size];
                    for (int u = x - radius; u <= x + radius; u++) {
                        for (int v = y - radius; v <= y + radius; v++) {
                            int eu = Math.Abs(-Math.Abs(u - xrange) + xrange);
                            int ev = Math.Abs(-Math.Abs(v - yrange) + yrange); //mirror at edges
                            output[u - x + radius + (v - y + radius) * size] = Image[eu, ev].R;
                        }
                    }
                    Array.Sort(output);
                    int op = output[output.Length / 2];
                    ImageOut[x, y] = Color.FromArgb(op, op, op);
                }
            }
        }

        private void Threshold(int threshold) {
            for (int x = 0; x < InputImage.Size.Width; x++) {
                for (int y = 0; y < InputImage.Size.Height; y++) {
                    Color pixelColor = Image[x, y];
                    if (pixelColor.R < threshold) Image[x, y] = Color.FromArgb(0, 0, 0);
                    else ImageOut[x, y] = Color.FromArgb(255, 255, 255);
                }
            }
        }

        private void Erosion(SEP[] structure) {
            if (IsBinary()) {
                MakeWhite();
                SEP[] mirror = Mirror(structure);
                for (int x = 0; x < InputImage.Size.Width; x++) {
                    for (int y = 0; y < InputImage.Size.Height; y++) {
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
                for (int x = 0; x < InputImage.Size.Width; x++) {
                    for (int y = 0; y < InputImage.Size.Height; y++) {
                        List<int> outs = new List<int>(structure.Length);
                        foreach (SEP sep in structure) {
                            int newX = x + sep.C.X;
                            int newY = y + sep.C.Y;
                            if (ClampX(newX) == newX && ClampY(newY) == newY) {
                                outs.Add(Image[newX, newY].R + sep.V);
                            }
                        }
                        int output = ClampCol(outs.Min());
                        ImageOut[x, y] = Color.FromArgb(output, output, output);
                    }
                }
            }
            RefreshImage();
        }

        private void Dilation(SEP[] structure) {
            if (IsBinary()) {
                MakeBlack();
                SEP[] mirror = Mirror(structure);
                for (int x = 0; x < InputImage.Size.Width; x++) {
                    for (int y = 0; y < InputImage.Size.Height; y++) {
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
                for (int x = 0; x < InputImage.Size.Width; x++) {
                    for (int y = 0; y < InputImage.Size.Height; y++) {
                        List<int> outs = new List<int>(structure.Length);
                        foreach (SEP sep in structure) {
                            int newX = x + sep.C.X;
                            int newY = y + sep.C.Y;
                            if (ClampX(newX) == newX && ClampY(newY) == newY) {
                                outs.Add(Image[newX, newY].R - sep.V);
                            }
                        }
                        int output = ClampCol(outs.Max());
                        ImageOut[x, y] = Color.FromArgb(output, output, output);
                    }
                }
            }
            RefreshImage();
        }

        private void ImgOpening(SEP[] structure) {
            Erosion(structure);
            Dilation(structure);
        }

        private void ImgClosing(SEP[] structure) {
            Dilation(structure);
            Erosion(structure);
        }

        private int ValueCount(bool show) {
            List<int> values = new List<int>(256);
            for (int x = 0; x < InputImage.Size.Width; x++) {
                for (int y = 0; y < InputImage.Size.Height; y++) {
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
            for (int y = 0; y < InputImage.Size.Height; y++) {
                for (int x = 0; x < InputImage.Size.Width; x++) {
                    if (Image[x, y].R > 0) {
                        current = new Coord(x, y);
                        List<Coord> history = new List<Coord>();
                        while (!(history.Count > 2 && (history[0] == last && history[1] == current))) { // Check end condition
                            history.Add(current);
                            int value = 0;
                            Coord next = last.Clone();
                            int counter = 0;
                            while (value == 0) {
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

        private List<Coord> Fourier(List<Coord> boundary, int amount_of_descriptors = -1, int sample_density = 1, float stepsize = 0.5f) {
            stepsize /= sample_density;
            List<Coord> bound = new List<Coord>();
            for (int i = 0; i < boundary.Count; i++) {
                if (i % sample_density == 0) bound.Add(boundary[i]);
            }
            int N = bound.Count;
            if (amount_of_descriptors < 0) amount_of_descriptors = N;

            // Local methods for Fourier functionality
            Complex etopowerix(float x) {
                return new Complex((float)Math.Cos(x), (float)Math.Sin(x));
            }
            Complex Z(float k) {
                Complex accum = new Complex();
                for (int m = 0; m < bound.Count; m++) {
                    accum += new Complex(bound[m].X, bound[m].Y) * etopowerix((-2f * (float)Math.PI * m * k) / N);
                }
                return accum * new Complex(1 / (float)N, 0);
            }

            // Calculate Zs
            float max = 0;
            Complex newest = new Complex();
            List<Complex> Zs = new List<Complex>();
            for (int k = -amount_of_descriptors / 2; k <= amount_of_descriptors / 2; k++) {
                newest = Z(k);
                float length = (float)Math.Sqrt(newest.I * newest.I + newest.R * newest.R);
                if (max < length) max = length;
                Zs.Add(newest);
            }

            // Create a graph in the middle pictureBox
            int width = pictureBox2.Width;
            int height = pictureBox2.Height;
            Bitmap GraphImage = new Bitmap(width, height);
            for (int x = 0; x < width; x++) {
                for (int y = 0; y < height; y++) {
                    GraphImage.SetPixel(x, y, Color.White);
                }
            }
            for (int z = 0; z < Zs.Count; z++) {
                int x = (int)((width / (float)Zs.Count) * (z + 0.5));
                int ymod = -(int)(((float)Math.Sqrt(Zs[z].I * Zs[z].I + Zs[z].R * Zs[z].R) / max) * ((height / 2) - 1));
                for (int y = ymod; y != 0; y += 1) {
                    GraphImage.SetPixel(x, y + height / 2, Black());
                }
                int yr = -(int)((Zs[z].R / max) * ((height / 2) - 1));
                int dirr = -Math.Sign(yr);
                for (int y = yr; y != 0; y += dirr) {
                    GraphImage.SetPixel(x, y + height / 2, Color.Blue);
                }
                int yi = -(int)(((Zs[z].I) / max) * ((height / 2) - 1));
                int diri = -Math.Sign(yi);
                for (int y = yi; y != 0; y += diri) {
                    Color temp = GraphImage.GetPixel(x, y + height / 2);
                    temp = Color.FromArgb(255, 0, 255 - temp.R);
                    GraphImage.SetPixel(x, y + height / 2, temp);
                }
                Color temp2 = GraphImage.GetPixel(x, 0 / 2);
                temp2 = Color.FromArgb(255, 0, 255 - temp2.R);
                GraphImage.SetPixel(x, height / 2, temp2);
            }
            pictureBox2.Image = GraphImage;

            //create reconstruction
            List<Coord> retlist = new List<Coord>();
            for (float m = 0f; m < N; m += stepsize) {
                Complex accum = new Complex();
                for (int k = -(amount_of_descriptors / 2); k <= (amount_of_descriptors / 2); k++) {
                    accum += Zs[k + (amount_of_descriptors / 2)] * etopowerix(2f * (float)Math.PI * m * k / N);
                }
                retlist.Add(new Coord(ClampX((int)accum.R), ClampY((int)accum.I)));
            }
            return retlist;
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

        private float[,] PrewittHorizontal() {
            return new float[3, 3] { { 1, 1, 1 }, { 0, 0, 0 }, { -1, -1, -1 } };
        }

        private float[,] PrewittVertical() {
            return new float[3, 3] { { 1, 0, -1 }, { 1, 0, -1 }, { 1, 0, -1 } };
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

        private SEP[] SquareStructElem(int size) {
            if (size % 2 < 1) throw new Exception("Structuring element size not an odd number.");
            List<SEP> seps = new List<SEP>();
            int half = size / 2;
            for (int x = -half; x < half; x++) {
                for (int y = -half; y < half; y++) {
                    seps.Add(new SEP(x, y, 0));
                }
            }
            SEP[] sepsAR = seps.ToArray();
            return sepsAR;
        }

        // Other supportive functions

        private float NormalPDF(float sigma, float x) {
            return (float)(1 / (sigma * Math.Sqrt(2 * Math.PI)) * Math.Exp(-(x * x) / (2 * sigma * sigma)));
        }

        private bool IsBinary() {
            return ValueCount(false) <= 2;
        }

        private int ClampX(int x) {
            if (x < 0) return 0;
            else if (x >= InputImage.Width) return InputImage.Width - 1;
            else return x;
        }

        private int ClampY(int y) {
            if (y < 0) return 0;
            else if (y >= InputImage.Height) return InputImage.Height - 1;
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
            for (int x = 0; x < InputImage.Size.Width; x++) {
                for (int y = 0; y < InputImage.Size.Height; y++) {
                    ImageOut[x, y] = Black();
                }
            }
        }

        private void MakeWhite() {
            for (int x = 0; x < InputImage.Size.Width; x++) {
                for (int y = 0; y < InputImage.Size.Height; y++) {
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

        private void PaintList(List<Coord> list, bool reset = true) {
            if (reset) MakeWhite();
            foreach (Coord c in list) {
                ImageOut[c.X, c.Y] = Black();
            }
            if (reset) RefreshImage();
        }

        private void PaintList(List<Coord> list, Color color, bool reset = true) {
            if (reset) MakeWhite();
            foreach (Coord c in list) {
                ImageOut[c.X, c.Y] = color;
            }
            if (reset) RefreshImage();
        }

        private void RefreshImage() {
            Image = (Color[,])ImageOut.Clone();
        }

        // Structs

        /// <summary>
        /// A simple container for a complex number in the form of a + bi
        /// </summary>
        private class Complex {
            public Complex() {
                R = 0;
                I = 0;
            }
            public Complex(float r, float i) {
                this.R = r;
                this.I = i;
            }
            public float R { get; set; }
            public float I { get; set; }
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

        /// <summary>
        /// A struct containing a list of descriptor amounts
        /// </summary>
        private struct DescAmounts {
            public DescAmounts(int max) {
                List<int> amounts = new List<int>();
                amounts.Add(1);
                int a = 2;
                while (a < max) {
                    amounts.Add(a);
                    a *= a;
                }
                amounts.Add(max);
                List = amounts.ToArray();
            }
            public int[] List { get; set; }
        }

        //==============================================================================================

    }
}
