using System;

namespace GenerateRandomText.Generators
{
    class GenerateRandomWord : IRandomWordGenerator
    {
        const string Chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz";
        readonly Random _random = new Random();

        private readonly int _minChars;
        private readonly int _maxChars;

        public GenerateRandomWord(int minChars, int maxChars)
        {
            this._minChars = minChars;
            this._maxChars = maxChars;
        }
        
        public char[] GetWord()
        {
            int currentMaxChars = _random.Next(_minChars - 1, _maxChars - 1);
            char [] randomWord = new char[currentMaxChars];

            for (int i = 0; i <= currentMaxChars-1; i++)
            {
                randomWord[i] = Chars[_random.Next(Chars.Length)];
            }

            return randomWord;
        }

        public string Name => "Random words";
        public bool IsEnabled => true;
    }
}
