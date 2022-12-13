namespace AoC.Solutions
{
    public class Day6 : ISolution
    {
        public string CalculateFirstTask(string[] input)
        {
            return GetPacketMarker(input, 4).ToString();
        }

        public string CalculateSecondTask(string[] input)
        {
            return GetPacketMarker(input, 14).ToString();
        }

        private static int GetPacketMarker(string[] input, int sequenceLength)
        {
            var inputCharacters = input[0];
            var characters = new Dictionary<char, int>();
            foreach (var character in inputCharacters.Take(sequenceLength - 1))
            {
                Add(character, characters);
            }

            for (int i = sequenceLength - 1; i != input.Length; i++)
            {
                Add(inputCharacters[i], characters);

                if (characters.Keys.Count == sequenceLength)
                {
                    return i + 1;
                }

                Subtract(inputCharacters[i - sequenceLength + 1], characters);
            }
            return -1;
        }

        private static void Add(char key, Dictionary<char, int> dict)
        {
            if (dict.ContainsKey(key))
            {
                dict[key] += 1;
            }
            else
            {
                dict[key] = 1;
            }
        }

        private static void Subtract(char key, Dictionary<char, int> dict)
        {
            if (dict[key] == 1)
            {
                dict.Remove(key);
            }
            else
            {
                dict[key] -= 1;
            }
        }
    }
}