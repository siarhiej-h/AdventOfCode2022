namespace AoC.Solutions
{
    public class Day9 : ISolution
    {
        public string CalculateFirstTask(string[] input)
        {
            var result = GetTailPositionsVisitedCount(input, tailsCount: 1);
            return result.ToString();
        }

        public string CalculateSecondTask(string[] input)
        {
            var result = GetTailPositionsVisitedCount(input, tailsCount: 9);
            return result.ToString();
        }

        private static int GetTailPositionsVisitedCount(string[] input, int tailsCount)
        {
            var tailPositions = new List<Position>();
            for (int i = 0; i != tailsCount; i++)
            {
                var pos = new Position { X = 0, Y = 0 };
                tailPositions.Add(pos);
            }

            var headPos = new Position { X = 0, Y = 0 };

            var positionsVisited = new HashSet<string>();
            var tailPos = tailPositions.Last();

            positionsVisited.Add($"{tailPos.X}#{tailPos.Y}");
            foreach (var line in input)
            {
                var parts = line.Split(" ");
                var direction = parts[0];
                var length = int.Parse(parts[1]);

                for (int i = 0; i != length; i++)
                {
                    switch (direction)
                    {
                        case "R":
                            headPos.X += 1;
                            break;
                        case "L":
                            headPos.X -= 1;
                            break;
                        case "U":
                            headPos.Y += 1;
                            break;
                        case "D":
                            headPos.Y -= 1;
                            break;
                        default:
                            break;
                    }

                    var head = headPos;
                    var tail = tailPositions[0];
                    for (int j = 1; j <= tailsCount; j++)
                    {
                        var (x, y) = GetTailMove(head, tail);
                        tail.X += x;
                        tail.Y += y;

                        if (j == tailPositions.Count)
                            break;
                        head = tail;
                        tail = tailPositions[j];
                    }

                    tail = tailPositions.Last();
                    var tailPosString = $"{tail.X}#{tail.Y}";
                    positionsVisited.Add(tailPosString);
                }
            }

            return positionsVisited.Count;
        }

        private static (int x, int y) GetTailMove(Position headPos, Position tailPos)
        {
            var x = headPos.X - tailPos.X;
            var y = headPos.Y - tailPos.Y;
            if (Math.Abs(x) <= 1 && Math.Abs(y) <= 1)
            {
                return (0, 0);
            }

            return (Math.Sign(x), Math.Sign(y));
        }

        private class Position
        {
            public int X { get; set; }
            public int Y { get; set; }
        }
    }
}