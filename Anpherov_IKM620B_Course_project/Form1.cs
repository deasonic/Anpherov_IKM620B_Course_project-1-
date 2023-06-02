﻿using System;
using System.Text;
using System.Windows.Forms;
using System.IO;
using Timer = System.Windows.Forms.Timer;
using System.IO.Ports;

namespace Anpherov_IKM_Course_project
{
    public partial class Form1 : Form
    {
        internal MajorWork MajorObject { get; private set; }

        private bool Mode;
        private SaveFileDialog sf;
        private MajorWork MojorObject;
        ToolStripLabel dateLabel;
        ToolStripLabel timeLabel;
        ToolStripLabel infoLabel;
        Timer timer;
        public Form1()
        {
            InitializeComponent();
            infoLabel = new ToolStripLabel();
            infoLabel.Text = "Поточні дата і час:";
            dateLabel = new ToolStripLabel();
            timeLabel = new ToolStripLabel();

            statusStrip1.Items.Add(infoLabel);
            statusStrip1.Items.Add(dateLabel);
            statusStrip1.Items.Add(timeLabel);

            timer = new Timer() { Interval = 1000 };
            timer.Tick += timer_Tick;
            timer.Start();
        }
        void timer_Tick(object sender, EventArgs e)
        {
            dateLabel.Text = DateTime.Now.ToLongDateString();
            timeLabel.Text = DateTime.Now.ToLongTimeString();
        }

        private void tClock_Tick(object sender, EventArgs e)
        {
            tClock.Stop();
            MessageBox.Show("Минуло 25 секунд", "Увага");
            tClock.Start();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            MajorObject = new MajorWork();
            MajorObject.SetTime();
            MajorObject.Modify = false;
            About A = new About();
            A.tAbout.Start();
            A.ShowDialog();
            this.Mode = true;

            toolTip1.SetToolTip(bSearch, "Натисніть на кнопку для пошуку");
            toolTip1.IsBalloon = true;
            string[] ports = SerialPort.GetPortNames();
            foreach (string port in ports)
            {
                comboBox1.Items.Add(port);
            };

        }

        private void bStart_Click(object sender, EventArgs e)
        {
            if (Mode)
            {
                tbInput.Enabled = true;
                tbInput.Focus();
                tClock.Start();
                bStart.Text = "Стоп";
                this.Mode = false;
                пускToolStripMenuItem.Text = "Стоп";
            }
            else
            {
                tbInput.Enabled = false;
                tClock.Stop();
                bStart.Text = "Пуск";
                this.Mode = true;
                MajorObject.Write(tbInput.Text);
                MajorObject.Task();
                label1.Text = MajorObject.Read();
                пускToolStripMenuItem.Text = "Старт";
            }
        }

        private void tbInput_KeyPress(object sender, KeyPressEventArgs e)
        {
            tClock.Stop();
            tClock.Start();
            if ((e.KeyChar >= '0') & (e.KeyChar <= '9') | (e.KeyChar == (char)8))
            {
                return;
            }
            else
            {
                tClock.Stop();
                MessageBox.Show("Неправильний символ", "Помилка");
                tClock.Start();
                e.KeyChar = (char)0;
            }
        }

        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            string s;
            s = (System.DateTime.Now - MajorObject.GetTime()).ToString();
            MessageBox.Show(s, "Час роботи програми");
        }

        private void вихідToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void проПрограмуToolStripMenuItem_Click(object sender, EventArgs e)
        {
            About A = new About();
            A.progressBar1.Hide();
            A.ShowDialog();
        }

        private void зберегтиЯкToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (sfdSave.ShowDialog() == DialogResult.OK)
            {
                MajorObject.WriteSaveFileName(sfdSave.FileName);
                MajorObject.Generator();
                MajorObject.SaveToFile();
            }
        }

        private void відкритиToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (ofdOpen.ShowDialog() == DialogResult.OK)
            {
                MajorObject.WriteOpenFileName(ofdOpen.FileName);
                MajorObject.ReadFromFile(dgwOpen);
            }
        }

        private void проНакопичувачіToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string[] Disks = System.IO.Directory.GetLogicalDrives();
            string disk = "";
            for (int i = 0; i < Disks.Length; i++)
            {
                try
                {
                    System.IO.DriveInfo D = new System.IO.DriveInfo(Disks[i]);
                    disk += D.Name + "-" + (D.TotalSize / (1024 * 1024 * 1024)).ToString() + "-" + (D.TotalFreeSpace / (1024 * 1024 * 1024)).ToString() + "GB" + (char)13;
                }
                catch
                {
                    disk += Disks[i] + "- не готовий" + (char)13;
                }
            }
            MessageBox.Show(disk, "Накопичувачі");
        }

        private void зберегтиToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (MajorObject.SaveFileNameExists())
                MajorObject.SaveToFile();
            else
                зберегтиЯкToolStripMenuItem_Click(sender, e);
        }

        private void новийToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MajorObject.NewRec();
            tbInput.Clear();
            label1.Text = "";
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (MajorObject.Modify)
                if (MessageBox.Show("Дані не були збережені. Продовжити вихід?", "УВАГА", MessageBoxButtons.YesNo) == DialogResult.No)
                    e.Cancel = true;
        }

        private void bSearch_Click(object sender, EventArgs e)
        {
            MajorObject.Find(tbSearch.Text);
        }

        private void Push_Click(object sender, EventArgs e)
        {
            MajorObject.myStack.Push(Stacktb.Text);
            MajorObject.myArr[MajorObject.myArr.Length - MajorObject.myStack.Count] = Stacktb.Text;
            LabelStack.Text = "";

            for (int i = 0; i < MajorObject.myArr.Length; i++)
            {
                if (MajorObject.myArr[i] != null)
                {
                    LabelStack.Text += MajorObject.myArr[i] + (char)13;
                }
                else
                {
                    continue;
                }
            }
        }

        private void Peek_Click(object sender, EventArgs e)
        {
            if (MajorObject.myStack.Count > 0)
            {
                MessageBox.Show("Peek" + MajorObject.myStack.Peek());
            }
            if (MajorObject.myStack.Count == 0)
                MessageBox.Show("\nСтек пустий");
        }

        private void Pop_Click(object sender, EventArgs e)
        {
            if (MajorObject.myStack.Count == 0)
                MessageBox.Show("\nСтек пустий!");
            else
            {
                MajorObject.myArr[MajorObject.myArr.Length - MajorObject.myStack.Count] = null;

                if (MajorObject.myStack.Count > 0)
                {
                    MessageBox.Show("Pop " + MajorObject.myStack.Pop());
                }
                LabelStack.Text = "";
                for (int i = 0; i < MajorObject.myArr.Length; i++)
                {
                    if (MajorObject.myArr[i] != null)

                    {
                        LabelStack.Text += MajorObject.myArr[i] + (char)13;
                    }
                    else
                    {
                        continue;
                    }
                }
                if (MajorObject.myStack.Count == 0)
                    MessageBox.Show("\nСтек пустий!");
            }
        }

        private void Enqueue_Click(object sender, EventArgs e)
        {
            MajorObject.myQueue.Enqueue(Queuetb.Text);
            MajorObject.smyQueue[MajorObject.myQueue.Count - 1] = Queuetb.Text;
            LabelQueue.Text = "";
            for (int i = 0; i < MajorObject.smyQueue.Length; i++)
            {
                if (MajorObject.smyQueue[i] != null)
                {
                    LabelQueue.Text += MajorObject.smyQueue[i] + (char)13;
                }
                else
                {
                    continue;
                }
            }
        }

        private void Peek_q_Click(object sender, EventArgs e)
        {
            if (MajorObject.myQueue.Count > 0)
            {
                MessageBox.Show("Peek " + MajorObject.myQueue.Peek());
            }
            if (MajorObject.myQueue.Count == 0)
                MessageBox.Show("\nЧерга порожня!");
        }

        private void Dequeue_Click(object sender, EventArgs e)
        {
            if (MajorObject.myQueue.Count == 0)

                MessageBox.Show("\nЧерга порожня!");
            else
            {
                MajorObject.smyQueue[0] = null;


                for (int i = 0; i < MajorObject.smyQueue.Length - 1; i++)
                {
                    MajorObject.smyQueue[i] = MajorObject.smyQueue[i + 1];
                }

                if (MajorObject.myQueue.Count > 0)
                {
                    MessageBox.Show("Dequeue " + MajorObject.myQueue.Dequeue());
                }

                LabelQueue.Text = "";
                for (int i = 0; i < MajorObject.smyQueue.Length - 1; i++)
                {
                    if (MajorObject.smyQueue[i] != null)
                    {
                        LabelQueue.Text += MajorObject.smyQueue[i] + (char)13;
                    }
                    else
                    {
                        continue;
                    }
                }
                if (MajorObject.myQueue.Count == 0)
                    MessageBox.Show("\nЧерга порожня!");
            }
        }

        private void зберегтиЯкToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            SaveFileDialog sf = new SaveFileDialog();

            sf.Filter = @"Текстовий файл (*.txt)|*.txt|Текстові файлиTXT(*.txt)|*.txt|CSV-файл (*.csv)|*.csv|Bin-файл (*.bin)|*.bin";

            if (sf.ShowDialog() == DialogResult.OK)
            {
                MajorObject.WriteSaveTextFileName(sf.FileName);
                MajorObject.SaveToTextFile(sf.FileName, dgwOpen);
            }
        }

        private void зберегтиToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            if (MajorObject.SaveTextFileNameExists())

                MajorObject.SaveToTextFile(MajorObject.ReadSaveTextFileName(), dgwOpen);
            else
                зберегтиЯкToolStripMenuItem1_Click(sender, e);
        }

        private void відкритиToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            OpenFileDialog o = new OpenFileDialog();

            o.Filter = @"Текстовий файл (*.txt)|*.txt|Текстовий файлTXT(*.txt)|*.txt|CSV-файл (*.csv)|*.csv|Bin-файл (*.bin)|*.bin";

            if (o.ShowDialog() == DialogResult.OK)
            {
                richTextBox1.Text = File.ReadAllText(o.FileName, Encoding.Default);
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (button2.Text == "Старт")

            {
                if (port.IsOpen) port.Close();
                #region Задаем параметры порта
                port.PortName = comboBox1.Text;
                port.BaudRate = Convert.ToInt32(comboBox2.Text);
                port.DataBits = Convert.ToInt32(comboBox3.Text);
                switch (comboBox4.Text)
                {
                    case "Пробел":
                        port.Parity = Parity.Space;
                        break;
                    case "Чет":
                        port.Parity = Parity.Even;
                        break;
                    case "Нечет":
                        port.Parity = Parity.Odd;
                        break;
                    case "Маркер":
                        port.Parity = Parity.Mark;
                        break;
                    default:
                        port.Parity = Parity.None;
                        break;
                }
                switch (comboBox5.Text)
                {
                    case "2":
                        port.StopBits = StopBits.Two;
                        break;
                    case "1.5":
                        port.StopBits = StopBits.OnePointFive;
                        break;
                    case "Нет":
                        port.StopBits = StopBits.None;
                        break;

                    default:
                        port.StopBits = StopBits.One;
                        break;
                }
                switch (comboBox6.Text)
                {
                    case "Xon/Xoff":
                        port.Handshake = Handshake.XOnXOff;
                        break;
                    case "Аппаратное":
                        port.Handshake = Handshake.RequestToSend;
                        break;
                    default:
                        port.Handshake = Handshake.None;
                        break;
                }
                #endregion
                try
                {
                    port.Open();
                    button2.Text = "Стоп";
                    // button2.Enabled = false;
                }
                catch
                {
                    MessageBox.Show("Порт " + port.PortName + " неможливо відкрити!",

                    "Помилка!", MessageBoxButtons.OK, MessageBoxIcon.Warning);

                    comboBox1.SelectedText = "";
                    button2.Text = "Старт";
                }
            }
            else
            {
                if (port.IsOpen) port.Close();
                button2.Text = "Старт";
                // button2.Enabled = true;
            }
        }
    }
}