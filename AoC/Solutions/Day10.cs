namespace AoC.Solutions
{
    public class Day10 : ISolution
    {
        public string CalculateFirstTask(string[] input)
        {
            var instructions = ParseInput(input);
            var cyclesToCheck = new HashSet<int>() { 20, 60, 100, 140, 180, 220 };
            var (left, add) = instructions.Dequeue();
            long sum = 0;
            long strength = 1;
            for (int i = 1; i != 241; i++)
            {
                if (cyclesToCheck.Contains(i))
                {
                    sum += i * strength;
                }

                left--;
                if (left == 0)
                {
                    strength += add;                    
                    if (instructions.Any())
                    {
                        (left, add) = instructions.Dequeue();
                    }
                }
            }
            return sum.ToString();
        }

        public string CalculateSecondTask(string[] input)
        {
            var instructions = ParseInput(input);
            
            var (left, add) = instructions.Dequeue();
            long spriteCenter = 1;
            var chars = new List<char>();
            for (int i = 1; i != 241; i++)
            {
                int position = i - 1;
                position %= 40;

                if (Math.Abs(spriteCenter - position) <= 1)
                {
                    chars.Add('#');
                }
                else
                {
                    chars.Add('.');
                }

                left--;
                if (left == 0)
                {
                    spriteCenter += add;
                    if (instructions.Any())
                    {
                        (left, add) = instructions.Dequeue();
                    }
                }
            }

            return string.Join("\r\n",
                Enumerable.Range(0, 6)
                    .Select(row => string.Join("", 
                        chars.Skip(row * 40).Take(40))));
        }

        private static Queue<(int cycles, long addition)> ParseInput(string[] input)
        {
            var instructions = new Queue<(int cycles, long addition)>();
            foreach (var line in input)
            {
                if (line.StartsWith("noop"))
                {
                    instructions.Enqueue((1, 0));
                }
                else if (line.StartsWith("addx"))
                {
                    var parts = line.Split(" ");
                    var addValue = long.Parse(parts[1]);
                    instructions.Enqueue((2, addValue));
                }
            }
            return instructions;
        }
    }
}