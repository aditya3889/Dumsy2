using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
namespace Dumsy2
{
    [DataContract]
    public class Movie
    {
        [DataMember]
        public string Title { get; set; }
        [DataMember]
        public string Language { get; set; }
        [DataMember]
        public string Genre { get; set; }
        [DataMember]
        public string YearOfRelease { get; set; }
        [DataMember]
        public string Level { get; set; }
        [DataMember]
        public string Cast { get; set; }
        [DataMember]
        public Guid MovieIdentifier { get; set; }

        public static string RemoveSpecialCharacters(string str)
        {
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < str.Length; i++)
            {
                if ((str[i] >= '0' && str[i] <= '9') || (str[i] >= 'A' && str[i] <= 'z' || (str[i] == '.' || str[i] == ',' || str[i] == ' ')))
                    sb.Append(str[i]);
            }

            return sb.ToString();
        }
 
        public Movie(string title, string language, string genre, string yearOfRelease, string level, string cast, Guid movieIdentifier)
        {
            Title = title;
            Language = language;
            Genre = genre;
            YearOfRelease = yearOfRelease;
            Level = level;
            Cast = RemoveSpecialCharacters(cast);
            MovieIdentifier = movieIdentifier;
        }

        public static Movie MovieSelector(List<Movie> movies, string language, string difficultyLevel, List<Guid> movieIndex)
        {
            return MovieSelector(movies, language, difficultyLevel, movieIndex, 0);
        }

        public static Movie MovieSelector(List<Movie> movies, string language, string difficultyLevel, List<Guid> movieIndex, int recursionDepth)
        {
            if (recursionDepth >= Constants.MaxDepth)
            {
                return null;
            }

            Movie[] selectedMovies;

            selectedMovies = movies.Where(t => !movieIndex.Contains(t.MovieIdentifier)).ToArray();

            if (!language.Equals(Constants.Random, StringComparison.InvariantCultureIgnoreCase))
            {
                selectedMovies = selectedMovies.Where(t => t.Language == language).Select(t => t).ToArray();
            }

            if (!difficultyLevel.Equals(Constants.Random, StringComparison.InvariantCultureIgnoreCase))
            {
                selectedMovies = selectedMovies.Where(t => t.Level.Equals(difficultyLevel, StringComparison.InvariantCultureIgnoreCase)).Select(t => t).ToArray();
            }

            if (selectedMovies.Any())
            {
                int numMovies = selectedMovies.Count();
                Random r = new Random();
                if (numMovies != 0)
                {
                    Movie returnedMovie = selectedMovies[r.Next(0, numMovies)];
                    movieIndex.Add(returnedMovie.MovieIdentifier);
                    Helpers.SetValueToStorage(Constants.ViewedIndex, movieIndex);
                    return returnedMovie;
                }
                else
                {
                    movieIndex.Clear();
                    Helpers.SetValueToStorage(Constants.ViewedIndex, movieIndex); //Couldn't find any movie matching the criteria.
                }
            }
            else
            {
                movieIndex.Clear();
                Helpers.SetValueToStorage(Constants.ViewedIndex, movieIndex); //Couldn't find any movie matching the criteria.
            }

            return MovieSelector(movies, language, difficultyLevel, movieIndex, recursionDepth + 1);
        }
    }
}
