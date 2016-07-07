using System;
using System.Globalization;
using System.Text.RegularExpressions;

namespace InvertedTomato {
    public static class StringExtensions {
        /// <summary>
        /// Implementation of PHP's substr function.
        /// </summary>
        /// <param name="target"></param>
        /// <param name="startPosition"></param>
        /// <param name="endPosition"></param>
        /// <returns></returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Substr")]
        public static string Substr(this string target, int startPosition) {
            if (null == target) {
                throw new ArgumentNullException("target");
            }

            return target.Substr(startPosition, 0);
        }

        /// <summary>
        /// Implementation of PHP's substr function.
        /// </summary>
        /// <param name="target"></param>
        /// <param name="startPosition"></param>
        /// <param name="endPosition"></param>
        /// <returns></returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Substr")]
        public static string Substr(this string target, int startPosition, int endPosition) {
            if (null == target) {
                throw new ArgumentNullException("target");
            }

            // Start
            if (startPosition < 0) {
                startPosition += target.Length; // If negative, reverse it
            }
            if (startPosition >= target.Length) {
                return ""; // If longer than string, bail
            }
            if (startPosition > 0) {
                target = target.Substring(startPosition); // Clip start
            }

            // End
            if (endPosition < 0) {
                endPosition += target.Length; // If genitive, reverse it
            }
            if (endPosition >= target.Length) {
                return target; // Longer than string, bail
            }
            if (endPosition < 0) {
                return "";
            }
            if (endPosition != 0) {
                target = target.Substring(0, endPosition); // Clip end
            }

            // return
            return target;
        }

        /// <summary>
        /// Truncate a string to be equal to or less than the string length. 
        /// </summary>
        /// <returns></returns>
        public static string Truncate(this string target, int length) {
            if (null == target) {
                throw new ArgumentNullException("target");
            }
            if (length < 0) {
                throw new ArgumentOutOfRangeException("length");
            }

            length = Math.Min(target.Length, length);
            return target.Substring(0, length);
        }

        /// <summary>
        /// Count the number of times a given character is within the string.
        /// </summary>
        /// <param name="target"></param>
        /// <param name="character"></param>
        /// <returns></returns>
        public static int CountCharacter(this string target, char character) {
            if (null == target) {
                throw new ArgumentNullException("target");
            }

            var count = 0;
            for (var i = 0; i < target.Length; i++) {
                if (character == target[i]) {
                    count++;
                }
            }

            return count;
        }

        /// <summary>
        /// Convert to sentence case.
        /// </summary>
        /// <param name="target"></param>
        /// <param name="culture"></param>
        /// <returns></returns>
        public static string ToSentenceCase(this string target, CultureInfo culture) {
            if (null == target) {
                throw new ArgumentNullException("target");
            }

            // Start by converting entire string to lower case
            var lowerCase = target.ToLower(culture);

            // Matches the first sentence of a string, as well as subsequent sentences
            var r = new Regex(@"(^[a-z])|\.\s+(.)", RegexOptions.ExplicitCapture);

            // MatchEvaluator delegate defines replacement of sentence starts to uppercase
            return r.Replace(lowerCase, s => s.Value.ToUpper(culture));
        }

        /// <summary>
        /// Converts Pascal (e.g. ThisIsAName1) to a sentence (This is a name 1)
        /// </summary>
        /// <param name="target"></param>
        /// <returns></returns>
        public static string PascalToSentence(this string target, CultureInfo culture) {
            if (null == target) {
                throw new ArgumentNullException("target");
            }

            return Regex.Replace(target, "[a-z][A-Z0-9]", m => m.Value[0] + " " + char.ToLower(m.Value[1], culture));
        }

        /// <summary>
        /// Converts Pascal (e.g. ThisIsAName1) to a title (This Is A Name 1)
        /// </summary>
        /// <param name="target"></param>
        /// <returns></returns>
        public static string PascalToTitle(this string target) {
            if (null == target) {
                throw new ArgumentNullException("target");
            }

            // TODO: This does not respect culture

            return Regex.Replace(target, "([a-z](?=[A-Z0-9])|[A-Z0-9](?=[A-Z0-9][a-z]))", "$1 ");
        }
        /// <summary>
        /// Converts the specified string to title case (except for words that are entirely in uppercase, which are considered to be acronyms).
        /// </summary>
        /// <param name="target"></param>
        /// <returns></returns>
        public static string ToTitleCase(this string target) {
            if (null == target) {
                throw new ArgumentNullException("target");
            }

            return CultureInfo.InvariantCulture.TextInfo.ToTitleCase(target);
        }

        /// <summary>
        /// Returns the v or an empty string.
        /// </summary>
        /// <param name="target"></param>
        /// <returns></returns>
        public static string ValueOrBlank(this string target) {
            if (null == target) {
                throw new ArgumentNullException("target");
            }

            return target.ValueOr("");
        }

        /// <summary>
        /// Returns the v or v.
        /// </summary>
        /// <param name="target"></param>
        /// <returns></returns>
        public static string ValueOr(this string target, string alternative) {
            if (null == target) {
                return alternative;
            }

            return target;
        }

        /// <summary>
        /// Handy access to IsNullOrEmpty.
        /// </summary>
        /// <param name="target"></param>
        /// <returns></returns>
        public static bool IsNullOrEmpty(this string target) {
            return string.IsNullOrEmpty(target);
        }
    }
}
