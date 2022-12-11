namespace AoC.Solutions
{
    public class Day1 : ISolution
    {
        public string CalculateFirstTask(string[] input)
        {
            long maxValue = 0;
            long currentValue = 0;
            foreach (var line in input)
            {
                if (string.IsNullOrEmpty(line))
                {
                    maxValue = long.Max(maxValue, currentValue);
                    currentValue = 0;
                    continue;
                }

                var value = long.Parse(line);
                currentValue += value;
            }

            if (currentValue != 0)
            {
                maxValue = long.Max(maxValue, currentValue);
            }
            return maxValue.ToString();
        }

        public string CalculateSecondTask(string[] input)
        {
            long currentValue = 0;
            var sortedList = new SortedList<long, long>(4);
            long howMany = 3;

            foreach (var line in input)
            {
                if (string.IsNullOrEmpty(line))
                {
                    AddToList(currentValue, sortedList, howMany);
                    currentValue = 0;

                    continue;
                }

                var value = long.Parse(line);
                currentValue += value;
            }

            if (currentValue != 0)
            {
                AddToList(currentValue, sortedList, howMany);
            }

            long sum = 0;
            long left = howMany;
            foreach (var (calories, numberOfElves) in sortedList.Reverse())
            {
                sum += long.Min(left, numberOfElves) * calories;
                left -= numberOfElves;
                if (left <= 0)
                    break;
            }

            return sum.ToString();
        }

        private static void AddToList(long currentValue, SortedList<long, long> list, long topCount)
        {
            if (list.ContainsKey(currentValue))
            {
                list[currentValue] += 1;
            }
            else
            {
                list.Add(currentValue, 1);
            }

            if (list.Count == topCount + 1)
            {
                list.RemoveAt(0);
            }
        }
    }
}
