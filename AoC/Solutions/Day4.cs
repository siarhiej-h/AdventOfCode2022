using System.Text.RegularExpressions;

namespace AoC.Solutions
{
    public class Day4 : ISolution
    {
        private static Regex Regex = new Regex("(?<ls>\\d+)-(?<le>\\d+),(?<rs>\\d+)-(?<re>\\d+)", RegexOptions.Compiled);

        public string CalculateFirstTask(string[] input)
        {
            var total = input.Count(line =>
            {
                var match = Regex.Match(line);
                var leftStart = GetValue(match, "ls");
                var leftEnd = GetValue(match, "le");
                var rightStart = GetValue(match, "rs");
                var rightEnd = GetValue(match, "re");
                return IsFullyContained(leftStart, leftEnd, rightStart, rightEnd);
            });
            return total.ToString();
        }

        public string CalculateSecondTask(string[] input)
        {
            var total = input.Count(line =>
            {
                var match = Regex.Match(line);
                var leftStart = GetValue(match, "ls");
                var leftEnd = GetValue(match, "le");
                var rightStart = GetValue(match, "rs");
                var rightEnd = GetValue(match, "re");
                return IsOverlap(leftStart, leftEnd, rightStart, rightEnd);
            });
            return total.ToString();
        }

        private static int GetValue(Match match, string index)
        {
            return int.Parse(match.Groups[index].Value);
        }

        private static bool IsFullyContained(int leftStart, int leftEnd, int rightStart, int rightEnd)
        {
            if (leftStart == rightStart || leftEnd == rightEnd)
                return true;

            if (leftStart < rightStart && leftEnd > rightEnd)
                return true;

            if (rightStart < leftStart && rightEnd > leftEnd)
                return true;

            return false;
        }

        private static bool IsOverlap(int leftStart, int leftEnd, int rightStart, int rightEnd)
        {
            if (rightStart <= leftEnd && rightStart >= leftStart)
                return true;

            if (leftStart <= rightEnd && leftStart >= rightStart)
                return true;

            return false;
        }
    }
}