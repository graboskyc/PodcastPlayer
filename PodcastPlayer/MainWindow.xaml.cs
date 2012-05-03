using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.ComponentModel;
using System.Windows.Threading;

namespace PodcastPlayer
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private string sourceMedia;
        private bool isPlaying = false;
        private DispatcherTimer timer;

        public MainWindow()
        {
            InitializeComponent();
            TaskbarItemInfo.Overlay = null;
            timer = new DispatcherTimer();
        }
        
        private void timerUpdate(object sender, EventArgs e)
        {
            currentTime.Text = String.Format("{0:00}:{1:00}:{2:00}", me.Position.Hours, me.Position.Minutes, me.Position.Seconds);
            TimeLineSlider.Value = Convert.ToInt32(me.Position.TotalSeconds);
            
            TaskbarItemInfo.ProgressValue = me.Position.TotalSeconds / TimeLineSlider.Maximum;
        }

        private void Command_CanExecute(object sender, System.Windows.Input.CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = me.Source != null;
        }

        private void PlayCommand_Executed(object sender, System.Windows.Input.ExecutedRoutedEventArgs e)
        {
            TogglePlayPause();
        }

        private void StopCommand_Executed(object sender, System.Windows.Input.ExecutedRoutedEventArgs e)
        {
            me.Stop();
            isPlaying = false;

            BitmapImage img = new BitmapImage();
            img.BeginInit();
            img.UriSource = new Uri("/Icons/Play.png", UriKind.Relative);
            img.EndInit();
            PlayButton.Source = img;
            ThumbPlayButton.ImageSource = img;
        }

        private void PauseCommand_Executed(object sender, System.Windows.Input.ExecutedRoutedEventArgs e)
        {
            TogglePlayPause();
        }

        private void OpenCommand_Executed(object sender, System.Windows.Input.ExecutedRoutedEventArgs e)
        {
            
        }

        private void OpenCommand_CanExecute(object sender, System.Windows.Input.CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = !isPlaying;
        }

        private void me_MediaFailed(object sender, ExceptionRoutedEventArgs e)
        {
            TaskbarItemInfo.Overlay = (ImageSource)Resources["ErrorImage"];
            MessageBox.Show("ERROR: " + e.ToString());
        }

        private void me_MediaOpened(object sender, EventArgs e)
        {
            TimeLineSlider.Maximum = me.NaturalDuration.TimeSpan.TotalSeconds;

            totalTime.Text = String.Format("{0:00}:{1:00}:{2:00}", me.NaturalDuration.TimeSpan.Hours, me.NaturalDuration.TimeSpan.Minutes, me.NaturalDuration.TimeSpan.Seconds);
        }

        private void TimeLineSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> args)
        {
            int SliderValue = (int)TimeLineSlider.Value;
            TimeSpan ts = new TimeSpan(0, 0, 0, 0, SliderValue);
            me.Position = ts;
        }

        private void TogglePlayPause()
        {
            BitmapImage img = new BitmapImage();
            img.BeginInit();

            if (isPlaying)
            {
                isPlaying = false;
                me.Pause();
                img.UriSource = new Uri("/Icons/Play.png", UriKind.Relative);
                timer.Stop();
                TaskbarItemInfo.ProgressState = System.Windows.Shell.TaskbarItemProgressState.Paused;
            }
            else
            {
                isPlaying = true;
                me.Play();
                img.UriSource = new Uri("/Icons/Pause.png", UriKind.Relative);
                timer.Interval = TimeSpan.FromMilliseconds(100);
                timer.Tick += timerUpdate;
                timer.Start();
                TaskbarItemInfo.ProgressState = System.Windows.Shell.TaskbarItemProgressState.Normal;
            }

            img.EndInit();
            PlayButton.Source = img;
            ThumbPlayButton.ImageSource = img;          

        }

        private void PlayButton_MouseDown(object sender, MouseButtonEventArgs e)
        {
            TogglePlayPause();
        }

        private void StopButton_MouseDown(object sender, MouseButtonEventArgs e)
        {
            me.Stop();
            isPlaying = false;

            BitmapImage img = new BitmapImage();
            img.BeginInit();
            img.UriSource = new Uri("/Icons/Play.png", UriKind.Relative);
            img.EndInit();
            PlayButton.Source = img;
            ThumbPlayButton.ImageSource = img;
        }

        private void BrowseButton_MouseDown(object sender, MouseButtonEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog ofd = new Microsoft.Win32.OpenFileDialog();
            ofd.Title = "Select a media file";
            ofd.Filter = "MP3|*.mp3|All Files|*.*";

            if ((bool)ofd.ShowDialog())
            {
                sourceMedia = ofd.FileName;
                me.Source = new Uri(sourceMedia, UriKind.RelativeOrAbsolute);
            }
        }

        private void TimeLineSlider_MouseUp(object sender, MouseButtonEventArgs e)
        {
            int SliderValue = (int)TimeLineSlider.Value;
            TimeSpan ts = TimeSpan.FromSeconds(SliderValue);
            me.Position = ts;
        }

        private void BackButton_MouseDown(object sender, MouseButtonEventArgs e)
        {
            me.Position = me.Position.Subtract(TimeSpan.FromSeconds(5));
        }

        private void ForwardButton_MouseDown(object sender, MouseButtonEventArgs e)
        {
            me.Position = me.Position.Add(TimeSpan.FromSeconds(30));
        }

        private void ThumbBackButton_Click(object sender, EventArgs e)
        {
            me.Position = me.Position.Subtract(TimeSpan.FromSeconds(5));
        }

        private void ThumbForwardButton_Click(object sender, EventArgs e)
        {
            me.Position = me.Position.Add(TimeSpan.FromSeconds(30));
        }
    }
}
