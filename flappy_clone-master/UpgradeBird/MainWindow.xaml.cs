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
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace UpgradeBird
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        int straight_counter = 0;

        double topScore = 0;

        bool spacedown = false;
        double ob_gap_end = 60;

        int[] xs = new int[30];
        int[] ys = new int[30];

        int mouseupthreshold = 30;

        double ob_width = 50.0;
        double ob_gap_base = 200.0;
        double ob_speed = 3.0;
        double partitions = 3.0;

        double player_forward = 100.0;
        double player_size = 15.0;
        double G = 0.3;
        double player_lift = 200.0;
        double player_speed = 0.0;

        DispatcherTimer timer;
        int counter = 0;
        int lastmousecounter = 0;

        Random r = new Random();

        List<Obstacle> obstactles = new List<Obstacle>();

        public MainWindow()
        {
            InitializeComponent();
            Loaded += MainWindow_Loaded;
        }

        void timer_Tick(object sender, EventArgs e)
        {
            straight_counter++;
            counter++;
            if ((counter - lastmousecounter) > mouseupthreshold)
                counter += 5;
            if (spacedown)
                counter -= 10;

            topScore = counter > topScore ? counter : topScore;


            canvas_base.Children.Clear();

            //stars
            for (int i = 0; i < 30; i++)
            {
                Ellipse star = new Ellipse()
                {
                    Width = 2,
                    Height = 2,
                    StrokeThickness = 0.0,
                    Fill = new SolidColorBrush(Colors.White)
                };
                if (xs[i] - straight_counter * 1.5 < 0)
                    xs[i] += (int)canvas_base.ActualWidth;
                star.SetValue(Canvas.TopProperty, (double)ys[i]);
                star.SetValue(Canvas.LeftProperty, ((double)xs[i] - straight_counter * 1.5) % canvas_base.ActualWidth);
                canvas_base.Children.Add(star);
            }

            TextBlock score = new TextBlock();
            if ((counter - lastmousecounter) > mouseupthreshold)
            {
                score.Foreground = new SolidColorBrush(Colors.Green);
                score.FontWeight = FontWeights.Bold;
            }

            //score
            score.Background = new SolidColorBrush(Colors.White);
            score.Margin = new Thickness(5.0);
            score.FontSize = 20.0;
            score.Text = "  " + counter.ToString() + "  ";

            TextBlock topScoretext = new TextBlock();
            topScoretext.FontWeight = FontWeights.Bold;

            //topScore
            topScoretext.Background = new SolidColorBrush(Colors.White);
            topScoretext.Margin = new Thickness(5, 35, 5, 5);
            topScoretext.FontSize = 20.0;
            topScoretext.Text = "  " + topScore.ToString() + "  ";
            if (topScore == counter && counter % 20 < 10)
                topScoretext.Text = "  " + topScore.ToString() + " ! ";


            double height = canvas_base.ActualHeight;
            double width = canvas_base.ActualWidth;
            //加速
            if (!spacedown || (spacedown && counter <= 0))
            {
                player_speed += G;
                player_lift -= player_speed;
            }

            //點點

            Rectangle you = new Rectangle()
            {
                Width = player_size,
                Height = player_size,
                Stroke = new SolidColorBrush(Colors.White),
                StrokeThickness = 2.0,
                Fill = new SolidColorBrush(Colors.Red)
            };
            if ((counter - lastmousecounter) > mouseupthreshold && counter % 10 < 5)
                you.Fill = new SolidColorBrush(Colors.White);
            you.SetValue(Canvas.TopProperty, height - player_lift);
            you.SetValue(Canvas.LeftProperty, player_forward);

            //障礙物
            foreach (Obstacle ob in obstactles)
            {
                double ob_gap = ob_gap_base * ob.left / canvas_base.ActualWidth + ob_gap_end;
                double top_height = (height - ob_gap) * Math.Pow(Math.Sin((ob.height + ob.neg * 2 * ob.left / canvas_base.ActualWidth)), 2.0);
                Color color = ob.hit ? Colors.Red : Colors.Green;
                Rectangle top = new Rectangle()
                {
                    Width = ob_width,
                    Height = top_height,
                    Stroke = new SolidColorBrush(Colors.White),//上面的邊框
                    StrokeThickness = 2.0,
                    Fill = new SolidColorBrush(color)
                };
                top.SetValue(Canvas.TopProperty, 0.0);
                top.SetValue(Canvas.LeftProperty, ob.left);

                Rectangle bottom = new Rectangle()
                {
                    Width = ob_width,
                    Height = height - top_height - ob_gap,
                    Stroke = new SolidColorBrush(Colors.White), //障礙物下面的邊框
                    StrokeThickness = 2.0,
                    Fill = new SolidColorBrush(color)
                };
                bottom.SetValue(Canvas.TopProperty, top_height + ob_gap);
                bottom.SetValue(Canvas.LeftProperty, ob.left);

                ob.visual_rect_top = top;
                ob.visual_rect_bottom = bottom;
                canvas_base.Children.Add(top);
                canvas_base.Children.Add(bottom);

                ob.left -= ob_speed;

                if (ob.left + ob_width < 0.0)
                {
                    ob.left = width;
                    ob.height = r.NextDouble();
                    ob.neg = (r.Next() % 2) * 2 - 1;
                    ob.hit = false;
                }
            }

            canvas_base.Children.Add(score);
            canvas_base.Children.Add(topScoretext);

            if (counter > 30 || (counter < 30 && counter % 5 < 3))
                canvas_base.Children.Add(you);


            foreach (Obstacle obstacle in obstactles)
            {
                if (!obstacle.hit && collision(you, obstacle.visual_rect_top) || collision(you, obstacle.visual_rect_bottom))
                {
                    obstacle.hit = true;
                    Console.Beep(1000, 100);
                    Console.Beep(1000, 100);
                    Console.Beep(1000, 100);
                    

                    new Window1().ShowDialog();
                    resetAll();
                    return;
                }
            }
        }

        void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
        

            new Window2().ShowDialog();
            for (int i = 0; i < 30; i++)
            {
                xs[i] = r.Next() % (int)canvas_base.ActualWidth;
                ys[i] = r.Next() % (int)canvas_base.ActualHeight;
            }

            MouseDown += canvas_base_MouseDown;
            KeyDown += MainWindow_KeyDown;
            KeyUp += MainWindow_KeyUp;

            timer = new DispatcherTimer() { Interval = TimeSpan.FromSeconds(0.02) };
            timer.Tick += timer_Tick;
            timer.Start();

            resetAll();
            
            
        }

        void MainWindow_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Space)
                spacedown = false;
        }

        void MainWindow_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Space && counter > 100)
            {
                spacedown = true;
                player_speed = 0;
                counter -= 100;
            }

        }

        private void resetAll()
        {
            System.Media.SoundPlayer player = new System.Media.SoundPlayer();
            player.SoundLocation = @"D:\02 I'm Yours.wav";
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
            counter = 0;

            obstactles.Clear();
            for (int i = 0; i < partitions; i++)
                obstactles.Add(new Obstacle() { height = r.NextDouble(), left = 500 + (canvas_base.ActualWidth + ob_width) * (i / partitions), neg = (r.Next() % 2) * 2 - 1 });


            player_lift = 200.0;
            player_speed = 0.0;





        }

        void canvas_base_MouseDown(object sender, MouseButtonEventArgs e)
        {
            player_speed = -5;
            lastmousecounter = counter;
        }

        //撞到
        bool collision(Rectangle r1, Rectangle r2)
        {
            double r1L = (double)r1.GetValue(Canvas.LeftProperty);
            double r1T = (double)r1.GetValue(Canvas.TopProperty);
            double r1R = r1L + r1.Width;
            double r1B = r1T + r1.Height;

            double r2L = (double)r2.GetValue(Canvas.LeftProperty);
            double r2T = (double)r2.GetValue(Canvas.TopProperty);
            double r2R = r2L + r2.Width;
            double r2B = r2T + r2.Height;

            if (r1T < 0)
                return true;
            if (r1B > canvas_base.ActualHeight)
                return true;

            return r1R > r2L && r1L < r2R && r1B > r2T && r1T < r2B;
        }

        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            player_size += 5.0;
        }

        private void MenuItem1_Click(object sender, RoutedEventArgs e)
        {
            timer.IsEnabled = !(timer.IsEnabled);
        }

        private void MenuItem2_Click(object sender, RoutedEventArgs e)
        {
            player_size -= 5.0;
        }

       

        class Obstacle
        {
            public bool hit = false;
            public Rectangle visual_rect_top;
            public Rectangle visual_rect_bottom;
            public double left;
            public double height;
            public double neg;
        }

        private void Window_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            Key kPressed;

            if (e.Key == Key.ImeProcessed)
            {
                kPressed = e.ImeProcessedKey;
            }
            else{ 
                kPressed = e.Key;
            }
            String sBC = kPressed.ToString();
            if(sBC[0]=='D'){
                if(sBC.Length==2){
                    sBC = sBC.Substring(1, 1);
                }
            }
            if (sBC[0] == 'N')
            {
                    sBC = sBC.Substring(6, 1);
                            }

            switch (sBC)
            {
                case "1":
                    player_size -= 5.0;
                    break;
                case "2":
                    player_size += 5.0;
                    break;
                case "3":
                    timer.IsEnabled = !(timer.IsEnabled);
                    break;
            }

        }
    }
}
