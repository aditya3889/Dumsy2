using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Microsoft.Phone.Controls;
using System.Threading;

namespace Dumsy2
{
    public partial class MainPage : PhoneApplicationPage
    {
        public MainPage()
        {
            InitializeComponent();
        }

        private void SetApplicationBarButtons()
        {
            ApplicationBar.Buttons.Clear();
            Color themeColor = (Color)Application.Current.Resources["PhoneForegroundColor"];

            if (((PanoramaItem)Panorama.SelectedItem).Name.Equals("MainPageItem", StringComparison.InvariantCultureIgnoreCase))
            {
                if (themeColor.ToString() != "#FFFFFFFF")
                {
                    Microsoft.Phone.Shell.ApplicationBarIconButton b = new Microsoft.Phone.Shell.ApplicationBarIconButton();
                    b.Text = "Next";
                    b.IconUri = new Uri("Images/Light/appbar.next.rest.png", UriKind.Relative);
                    b.Click += Next_Click;
                    ApplicationBar.Buttons.Add(b);

                    b = new Microsoft.Phone.Shell.ApplicationBarIconButton();
                    b.Text = "Play";
                    b.Click += Play_Click;
                    b.IconUri = new Uri("Images/Light/appbar.transport.play.rest.png", UriKind.Relative);
                    ApplicationBar.Buttons.Add(b);
                }
                else
                {
                    Microsoft.Phone.Shell.ApplicationBarIconButton b = new Microsoft.Phone.Shell.ApplicationBarIconButton();
                    b.Text = "Next";
                    b.IconUri = new Uri("Images/Dark/appbar.next.rest.png", UriKind.Relative);
                    b.Click += Next_Click;
                    ApplicationBar.Buttons.Add(b);

                    b = new Microsoft.Phone.Shell.ApplicationBarIconButton();
                    b.Text = "Play";
                    b.Click += Play_Click;
                    b.IconUri = new Uri("Images/Dark/appbar.transport.play.rest.png", UriKind.Relative);
                    ApplicationBar.Buttons.Add(b);
                }
            }
            else
            {
                if (themeColor.ToString() != "#FFFFFFFF")
                {
                    Microsoft.Phone.Shell.ApplicationBarIconButton b = new Microsoft.Phone.Shell.ApplicationBarIconButton();
                    b.Text = "Next";
                    b.IconUri = new Uri("Images/Light/appbar.save.rest.png", UriKind.Relative);
                    b.Click += Save_Settings;
                    ApplicationBar.Buttons.Add(b);
                }
                else
                {
                    Microsoft.Phone.Shell.ApplicationBarIconButton b = new Microsoft.Phone.Shell.ApplicationBarIconButton();
                    b.Text = "Next";
                    b.IconUri = new Uri("Images/Dark/appbar.save.rest.png", UriKind.Relative);
                    b.Click += Save_Settings;
                    ApplicationBar.Buttons.Add(b);
                }
            }
        }

        private DateTime? startDatetime = null;
        private DateTime? endDateTime = null;

        private void Play_Click(object sender, EventArgs e)
        {
            if (selectedMovie != null)
            {
                startDatetime = DateTime.UtcNow;
                endDateTime = startDatetime.Value.AddMinutes(2);
            }
        }
        void dt_Tick(object sender, EventArgs e)
        {
            if (endDateTime.HasValue && startDatetime.HasValue)
            {
                if (endDateTime.Value < DateTime.UtcNow)
                {
                    Timer.Text = "0:00";
                }
                else
                {
                    int seconds = (int)((endDateTime.Value - DateTime.UtcNow).TotalSeconds);
                    Timer.Text = String.Format("{0}:{1:D2}", seconds / 60, seconds % 60);
                }
            }
        }

        private void Save_Settings(object sender, EventArgs e)
        {
            string language = languages[this.SelectedLanguage.SelectedIndex].ToString();
            string difficultyLevel = difficulty[this.SelectedDifficulty.SelectedIndex].ToString();
            Helpers.SetValueToStorage(Constants.DifficultySetting, difficultyLevel);
            Helpers.SetValueToStorage(Constants.LanguageSetting, language);
        }

        private void PhoneApplicationPage_Loaded_1(object sender, RoutedEventArgs e)
        {
            SetApplicationBarButtons();

            System.Windows.Threading.DispatcherTimer dt = new System.Windows.Threading.DispatcherTimer();
            dt.Interval = new TimeSpan(0, 0, 0, 0, 500); // 500 Milliseconds
            dt.Tick += new EventHandler(dt_Tick);
            dt.Start();

            if (languages == null)
            {
                languages = new System.Collections.ObjectModel.ObservableCollection<string>();
                languages.Add("English");
                languages.Add("Hindi");

                this.SelectedLanguage.ItemsSource = languages;

                var language = Helpers.GetInfoFromMemory(Constants.LanguageSetting);
                if (language != null)
                {
                    int index = languages.IndexOf(language.ToString());
                    if (index >= 0 && index < languages.Count)
                    {
                        this.SelectedLanguage.SelectedIndex = index;
                    }
                }
            }

            if (difficulty == null)
            {
                difficulty = new List<string>();
                difficulty.Add("Easy");
                difficulty.Add("Difficult");
                difficulty.Add("Random");

                this.SelectedDifficulty.ItemsSource = difficulty;

                var difficultyLevel = Helpers.GetInfoFromMemory(Constants.DifficultySetting);
                if (difficultyLevel != null)
                {
                    int index = difficulty.IndexOf(difficultyLevel.ToString());
                    if (index >= 0 && index < difficulty.Count)
                    {
                        this.SelectedDifficulty.SelectedIndex = index;
                    }
                }
            }



            if (movieSet == null)
            {
                //This is a fake and should be replaced by what is picked up from isolated storage.
                ThreadPool.QueueUserWorkItem(o =>
                    {
                        Dispatcher.BeginInvoke(() =>
                        {
                            //Loading the list of movies is a potentially time-consuming process and should not block the UI Thread.
                            var movieVersion = Helpers.GetInfoFromMemory(Constants.MovieVersion);
                            var movies = Helpers.GetInfoFromMemory(Constants.MovieSet);
                            if (movieVersion == null || Int32.Parse(movieVersion.ToString()) != Constants.CurrentMovieVersion || movies == null)
                            {
                                movieSet = new List<Movie>();
                                var resource = Application.GetResourceStream(new Uri(@"/Dumsy2;component/DataSheet.csv", UriKind.Relative));
                                using (System.IO.StreamReader sr = new System.IO.StreamReader(resource.Stream))
                                {
                                    string curLine;
                                    while ((curLine = sr.ReadLine()) != null)
                                    {
                                        string[] attributes = curLine.Split(new char[]{','});
                                        if (attributes.Length == 6)
                                        {
                                            Movie movie = new Movie(attributes[0], attributes[4], attributes[2].Replace(':', ','), "-", attributes[3], attributes[1].Replace(':', ','), Guid.Parse(attributes[5]));
                                            movieSet.Add(movie);
                                        }
                                    }
                                }
                                Helpers.SetValueToStorage(Constants.MovieSet, movieSet);
                                Helpers.SetValueToStorage(Constants.MovieVersion, Constants.CurrentMovieVersion);
                            }
                            else
                            {
                                movieSet = (List<Movie>)(movies);
                            }
                        });
                        Thread.Sleep(0);
                    });
            }

            if (viewedMovies == null)
            {
                viewedMovies = (List<Guid>)(Helpers.GetInfoFromMemory(Constants.ViewedIndex));
                if (viewedMovies == null)
                {
                    viewedMovies = new List<Guid>();
                    Helpers.SetValueToStorage(Constants.ViewedIndex, viewedMovies);
                }
            }
        }

        System.Collections.ObjectModel.ObservableCollection<string> languages = null;
        List<string> difficulty = null;
        List<Movie> movieSet = null;
        Movie selectedMovie = null;

        private void Next_Click(object sender, EventArgs e)
        {
            startDatetime = null;
            endDateTime = null;

            if (movieSet != null)
            {
                string language = languages[this.SelectedLanguage.SelectedIndex].ToString();
                string difficultyLevel = difficulty[this.SelectedDifficulty.SelectedIndex].ToString();

                selectedMovie = Movie.MovieSelector(movieSet, language, difficultyLevel, viewedMovies);

                if (selectedMovie != null)
                {
                    this.MovieName.Text = selectedMovie.Title;
                    this.MoreInfo.Visibility = Visibility.Visible;
                    this.Year.Text = String.Empty;
                    this.Genre.Text = String.Empty;
                    this.Cast.Text = String.Empty;
                }
            }
        }

        private void MoreInfo_Click(object sender, RoutedEventArgs e)
        {
            if (selectedMovie != null)
            {
                this.MoreInfo.Visibility = Visibility.Collapsed;
                this.Year.Text = Constants.Year + ": " + selectedMovie.YearOfRelease;
                this.Genre.Text = Constants.Genre + ": " + selectedMovie.Genre;
                this.Cast.Text = Constants.Cast + ": " + selectedMovie.Cast.Trim();
            }
        }

        private List<Guid> viewedMovies = null;

        private void Panorama_SelectionChanged_1(object sender, SelectionChangedEventArgs e)
        {
            SetApplicationBarButtons();
        }
    }
}