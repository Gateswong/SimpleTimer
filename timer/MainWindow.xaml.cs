using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace timer
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            this.Topmost = true;
            
            // 参数处理：待执行的命令
            try
            {
                if (Environment.GetCommandLineArgs().Contains("-r"))
                {
                    command = Environment.GetCommandLineArgs()[Environment.GetCommandLineArgs().ToList().IndexOf("-r") + 1];

                    this.ToolTip = "计时完成后将执行\n" + command;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("-r参数后缺少命令");
                Environment.Exit(0);
            }

            // 参数处理：时间
            try
            {
                string duraStr = Environment.GetCommandLineArgs()[1];
                if (!int.TryParse(duraStr, out duration))
                {
                    string timeMark = duraStr.Substring(duraStr.Length - 1);
                    duration = int.Parse(duraStr.Substring(0, duraStr.Length - 1)) *
                        (timeMark == "s" ? 1 :
                        (timeMark == "m" ? 60 :
                        (timeMark == "h" ? 60 * 60 :
                        (timeMark == "d" ? 60 * 60 * 24 :
                        (timeMark == "w" ? 60 * 60 * 24 * 7 : 0)))));
                }
                else {
                    duration = duration * 60;
                }

                if (duration == 0)
                {
                    throw new Exception("使用了错误的参数");
                }
            }
            catch (Exception ex)
            {
                help();
                Environment.Exit(0);
            }

            // 启动定时器
            ParseTime();
            timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromSeconds(1);
            timer.Tick += timer_Tick;
            timer.Start();
        }

        private void ParseTime()
        {
            int hh = duration / 60 / 60;
            int mm = duration % (60 * 60) / 60;
            int ss = duration % 60;

            string t = "";
            if (hh > 0)
            {
                t = string.Format("{0}:{1}:{2}", hh.ToString("00"), mm.ToString("00"), ss.ToString("00"));
            }
            else
            {
                t = string.Format("{0}:{1}", mm.ToString("00"), ss.ToString("00"));
            }

            lbTime.Content = t;
        }

        void timer_Tick(object sender, EventArgs e)
        {
            duration--;
            ParseTime();
            if (duration == 0)
            {
                if (command != null)
                {
                    try
                    {
                        System.Diagnostics.Process.Start(command);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("命令执行失败：\n" + command);
                    }

                    Environment.Exit(0);
                }
                else
                {
                    System.Media.SoundPlayer p = new System.Media.SoundPlayer();
                    p.Stream = Properties.Resources.Hypersonic2_093_MSG;
                    p.Play();
                    timer.Tick -= timer_Tick;
                    timer.Tick += timer_Noticce;
                    timer.Interval = TimeSpan.FromSeconds(0.5);
                }
            }
        }

        private void timer_Noticce(object sender, EventArgs e)
        {
            if (lbTime.Visibility == System.Windows.Visibility.Hidden)
            {
                lbTime.Visibility = System.Windows.Visibility.Visible;
            }
            else
            {
                lbTime.Visibility = System.Windows.Visibility.Hidden;
            }
        }

        private void help()
        {
            MessageBox.Show("使用帮助");
        }

        private string command = null;
        private int duration = 0;
        private DispatcherTimer timer;

        private void Window_MouseDown_1(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                this.DragMove();
            }
        }

        private void Window_MouseDoubleClick_1(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                this.Close();
            }
        }
    }
}
