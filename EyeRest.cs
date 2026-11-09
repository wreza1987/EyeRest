using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows.Forms;

public class EyeRestApp : Form
{
    [DllImport("user32.dll")]
    private static extern bool SetProcessDPIAware();

    [DllImport("user32.dll")]
    private static extern short GetAsyncKeyState(int vKey);
    private const int VK_ESCAPE = 0x1B;

    private TextBox txtInterval;
    private TextBox txtDuration;
    private Button btnColor;
    private Label lblStatus;
    private Timer workTimer;
    private Timer restTimer;
    private Timer keyWatchTimer;
    private NotifyIcon trayIcon;
    private ContextMenuStrip trayMenu;

    private List<Form> overlayForms = new List<Form>();
    private List<Label> counterLabels = new List<Label>();

    private int intervalMin = 30;
    private int durationSec = 30;
    private Color bgColor = Color.Black;
    private int remainingWorkSec;
    private int remainingRestSec;
    private bool cursorHidden = false;
    private bool isExiting = false;
    private bool balloonShown = false;

    private string configPath = Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
        "eye_rest_config.txt"
    );

    public EyeRestApp()
    {
        LoadConfig();
        BuildUI();
        BuildTrayIcon();

        workTimer = new Timer() { Interval = 1000 };
        workTimer.Tick += WorkTimer_Tick;

        restTimer = new Timer() { Interval = 1000 };
        restTimer.Tick += RestTimer_Tick;

        // پایش دائمی کلید Escape حتی در صورت خارج شدن فوکوس در چند مانیتور
        keyWatchTimer = new Timer() { Interval = 100 };
        keyWatchTimer.Tick += (s, e) =>
        {
            if (overlayForms.Count > 0 && (GetAsyncKeyState(VK_ESCAPE) & 0x8000) != 0)
            {
                CloseOverlay();
            }
        };

        StartWorkTimer();
    }

    private void LoadConfig()
    {
        if (File.Exists(configPath))
        {
            try
            {
                string[] parts = File.ReadAllText(configPath).Split('|');
                intervalMin = int.Parse(parts[0]);
                durationSec = int.Parse(parts[1]);
                bgColor = ColorTranslator.FromHtml(parts[2]);
            }
            catch { }
        }
    }

    private void SaveConfig()
    {
        try
        {
            string data = string.Format("{0}|{1}|{2}", intervalMin, durationSec, ColorTranslator.ToHtml(bgColor));
            File.WriteAllText(configPath, data);
        }
        catch { }
    }

    private Icon CreateCustomTrayIcon()
    {
        Bitmap bmp = new Bitmap(16, 16);
        using (Graphics g = Graphics.FromImage(bmp))
        {
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.Clear(Color.Transparent);
            using (Brush b = new SolidBrush(Color.FromArgb(0, 120, 215)))
                g.FillEllipse(b, 1, 1, 14, 14);
            using (Brush b = new SolidBrush(Color.White))
                g.FillEllipse(b, 5, 5, 6, 6);
        }
        return Icon.FromHandle(bmp.GetHicon());
    }

    private void BuildTrayIcon()
    {
        trayMenu = new ContextMenuStrip();
        trayMenu.RightToLeft = RightToLeft.Yes;
        trayMenu.Items.Add("باز کردن تنظیمات", null, (s, e) => ShowSettingsWindow());
        trayMenu.Items.Add("تست استراحت همین الان", null, (s, e) => ShowOverlay());
        trayMenu.Items.Add("ریست تایمر", null, (s, e) => StartWorkTimer());
        trayMenu.Items.Add(new ToolStripSeparator());
        trayMenu.Items.Add("خروج کامل از برنامه", null, (s, e) => ExitApp());

        trayIcon = new NotifyIcon()
        {
            Icon = CreateCustomTrayIcon(),
            ContextMenuStrip = trayMenu,
            Visible = true,
            Text = "استراحت چشم"
        };

        trayIcon.DoubleClick += (s, e) => ShowSettingsWindow();
    }

    private void BuildUI()
    {
        this.Text = "تنظیمات استراحت چشم";
        this.Size = new Size(320, 275);
        this.FormBorderStyle = FormBorderStyle.FixedDialog;
        this.MaximizeBox = false;
        this.StartPosition = FormStartPosition.CenterScreen;
        this.Font = new Font("Tahoma", 9F);
        this.RightToLeft = RightToLeft.Yes;
        this.RightToLeftLayout = true;
        this.Icon = CreateCustomTrayIcon();

        Label lbl1 = new Label() { Text = "فاصله زمانی (دقیقه):", Location = new Point(20, 20), AutoSize = true };
        txtInterval = new TextBox() { Text = intervalMin.ToString(), Location = new Point(175, 17), Width = 95, TextAlign = HorizontalAlignment.Center };

        Label lbl2 = new Label() { Text = "مدت استراحت (ثانیه):", Location = new Point(20, 55), AutoSize = true };
        txtDuration = new TextBox() { Text = durationSec.ToString(), Location = new Point(175, 52), Width = 95, TextAlign = HorizontalAlignment.Center };

        Label lbl3 = new Label() { Text = "رنگ صفحه:", Location = new Point(20, 90), AutoSize = true };
        btnColor = new Button()
        {
            Text = "انتخاب رنگ",
            Location = new Point(175, 86),
            Width = 95,
            BackColor = bgColor,
            ForeColor = GetContrastColor(bgColor),
            FlatStyle = FlatStyle.Flat
        };
        btnColor.Click += PickColor_Click;

        lblStatus = new Label()
        {
            Text = "",
            Location = new Point(20, 125),
            Size = new Size(255, 25),
            TextAlign = ContentAlignment.MiddleCenter,
            ForeColor = Color.FromArgb(0, 102, 204),
            Font = new Font("Tahoma", 9.5F, FontStyle.Bold)
        };

        Button btnSave = new Button() { Text = "ذخیره و مخفی شدن", Location = new Point(20, 160), Width = 135, Height = 30 };
        btnSave.Click += ApplySettings_Click;

        Button btnTest = new Button() { Text = "تست همین الان", Location = new Point(165, 160), Width = 105, Height = 30 };
        btnTest.Click += (s, e) => ShowOverlay();

        Button btnExit = new Button() { Text = "خروج کامل از برنامه", Location = new Point(20, 198), Width = 250, Height = 28 };
        btnExit.Click += (s, e) => ExitApp();

        this.Controls.AddRange(new Control[] { lbl1, txtInterval, lbl2, txtDuration, lbl3, btnColor, lblStatus, btnSave, btnTest, btnExit });

        this.Resize += (s, e) =>
        {
            if (this.WindowState == FormWindowState.Minimized)
                HideToTray();
        };
    }

    protected override void OnFormClosing(FormClosingEventArgs e)
    {
        if (!isExiting && e.CloseReason == CloseReason.UserClosing)
        {
            e.Cancel = true;
            HideToTray();
            return;
        }
        base.OnFormClosing(e);
    }

    private void HideToTray()
    {
        this.Hide();
        if (!balloonShown)
        {
            trayIcon.ShowBalloonTip(2000, "استراحت چشم", "برنامه در کنار ساعت ویندوز در حال اجراست.", ToolTipIcon.Info);
            balloonShown = true;
        }
    }

    private void ShowSettingsWindow()
    {
        this.Show();
        this.WindowState = FormWindowState.Normal;
        this.Activate();
    }

    private void ExitApp()
    {
        isExiting = true;
        CloseOverlay();
        trayIcon.Visible = false;
        trayIcon.Dispose();
        Application.Exit();
    }

    private Color GetContrastColor(Color c)
    {
        double luminance = 0.299 * c.R + 0.587 * c.G + 0.114 * c.B;
        return luminance > 128 ? Color.FromArgb(34, 34, 34) : Color.FromArgb(204, 204, 204);
    }

    private void PickColor_Click(object sender, EventArgs e)
    {
        using (ColorDialog cd = new ColorDialog())
        {
            cd.Color = bgColor;
            if (cd.ShowDialog() == DialogResult.OK)
            {
                bgColor = cd.Color;
                btnColor.BackColor = bgColor;
                btnColor.ForeColor = GetContrastColor(bgColor);
            }
        }
    }

    private void ApplySettings_Click(object sender, EventArgs e)
    {
        int newInterval, newDuration;
        if (!int.TryParse(txtInterval.Text, out newInterval) || !int.TryParse(txtDuration.Text, out newDuration) || newInterval <= 0 || newDuration <= 0)
        {
            MessageBox.Show("لطفاً اعداد صحیح و بزرگتر از صفر وارد کنید.", "خطا", MessageBoxButtons.OK, MessageBoxIcon.Error);
            return;
        }
        intervalMin = newInterval;
        durationSec = newDuration;
        SaveConfig();
        StartWorkTimer();
        HideToTray();
    }

    private void StartWorkTimer()
    {
        workTimer.Stop();
        remainingWorkSec = intervalMin * 60;
        UpdateWorkStatus();
        workTimer.Start();
    }

    private void WorkTimer_Tick(object sender, EventArgs e)
    {
        remainingWorkSec--;
        UpdateWorkStatus();
        if (remainingWorkSec <= 0)
        {
            ShowOverlay();
        }
    }

    private void UpdateWorkStatus()
    {
        int mins = remainingWorkSec / 60;
        int secs = remainingWorkSec % 60;
        string timeStr = string.Format("{0:D2}:{1:D2}", mins, secs);
        lblStatus.Text = "استراحت بعدی تا: " + timeStr;
        trayIcon.Text = "استراحت چشم - باقی‌مانده: " + timeStr;
    }

    private void ShowOverlay()
    {
        workTimer.Stop();
        if (overlayForms.Count > 0) return;

        remainingRestSec = durationSec;
        Color contrast = GetContrastColor(bgColor);

        // ایجاد صفحه پوشش برای تک‌تک مانیتورهای متصل
        foreach (Screen screen in Screen.AllScreens)
        {
            Form overlay = new Form()
            {
                FormBorderStyle = FormBorderStyle.None,
                StartPosition = FormStartPosition.Manual,
                Bounds = screen.Bounds,
                TopMost = true,
                BackColor = bgColor,
                ShowInTaskbar = false,
                KeyPreview = true
            };

            Label lbl = new Label()
            {
                AutoSize = true,
                ForeColor = contrast,
                BackColor = Color.Transparent,
                Font = new Font("Segoe UI", 22F, FontStyle.Bold),
                Text = remainingRestSec + "s"
            };

            overlay.Controls.Add(lbl);
            overlay.Load += (s, e) =>
            {
                lbl.Location = new Point(overlay.ClientSize.Width - 110, overlay.ClientSize.Height - 80);
                overlay.Activate();
            };

            overlay.KeyDown += (s, e) =>
            {
                if (e.KeyCode == Keys.Escape) CloseOverlay();
            };

            overlayForms.Add(overlay);
            counterLabels.Add(lbl);
            overlay.Show();
        }

        if (!cursorHidden)
        {
            Cursor.Hide();
            cursorHidden = true;
        }

        restTimer.Start();
        keyWatchTimer.Start();
    }

    private void RestTimer_Tick(object sender, EventArgs e)
    {
        remainingRestSec--;
        if (remainingRestSec <= 0)
        {
            CloseOverlay();
        }
        else
        {
            foreach (Label lbl in counterLabels)
            {
                lbl.Text = remainingRestSec + "s";
            }
        }
    }

    private void CloseOverlay()
    {
        restTimer.Stop();
        keyWatchTimer.Stop();

        if (cursorHidden)
        {
            Cursor.Show();
            cursorHidden = false;
        }

        foreach (Form f in overlayForms)
        {
            f.Close();
            f.Dispose();
        }
        overlayForms.Clear();
        counterLabels.Clear();

        StartWorkTimer();
    }

    [STAThread]
    public static void Main()
    {
        SetProcessDPIAware();
        Application.EnableVisualStyles();
        Application.SetCompatibleTextRenderingDefault(false);
        Application.Run(new EyeRestApp());
    }
}