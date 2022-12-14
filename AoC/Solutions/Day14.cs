namespace AoC.Solutions
{
    public class Day14 : ISolution
    {
        public string CalculateFirstTask(string[] input)
        {
            var units = CalculateUnitsOfSand(input, false);
            return units.ToString();
        }

        public string CalculateSecondTask(string[] input)
        {
            var units = CalculateUnitsOfSand(input, true) + 1;
            return units.ToString();
        }

        private static int CalculateUnitsOfSand(string[] input, bool solidGround)
        {
            int sandCount = 0;
            var grid = PopulateCave(input, solidGround, out var sandHorizontalPosition);
            while (CanRest(grid, sandHorizontalPosition, out (int row, int col) finalPosition) && finalPosition.row > 0)
            {
                grid[finalPosition.row][finalPosition.col] = true;
                sandCount++;
            }

            return sandCount;
        }

        private static bool CanRest(List<List<bool>> grid, int sandColumn, out (int row, int col) restPoint)
        {
            restPoint = (0, 0);

            int nextRow = 1;
            int col = sandColumn;

            var gridWidth = grid[0].Count;
            while (nextRow != grid.Count)
            {
                // hit solid ground
                if (grid[nextRow][col])
                {
                    // falling out of the picture
                    if (col == 0)
                        return false;

                    // roll to the left
                    if (!grid[nextRow][col - 1])
                    {
                        nextRow++;
                        col--;
                        continue;
                    }

                    if (col == gridWidth - 1)
                        return false;

                    // roll to the right
                    if (!grid[nextRow][col + 1])
                    {
                        nextRow++;
                        col++;
                        continue;
                    }

                    // really solid ground
                    if (grid[nextRow][col - 1] && grid[nextRow][col + 1])
                    {
                        restPoint = (nextRow - 1, col);
                        return true;
                    }
                }
                else
                {
                    // proceed down
                    nextRow++;
                }
            }

            return false;
        }

        private static List<List<bool>> PopulateCave(IEnumerable<string> input, bool addStableGround, out int sandSourcePosition)
        {
            int leftBorder = int.MaxValue;
            int rightBorder = int.MinValue;
            int downBorder = 0;

            var listOfPoints = new List<List<(int row, int col)>>();
            foreach (var line in input)
            {
                var points = new List<(int row, int col)>();

                var positions = line.Split(" -> ");

                foreach (var position in positions)
                {
                    var parts = position.Split(",");
                    var horizontalCoordinate = int.Parse(parts[0]);
                    if (!addStableGround)
                    {
                        rightBorder = Math.Max(rightBorder, horizontalCoordinate);
                        leftBorder = Math.Min(leftBorder, horizontalCoordinate);
                    }

                    var verticalCoordinate = int.Parse(parts[1]);
                    downBorder = Math.Max(downBorder, verticalCoordinate);

                    points.Add((row: verticalCoordinate, col: horizontalCoordinate));
                }

                listOfPoints.Add(points);
            }

            int gridWidth;
            int horizontalOffset;
            if (addStableGround)
            {
                downBorder += 2;
                gridWidth = downBorder * 2 + 1;
                horizontalOffset = gridWidth / 2 - 500;
            }
            else
            {
                gridWidth = rightBorder - leftBorder + 1;
                horizontalOffset = -leftBorder;
            }

            var gridHeight = downBorder + 1;
            var grid = new List<List<bool>>();
            for (int i = 0; i != gridHeight; i++)
            {
                var row = new List<bool>();
                grid.Add(row);
                for (int j = 0; j != gridWidth; j++)
                {
                    row.Add(false);
                }
            }

            if (addStableGround)
            {
                FillRockStructures((downBorder, 0), (downBorder, gridWidth - 1), grid);
            }

            foreach (var line in listOfPoints)
            {
                var startPoint = line.First();
                foreach (var point in line.Skip(1))
                {
                    FillRockStructures(startPoint, point, grid, horizontalOffset);
                    startPoint = point;
                }
            }

            sandSourcePosition = 500 + horizontalOffset;
            return grid;
        }

        private static void FillRockStructures((int row, int col) left, (int row, int col) right, List<List<bool>> grid, int xOffset = 0)
        {
            if (left.col == right.col)
            {
                if (left.row > right.row)
                {
                    for (int i = right.row; i <= left.row; i++)
                    {
                        grid[i][left.col + xOffset] = true;
                    }
                }
                else
                {
                    for (int i = left.row; i <= right.row; i++)
                    {
                        grid[i][left.col + xOffset] = true;
                    }
                }
            }
            else if (left.row == right.row)
            {
                if (left.col > right.col)
                {
                    for (int i = right.col; i <= left.col; i++)
                    {
                        grid[left.row][i + xOffset] = true;
                    }
                }
                else
                {
                    for (int i = left.col; i <= right.col; i++)
                    {
                        grid[left.row][i + xOffset] = true;
                    }
                }
            }
        }
    }
}