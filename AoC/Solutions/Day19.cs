using System.Text.RegularExpressions;

namespace AoC.Solutions
{
    public partial class Day19 : ISolution
    {
        [GeneratedRegex("Blueprint (?<id>\\d+): Each ore robot costs (?<ore_ore_cost>\\d+) ore. Each clay robot costs (?<clay_ore_cost>\\d+) ore. Each obsidian robot costs (?<obsidian_ore_cost>\\d+) ore and (?<obsidian_clay_cost>\\d+) clay. Each geode robot costs (?<geode_ore_cost>\\d+) ore and (?<geode_obsidian_cost>\\d+) obsidian.", RegexOptions.Compiled)]
        private static partial Regex RegexInput();

        public string CalculateFirstTask(string[] input)
        {
            var blueprints = GetBlueprints(input);
            
            long quality = 0L;
            foreach (var bp in blueprints)
            {
                var resources = new Resources(0, 0, 0, 0);
                var robots = new Robots(1, 0, 0, 0);
                resources = GetMaxGeodes(bp, robots, resources, maxMinutes: 24);
                quality += resources.Geode * bp.Id;
            }

            return quality.ToString();
        }

        public string CalculateSecondTask(string[] input)
        {
            var blueprints = GetBlueprints(input);

            long result = 1;
            foreach (var bp in blueprints.Take(3))
            {
                var resources = new Resources(0, 0, 0, 0);
                var robots = new Robots(1, 0, 0, 0);
                resources = GetMaxGeodes(bp, robots, resources, maxMinutes: 32);
                result *= resources.Geode;
            }

            return result.ToString();
        }

        private static List<Blueprint> GetBlueprints(string[] input)
        {
            var blueprints = new List<Blueprint>();
            foreach (var line in input)
            {
                var match = RegexInput().Match(line);
                var id = int.Parse(match.Groups["id"].Value);
                var ore_robot_ore = int.Parse(match.Groups["ore_ore_cost"].Value);
                var clay_robot_ore = int.Parse(match.Groups["clay_ore_cost"].Value);
                var obs_robot_ore = int.Parse(match.Groups["obsidian_ore_cost"].Value);
                var obs_robot_clay = int.Parse(match.Groups["obsidian_clay_cost"].Value);
                var geode_robot_ore = int.Parse(match.Groups["geode_ore_cost"].Value);
                var geode_robot_obsidian = int.Parse(match.Groups["geode_obsidian_cost"].Value);

                var ore = new OreRobotCost(ore_robot_ore);
                var clay = new ClayRobotCost(clay_robot_ore);
                var obs = new ObsidianRobotCost(obs_robot_ore, obs_robot_clay);
                var geode = new GeodeRobotCost(geode_robot_ore, geode_robot_obsidian);
                var bluePrint = new Blueprint(id, ore, clay, obs, geode);
                blueprints.Add(bluePrint);
            }
            return blueprints;
        }

        private static Resources GetMaxGeodes(Blueprint bp, Robots robotSetup, Resources resources, int maxMinutes = 24)
        {
            var queue = new Stack<(Robots robotSetup, Resources resources, int minutesLeft)>();
            queue.Push((robotSetup, resources, maxMinutes));

            var maxOreCost = new[] { bp.OreRobotCost.Ore, bp.ClayRobotCost.Ore, bp.ObsidianRobotCost.Ore, bp.GeodeRobotCost.Ore }.Max();
            var visited = new HashSet<(Robots robotSetup, Resources resources, int minute)>();
            Resources max = resources;
            while (queue.Any())
            {
                var state = queue.Pop();

                (var robots, resources, var minutesLeft) = state;

                // cap exceeding resources
                var oreCap = minutesLeft * maxOreCost - robots.Ore * (minutesLeft - 1);
                resources.Ore = Math.Min(resources.Ore, oreCap);

                var clayCap = minutesLeft * bp.ObsidianRobotCost.Clay - robots.Clay * (minutesLeft - 1);
                resources.Clay = Math.Min(resources.Clay, clayCap);

                var obsidianCap = minutesLeft * bp.GeodeRobotCost.Obsidian - robots.Obsidian * (minutesLeft - 1);
                resources.Obsidian = Math.Min(resources.Obsidian, obsidianCap);

                state = (robots, resources, minutesLeft);
                if (visited.Contains(state))
                {
                    continue;
                }

                visited.Add(state);

                if (resources.Geode > max.Geode)
                    max = resources;

                if (minutesLeft == 0)
                {
                    continue;
                }

                foreach (var move in GetAvailableMoves(bp, robots, resources, maxOreCost))
                {
                    var next = (move.robots, move.resources, state.minutesLeft - 1);
                    queue.Push(next);
                }
            }
            return max;
        }

        private static IEnumerable<(Robots robots, Resources resources)> GetAvailableMoves(Blueprint bp, Robots robots, Resources resources, int maxOreCost)
        {
            var newResources = new Resources(resources.Ore + robots.Ore, resources.Clay + robots.Clay, resources.Obsidian + robots.Obsidian, resources.Geode + robots.Geode);
            yield return (robots, newResources);

            if (resources.Ore >= bp.GeodeRobotCost.Ore && resources.Obsidian >= bp.GeodeRobotCost.Obsidian)
            {
                var newRobotSetup = new Robots(robots.Ore, robots.Clay, robots.Obsidian, robots.Geode + 1);
                newResources = new Resources(resources.Ore - bp.GeodeRobotCost.Ore + robots.Ore, resources.Clay + robots.Clay, resources.Obsidian - bp.GeodeRobotCost.Obsidian + robots.Obsidian, resources.Geode + robots.Geode);
                yield return (newRobotSetup, newResources);
            }
            if (resources.Ore >= bp.ObsidianRobotCost.Ore && resources.Clay >= bp.ObsidianRobotCost.Clay && robots.Obsidian < bp.GeodeRobotCost.Obsidian)
            {
                var newRobotSetup = new Robots(robots.Ore, robots.Clay, robots.Obsidian + 1, robots.Geode);
                newResources = new Resources(resources.Ore - bp.ObsidianRobotCost.Ore + robots.Ore, resources.Clay - bp.ObsidianRobotCost.Clay + robots.Clay, resources.Obsidian + robots.Obsidian, resources.Geode + robots.Geode);
                yield return (newRobotSetup, newResources);
            }
            if (resources.Ore >= bp.ClayRobotCost.Ore && robots.Clay < bp.ObsidianRobotCost.Clay)
            {
                var newRobotSetup = new Robots(robots.Ore, robots.Clay + 1, robots.Obsidian, robots.Geode);
                newResources = new Resources(resources.Ore - bp.ClayRobotCost.Ore + robots.Ore, resources.Clay + robots.Clay, resources.Obsidian + robots.Obsidian, resources.Geode + robots.Geode);
                yield return (newRobotSetup, newResources);
            }
            if (resources.Ore >= bp.OreRobotCost.Ore && robots.Ore < maxOreCost)
            {
                var newRobotSetup = new Robots(robots.Ore + 1, robots.Clay, robots.Obsidian, robots.Geode);
                newResources = new Resources(resources.Ore - bp.OreRobotCost.Ore + robots.Ore, resources.Clay + robots.Clay, resources.Obsidian + robots.Obsidian, resources.Geode + robots.Geode);
                yield return (newRobotSetup, newResources);
            }
        }

        private record class OreRobotCost(int Ore);

        private record class ClayRobotCost(int Ore);

        private record class ObsidianRobotCost(int Ore, int Clay);

        private record class GeodeRobotCost(int Ore, int Obsidian);

        private record class Blueprint(int Id, OreRobotCost OreRobotCost, ClayRobotCost ClayRobotCost, ObsidianRobotCost ObsidianRobotCost, GeodeRobotCost GeodeRobotCost);

        private record struct Resources(int Ore, int Clay, int Obsidian, int Geode);

        private record struct Robots(int Ore, int Clay, int Obsidian, int Geode);
    }
}