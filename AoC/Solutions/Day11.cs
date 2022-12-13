using System.Linq.Expressions;

namespace AoC.Solutions
{
    public class Day11 : ISolution
    {
        public string CalculateFirstTask(string[] input)
        {
            return HandleMonkeyBusiness(input, iterations: 20, worryReliefFactor: true).ToString();
        }

        public string CalculateSecondTask(string[] input)
        {
            return HandleMonkeyBusiness(input, iterations: 10000, worryReliefFactor: false).ToString();
        }

        private static long HandleMonkeyBusiness(string[] input, int iterations, bool worryReliefFactor)
        {
            var monkeys = new List<Monkey>();
            long divisor = 1;
            foreach (var chunk in input.Chunk(7))
            {
                var monkey = new Monkey(chunk);
                monkeys.Add(monkey);
                divisor *= monkey.TestValue;
            }

            for (int i = 0; i != iterations; i++)
            {
                foreach (var monkey in monkeys)
                {
                    foreach (var item in monkey.Items)
                    {
                        var newValue = monkey.Compute(item);
                        if (worryReliefFactor)
                        {
                            newValue = Convert.ToInt32(Math.Floor((decimal)newValue / 3));
                        }

                        if (newValue % monkey.TestValue == 0)
                        {
                            monkeys[monkey.PositiveTarget].Items.Add(newValue % divisor);
                        }
                        else
                        {
                            monkeys[monkey.NegativeTarget].Items.Add(newValue % divisor);
                        }
                        monkey.Inspect();
                    }
                    monkey.Items.Clear();
                }
            }

            return monkeys
                .OrderByDescending(m => m.Inspected)
                .Take(2)
                .Select(m => m.Inspected)
                .Aggregate(1L, (m1, m2) => m1 * m2);
        }

        private class Monkey
        {
            public List<long> Items { get; private set; } = new List<long>();

            public long TestValue { get; private set; }

            public int PositiveTarget { get; private set; }

            public int NegativeTarget { get; private set; }

            public int Number { get; private set; }

            public long Inspected { get; private set; } = 0;

            public Func<long, long> Compute { get; private set; }

            public void Inspect()
            {
                Inspected++;
            }

            public Monkey(string[] chunk)
            {
                Number = int.Parse(chunk[0].Split(" ")[1].Replace(":", ""));

                var items = chunk[1].Split(": ")[1].Split(", ").Select(s => long.Parse(s)).ToList();
                Items = items;

                var expression = chunk[2].Split("= ")[1];
                Compute = expression.BuildExpression();

                var test = long.Parse(chunk[3].Split("divisible by ")[1]);
                TestValue = test;

                var positiveTarget = int.Parse(chunk[4].Split("throw to monkey ")[1]);
                PositiveTarget = positiveTarget;

                var negativeTarget = int.Parse(chunk[5].Split("throw to monkey ")[1]);
                NegativeTarget = negativeTarget;
            }
        }
    }

    public static class Day11Extensions
    {
        internal static Func<long, long> BuildExpression(this string expressionString)
        {
            var parts = expressionString.Split(" ");
            var oldValueArg = parts[0];
            var operation = parts[1];
            var rightArg = parts[2];

            var oldValueParameter = Expression.Parameter(typeof(long), oldValueArg);

            Expression right;
            if (long.TryParse(rightArg, out var rightConst))
            {
                right = Expression.Constant(rightConst, typeof(long));
            }
            else
            {
                right = oldValueParameter;
            }

            Expression expression;
            if (operation == "+")
            {
                expression = Expression.Add(oldValueParameter, right);
            }
            else if (operation == "*")
            {
                expression = Expression.Multiply(oldValueParameter, right);
            }
            else
            {
                throw new InvalidOperationException("Unsupported operation");
            }

            return Expression.Lambda<Func<long, long>>(expression, oldValueParameter).Compile();
        }
    }
}