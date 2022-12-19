namespace AoC.Solutions
{
    public class Day12 : ISolution
    {
        public string CalculateFirstTask(string[] input)
        {
            ParseInput(input, false, out var grid, out var possibleStarts, out var end);
            return TryGetShortestPath(possibleStarts, end, grid, out var result) ? result.ToString() : string.Empty;
        }

        public string CalculateSecondTask(string[] input)
        {
            ParseInput(input, true, out var grid, out var possibleStarts, out var end);
            return TryGetShortestPath(possibleStarts, end, grid, out var result) ? result.ToString() : string.Empty;
        }

        private static bool TryGetShortestPath(HashSet<Pos> startPositions, Pos target, Grid grid, out int result, HashSet<Pos>? visited = null, int distance = 0)
        {
            visited ??= new HashSet<Pos>();

            var nextPositions = new HashSet<Pos>();
            foreach (var startPos in startPositions)
            {
                if (startPos == target)
                {
                    result = distance;
                    return true;
                }

                visited.Add(startPos);
                foreach (var nextMove in GetPossibleMoves(startPos, grid))
                {
                    if (visited.Contains(nextMove))
                        continue;

                    nextPositions.Add(nextMove);
                }
            }
            if (!nextPositions.Any())
            {
                result = -1;
                return false;
            }
            return TryGetShortestPath(nextPositions, target, grid, out result, visited, distance + 1);
        }

        private static IEnumerable<Pos> GetPossibleMoves(Pos start, Grid grid)
        {
            var height = grid[start];
            var nextPos = new Pos(start.Row, start.Col - 1);
            if (grid.TryGetValue(nextPos, out var nextHeight) && nextHeight - height <= 1)
            {
                yield return nextPos;
            }

            nextPos = new Pos(start.Row, start.Col + 1);
            if (grid.TryGetValue(nextPos, out nextHeight) && nextHeight - height <= 1)
            {
                yield return nextPos;
            }

            nextPos = new Pos(start.Row - 1, start.Col);
            if (grid.TryGetValue(nextPos, out nextHeight) && nextHeight - height <= 1)
            {
                yield return nextPos;
            }

            nextPos = new Pos(start.Row + 1, start.Col);
            if (grid.TryGetValue(nextPos, out nextHeight) && nextHeight - height <= 1)
            {
                yield return nextPos;
            }
        }

        private static int GetHeight(char ch)
        {
            return ch - 'a';
        }

        private static void ParseInput(string[] input, bool includeAllStarts, out Grid grid, out HashSet<Pos> possibleStarts, out Pos end)
        {
            grid = new Grid();
            possibleStarts = new HashSet<Pos>();
            end = new Pos(0, 0);
            int row = 0;
            foreach (var line in input)
            {
                int column = 0;
                foreach (var ch in line)
                {
                    int height;
                    var pos = new Pos(row, column);
                    if (ch == 'S')
                    {
                        possibleStarts.Add(pos);
                        height = 0;
                    }
                    else if (ch == 'a' && includeAllStarts)
                    {
                        possibleStarts.Add(pos);
                        height = 0;
                    }
                    else if (ch == 'E')
                    {
                        end = pos;
                        height = GetHeight('z');
                    }
                    else
                    {
                        height = GetHeight(ch);
                    }

                    grid.Add(pos, height);

                    column++;
                }
                row++;
            }
        }

        private record struct Pos(int Row, int Col);

        private class Grid
        {
            private Dictionary<Pos, int> HeightMap { get; set; } = new Dictionary<Pos, int>();

            public int Rows { get; private set; } = 0;

            public int Columns { get; private set; } = 0;

            public int this[Pos pos]
            {
                get => HeightMap[pos];
            }

            public void Add(Pos pos, int height)
            {
                HeightMap[pos] = height;
                this.Rows = Math.Max(this.Rows, pos.Row);
                this.Columns = Math.Max(this.Columns, pos.Col);
            }

            public bool TryGetValue(Pos pos, out int value)
            {
                return HeightMap.TryGetValue(pos, out value);
            }

            public bool ContainsKey(Pos pos)
            {
                return HeightMap.ContainsKey(pos);
            }
        }
    }
}