using System;
using System.Drawing;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.Threading;

namespace YahyaStealth
{
    public class MacroForm : Form
    {
        // Windows API Tanımlamaları
        [DllImport("user32.dll")] static extern uint SendInput(uint nInputs, INPUT[] pInputs, int cbSize);
        [DllImport("user32.dll")] static extern short GetAsyncKeyState(int vKey);

        [StructLayout(LayoutKind.Sequential)]
        struct MOUSEINPUT { public int dx, dy; public uint mouseData, dwFlags, time; public IntPtr dwExtraInfo; }

        [StructLayout(LayoutKind.Explicit)]
        struct INPUT { [FieldOffset(0)] public int type; [FieldOffset(8)] public MOUSEINPUT mi; }

        // Sabitler
        const uint MOUSEEVENTF_LEFTDOWN = 0x0002;
        const uint MOUSEEVENTF_LEFTUP = 0x0004;
        const int VK_F = 0x46; // Açma-Kapama Tuşu
        const int VK_LBUTTON = 0x01; // Sol Tık

        bool isEnabled = false;
        Label statusLabel;
        Random rnd = new Random();

        public MacroForm()
        {
            // Menü Tasarımı ve Ayarları
            this.Text = "Yahya Stealth v18.2";
            this.Size = new Size(320, 220);
            this.BackColor = Color.FromArgb(25, 25, 25); // Koyu Tema
            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;

            // Başlık
            Label titleLabel = new Label();
            titleLabel.Text = "STEALTH MOUSE MODULE";
            titleLabel.ForeColor = Color.Cyan;
            titleLabel.Font = new Font("Segoe UI", 12, FontStyle.Bold);
            titleLabel.Dock = DockStyle.Top;
            titleLabel.TextAlign = ContentAlignment.MiddleCenter;
            titleLabel.Height = 50;

            // Durum Göstergesi
            statusLabel = new Label();
            statusLabel.Text = "DURUM: KAPALI";
            statusLabel.ForeColor = Color.Red;
            statusLabel.Font = new Font("Segoe UI", 16, FontStyle.Bold);
            statusLabel.Dock = DockStyle.Fill;
            statusLabel.TextAlign = ContentAlignment.MiddleCenter;

            // Bilgi Alt Başlığı
            Label infoLabel = new Label();
            infoLabel.Text = "[F] Tuşu ile Aktif Et\nSol Tık Basılıyken Ekstra Vurur";
            infoLabel.ForeColor = Color.Gray;
            infoLabel.Dock = DockStyle.Bottom;
            infoLabel.TextAlign = ContentAlignment.MiddleCenter;
            infoLabel.Height = 60;

            this.Controls.Add(statusLabel);
            this.Controls.Add(titleLabel);
            this.Controls.Add(infoLabel);

            // Makro Döngüsü İçin Ayrı Bir Thread Başlat
            Thread macroThread = new Thread(RunMacro) { IsBackground = true };
            macroThread.Start();
        }

        void RunMacro()
        {
            while (true)
            {
                // F Tuşu (Toggle) Kontrolü
                if ((GetAsyncKeyState(VK_F) & 1) == 1)
                {
                    isEnabled = !isEnabled;
                    // Menüdeki yazıyı güncelle (UI Thread üzerinden)
                    this.Invoke((MethodInvoker)(() => {
                        statusLabel.Text = isEnabled ? "DURUM: AKTİF" : "DURUM: KAPALI";
                        statusLabel.ForeColor = isEnabled ? Color.Lime : Color.Red;
                    }));
                }

                // Makro Aktifse ve Fiziksel Sol Tık Basılıysa
                if (isEnabled && (GetAsyncKeyState(VK_LBUTTON) & 0x8000) != 0)
                {
                    PerformClick();
                    // 16-20 CPS arası rastgele gecikme (Doğal vuruş)
                    Thread.Sleep(rnd.Next(45, 62));
                }
                Thread.Sleep(1);
            }
        }

        void PerformClick()
        {
            INPUT[] inputs = new INPUT[2];
            inputs[0].type = 0; // INPUT_MOUSE
            inputs[0].mi.dwFlags = MOUSEEVENTF_LEFTDOWN;
            inputs[1].type = 0;
            inputs[1].mi.dwFlags = MOUSEEVENTF_LEFTUP;
            SendInput(2, inputs, Marshal.SizeOf(typeof(INPUT)));
        }

        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.Run(new MacroForm());
        }
    }
}
