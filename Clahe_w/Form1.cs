using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Clahe_w
{
    public partial class Form1 : Form
    {
        // BAD PRACTICE TO PUT THIS HERE... s'en fout
        String filename = @"C:\Users\chris\Documents\misc\spider_small.bmp";
        Bitmap im1,im2;
        public Form1()
        {
            InitializeComponent();
            // load image here **BAD PLACE TO DO IT - s'en fout**
            im1 = new Bitmap(filename);
            pictureBox1.Image = im1;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Clahe imClahe = new Clahe(4, 4, 256, 2.3F, im1);
            im2 = imClahe.Process();
            pictureBox2.Image = im2;
        }
    }
}
