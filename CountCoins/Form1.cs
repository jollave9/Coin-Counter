using System;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ImageProcess2;
using AForge.Imaging;
using AForge.Imaging.Filters;

namespace CountCoins
{
    public partial class Form1 : Form
    {
        Bitmap loaded, processed;

        public Form1()
        {
            InitializeComponent();
        }

        private void openFileDialog1_FileOk(object sender, CancelEventArgs e)
        {
            loaded = new Bitmap(openFileDialog1.FileName);
            pictureBox1.Image = loaded;

            processed = new Bitmap(openFileDialog1.FileName);

            GrayscaleBT709 g = new GrayscaleBT709();
            processed = g.Apply((Bitmap)processed);

            Threshold t = new Threshold(128);
            processed = t.Apply((Bitmap)processed);

            BlobsFiltering b = new BlobsFiltering();
            b.MinWidth = 147;
            b.MinHeight = 147;
            b.CoupledSizeFiltering = true;
            processed = b.Apply((Bitmap)processed);

            HomogenityEdgeDetector h = new HomogenityEdgeDetector();
            processed = h.Apply((Bitmap)processed);

            pictureBox2.Image = processed;

            BlobCounterBase bc = new BlobCounter();
            bc.ProcessImage(processed);
            Blob[] blobs = bc.GetObjectsInformation();


            //5 cents 7 pieces 700-800 pixels
            //10 cents 11 pieces 801-1000 pixels
            //25 cents 28 pieces 1001-1200 pixels
            //1 peso 13 pieces 1200-1500 pixels
            //5 peso 5 pieces 1501-1700 pixels

            int paybcents = 0, tencents = 0, twenypaybcents = 0, wanpeso = 0, paybpeso = 0;
            foreach (Blob blob in blobs)
            {
                if (blob.Area >= 700 && blob.Area <= 800)
                    paybcents++;
                else if (blob.Area >= 801 && blob.Area <= 1000)
                    tencents++;
                else if (blob.Area >= 1001 && blob.Area <= 1200)
                    twenypaybcents++;
                else if (blob.Area >= 1200 && blob.Area <= 1500)
                    wanpeso++;
                else if (blob.Area >= 1501 && blob.Area <= 1700)
                    paybpeso++;

            }
            double paybcentsbalyo = paybcents * 0.05;
            double tencentsbalyo = tencents * 0.1;
            double twenypaybcentsbalyo = twenypaybcents * 0.25;
            double wanpesobalyo = wanpeso * 1;
            double paybpesobalyo = paybpeso * 5;

            double total = paybcentsbalyo + tencentsbalyo + twenypaybcentsbalyo + wanpesobalyo + paybpesobalyo;
            MessageBox.Show("5 cents: " + paybcents + " pieces value: " + paybcentsbalyo + "\n10 cents: " + tencents + " pieces value: " + tencentsbalyo + "\n25 cents: " + twenypaybcents + " pieces value:" + twenypaybcentsbalyo + "\n1 peso: " + wanpeso + " pieces value:" + wanpesobalyo + "\n5 peso: " + paybpeso + " pieces value:" + paybpesobalyo + "\nTotal Value: " + total);

        }


        private void loadImageToolStripMenuItem_Click(object sender, EventArgs e)
        {
            openFileDialog1.ShowDialog();

        }
    }
}
