namespace AoC.Solutions
{
    public class Day7 : ISolution
    {
        public string CalculateFirstTask(string[] input)
        {
            var directory = ParseInput(input);
            return directory.Aggregate(0L, 100000, SumSmallerThanParam).accum.ToString();
        }

        public string CalculateSecondTask(string[] input)
        {
            var directory = ParseInput(input);
            var rootSize = directory.Size();

            var required = 30000000 - (70000000 - rootSize);
            return directory.Aggregate(long.MaxValue, required, FindSmallestAndBiggerThanMinimumRequired).accum.ToString();
        }

        private static long FindSmallestAndBiggerThanMinimumRequired(long acc, long minRequired, long size)
        {
            return size >= minRequired ? Math.Min(acc, size) : acc;
        }

        private static long SumSmallerThanParam(long acc, long param, long size)
        {
            return size <= param ? acc + size : acc;
        }

        private static Directory ParseInput(string[] input)
        {
            var rootDir = new Directory("/");
            var currentDir = rootDir;

            foreach (var line in input)
            {
                if (line.StartsWith("$"))
                {
                    var (command, arg) = line.CommandFromString();
                    switch (command)
                    {
                        case Command.CD_OUTER:
                            currentDir = rootDir;
                            break;
                        case Command.LS:
                            break;
                        case Command.CD_OUT:
                            if (currentDir.Parent != null)
                                currentDir = currentDir.Parent;
                            break;
                        case Command.CD_IN:
                            currentDir = currentDir.Directories.First(d => d.Path == arg);
                            break;
                        default:
                            throw new ArgumentException("Unknown Command");
                    }
                }
                else
                {
                    if (line.StartsWith("dir"))
                    {
                        var name = line.Split(" ")[1];
                        currentDir.AddDir(name);
                    }
                    else
                    {
                        var parts = line.Split(" ");
                        var sizeStr = parts[0];
                        var fileName = parts[1];
                        var size = long.Parse(sizeStr);
                        currentDir.AddFile(size);
                    }
                }
            }

            return rootDir;
        }
    }

    public enum Command
    {
        CD_OUTER,
        CD_IN,
        CD_OUT,
        LS
    }

    public static class Day7Extensions
    {
        public static (Command, string arg) CommandFromString(this string s1)
        {
            return s1 switch
            {
                "$ cd /" => (Command.CD_OUTER, string.Empty),
                "$ cd .." => (Command.CD_OUT, string.Empty),
                string x when x.Contains("cd") => GetCdCommand(s1),
                "$ ls" => (Command.LS, string.Empty),
                _ => throw new ArgumentException("Unknown Command"),
            };
        }

        private static (Command command, string arg) GetCdCommand(string s1)
        {
            var parts = s1.Split(" ");
            if (parts.Length == 3 && parts[1] == "cd")
            {
                return (Command.CD_IN, parts[2]);
            }

            throw new ArgumentException("Unknown Command");
        }
    }

    public class Directory
    {
        public string Path { get; private set; }

        public List<Directory> Directories { get; private set; } = new List<Directory>();

        public Directory? Parent { get; private set; }

        public List<long> Files { get; private set; } = new List<long>();

        public void AddFile(long size)
        {
            Files.Add(size);
        }

        public Directory(string name, Directory? parent = null)
        {
            Path = name;
            Parent = parent;
        }

        public Directory AddDir(string name)
        {
            var dir = new Directory(name, this);
            Directories.Add(dir);
            return dir;
        }

        public (long accum, long size) Aggregate(long accum, long param, Func<long, long, long, long> accumFunc)
        {
            long size = Files.Sum();
            foreach (var dir in Directories)
            {
                (accum, long dirSize) = dir.Aggregate(accum, param, accumFunc);
                size += dirSize;
            }

            accum = accumFunc(accum, param, size);

            return (accum, size);
        }

        public long Size()
        {
            long size = 0;
            foreach (var dir in Directories)
            {
                size += dir.Size();
            }

            size += Files.Sum();
            return size;
        }
    }
}