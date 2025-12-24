using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Rebar;

namespace Steganography
{
    public partial class SteganographyForm : Form

    { 
        int counter = 0;
        System.Windows.Forms.ToolTip tt = new System.Windows.Forms.ToolTip();

        public SteganographyForm()
        {
            InitializeComponent();
        }

     
        private void ResetBotton2_Click(object sender, EventArgs e)
        {
            InputKey.Clear();
        }

        private void CopyButton1_Click(object sender, EventArgs e)
        {
            
            if (!string.IsNullOrEmpty(InputKey.Text))
            {
                Clipboard.SetText(InputKey.Text);
            }

            else
            {

                MessageBox.Show("There is no key to copy.", "error", MessageBoxButtons.OK, MessageBoxIcon.Error); //หน้าเตือนว่า               

            }


        }

        private void ResetBotton1_Click(object sender, EventArgs e)
        {
            InputPlaintext.Clear();
            namefile.Clear();

        }

        private void ResetBotton4_Click(object sender, EventArgs e)
        {
            InputPlaintext.Clear();
        }

        private void ResetBotton3_Click(object sender, EventArgs e)
        {
            InputCovertext.Clear();
        }

        private void InputKey_TextChanged(object sender, EventArgs e)
        {
            tt.SetToolTip(InputKey, "key ที่ใช้ในการเข้ารหัส"); //แสดงคำอธิบายเมื่อเอาเมาส์ไปชี้แต่ยังไม่ขึ้น!!!
        }

        private void PasteButton1_Click(object sender, EventArgs e)
        {
            if (Clipboard.ContainsText())
            {
                InputkeyDecry.Text = Clipboard.GetText();
            }
        }

        private void EncrytionButton1_Click(object sender, EventArgs e)//ปุ่มเข้ารหัส
        {
            if (string.IsNullOrEmpty(InputKey.Text)) // ตรวจสอบว่า InputKey ว่างหรือไม่
            {
                MessageBox.Show("Please enter your key", "Wraning", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return; // หยุดการทำงานหากไม่มีคีย์
            }

            byte[] x = Encoding.UTF8.GetBytes(InputPlaintext.Text); //รับข้อความจากinputplaintextมาเป็นเป็นbyte
            var key = new Rfc2898DeriveBytes(InputKey.Text, x, 10000);//เอาคีย์ที่ได้มา hash10,000รอบ
            //Rfc2898DeriveBytes = คลาสที่ใช้ในการสร้างคีย์เข้ารหัสโดยใช้key
            using (Aes aes = Aes.Create())//การสร้างตัวเข้ารหัส AES
            {
                aes.KeySize = 256;
                aes.Key = key.GetBytes(32);  // เตรียมคีย์ขนาด 32 Bytes เพื่อเอาไปใช้ในการเข้ารหัส 256 bit
                aes.IV = key.GetBytes(16);   // ถามจารว่าคืออะไร

                using (var ms = new MemoryStream())//สร้างตัวเก็บข้อความที่เข้ารหัสเเล้ว ไว้รอดึงไปถอด ()ใช้เสร็จเเล้วลบอัตโนมัติ
                using (var cs = new CryptoStream(ms, aes.CreateEncryptor(), CryptoStreamMode.Write))
                //สร้าง CryptoStream เพื่อทำการ เข้ารหัส
                // aes.CreateEncryptor() = ให้ CryptoStream ใช้ตัวเข้ารหัส AES นี้ ในการแปลงข้อมูล
                // CryptoStreamMode.Write = เอาข้อมูลเข้า CryptoStream -> ให้มันเข้ารหัส แล้วส่งไปลง MemoryStream
                {
                    byte[] inputBytes = Encoding.UTF8.GetBytes(InputPlaintext.Text);
                    //แปลงข้อความ (String) -> เป็นชุดตัวเลข byte เพื่อเตรียมส่งให้ CryptoStream เข้ารหัส
                    cs.Write(inputBytes, 0, inputBytes.Length);
                    //ส่งข้อมูล plaintext byte เข้าไปใน CryptoStream เพื่อให้มันเข้ารหัส
                    //cs = CryptoStream 
                    //Write = เขียนข้อมูลเข้าไปในCryptoStream
                    //inputBytes = plaintext ที่แปลงเป็น byte แล้ว
                    //0 = เริ่มเขียนตั้งแต่ตำแหน่ง index 0
                    //inputBytes.Length = เขียนเท่ากับจำนวน byte ทั้งหมด

                    cs.Close();//ปิดStreamและปล่อย ciphertext ที่เหลือออกมา

                    InputPayload.Text = Convert.ToBase64String(ms.ToArray());
                    //ToBase64String = ฟังก์ชันที่แปลง byte เป็นข้อความ (String) แบบ Base64
                    //ms.ToArray() = byte ของ ciphertext
                }
            }
        }

       

        private void Browser_Click(object sender, EventArgs e)
        {
            OpenFileDialog dlg = new OpenFileDialog();
            dlg.Filter = "All Supported Files|*.txt;*.pdf;*.doc;*.docx";
            dlg.Multiselect = false; //เลือกหลายตัวเลือก

            if (dlg.ShowDialog() == DialogResult.OK)
            {
                //string folderPath = Path.GetDirectoryName(dlg.FileName);
                namefile.Text = Path.GetFileName(dlg.FileName);
                //GetProfileList(folderPath);
                string filePath = dlg.FileName;

                // แสดงชื่อไฟล์
                InputPlaintext.Text = Path.GetFileName(filePath);

                // อ่านเนื้อหาในไฟล์
                string content = File.ReadAllText(filePath);

                // แสดงเนื้อหาใน RichTextBox
                InputPlaintext.Text = content;
                
                //pt = ltext.Text;

            }
        }

        private void btOptions_Click(object sender, EventArgs e)
        {
           OptionStaganography form = new OptionStaganography();
            form.ShowDialog();
        }
     
        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void BtMs_Click(object sender, EventArgs e)
        {
            Misspelling form = new Misspelling();
            form.ShowDialog();
        }

        private void Btsp_Click(object sender, EventArgs e)
        {
            ChkbxZWSP form = new ChkbxZWSP();
            form.ShowDialog();
        }

        private void SteganographyForm_Load(object sender, EventArgs e)
        {

        }

        private void Ciphertextinput_TextChanged(object sender, EventArgs e)
        {

        }

        private void Steganogranophy_Click(object sender, EventArgs e)
        {

        }

        private void BtS_Click(object sender, EventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();

            saveFileDialog.Title = "Save plaintext";
            saveFileDialog.Filter = "Text file (*.txt)|*.txt|All files (*.*)|*.*";
            saveFileDialog.DefaultExt = "txt";
            saveFileDialog.FileName = "plaintext.txt";

            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                File.WriteAllText(saveFileDialog.FileName, InputPlaintext.Text);
                MessageBox.Show("บันทึกไฟล์เรียบร้อยแล้ว", "Success");
            }
        }

        private void namefile_TextChanged(object sender, EventArgs e)
        {

        }

        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void securityNoticeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MessageBox.Show(
        "โปรแกรมนี้จัดทำขึ้นเพื่อการศึกษาเท่านั้น\n" +
        "ไม่ควรใช้กับข้อมูลลับหรือข้อมูลจริง\n" +
        "ผู้พัฒนาไม่รับผิดชอบต่อความเสียหายใด ๆ ที่อาจเกิดขึ้น",
        "Security Warning",
        MessageBoxButtons.OK,
        MessageBoxIcon.Warning
    );
        }

        private void fileFormatToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MessageBox.Show(
        "Supported File Formats\n" +
        "Text file: .txt only!!\n",
        "File format",
        MessageBoxButtons.OK,
        MessageBoxIcon.Information
    ); 
        }

        private void TabEncrytion_Click(object sender, EventArgs e)
        {

        }
    }
}
