using System.Collections;
using System.Text.RegularExpressions;

namespace AoC.Solutions
{
    public partial class Day16 : ISolution
    {
        [GeneratedRegex("Valve (?<name>\\w+) has flow rate=(?<flowRate>-?\\d+); tunnels? leads? to valves? ((?<nextName>\\w+),?\\s?)+", RegexOptions.Compiled)]
        private static partial Regex RegexInput();

        private static List<Valve> GetValveTree(string[] input)
        {
            var valvesDict = new Dictionary<string, List<string>>();
            var valves = new Dictionary<string, Valve>();
            foreach (var line in input)
            {
                var match = RegexInput().Match(line);
                var flowRate = long.Parse(match.Groups["flowRate"].Value);
                var name = match.Groups["name"].Value;
                var nextLevels = match.Groups["nextName"].Captures.Select(c => c.Value).AsEnumerable();
                valvesDict.Add(name, nextLevels.ToList());

                var valve = new Valve(flowRate, name);
                valves.Add(name, valve);
            }

            foreach (var valve in valves.Values)
            {
                var nextValves = valvesDict[valve.Name];
                valve.Valves.AddRange(nextValves.Select(v => valves[v]));
            }

            return valves.Values.ToList();
        }

        public string CalculateFirstTask(string[] input)
        {
            var valves = GetValveTree(input.ToArray()).ToDictionary(v => v.Name);
            var valvesToOpen = valves.Values.Where(v => v.FlowRate > 0).ToList();
            var root = valves["AA"];
            var distances = GetDistancesDictionary(valves, valvesToOpen, root);

            return Simulate(root, valvesToOpen, distances, 1).ToString();
        }

        public string CalculateSecondTask(string[] input)
        {
            return string.Empty;
            var valves = GetValveTree(input.ToArray()).ToDictionary(v => v.Name);
            var valvesToOpen = valves.Values.Where(v => v.FlowRate > 0).ToList();
            var root = valves["AA"];
            var distances = GetDistancesDictionary(valves, valvesToOpen, root);

            return Simulate(root, valvesToOpen, distances, 2, 26).ToString();
        }

        private static long Simulate(Valve start, List<Valve> valves, Dictionary<string, Dictionary<string, int>> distances, int players, int minutesLeftStart = 30)
        {
            var allOpen = (int)Math.Pow(2, valves.Count) - 1;
            var pressure = SimulatePlayers(start, valves, allOpen, distances, players, minutesLeftStart);
            return pressure;
        }

        private static long SimulatePlayers(Valve start, List<Valve> valves, int allOpenBitMap, Dictionary<string, Dictionary<string, int>> distances, int players, int minutesLeftStart)
        {
            long totalPressure = 0;
            if (players > 2)
            {
                totalPressure = SimulatePlayers(start, valves, allOpenBitMap / 2, distances, players - 1, minutesLeftStart);
            }

            if (players == 1)
            {
                return GetMaxPressure(start, valves, allOpenBitMap, distances, minutesLeftStart);
            }

            long max = 0;
            for (int state = allOpenBitMap - 1; state >= allOpenBitMap / 2 + 1; state--)
            {
                var complement = state ^ allOpenBitMap;

                var p1 = GetMaxPressure(start, valves, state, distances, minutesLeftStart);
                var p2 = GetMaxPressure(start, valves, complement, distances, minutesLeftStart);

                max = Math.Max(max, p1 + p2);
            }

            return totalPressure + max;
        }

        private static long GetMaxPressure(Valve start, List<Valve> valves, int openValves, Dictionary<string, Dictionary<string, int>> distances, int minutesLeftStart)
        {
            var stack = new Stack<(Valve start, long pressure, int openValves, int timeLeft)>();
            stack.Push((start, 0L, openValves, minutesLeftStart));

            long maxPressure = 0L;
            while (stack.Any())
            {
                (Valve valve, long pressure, int valvesState, int timeLeft) = stack.Pop();

                var vIndex = valve.Name == "AA" ? -1 : valves.IndexOf(valve);

                if (pressure > maxPressure)
                    maxPressure = pressure;

                var valvesToVisit = new BitArray(new[] { valvesState });
                foreach (var (nextValve, index) in valves.Select((v, i) => (v, i)).Where((v, i) => valvesToVisit[i]))
                {
                    var nextTimeLeft = timeLeft - distances[valve.Name][nextValve.Name] - 1;
                    if (nextTimeLeft > 0)
                    {
                        var newValvesToVisit = (BitArray)valvesToVisit.Clone();
                        newValvesToVisit.Set(index, false);
                        var intValue = ConvertToInt(newValvesToVisit);

                        var nextPressure = pressure + nextValve.FlowRate * nextTimeLeft;
                        stack.Push((nextValve, nextPressure, intValue, nextTimeLeft));
                    }
                }
            }

            return maxPressure;
        }

        private static int Distance(Dictionary<string, Valve> valves, string start, string target)
        {
            var visited = new HashSet<string>();
            return DistanceX(valves, new List<string>() { start }, target, visited, 0);
        }

        private static int DistanceX(Dictionary<string, Valve> valves, List<string> start, string target, HashSet<string> visited, int distance)
        {
            if (start.Any(s => s == target))
            {
                return distance;
            }

            foreach (var valve in start)
            {
                visited.Add(valve);
            }

            start = start.SelectMany(s => valves[s].Valves.Select(v => v.Name)).Where(v => !visited.Contains(v)).ToList();

            distance++;
            return DistanceX(valves, start, target, visited, distance);
        }

        private static Dictionary<string, Dictionary<string, int>> GetDistancesDictionary(Dictionary<string, Valve> valves, List<Valve> valvesToOpen, Valve root)
        {
            var dict = new Dictionary<string, Dictionary<string, int>>();
            foreach (var valve in valvesToOpen.Union(new[] { root }))
            {
                var dictInner = new Dictionary<string, int>();
                foreach (var valveToOpen in valvesToOpen)
                {
                    var distance = Distance(valves, valve.Name, valveToOpen.Name);
                    dictInner.Add(valveToOpen.Name, distance);
                }
                dict.Add(valve.Name, dictInner);
            }

            return dict;
        }

        private static int ConvertToInt(BitArray bitArray)
        {
            if (bitArray.Length > 32)
                throw new ArgumentException("Argument length shall be at most 32 bits.");

            int[] array = new int[1];
            bitArray.CopyTo(array, 0);
            return array[0];
        }

        private class Valve
        {
            public long FlowRate { get; set; }

            public string Name { get; set; }

            public List<Valve> Valves { get; set; } = new List<Valve>();

            public Valve(long flowRate, string name)
            {
                FlowRate = flowRate;
                Name = name;
            }
        }
    }
}