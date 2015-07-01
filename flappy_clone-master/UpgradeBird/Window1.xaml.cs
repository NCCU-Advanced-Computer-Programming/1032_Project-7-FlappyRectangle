using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace UpgradeBird
{
    /// <summary>
    /// Window1.xaml 的互動邏輯
    /// </summary>
    public partial class Window1 : Window
    {
        public Window1()
        {
            InitializeComponent();
            Loaded += Window_Loaded;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
            
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            System.Media.SoundPlayer player = new System.Media.SoundPlayer();
            player.SoundLocation = @"D:\0003.wav";
            try
            {
                player.Load();
                player.Play();
            }
            catch (System.IO.FileNotFoundException err)
            {
                MessageBox.Show("找不到音效檔 " + err.FileName);
            }
            catch (InvalidOperationException err)
            {
                MessageBox.Show("播放發生錯誤：" + err.Message);
            }

        }

        private void Button_Click1(object sender, RoutedEventArgs e)
        {
            Environment.Exit(Environment.ExitCode);
        }

        private void Button_Click2(object sender, RoutedEventArgs e)
        {
            AboutBox1 ab = new AboutBox1();
            ab.Show();
        }
        
     
    }
}
