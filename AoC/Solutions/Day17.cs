namespace AoC.Solutions
{
    public class Day17 : ISolution
    {
        public string CalculateFirstTask(string[] input)
        {
            long rocksToCalculate = 2022;
            return Simulate(input[0], rocksToCalculate).ToString();
        }

        public string CalculateSecondTask(string[] input)
        {
            long rocksToCalculate = 1000000000000;
            return Simulate(input[0], rocksToCalculate).ToString();
        }

        private static long Simulate(string input, long rocksToCalculate)
        {
            var grid = new List<List<bool>>();
            var windDirections = new Queue<Move>();

            long rockIndex = 0;
            long removed = 0;
            
            var stateSequence = new Queue<(string, RockType rockType, int movesLeft)>();
            var removedCounts = new List<long>();
            var states = new Dictionary<(string, RockType rockType, int movesLeft), long>();
            while (rockIndex < rocksToCalculate)
            {
                var (rock, rockType) = GetNextRock(rockIndex);

                for (int i = 0; i != 4; i++)
                {
                    if (CanMoveHorizontally(windDirections, rock, grid, input, out var nextMove))
                    {
                        MoveHorizontally(rock, nextMove);
                    }
                }

                var canMoveDown = CanMoveDown(rock, grid);
                while (canMoveDown)
                {
                    MoveDown(rock);
                    if (CanMoveHorizontally(windDirections, rock, grid, input, out var nextMove))
                    {
                        MoveHorizontally(rock, nextMove);
                        canMoveDown = CanMoveDown(rock, grid);
                    }
                    else
                    {
                        canMoveDown = CanMoveDown(rock, grid);
                    }
                }

                var state = string.Join("", grid.SelectMany(row => row.Select(x => x ? '#' : '.')));
                var key = (state, rockType, windDirections.Count);
                if (states.TryGetValue(key, out var rocksFell))
                {
                    var frequency = rockIndex - rocksFell;

                    // one cycle has been processed while indexing states
                    var cyclesLeft = (rocksToCalculate - rocksFell) / frequency - 1;
                    while (stateSequence.First() != key)
                    {
                        stateSequence.Dequeue();
                    }

                    var cycleTotal = removedCounts.Reverse<long>().Take(stateSequence.Count).Sum();
                    removed += cycleTotal * cyclesLeft;
                    rockIndex += frequency * cyclesLeft;
                    states.Clear();

                    var remove = AddRockToGrid(rock, grid);
                    removed += remove;
                }
                else
                {
                    var remove = AddRockToGrid(rock, grid);
                    if (remove > 0)
                    {
                        removedCounts.Add(remove);
                        removed += remove;
                        stateSequence.Enqueue(key);
                        states.Add(key, rockIndex);
                    }
                }

                rockIndex++;
            }
            return removed + grid.Count;
        }

        private static int AddRockToGrid(List<Point> rock, List<List<bool>> grid)
        {
            var baseLine = grid.Count;

            var maxAbove = rock.Max(r => r.Y);

            if (maxAbove >= 0)
            {
                for (int i = 0; i <= maxAbove; i++)
                {
                    grid.Add(Enumerable.Repeat(false, 7).ToList());
                }
            }

            int min = int.MaxValue;
            int max = int.MinValue;
            foreach (var point in rock)
            {
                min = Math.Min(baseLine + point.Y, min);
                max = Math.Max(baseLine + point.Y, max);
                grid[baseLine + point.Y][point.X] = true;
            }

            for (int i = max; i >= min; i--)
            {
                // SINGLE line blocking the way
                if (grid[i].All(x => x))
                {
                    var removed = i + 1;
                    grid.RemoveRange(0, removed);
                    return removed;
                }

                if (i - 1 < 0)
                    continue;

                // TWO lines blocking the way
                if (grid[i].Zip(grid[i - 1]).All(x => x.First || x.Second))
                {
                    grid.RemoveRange(0, i);
                    return i;
                }
            }
            return 0;
        }

        private static void MoveHorizontally(List<Point> rock, Move move)
        {
            int direction = move == Move.Left ? -1 : 1;
            for (int i = 0; i < rock.Count; i++)
            {
                rock[i] = new Point(rock[i].X + direction, rock[i].Y);
            }
        }

        private static void MoveDown(List<Point> rock)
        {
            for (int i = 0; i < rock.Count; i++)
            {
                rock[i] = new Point(rock[i].X, rock[i].Y - 1);
            }
        }

        private static bool CanMoveHorizontally(Queue<Move> moves, List<Point> rock, List<List<bool>> grid, string input, out Move nextMove)
        {
            if (moves.Count == 0)
            {
                RefillMoves(moves, input);
            }

            nextMove = moves.Dequeue();
            if (nextMove == Move.Left)
            {
                return CanMoveLeft(rock, grid);
            }
            else
            {
                return CanMoveRight(rock, grid);
            }
        }

        private static bool CanMoveLeft(List<Point> rock, List<List<bool>> grid)
        {
            var baseLine = grid.Count;
            foreach (var point in rock)
            {
                if (point.X == 0)
                    return false;

                if (point.Y < 0 && grid[baseLine + point.Y][point.X - 1])
                {
                    return false;
                }
            }
            return true;
        }

        private static bool CanMoveRight(List<Point> rock, List<List<bool>> grid)
        {
            var baseLine = grid.Count;
            foreach (var point in rock)
            {
                if (point.X == 6)
                    return false;

                if (point.Y < 0 && grid[baseLine + point.Y][point.X + 1])
                {
                    return false;
                }
            }
            return true;
        }

        private static bool CanMoveDown(List<Point> rock, List<List<bool>> grid)
        {
            var baseLine = grid.Count;
            foreach (var point in rock)
            {
                var positionFromGround = point.Y + baseLine;
                // hit ground
                if (positionFromGround == 0)
                    return false;

                if (positionFromGround > grid.Count)
                    continue;

                if (grid[positionFromGround - 1][point.X])
                {
                    return false;
                }
            }
            return true;
        }

        private static void RefillMoves(Queue<Move> moves, string input)
        {
            foreach (var ch in input)
            {
                if (ch == '>')
                {
                    moves.Enqueue(Move.Right);
                }
                else
                {
                    moves.Enqueue(Move.Left);
                }
            }
        }

        private static (List<Point>, RockType) GetNextRock(long rockCount)
        {
            var points = new List<Point>();
            switch (rockCount % 5)
            {
                case 0:
                    // line ---
                    points.Add(new Point(2, 0));
                    points.Add(new Point(3, 0));
                    points.Add(new Point(4, 0));
                    points.Add(new Point(5, 0));
                    return (points, RockType.HorizontalLine);
                case 1:
                    // cross +
                    points.Add(new Point(3, 0));
                    points.Add(new Point(2, 1));
                    points.Add(new Point(3, 1));
                    points.Add(new Point(4, 1));
                    points.Add(new Point(3, 2));
                    return (points, RockType.Cross);
                case 2:
                    // L-shape __|
                    points.Add(new Point(2, 0));
                    points.Add(new Point(3, 0));
                    points.Add(new Point(4, 0));
                    points.Add(new Point(4, 1));
                    points.Add(new Point(4, 2));
                    return (points, RockType.LShape);
                case 3:
                    // vertical line |
                    points.Add(new Point(2, 0));
                    points.Add(new Point(2, 1));
                    points.Add(new Point(2, 2));
                    points.Add(new Point(2, 3));
                    return (points, RockType.VerticalLine);
                case 4:
                    // cube |-|
                    points.Add(new Point(2, 0));
                    points.Add(new Point(2, 1));
                    points.Add(new Point(3, 0));
                    points.Add(new Point(3, 1));
                    return (points, RockType.Cube);
                default:
                    throw new Exception();
            }
        }

        private record struct Point(int X, int Y);

        private enum RockType
        {
            VerticalLine,
            HorizontalLine,
            Cross,
            Cube,
            LShape,
        }

        private enum Move
        {
            Left,
            Right
        }
    }
}