namespace AoC.Solutions
{
    public class Day3 : ISolution
    {
        public string CalculateFirstTask(string[] input)
        {
            var sum = input.Sum(line =>
                    new HashSet<char>(line.Take(line.Length / 2))
                        .Intersect(new HashSet<char>(line.Reverse().Take(line.Length / 2)))
                    .Sum(GetPriority));

            return sum.ToString();
        }

        public string CalculateSecondTask(string[] input)
        {
            var sum = input.Chunk(3).Sum(chunk =>
                chunk.Skip(1)
                .Aggregate(
                    new HashSet<char>(chunk.First()),
                    (ac, line) =>
                    {
                        ac.IntersectWith(new HashSet<char>(line));
                        return ac;
                    }).Sum(GetPriority));
            return sum.ToString();
        }

        private static int GetPriority(char character)
        {
            if (char.IsLower(character))
            {
                return character - 'a' + 1;
            }

            return character - 'A' + 27;
        }
    }
}