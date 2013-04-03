using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Dumsy2
{
    public class Constants
    {
        public enum DifficultyLevel { Easy, Difficult, Random };
        public enum Language { English, Hindi };
        public const string Random = "Random";
        public const string Genre = "Genre";
        public const string Year = "Year";
        public const string Cast = "Cast";
        public const string ViewedIndex = "ViewedMovies";

        public const string LanguageSetting = "languagesetting";
        public const string DifficultySetting = "difficultysetting";

        public const int CurrentMovieVersion = 1;
        public const string MovieVersion = "MovieVersion";
        public const string MovieSet = "MovieSet";
        public const int MaxDepth = 2;
    }
}
