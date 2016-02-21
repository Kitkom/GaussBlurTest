using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GaussBlurTest
{
    public partial class Form1 : Form
    {
        private Bitmap picture;

        public Form1()
        {
            InitializeComponent();
            picture = null;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (picture == null)
                return;

            Double sigma = Convert.ToDouble(sigmaTxtBox.Text);
            Int32 range = Convert.ToInt32(rangeTextBox.Text) + 1;

            //core algorithm
            //weight
            Double[,] weight = new Double[range, range];
            Double sum = 0;
            for (Int32 col = 0; col < range; ++col)
                for (Int32 row = 0; row < range; ++row)
                {
                    weight[col, row] = Math.Exp(-((Double)(col * col + row * row)/ 2 / sigma / sigma));
                    sum += weight[col, row];
                    if (col + row == 0)
                        continue;
                    if (col * row == 0)
                        sum += weight[col, row];
                    else
                        sum += 3 * weight[col, row];
                }
            for (Int32 col = 0; col < range; ++col)
                for (Int32 row = 0; row < range; ++row)
                    weight[col, row] /= sum;

            //pixel
            Int32 width = picture.Width;
            Int32 height = picture.Height;
            Bitmap newPicture = new Bitmap(width, height);
            for (Int32 col = 0; col < width; ++col)
                for (Int32 row = 0; row < height; ++row)
                {
                    Double alpha = 0;
                    Double red = 0;
                    Double green = 0;
                    Double blue = 0;
                    for (Int32 x = - range + 1; x < range; ++x)
                        for (Int32 y = - range + 1; y < range; ++y)
                        {
                            Double w = weight[Math.Abs(x), Math.Abs(y)];
                            if ((col + x >= width) || (col + x < 0) || (row + y >= height) || (row + y < 0))
                                continue;
                            Color loader = picture.GetPixel(col + x, row + y);
                            alpha += w * loader.A;
                            red += w * loader.R;
                            green += w * loader.G;
                            blue += w * loader.B;
                        }
                    newPicture.SetPixel(col, row, Color.FromArgb((int)alpha, (int)red, (int)green, (int)blue));
                }

            picture = newPicture;
            pictureBox1.Image = newPicture;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            openFileDialog1.ShowDialog();
            picture = (Bitmap)Image.FromFile(openFileDialog1.FileName);
            pictureBox1.Image = picture;
        }
    }
}
