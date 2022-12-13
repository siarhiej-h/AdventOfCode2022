namespace AoC.Solutions
{
    public class Day12 : ISolution
    {
        public string CalculateFirstTask(string[] input)
        {
            var (grid, startPoints, endPoint) = ParseInput(input, false);
            return GetMinimumSteps(grid, startPoints, endPoint).ToString();
        }

        public string CalculateSecondTask(string[] input)
        {
            var (grid, startPoints, endPoint) = ParseInput(input, true);
            return GetMinimumSteps(grid, startPoints, endPoint).ToString();
        }

        private static int GetMinimumSteps(List<List<int>> grid, List<(int row, int column)> possibleStarts, (int row, int column) end)
        {
            int minSteps = int.MaxValue;
            var visited = new Dictionary<(int row, int col), int>();
            foreach (var start in possibleStarts)
            {
                var positions = new Stack<Position>();
                var position = new Position(start, grid);
                int movesAvailable = position.PossibleMoves.Count;

                positions.Push(position);

                while (movesAvailable > 0)
                {
                    if (visited.TryGetValue((position.Row, position.Column), out var count))
                    {
                        if (positions.Count < count)
                        {
                            visited[(position.Row, position.Column)] = positions.Count;
                        }
                    }
                    else
                    {
                        visited.Add((position.Row, position.Column), positions.Count);
                    }

                    if (position.Row - end.row == 0 && position.Column - end.column == 0)
                    {
                        minSteps = Math.Min(minSteps, positions.Count - 1);
                        position = positions.Pop();
                        movesAvailable -= position.PossibleMoves.Count;
                        position = positions.Peek();
                    }
                    else if (position.PossibleMoves.TryPop(out var next))
                    {
                        movesAvailable--;

                        if (visited.TryGetValue(next, out var longest))
                        {
                            if (longest <= positions.Count)
                                continue;
                            visited[next] = positions.Count;
                        }

                        position = new Position(next, grid);

                        movesAvailable += position.PossibleMoves.Count;
                        positions.Push(position);
                    }
                    else
                    {
                        positions.Pop();
                        position = positions.Peek();
                    }
                }
            }
            return minSteps;
        }

        private static (List<List<int>> grid, List<(int row, int column)> startPoints, (int row, int column) endPoint) ParseInput(string[] input, bool includeNonStart)
        {
            var grid = new List<List<int>>();

            var possibleStarts = new List<(int row, int column)>();
            var (endRow, endColumn) = (0, 0);

            int row = 0;
            foreach (var line in input)
            {
                int column = 0;
                var list = new List<int>();
                foreach (var ch in line)
                {
                    int height;
                    if (ch == 'S' || (ch == 'a' && includeNonStart))
                    {
                        possibleStarts.Add((row, column));
                        height = 0;
                    }
                    else if (ch == 'E')
                    {
                        endRow = row;
                        endColumn = column;
                        height = GetHeight('z');
                    }
                    else
                    {
                        height = GetHeight(ch);
                    }
                    list.Add(height);

                    column++;
                }
                grid.Add(list);
                row++;
            }
            return (grid, possibleStarts, (endRow, endColumn));
        }

        private static int GetHeight(char ch)
        {
            return ch - 'a';
        }

        private class Position
        {
            public int Row { get; private set; }
            public int Column { get; private set; }

            public Stack<(int row, int column)> PossibleMoves { get; private set; }

            public Position((int row, int column) pos, List<List<int>> grid)
            {
                Row = pos.row;
                Column = pos.column;

                var height = grid[pos.row][pos.column];
                PossibleMoves = GetPossibleMoves(height, pos.row, pos.column, grid);
            }

            private static Stack<(int row, int column)> GetPossibleMoves(int height, int row, int column, List<List<int>> grid)
            {
                Stack<(int row, int column)> moves = new Stack<(int row, int column)>();

                var rowValues = grid[row];
                var rows = grid.Count;
                var cols = grid[0].Count;

                (int, int) next;
                if (column > 0 && rowValues[column - 1] - height <= 1)
                {
                    next = (row, column - 1);
                    moves.Push(next);
                }
                if (column < cols - 1 && rowValues[column + 1] - height <= 1)
                {
                    next = (row, column + 1);
                    moves.Push(next);
                }

                if (row > 0 && grid[row - 1][column] - height <= 1)
                {
                    next = (row - 1, column);
                    moves.Push(next);
                }

                if (row < rows - 1 && grid[row + 1][column] - height <= 1)
                {
                    next = (row + 1, column);
                    moves.Push(next);
                }

                return moves;
            }
        }
    }
}