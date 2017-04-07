using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ItsTheFNDictionary.Extensions
{
    public static class StringExtensions
    {
        /// <summary>
        /// Words to add 'fuckin' after.
        /// </summary>
        public static string[] Words => new[]
        {
            "a ",
            "any ",
            "he ",
            "she ",
            "it ",
            "the ",
            "its ",
            "that's ",
            "what's ",
            "it's ",
            "that is ",
            "what is ",
            "it is "
        };

        /// <summary>
        /// Random error messages for users.
        /// </summary>
        public static string[] ErrorMessages => new[]
        {
            "This shit don't exist. Try again.",
            "Bro or Broette, why you no speak english?",
            "Come on now. You know that's not a word.",
            "Everytime you spell a word wrong, a kitten dies.",
            "You gone done broke the search, damn...",
            "Don't tell you me you put multiple words again.",
            "No... just no...",
            "Just reading the error messages now?",
            "Hahaha, that's not a word...",
            "Ok, fuck off, you know I can't look that up.",
            "An error occurred while displaying the previous error."
        };

        /// <summary>
        /// Classes of words to check against.
        /// </summary>
        public static string[] WordClasses => new[]
        {
            "noun",
            "verb",
            "adjective",
            "adverb",
            "pronoun",
            "preposition",
            "conjunction",
            "determiner",
            "exclamation",
            "interjection",
            "slang",
            "article"
        };

        /// <summary>
        /// Random object for randomizing the error messages.
        /// </summary>
        public static Random Randomizer => new Random();

        /// <summary>
        /// Safely replace string.
        /// </summary>
        /// <param name="input"></param>
        /// <param name="find"></param>
        /// <param name="replace"></param>
        /// <param name="matchWholeWord"></param>
        /// <returns></returns>
        public static string SafeReplace(this string input, string find, string replace, bool matchWholeWord)
        {
            var textToFind = matchWholeWord ? $@"\b{find}\b" : find;
            return Regex.Replace(input, textToFind, replace, RegexOptions.IgnoreCase);
        }

        /// <summary>
        /// Add fuckin to a parsed word.
        /// </summary>
        /// <param name="phrase"></param>
        /// <returns></returns>
        public static string AddFuckin(string phrase)
        {
            foreach (var t in Words)
            {
                if (phrase.IndexOf(t, StringComparison.Ordinal) != -1)
                    phrase = phrase.SafeReplace(t, t + "fuckin' ", true);
            }
            return phrase;
        }

        /// <summary>
        /// Retrieve a random error message.
        /// </summary>
        /// <returns></returns>
        public static string RandomErrorMessage()
        {
            return ErrorMessages[Randomizer.Next(0, ErrorMessages.Length)];
        }

        /// <summary>
        /// Check if the word class is contained in the the list of allowed word classes.
        /// </summary>
        /// <param name="wordClass"></param>
        /// <returns></returns>
        public static bool IsInWordClasses(string wordClass)
        {
            return WordClasses.Any(word => wordClass.ToLower().Contains(word) && !string.IsNullOrWhiteSpace(wordClass)); 
        }
    }
}
