namespace AoC.Solutions
{
    public class Day13 : ISolution
    {
        public string CalculateFirstTask(string[] input)
        {
            var parsedInput = ParseInput(input);
            return parsedInput
                .Select((pair, idx) => (decision: IsRightOrder(pair.left, pair.right), idx))
                .Where(input => input.decision == Decision.Right)
                .Sum(input => input.idx + 1)
                .ToString();
        }

        public string CalculateSecondTask(string[] input)
        {
            var parsedInput = ParseInput(input);
            var allItems = parsedInput.Select(i => (i.left, i.ls))
                .Concat(parsedInput.Select(i => (i.right, i.rs)))
                .ToList();
            var divider1 = (Item.FromStringSegment("[[6]]"), "[[6]]");
            var divider2 = (Item.FromStringSegment("[[2]]"), "[[2]]");
            allItems.Add(divider1);
            allItems.Add(divider2);
            allItems.Sort((left, right) =>
            {
                var decision = IsRightOrder(left.Item1, right.Item1);
                if (decision == null)
                    return 0;
                if (decision == Decision.Right)
                {
                    return -1;
                }
                return 1;
            });
            return ((allItems.IndexOf(divider2) + 1) * (allItems.IndexOf(divider1) + 1)).ToString();
        }

        private static List<(List<Item> left, string ls, List<Item> right, string rs)> ParseInput(IEnumerable<string> input)
        {
            var res = new List<(List<Item> left, string l, List<Item> right, string r)>();
            foreach (var lines in input.Chunk(3))
            {
                var leftPair = Item.FromStringSegment(lines[0]);
                var rightPair = Item.FromStringSegment(lines[1]);
                res.Add((leftPair, lines[0], rightPair, lines[1]));
            }
            return res;
        }

        private static Decision? IsRightOrder(List<Item> leftItems, List<Item> rightItems)
        {
            foreach (var (l, r) in leftItems.Zip(rightItems))
            {
                var decision = IsRightOrderSingle(l, r);
                if (decision != null)
                    return decision;
            }

            if (leftItems.Count == rightItems.Count)
                return null;

            return leftItems.Count > rightItems.Count ? Decision.NotRight : Decision.Right;
        }

        private static Decision? IsRightOrderSingle(Item left, Item right)
        {
            if (left.Value.HasValue && right.Value.HasValue)
            {
                if (left.Value == right.Value)
                    return null;
                return left.Value < right.Value ? Decision.Right : Decision.NotRight;
            }

            if (left.NestedItems is not null && right.NestedItems is not null)
            {
                return IsRightOrder(left.NestedItems, right.NestedItems);
            }

            if (left.Value.HasValue && !right.Value.HasValue)
            {
                left = new Item(new List<Item> { new Item(left.Value.Value) });
                return IsRightOrderSingle(left, right);
            }

            if (!left.Value.HasValue && right.Value.HasValue)
            {
                right = new Item(new List<Item> { new Item(right.Value.Value) });
                return IsRightOrderSingle(left, right);
            }

            return null;
        }

        private enum Decision
        {
            Right,
            NotRight
        }

        private class Item
        {
            public List<Item>? NestedItems { get; private set; }

            public int? Value { get; private set; } = null;

            public Item(List<Item> nestedItems)
            {
                NestedItems = nestedItems;
            }

            public Item(int value)
            {
                Value = value;
            }

            public static List<Item> FromString(string numbers)
            {
                return numbers.Split(",")
                    .Select(x => new Item(int.Parse(x)))
                    .ToList();
            }

            public static List<Item> FromStringSegment(string segment)
            {
                if (segment.StartsWith("["))
                {
                    segment = segment.Substring(1);
                }

                if (segment.EndsWith("]"))
                {
                    segment = segment.Substring(0, segment.Length - 1);
                }

                if (string.IsNullOrEmpty(segment))
                {
                    return Enumerable.Empty<Item>().ToList();
                }

                var items = new List<Item>();
                int previousSegmentEnd = -2;
                int segmentStart = -1;
                int segmentEnd = -1;
                int openCount = 0;
                for (int i = 0; i != segment.Length; i++)
                {
                    switch (segment[i])
                    {
                        case '[':
                            if (openCount == 0)
                                segmentStart = i;
                            openCount++;
                            continue;
                        case ']':
                            openCount--;
                            if (openCount == 0)
                            {
                                if (segmentStart > previousSegmentEnd + 2)
                                {
                                    var middle = segment.Substring(previousSegmentEnd + 2, segmentStart - previousSegmentEnd - 3);
                                    var nestedItems = FromString(middle);
                                    items.AddRange(nestedItems);
                                }

                                segmentEnd = i;
                                var segmentString = segment.Substring(segmentStart, segmentEnd - segmentStart + 1);
                                var item = new Item(nestedItems: FromStringSegment(segmentString));
                                items.Add(item);
                                previousSegmentEnd = segmentEnd;
                            }
                            continue;
                        default:
                            break;
                    }
                }

                if (segment.Length > previousSegmentEnd + 2)
                {
                    var right = segment.Substring(previousSegmentEnd + 2, segment.Length - previousSegmentEnd - 2);
                    var nestedItems = FromString(right);
                    items.AddRange(nestedItems);
                }

                return items;
            }
        }
    }
}