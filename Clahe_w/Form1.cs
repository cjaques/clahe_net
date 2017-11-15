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
        // Load image located in solution directory
        String startupPath = Directory.GetParent(Directory.GetCurrentDirectory()).Parent.FullName;
        String filename = @"\spider.bmp";
        Bitmap im1,im2;
        public Form1()
        {
            InitializeComponent();
            // load image here **BAD PLACE TO DO IT - s'en fout**
            im1 = new Bitmap(startupPath+filename);
            pictureBox1.Image = im1;
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            // read textboxes
            int nbrBinx = Int32.Parse(textBox1.Text);
            int nbrBinY = Int32.Parse(textBox2.Text);
            int grayLevels = Int32.Parse(textBox3.Text);
            float slope = (float)Double.Parse(textBox4.Text);
            Clahe imClahe = new Clahe(nbrBinx, nbrBinY, grayLevels, slope, im1);
            im2 = imClahe.Process();
            pictureBox2.Image = im2;
        }
    }
}
