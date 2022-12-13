using System.Text.RegularExpressions;

namespace AoC.Solutions
{
    public partial class Day5 : ISolution
    {
        [GeneratedRegex("move (?<count>\\d+) from (?<source_crate>\\d+) to (?<target_crate>\\d+)", RegexOptions.Compiled)]
        private static partial Regex RegexInstructions();

        [GeneratedRegex("((\\[(?<crate>\\w)\\])|(?<crate>\\s{3}))\\s?", RegexOptions.Compiled)]
        private static partial Regex RegexCrates();


        public string CalculateFirstTask(string[] input)
        {
            var parsedInput = ParseInput(input);
            foreach (var instruction in parsedInput.instructions)
            {
                var (count, source, target) = instruction;
                var sourceStack = parsedInput.stacks[source - 1];
                var targetStack = parsedInput.stacks[target - 1];
                for (int i = 0; i != count; i++)
                {
                    var value = sourceStack.Pop();
                    targetStack.Push(value);
                }
            }

            return string.Join("", parsedInput.stacks.Select(s => s.Pop()));
        }

        public string CalculateSecondTask(string[] input)
        {
            var parsedInput = ParseInput(input);
            foreach (var instruction in parsedInput.instructions)
            {
                var (count, source, target) = instruction;
                var sourceStack = parsedInput.stacks[source - 1];
                var targetStack = parsedInput.stacks[target - 1];

                var stack = new Stack<string>();
                for (int i = 0; i != count; i++)
                {
                    var value = sourceStack.Pop();
                    stack.Push(value);
                }
                foreach (var item in stack)
                {
                    targetStack.Push(item);
                }
            }
            return string.Join("", parsedInput.stacks.Select(s => s.Pop()));
        }

        private static (List<Stack<string>> stacks, List<(int count, int source, int target)> instructions) ParseInput(string[] input)
        {
            var stackLines = new List<string>();
            var instructions = new List<(int count, int source, int target)>();
            bool readInstructions = false;
            foreach (var line in input)
            {
                if (line == string.Empty)
                {
                    readInstructions = true;
                    continue;
                }

                if (readInstructions)
                {
                    var match = RegexInstructions().Match(line);
                    var count = int.Parse(match.Groups["count"].Value);
                    var source = int.Parse(match.Groups["source_crate"].Value);
                    var target = int.Parse(match.Groups["target_crate"].Value);
                    instructions.Add((count, source, target));
                }
                else
                {
                    stackLines.Add(line);
                }
            }

            var stacks = new List<Stack<string>>();
            foreach (var line in stackLines.Reverse<string>().Skip(1))
            {
                var matches = RegexCrates().Matches(line);
                if (!stacks.Any())
                {
                    for (int i = 0; i != matches.Count; i++)
                    {
                        stacks.Add(new Stack<string>());
                    }
                }

                int index = 0;
                foreach (var match in matches.AsEnumerable())
                {
                    var stack = stacks[index];

                    index++;
                    var crate = match.Groups["crate"].Value;
                    if (!string.IsNullOrWhiteSpace(crate))
                    {
                        stack.Push(crate);
                    }
                }
            }
            return (stacks, instructions);
        }
    }
}