namespace AoC.Solutions
{
    public class Day20 : ISolution
    {
        public string CalculateFirstTask(string[] input)
        {
            var originalList = GetOriginalList(input);
            var mixingList = new LinkedList<State>(originalList);

            var result = GetGroveCoordinate(originalList, mixingList, 1);
            return result.ToString();
        }

        public string CalculateSecondTask(string[] input)
        {
            const long magicCoefficient = 811589153;
            var originalList = GetOriginalList(input);
            originalList = new List<State>(originalList.Select(v => new State(v.Value * magicCoefficient, v.Position)));
            var mixingList = new LinkedList<State>(originalList);

            var result = GetGroveCoordinate(originalList, mixingList, 10);
            return result.ToString();
        }

        private static List<State> GetOriginalList(string[] input)
        {
            var values = new List<State>();
            for (int index = 0; index < input.Length; index++)
            {
                string? line = input[index];
                var value = long.Parse(line);

                var state = new State(value, index);
                values.Add(state);
            }
            return values;
        }

        private static void RunMixing(List<State> originalList, LinkedList<State> mixingList)
        {
            ArgumentNullException.ThrowIfNull(mixingList.First);
            ArgumentNullException.ThrowIfNull(mixingList.Last);

            foreach (var state in originalList)
            {
                var node = mixingList.Find(state) ?? throw new Exception();
                var remove = node;
                var moves = Math.Abs(state.Value);
                moves %= (mixingList.Count - 1);
                if (state.Value > 0)
                {
                    while (moves > 0)
                    {
                        moves--;
                        node = node.Next ?? mixingList.First;
                    }

                    mixingList.AddAfter(node, state);
                    mixingList.Remove(remove);
                }
                else if (state.Value < 0)
                {
                    while (moves > 0)
                    {
                        moves--;
                        node = node.Previous ?? mixingList.Last ?? throw new Exception();
                    }
                    mixingList.AddBefore(node, state);
                    mixingList.Remove(remove);
                }
            }
        }

        private static long FindNth(int n, LinkedList<State> mixedList)
        {
            ArgumentNullException.ThrowIfNull(mixedList.First);

            n %= mixedList.Count;
            var node = GetZeroNode(mixedList.First);
            for (int i = 0; i != n; i++)
            {
                node = node.Next ?? mixedList.First;
            }

            return node.Value.Value;
        }

        private static LinkedListNode<State> GetZeroNode(LinkedListNode<State> firstNode)
        {
            while (firstNode.Value.Value != 0)
            {
                firstNode = firstNode.Next ?? throw new Exception();
            }

            return firstNode;
        }

        private static long GetGroveCoordinate(List<State> originalList, LinkedList<State> mixingList, int numberOfMixes)
        {
            for (int j = 0; j != numberOfMixes; j++)
            {
                RunMixing(originalList, mixingList);
            }

            var x1 = FindNth(1000, mixingList);
            var x2 = FindNth(2000, mixingList);
            var x3 = FindNth(3000, mixingList);

            return x1 + x2 + x3;
        }

        private record struct State(long Value, int Position);
    }
}