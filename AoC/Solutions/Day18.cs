namespace AoC.Solutions
{
    public class Day18 : ISolution
    {
        public string CalculateFirstTask(string[] input)
        {
            var space = GetSpace(input);
            var sides = space.Cubes.Sum(cube => GetSideCubes(cube).Count(side => !IsSideCubePresent(space.XAxis, side)));
            return sides.ToString();
        }

        public string CalculateSecondTask(string[] input)
        {
            var space = GetSpace(input);

            var globalState = new Dictionary<Position, bool>();
            var sidesExternal = space.Cubes
                .Sum(cube => GetSideCubes(cube)
                .Count(side =>
                    !IsSideCubePresent(space.XAxis, side) && HasWayOut(side, space.Bounds, space.XAxis, globalState)));
            
            return sidesExternal.ToString();
        }

        private static Space GetSpace(string[] input)
        {
            var maxX = 0;
            var maxY = 0;
            var maxZ = 0;

            var cubes = new List<Position>();
            var xAxis = new Dictionary<int, Dictionary<int, HashSet<int>>>();

            foreach (var line in input)
            {
                var coords = line.Split(',').Select(c => int.Parse(c)).ToArray();
                var pos = new Position(coords[0], coords[1], coords[2]);

                if (!xAxis.TryGetValue(pos.X, out var yAxis))
                {
                    yAxis = new Dictionary<int, HashSet<int>>();
                    xAxis[pos.X] = yAxis;
                }

                if (!yAxis.TryGetValue(pos.Y, out var zAxis))
                {
                    zAxis = new HashSet<int>();
                    yAxis[pos.Y] = zAxis;
                }

                zAxis.Add(pos.Z);

                cubes.Add(pos);
                maxX = Math.Max(maxX, pos.X);
                maxY = Math.Max(maxY, pos.Y);
                maxZ = Math.Max(maxZ, pos.Z);
            }
            var gridCoords = new Position(maxX, maxY, maxZ);

            return new Space(xAxis, cubes, gridCoords);
        }

        private static bool HasWayOut(Position position, Position gridCoordinates, Dictionary<int, Dictionary<int, HashSet<int>>> xSpace, Dictionary<Position, bool> globalState)
        {
            var visited = new HashSet<Position>();
            return HasWayOutX(new HashSet<Position> { position }, gridCoordinates, xSpace, visited, globalState);
        }

        private static bool HasWayOutX(HashSet<Position> positions, Position gridCoordinates, Dictionary<int, Dictionary<int, HashSet<int>>> xSpace, HashSet<Position> visited, Dictionary<Position, bool> globalState)
        {
            var positionsToCheck = new HashSet<Position>();
            foreach (var position in positions)
            {
                if (globalState.TryGetValue(position, out var value))
                {
                    return value;
                }

                visited.Add(position);
                if (IsOutsideTheGrid(gridCoordinates, position))
                {
                    return true;
                }

                foreach (var side in GetSideCubes(position))
                {
                    if (!visited.Contains(side) && !IsSideCubePresent(xSpace, side))
                    {
                        positionsToCheck.Add(side);
                    }
                }
            }

            if (positionsToCheck.Count == 0)
            {
                return false;
            }

            var hasWayOut = HasWayOutX(positionsToCheck, gridCoordinates, xSpace, visited, globalState);
            foreach (var pos in positions)
            {
                globalState[pos] = hasWayOut;
            }
            return hasWayOut;
        }

        private static bool IsOutsideTheGrid(Position gridCoordinates, Position position)
        {
            return position.X > gridCoordinates.X || position.X == 0
                || position.Y > gridCoordinates.Y || position.Y == 0
                || position.Z > gridCoordinates.Z || position.Z == 0;
        }

        private static bool IsSideCubePresent(Dictionary<int, Dictionary<int, HashSet<int>>> xAxis, Position side)
        {
            return xAxis.TryGetValue(side.X, out var yAxis)
                    && yAxis.TryGetValue(side.Y, out var zAxis)
                    && zAxis.Contains(side.Z);
        }

        private static IEnumerable<Position> GetSideCubes(Position position)
        {
            yield return new Position(position.X + 1, position.Y, position.Z);
            yield return new Position(position.X - 1, position.Y, position.Z);
            yield return new Position(position.X, position.Y + 1, position.Z);
            yield return new Position(position.X, position.Y - 1, position.Z);
            yield return new Position(position.X, position.Y, position.Z + 1);
            yield return new Position(position.X, position.Y, position.Z - 1);
        }

        record struct Position(int X, int Y, int Z);

        record class Space(Dictionary<int, Dictionary<int, HashSet<int>>> XAxis, List<Position> Cubes, Position Bounds);
    }
}