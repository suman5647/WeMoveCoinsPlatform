using System;
using System.Linq;
using System.Text.RegularExpressions;
using WMC.Web.Utilities.Humanizer.Transformer;

namespace WMC.Web.Utilities.Humanizer
{
    public static class StringHumanizeExtensions
    {
        private static readonly Regex PascalCaseWordPartsRegex;
        private static readonly Regex FreestandingSpacingCharRegex;

        static StringHumanizeExtensions()
        {
            PascalCaseWordPartsRegex = new Regex(@"[A-Z]?[a-z]+|[0-9]+[a-z]*|[A-Z]+(?=[A-Z][a-z]|[0-9]|\b)",
                RegexOptions.IgnorePatternWhitespace | RegexOptions.ExplicitCapture | RegexOptionsUtil.Compiled);
            FreestandingSpacingCharRegex = new Regex(@"\s[-_]|[-_]\s", RegexOptionsUtil.Compiled);
        }

        static string FromUnderscoreDashSeparatedWords(string input)
        {
            return string.Join(" ", input.Split(new[] { '_', '-' }));
        }

        static string FromPascalCase(string input)
        {
            var result = string.Join(" ", PascalCaseWordPartsRegex
                .Matches(input).Cast<Match>()
                .Select(match => match.Value.ToCharArray().All(char.IsUpper) &&
                    (match.Value.Length > 1 || (match.Index > 0 && input[match.Index - 1] == ' ') || match.Value == "I")
                    ? match.Value
                    : match.Value.ToLower()));

            return result.Length > 0 ? char.ToUpper(result[0]) +
                result.Substring(1, result.Length - 1) : result;
        }

        /// <summary>
        /// Humanizes the input string; e.g. Underscored_input_String_is_turned_INTO_sentence -> 'Underscored input String is turned INTO sentence'
        /// </summary>
        /// <param name="input">The string to be humanized</param>
        /// <returns></returns>
        public static string Humanize(this string input)
        {
            // if input is all capitals (e.g. an acronym) then return it without change
            if (input.ToCharArray().All(char.IsUpper))
                return input;

            // if input contains a dash or underscore which preceeds or follows a space (or both, e.g. free-standing)
            // remove the dash/underscore and run it through FromPascalCase
            if (FreestandingSpacingCharRegex.IsMatch(input))
                return FromPascalCase(FromUnderscoreDashSeparatedWords(input));

            if (input.Contains("_") || input.Contains("-"))
                return FromUnderscoreDashSeparatedWords(input);

            return FromPascalCase(input);
        }

        /// <summary>
        /// Humanized the input string based on the provided casing
        /// </summary>
        /// <param name="input">The string to be humanized</param>
        /// <param name="casing">The desired casing for the output</param>
        /// <returns></returns>
        public static string Humanize(this string input, LetterCasing casing)
        {
            return input.Humanize().ApplyCase(casing);
        }

        public static string ApplyCase(this string input, LetterCasing casing)
        {
            switch (casing)
            {
                case LetterCasing.Title:
                    {
                        IStringTransformer[] transformers = new IStringTransformer[] { To.TitleCase };
                        return input.Transform(transformers);
                    }
                case LetterCasing.AllCaps:
                    {
                        IStringTransformer[] transformerArray3 = new IStringTransformer[] { To.UpperCase };
                        return input.Transform(transformerArray3);
                    }
                case LetterCasing.LowerCase:
                    {
                        IStringTransformer[] transformerArray2 = new IStringTransformer[] { To.LowerCase };
                        return input.Transform(transformerArray2);
                    }
                case LetterCasing.Sentence:
                    {
                        IStringTransformer[] transformerArray4 = new IStringTransformer[] { To.SentenceCase };
                        return input.Transform(transformerArray4);
                    }
            }
            throw new ArgumentOutOfRangeException("casing");
        }
    }
}