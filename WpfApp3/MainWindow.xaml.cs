using Microsoft.WindowsAPICodePack.Dialogs;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Media;

namespace WpfApp3
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private List<string> files = new List<string>();
        private MediaPlayer mediaPlayer = new MediaPlayer();
        private int currentTrackIndex = -1;
        private bool isRandom = false;
        private bool isRepeat = false;

        public MainWindow()
        {
            InitializeComponent();
            mediaPlayer.MediaEnded += AutoPlayNextSong;
            mediaPlayer.MediaOpened += MediaPlayer_MediaOpened;
        }

        private void audio_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            mediaPlayer.Volume = audio.Value;
        }

        private void time_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            mediaPlayer.Position = TimeSpan.FromSeconds(time.Value);
        }

        private void MediaPlayer_MediaOpened(object sender, EventArgs e)
        {
            time.Maximum = mediaPlayer.NaturalDuration.TimeSpan.TotalSeconds;
        }

        private void Folder_Go_Click(object sender, RoutedEventArgs e)
        {
            CommonOpenFileDialog dialog = new CommonOpenFileDialog { IsFolderPicker = true };
            if (dialog.ShowDialog() == CommonFileDialogResult.Ok)
            {
                files = Directory.GetFiles(dialog.FileName, "*.mp3").ToList();
                if (files.Count > 0)
                {
                    music.Items.Clear();
                    foreach (string file in files)
                    {
                        music.Items.Add(Path.GetFileName(file));
                    }
                    currentTrackIndex = 0;
                    PlayTrack();
                }
                else
                {
                    MessageBox.Show("Ту ли папку вы выбрали?");
                }
            }
        }

        private void PlayTrack()
        {
            if (currentTrackIndex >= 0 && currentTrackIndex < files.Count)
            {
                mediaPlayer.Open(new Uri(files[currentTrackIndex]));
                mediaPlayer.Play();
            }
        }

        private void back_Click(object sender, RoutedEventArgs e)
        {
            if (currentTrackIndex > 0)
            {
                currentTrackIndex--;
                PlayTrack();
            }
        }

        private void pause_Click(object sender, RoutedEventArgs e)
        {
            if (mediaPlayer.CanPause)
            {
                if (mediaPlayer.Position != TimeSpan.Zero)
                {
                    mediaPlayer.Pause();
                }
                else
                {
                    mediaPlayer.Play();
                }
            }
        }

        private void forward_Click(object sender, RoutedEventArgs e)
        {
            if (currentTrackIndex < files.Count - 1)
            {
                currentTrackIndex++;
                PlayTrack();
            }
        }

        private void repeat_Click(object sender, RoutedEventArgs e)
        {
            isRepeat = !isRepeat;
            if (isRepeat)
            {
                repeat.Background = Brushes.Green; 
            }
            else
            {
                repeat.Background = Brushes.Transparent; 
            }
        }

        private void shuffle_Click(object sender, RoutedEventArgs e)
        {
            isRandom = !isRandom;
            if (isRandom)
            {
                shuffle.Background = Brushes.Blue; 
            }
            else
            {
                shuffle.Background = Brushes.Transparent;
            }
        }


        private void AutoPlayNextSong(object sender, EventArgs e)
        {
            if (isRepeat)
            {
                mediaPlayer.Position = TimeSpan.Zero;
                mediaPlayer.Play();
            }
            else if (isRandom)
            {
                Random rnd = new Random();
                currentTrackIndex = rnd.Next(0, files.Count);
                PlayTrack();
            }
            else
            {
                if (currentTrackIndex < files.Count - 1)
                {
                    currentTrackIndex++;
                    PlayTrack();
                }
            }
        }
    }
}
