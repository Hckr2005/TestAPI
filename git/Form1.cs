using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace testfacereg2
{
    public partial class Form1 : Form
    {
        private PictureBox pictureBox;
        private Button btnChoose;
        private Button btnSend;
        private string imagePath;

        public Form1()
        {
            Initialize_Component();
        }

        private void Initialize_Component()
        {
            this.pictureBox = new PictureBox();
            this.btnChoose = new Button();
            this.btnSend = new Button();
            this.SuspendLayout();

            // pictureBox
            this.pictureBox.BorderStyle = BorderStyle.FixedSingle;
            this.pictureBox.Location = new Point(12, 12);
            this.pictureBox.Size = new Size(360, 240);
            this.pictureBox.SizeMode = PictureBoxSizeMode.Zoom;

            // btnChoose
            this.btnChoose.Location = new Point(12, 270);
            this.btnChoose.Size = new Size(75, 23);
            this.btnChoose.Text = "Chọn ảnh";
            this.btnChoose.Click += new EventHandler(this.BtnChoose_Click);

            // btnSend
            this.btnSend.Location = new Point(297, 270);
            this.btnSend.Size = new Size(75, 23);
            this.btnSend.Text = "Gửi";
            this.btnSend.Click += new EventHandler(this.BtnSend_Click);

            // Form1
            this.ClientSize = new Size(384, 311);
            this.Controls.Add(this.pictureBox);
            this.Controls.Add(this.btnChoose);
            this.Controls.Add(this.btnSend);
            this.Name = "Form1";
            this.Text = "Face Recognition";
            this.ResumeLayout(false);
        }

        private void BtnChoose_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Image Files|*.jpg;*.jpeg;*.png;*.bmp";
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                imagePath = openFileDialog.FileName;
                pictureBox.Image = Image.FromFile(imagePath);
            }
        }

        private async void BtnSend_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(imagePath))
            {
                MessageBox.Show("Vui lòng chọn một ảnh trước.");
                return;
            }

            try
            {
                using (HttpClient client = new HttpClient())
                {
                    byte[] imageBytes = File.ReadAllBytes(imagePath);
                    ByteArrayContent content = new ByteArrayContent(imageBytes);
                    HttpResponseMessage response = await client.PostAsync("http://127.0.0.1:5000/recognize", content);

                    if (response.IsSuccessStatusCode)
                    {
                        byte[] responseBytes = await response.Content.ReadAsByteArrayAsync();
                        using (MemoryStream ms = new MemoryStream(responseBytes))
                        {
                            pictureBox.Image = Image.FromStream(ms);
                        }
                    }
                    else
                    {
                        MessageBox.Show("Lỗi khi nhận diện khuôn mặt: " + response.ReasonPhrase);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Đã xảy ra lỗi: " + ex.Message);
            }
        }

        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Form1());
        }
    }
}
