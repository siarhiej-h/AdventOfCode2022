namespace AoC.Solutions
{
    public class Day8 : ISolution
    {
        public string CalculateFirstTask(string[] input)
        {
            var grid = ParseInput(input);
            int result = grid.Count * 4 - 4;
            for (int i = 1; i != grid.Count - 1; i++)
            {
                var row = grid[i];
                for (int j = 1; j != row.Count - 1; j++)
                {
                    var value = row[j];
                    if (IsVisible(value, i, j, grid))
                    {
                        result += 1;
                    }
                }
            }
            return result.ToString();
        }

        public string CalculateSecondTask(string[] input)
        {
            long result = 0;
            var grid = ParseInput(input);
            for (int i = 1; i != grid.Count - 1; i++)
            {
                var row = grid[i];
                for (int j = 1; j != row.Count - 1; j++)
                {
                    var value = row[j];
                    var (up, down, left, right) = GetViewDistances(value, i, j, grid);
                    var core = up * down * left * right;
                    result = Math.Max(result, core);
                }
            }
            return result.ToString();
        }

        private static List<List<int>> ParseInput(string[] input)
        {
            var digits = new List<List<int>>();
            foreach (var line in input)
            {
                var list = new List<int>();
                foreach (var ch in line.AsEnumerable())
                {
                    var digit = (int)Char.GetNumericValue(ch);
                    list.Add(digit);
                }
                digits.Add(list);
            }

            return digits;
        }

        private static bool IsVisible(int value, int row, int column, List<List<int>> grid)
        {
            bool isVisible = true;
            for (int i = 0; i != row; i++)
            {
                if (grid[i][column] >= value)
                {
                    isVisible = false;
                    break;
                }
            }

            if (isVisible)
                return isVisible;

            isVisible = true;
            for (int i = grid.Count - 1; i > row; i--)
            {
                if (grid[i][column] >= value)
                {
                    isVisible = false;
                    break;
                }
            }
            if (isVisible)
                return isVisible;

            isVisible = true;
            var rowValues = grid[row];
            for (int j = 0; j != column; j++)
            {
                if (grid[row][j] >= value)
                {
                    isVisible = false;
                    break;
                }
            }
            if (isVisible)
                return isVisible;

            isVisible = true;
            for (int j = rowValues.Count - 1; j > column; j--)
            {
                if (grid[row][j] >= value)
                {
                    isVisible = false;
                    break;
                }
            }
            if (isVisible)
                return isVisible;

            return false;
        }

        private static (int, int, int, int) GetViewDistances(int value, int row, int column, List<List<int>> grid)
        {
            int up = 0;
            for (int i = row - 1; i >= 0; i--)
            {
                if (grid[i][column] >= value)
                {
                    up = row - i;
                    break;
                }
                else
                {
                    up++;
                }
            }

            int down = 0;
            for (int i = row + 1; i != grid.Count; i++)
            {
                if (grid[i][column] >= value)
                {
                    down = i - row;
                    break;
                }
                else
                {
                    down++;
                }
            }

            int left = 0;
            var rowValues = grid[row];
            for (int j = column - 1; j >= 0; j--)
            {
                if (grid[row][j] >= value)
                {
                    left = column - j;
                    break;
                }
                else
                {
                    left++;
                }
            }

            int right = 0;
            for (int j = column + 1; j < rowValues.Count; j++)
            {
                if (grid[row][j] >= value)
                {
                    right = j - column;
                    break;
                }
                else
                {
                    right++;
                }
            }

            return (up, down, left, right);
        }
    }
}