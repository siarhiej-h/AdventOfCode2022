namespace AoC.Solutions
{
    public class Day21 : ISolution
    {
        public string CalculateFirstTask(string[] input)
        {
            var monkeys = GetMonkeys(input);
            var root = monkeys["root"];

            return CalculateMonkey(root, monkeys).ToString();
        }

        public string CalculateSecondTask(string[] input)
        {
            var monkeys = GetMonkeys(input);

            var human = monkeys["humn"];
            human.Value = null;

            var root = monkeys["root"];
            root.Value = 0;
            ArgumentNullException.ThrowIfNull(root.Op);
            root.Op = new Op(
                root.Op.Formula.Replace("+", "-"),
                "-",
                root.Op.LeftName,
                root.Op.RightName);

            return InvertedCalc(human, monkeys, inverseFirst: true).ToString();
        }

        private static Dictionary<string, Monkey> GetMonkeys(string[] input)
        {
            var monkeys = new Dictionary<string, Monkey>();
            foreach (var line in input)
            {
                var parts = line.Split(": ");
                var name = parts[0];

                Monkey monkey;
                if (long.TryParse(parts[1], out var value))
                {
                    monkey = new Monkey(name, value: value);
                }
                else
                {
                    monkey = new Monkey(name, formula: parts[1]);
                }
                monkeys.Add(name, monkey);
            }
            return monkeys;
        }

        private static long InvertedCalc(Monkey monkey, Dictionary<string, Monkey> monkeys, HashSet<string?>? needInversion = null, bool inverseFirst = false)
        {
            needInversion ??= new HashSet<string?>();

            if (inverseFirst || needInversion.Contains(monkey.Op?.LeftName) || needInversion.Contains(monkey.Op?.RightName))
            {
                needInversion.Add(monkey.Name);

                var monkeyUpTheYellingChain = monkeys.Values.First(m => m.Op?.LeftName == monkey.Name || m.Op?.RightName == monkey.Name);

                var formula = monkeyUpTheYellingChain.Op?.Formula ?? throw new Exception();
                var invertedOp = InvertFormula(monkey.Name, monkeyUpTheYellingChain.Op, monkeyUpTheYellingChain.Name, out string leftArg, out string rightArg);

                var left = monkeys[leftArg];
                left.Value ??= InvertedCalc(left, monkeys, needInversion);

                var right = monkeys[rightArg];
                right.Value ??= InvertedCalc(right, monkeys, needInversion);

                var calculatedValue = monkey.Calculate(left.Value.Value, right.Value.Value, invertedOp);
                return calculatedValue;
            }

            if (monkey.Value != null)
            {
                return monkey.Value.Value;
            }

            return CalculateMonkey(monkey, monkeys);
        }

        private static string InvertFormula(string targetMonkeyName, Op op, string main, out string leftMonkey, out string rightMonkey)
        {
            (_, var operation, leftMonkey, rightMonkey) = op;

            if (targetMonkeyName == leftMonkey)
            {
                leftMonkey = main;
                return InvertOp(operation);
            }

            switch (operation)
            {
                case "+":
                    (leftMonkey, rightMonkey) = (main, leftMonkey);
                    return "-";
                case "*":
                    (leftMonkey, rightMonkey) = (main, leftMonkey);
                    return "/";
                case "-":
                    rightMonkey = main;
                    return "-";
                case "/":
                    rightMonkey = main;
                    return "/";
                default:
                    throw new Exception();
            }
        }

        private static string InvertOp(string op)
        {
            return op switch
            {
                "+" => "-",
                "-" => "+",
                "*" => "/",
                "/" => "*",
                _ => throw new Exception(),
            };
        }

        private static long CalculateMonkey(Monkey root, Dictionary<string, Monkey> monkeys)
        {
            var stack = new Stack<Monkey>();
            stack.Push(root);

            while (stack.Count > 0)
            {
                var current = stack.Pop();

                var left = monkeys[current.Op?.LeftName ?? throw new Exception()];
                var right = monkeys[current.Op?.RightName ?? throw new Exception()];

                if (current.Op?.Formula != null)
                {
                    if ((left.Value == null || right.Value == null))
                    {
                        stack.Push(current);
                        if (left.Value == null)
                        {
                            stack.Push(left);
                        }

                        if (right.Value == null)
                        {
                            stack.Push(right);
                        }
                    }
                    else
                    {
                        current.Value = current.Calculate(left.Value.Value, right.Value.Value);
                    }
                }
            }

            return root.Value ?? throw new Exception();
        }

        private class Monkey
        {
            public Monkey(string name, long? value = null, string? formula = null)
            {
                Name = name;
                Value = value;
                if (formula != null)
                {
                    var parts = formula.Split(" ");
                    var op = new Op(formula, parts[1], parts[0], parts[2]);
                    Op = op;
                }
            }

            public long Calculate(long left, long right, string? operation = null)
            {
                return (operation ?? Op?.Operation) switch
                {
                    "+" => left + right,
                    "-" => left - right,
                    "*" => left * right,
                    "/" => left / right,
                    _ => throw new InvalidOperationException("Unsupported operation")
                };
            }

            public string Name { get; set; }

            public long? Value { get; set; }

            public Op? Op { get; set; }
        }

        private record Op(string Formula, string Operation, string LeftName, string RightName);
    }
}