using System;
using System.Text.RegularExpressions;

namespace AoC.Solutions
{
    public partial class Day15 : ISolution
    {
        [GeneratedRegex("Sensor at x=(?<sx>-?\\d+), y=(?<sy>-?\\d+): closest beacon is at x=(?<bx>-?\\d+), y=(?<by>-?\\d+)", RegexOptions.Compiled)]
        private static partial Regex RegexInput();

        private static List<(Pos sensor, long distance)> GetSensorRanges(string[] input)
        {
            var sensorsRanges = new List<(Pos sensor, long distance)>();
            foreach (var line in input)
            {
                var match = RegexInput().Match(line);
                var sensorX = long.Parse(match.Groups["sx"].Value);
                var sensorY = long.Parse(match.Groups["sy"].Value);
                var sensorPos = new Pos(sensorX, sensorY);

                var beaconX = long.Parse(match.Groups["bx"].Value);
                var beaconY = long.Parse(match.Groups["by"].Value);
                var beaconPos = new Pos(beaconX, beaconY);

                var distance = Distance(sensorPos, beaconPos);
                sensorsRanges.Add((sensorPos, distance));
            }
            return sensorsRanges;
        }

        public string CalculateFirstTask(string[] input)
        {
            var sensorsRanges = GetSensorRanges(input);
            var ranges = GetRangesHorizontal(2000000, sensorsRanges, false);

            if (!ranges.Any())
            {
                return string.Empty;
            }

            return ranges.Sum(r => r.End - r.Start).ToString();
        }

        public string CalculateSecondTask(string[] input)
        {
            var sensorsRanges = GetSensorRanges(input);

            var uphillPairLeftToDown = new List<long>();
            var downhillPairRightToDown = new List<long>();
            for (var i = 0; i < sensorsRanges.Count; i++)
            {
                var (sensor1, distance1) = sensorsRanges[i];
                var r1 = new Pos(sensor1.X + distance1, sensor1.Y);

                var ruA1 = r1.Y + r1.X;
                var rdA1 = r1.Y - r1.X;

                for (var j = 0; j < sensorsRanges.Count; j++)
                {
                    if (j == i)
                        continue;

                    var (sensor2, distance2) = sensorsRanges[j];
                    var l2 = new Pos(sensor2.X - distance2, sensor2.Y);
                    var luA2 = l2.Y - l2.X;
                    var ldA2 = l2.Y + l2.X;

                    if (rdA1 - luA2 == 2)
                        downhillPairRightToDown.Add(rdA1);

                    if (ldA2 - ruA1 == 2)
                        uphillPairLeftToDown.Add(ldA2);
                }
            }
            //    /    \ 
            //   /      \  /    \
            //  /        \/      \
            // /         /\  /\   \
            // \    /\  / / /  \   \
            //  \  /  \ \/ /    \  /
            //   \/    \/\/      \/
            //   /\    /\/\      /\
            //  /  \  / /\ \    /  \
            // /    \/  \ \ \  /   /
            // \         \/  \/   /
            //  \        /\      /
            //   \      /  \    /    
            //    \    /      

            var rowsToCheck = new List<long>();
            foreach (var ld2 in uphillPairLeftToDown)
            {
                foreach (var rd1 in downhillPairRightToDown)
                {
                    //  rd1 ld2 upper cross
                    //  y = x + rdA1
                    //  y = -x + ldA2
                    //  x + rdA1 = -x + ldA2
                    //  x = (ldA2 - rdA1) / 2
                    var x = (ld2 - rd1) / 2;
                    var y = -x + ld2 - 1;
                    rowsToCheck.Add(y);
                }
            }

            // check rows where sensor ranges cross. rowsToCheck might not be the answer for non-crossing ranges, so we have to check borders.
            foreach (var row in rowsToCheck.Concat(new[] { 0L, 4000000 }))
            {
                var ranges = GetRangesHorizontal(row, sensorsRanges);

                if (ranges.Count == 2)
                {
                    return (row + 4000000 * (ranges[0].End + 1)).ToString();
                }
            }
            foreach (var column in new[] { 0L, 4000000 })
            {
                var ranges = GetRangesVertical(column, sensorsRanges);

                if (ranges.Count == 2)
                {
                    return (ranges[0].End + 1 + 4000000 * (column)).ToString();
                }
            }

            return string.Empty;
        }

        private static List<Range> GetRangesHorizontal(long row, List<(Pos sensor, long distance)> sensorsRanges, bool cap = true)
        {
            return GetRanges(row, sensorsRanges, cap, p => p.X, GetSensorDistanceToRow);
        }

        private static List<Range> GetRangesVertical(long column, List<(Pos sensor, long distance)> sensorsRanges, bool cap = true)
        {
            return GetRanges(column, sensorsRanges, cap, p => p.Y, GetSensorDistanceToColumn);
        }

        private static long GetSensorDistanceToRow(Pos sensor, long row)
        {
            return Math.Abs(row - sensor.Y);
        }

        private static long GetSensorDistanceToColumn(Pos sensor, long column)
        {
            return Math.Abs(column - sensor.X);
        }

        private static List<Range> GetRanges(long coord, List<(Pos sensor, long signalStrength)> sensorsRanges, bool cap, Func<Pos, long> getCoordinate, Func<Pos, long, long> getDistance)
        {
            var ranges = new List<Range>();
            foreach (var sensorPower in sensorsRanges)
            {
                var (sensor, signalStrength) = sensorPower;
                var signalPower = signalStrength - getDistance(sensor, coord);
                if (signalPower > 0)
                {
                    var sensorCoordinate = getCoordinate(sensor);
                    long left = sensorCoordinate - signalPower;
                    if (cap)
                    {
                        left = Math.Max(0, left);
                    }

                    long right = sensorCoordinate + signalPower;
                    if (cap)
                    {
                        right = Math.Min(4000000, right);
                    }

                    ranges.Add(new Range(left, right));
                }
            }
            MergeRanges(ranges);
            return ranges;
        }

        private static void MergeRanges(List<Range> ranges)
        {
            ranges.Sort((r1, r2) =>
            {
                if (r1.Start < r2.Start)
                    return -1;

                if (r1.Start == r2.Start)
                {
                    return r1.End < r2.End ? -1 : r1.End == r2.End ? 0 : 1;
                }

                return 1;
            });

            if (ranges.Count < 2)
            {
                return;
            }

            var current = ranges.First();
            var newRanges = new List<Range>();
            foreach (var range in ranges.Skip(1))
            {
                if (range.Start > current.End)
                {
                    newRanges.Add(current);
                    current = range;
                    continue;
                }
                else if (range.End > current.End)
                {
                    current = new Range(current.Start, range.End);
                }
            }
            newRanges.Add(current);
            ranges.Clear();
            ranges.AddRange(newRanges);
        }

        private static long Distance(Pos pos1, Pos pos2)
        {
            return Math.Abs(pos1.X - pos2.X) + Math.Abs(pos1.Y - pos2.Y);
        }
    }

    record struct Range(long Start, long End);

    record struct Pos(long X, long Y);
}