namespace AoC.Solutions
{
    public class Day2 : ISolution
    {
        public string CalculateFirstTask(string[] input)
        {
            long total = 0;
            foreach (var line in input)
            {
                var choices = line.Split(" ");
                var first = choices[0];
                var second = choices[1];

                var opponentsHand = first.ShapeFromString();
                var yourHand = second.ShapeFromString();
                var outcome = GetOutcome(yourHand, opponentsHand);
                total += outcome.GetValue() + yourHand.GetValue();
            }
            return total.ToString();
        }

        public string CalculateSecondTask(string[] input)
        {
            long total = 0;
            foreach (var line in input)
            {
                var choices = line.Split(" ");
                var first = choices[0];
                var second = choices[1];

                var opponentsHand = first.ShapeFromString();
                var outcome = second.OutcomeFromString();
                var yourHand = GetYourHand(opponentsHand, outcome);
                total += outcome.GetValue() + yourHand.GetValue();
            }
            return total.ToString();
        }

        static Outcome GetOutcome(Shape yourHand, Shape opponentsHand)
        {
            return (yourHand, opponentsHand) switch
            {
                (Shape.Rock, Shape.Scissors) => Outcome.Victory,
                (Shape.Paper, Shape.Rock) => Outcome.Victory,
                (Shape.Scissors, Shape.Paper) => Outcome.Victory,
                (Shape x, Shape y) when x == y => Outcome.Draw,
                _ => Outcome.Loss,
            };
        }

        static Shape GetYourHand(Shape opponentsHand, Outcome outcome)
        {
            return (opponentsHand, outcome) switch
            {
                (Shape.Rock, Outcome.Victory) => Shape.Paper,
                (Shape.Rock, Outcome.Loss) => Shape.Scissors,
                (Shape.Paper, Outcome.Victory) => Shape.Scissors,
                (Shape.Paper, Outcome.Loss) => Shape.Rock,
                (Shape.Scissors, Outcome.Victory) => Shape.Rock,
                (Shape.Scissors, Outcome.Loss) => Shape.Paper,
                (Shape x, Outcome.Draw) => x,
                _ => throw new ArgumentException("Unknown Shape and Outcome combination"),
            };
        }
    }

    public enum Shape
    {
        Rock,
        Paper,
        Scissors
    }

    public enum Outcome
    {
        Draw,
        Victory,
        Loss
    }

    static class Extensions
    {
        public static int GetValue(this Shape s1)
        {
            return s1 switch
            {
                Shape.Rock => 1,
                Shape.Paper => 2,
                Shape.Scissors => 3,
                _ => throw new ArgumentException("Unknown Shape"),
            };
        }

        public static int GetValue(this Outcome o1)
        {
            return o1 switch
            {
                Outcome.Victory => 6,
                Outcome.Draw => 3,
                Outcome.Loss => 0,
                _ => throw new ArgumentException("Unknown Outcome"),
            };
        }

        public static Shape ShapeFromString(this string s)
        {
            return s switch
            {
                "A" => Shape.Rock,
                "B" => Shape.Paper,
                "C" => Shape.Scissors,
                "X" => Shape.Rock,
                "Y" => Shape.Paper,
                "Z" => Shape.Scissors,
                _ => throw new ArgumentException("Unknown string for shape"),
            };
        }

        public static Outcome OutcomeFromString(this string s)
        {
            return s switch
            {
                "X" => Outcome.Loss,
                "Y" => Outcome.Draw,
                "Z" => Outcome.Victory,
                _ => throw new ArgumentException("Unknown string for outcome"),
            };
        }
    }

}
