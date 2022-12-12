namespace AoC.Solutions
{
    public static class SolutionHelper
    {
        public static (string first, string second) GetResults(this ISolution solution, InputModel inputModel)
        {
            ArgumentException.ThrowIfNullOrEmpty(inputModel.Input);
            string[] lines = inputModel.Input.Split("\n");

            var firstResult = solution.CalculateFirstTask(lines);
            var secondResult = solution.CalculateSecondTask(lines);
            return (firstResult, secondResult);
        }
    }
}
