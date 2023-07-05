using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace WhatsAppAudioConverter
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        System.Windows.Forms.OpenFileDialog OpenDialog = new System.Windows.Forms.OpenFileDialog();
        System.ComponentModel.BackgroundWorker worker = new System.ComponentModel.BackgroundWorker();
        public MainWindow()
        {
            InitializeComponent();
            worker.DoWork += Worker_DoWork;
            worker.RunWorkerCompleted += Worker_RunWorkerCompleted;
            worker.ProgressChanged += Worker_ProgressChanged;
            worker.WorkerReportsProgress = true;
            worker.WorkerSupportsCancellation = true;
            OpenDialog.Filter = "Audio Files (*.aac,*.ma10,*.amr,*.opus,ogg,*.mp3)|*.aac;.*.ma10;*.amr;*.opus;*.mp3;*.ogg; |" + " All Files (*.*) | *.*; ";
            OpenDialog.FilterIndex = 1;
            OpenDialog.AutoUpgradeEnabled = true;
            OpenDialog.Title = "WhatsApp File Audio";
            OpenDialog.FileName = "Open File WhatsApp";
            OpenDialog.Multiselect = true;
            if (Directory.Exists(@"AudioFiles"))
                Directory.Delete(@"AudioFiles", recursive: true);
        }

        private void Worker_ProgressChanged(object sender, System.ComponentModel.ProgressChangedEventArgs e)
        {
            progress.Value = e.ProgressPercentage;
        }

        private void Worker_RunWorkerCompleted(object sender, System.ComponentModel.RunWorkerCompletedEventArgs e)
        {

            OpenFile.IsEnabled = true;
            Convert.IsEnabled = true;
            PanelLabelsName.Children.Clear();
            if (MessageBox.Show("All Files Converted Succesfully \n\n Open Files ??", "Done", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                try
                {
                    System.Diagnostics.Process.Start( Path.GetFullPath(App.Current.StartupUri.ToString()));

                }catch(Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }

            }
        }

        private void Worker_DoWork(object sender, System.ComponentModel.DoWorkEventArgs e)
        {
            try
            {
                if ((string)e.Argument == "ConvertAudio")
                {
                    if (!Directory.Exists(@"AudioFiles"))
                    {
                        Directory.CreateDirectory(@"AudioFiles");
                        for (int i = 0; i < OpenDialog.FileNames.Length; i++)
                        {
                            if (i != 10)
                            {
                                File.Copy(OpenDialog.FileNames[i], Path.ChangeExtension($@"AudioFiles/File_({i + 1})", ".mp3"));
                                System.Threading.Thread.Sleep(500);
                                worker.ReportProgress(i * (100 / 5));
                            }
                            else
                                break;
                        }
                    }


                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            finally
            {

            }
        }

        private void Close_Click(object sender, RoutedEventArgs e)
        {
            if (Directory.Exists(@"AudioFiles"))
                Directory.Delete(@"AudioFiles", recursive: true);


            App.Current.Shutdown();
        }

        private void OpenFile_Click(object sender, RoutedEventArgs e)
        {
            if (OpenDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {

                Convert.IsEnabled = true;
                Cancel.IsEnabled = true;
                PanelLabelsName.Children.Clear();
                for (int i = 0; i < OpenDialog.FileNames.Length; i++)
                {
                    if (i != 10)
                    {
                        PanelLabelsName.Children.Add(new TextBlock() { TextWrapping = TextWrapping.Wrap, Padding = new Thickness(10), Text = $"File Path ( {i + 1} ) : " + OpenDialog.FileNames[i], Width = 320, Margin = new Thickness(10), Background = Brushes.DarkCyan, Height = 50, FontSize = 10, Foreground = Brushes.White });
                    }
                    else
                        break;

                }
            }
        }



        private void Convert_Click(object sender, RoutedEventArgs e)
        {
            if (!worker.IsBusy)
            {
                OpenFile.IsEnabled = false;
                Convert.IsEnabled = false;
                worker.RunWorkerAsync("ConvertAudio");
            }
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            worker.CancelAsync();
        }
    }
}
