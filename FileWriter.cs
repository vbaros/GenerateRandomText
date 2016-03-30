using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GenerateRandomText
{
    public delegate void GenerateFinishedDelegate(long elapsed);
    public delegate void ProgressDelegate(int percent);

    class FileWriter
    {
        private readonly IRandomWordGenerator _randomWordGenerator;
        private readonly string _filePath;
        private readonly int _sizeInKBs;

        public event GenerateFinishedDelegate GenerateFinished;
        public event ProgressDelegate Progress;

        public FileWriter(IRandomWordGenerator randomWordGenerator, string filePath, int sizeInKBs)
        {
            _randomWordGenerator = randomWordGenerator;
            _filePath = filePath;
            _sizeInKBs = sizeInKBs;
        }

        private void OnGenerateFinished (long miliseconds)
        {
            GenerateFinishedDelegate handler = GenerateFinished;

            if (handler != null)
                handler(miliseconds);
        }

        private void OnProgressChanged (int percent)
        {
            ProgressDelegate handler = Progress;

            if (handler != null)
                handler(percent);
        }

        //public async Task GenerateFile()
        //{
        //    await Task.Run(() =>
        //    {
        //        Stopwatch stopwatch = Stopwatch.StartNew();

        //        int maxWordsPerLine = 20;
        //        int currentSize = 0;

        //        List<string> randomWordList = new List<string>(_sizeInKBs * 1024);

        //        // generate a wordlist with random words with maximum of 20 words per line
        //        while (true)
        //        {
        //            StringBuilder line = new StringBuilder();

        //            for (int i = 1; i <= maxWordsPerLine; i++)
        //            {
        //                char[] randomWord = _randomWordGenerator.GetWord();

        //                line.Append(randomWord);

        //                // increase currentSize
        //                currentSize += randomWord.Length;

        //                if (i != maxWordsPerLine)
        //                    line.Append(" ");
        //            }

        //            // increase currentSize with spaces and 2 newline chars (\r\n)
        //            currentSize += maxWordsPerLine + 1;

        //            // Add to the list
        //            randomWordList.Add(line.ToString());

        //            // if we have enough words, return
        //            if (currentSize >= 1024 * _sizeInKBs)
        //                break;
        //        }

        //        // stop stopwatch
        //        stopwatch.Stop();

        //        // write to a file;
        //        File.WriteAllLines(_filePath, randomWordList);

        //        // raise an event
        //        OnGenerateFinished(stopwatch.ElapsedMilliseconds);
        //    });
        //}

        public async Task GenerateFile()
        {
            float preload = 100 / (1024 * _sizeInKBs);

            await Task.Run(() =>
            {
                Stopwatch stopwatch = Stopwatch.StartNew();

                int maxWordsPerLine = 20;
                int currentSize = 0;

                List<string> randomWordList = new List<string>(_sizeInKBs * 1024);

                // generate a line with random words with maximum of 20 words
                while (true)
                {
                    StringBuilder line = new StringBuilder();

                    for (int i = 1; i <= maxWordsPerLine; i++)
                    {
                        char[] randomWord = _randomWordGenerator.GetWord();

                        line.Append(randomWord);

                        // increase currentSize
                        currentSize += randomWord.Length;

                        if (i != maxWordsPerLine)
                            line.Append(" ");
                    }

                    // increase currentSize with spaces and 2 newline chars (\r\n)
                    currentSize += maxWordsPerLine + 1;

                    // raise progress event
                    //OnProgressChanged(Math.Min((int)(currentSize / preload), 100));

                    // Add to the list
                    randomWordList.Add(line.ToString());

                    // if we have enough words, return
                    if (currentSize >= 1024 * _sizeInKBs)
                    {
                        OnProgressChanged(100);
                        break;
                    }
                }

                // write to a file;
                File.WriteAllLines(_filePath, randomWordList);

                // stop stopwatch and raise an event
                stopwatch.Stop();
                OnGenerateFinished(stopwatch.ElapsedMilliseconds);
            });
        }

        public async Task GenerateFileMultithreaded()
        {
            float percentCoefficient = 100/(1024*_sizeInKBs);

            await Task.Run(() =>
            {
                Stopwatch stopwatch = Stopwatch.StartNew();

                int maxWordsPerLine = 20;
                int currentSize = 0;

                List<string> randomWordList = new List<string>(_sizeInKBs * 1024);

                // generate a line with random words with maximum of 20 words
                while (true)
                {
                    StringBuilder line = new StringBuilder();

                    for (int i = 1; i <= maxWordsPerLine; i++)
                    {
                        char[] randomWord = _randomWordGenerator.GetWord();

                        line.Append(randomWord);

                        // increase currentSize
                        currentSize += randomWord.Length;

                        if (i != maxWordsPerLine)
                            line.Append(" ");
                    }

                    // increase currentSize with spaces and 2 newline chars (\r\n)
                    currentSize += maxWordsPerLine + 1;

                    // raise progress event
                    //OnProgressChanged(Math.Min((int)(currentSize/percentCoefficient), 100));

                    // Add to the list
                    randomWordList.Add(line.ToString());

                    // if we have enough words, return
                    if (currentSize >= 1024*_sizeInKBs)
                    {
                        OnProgressChanged(100);
                        break;
                    }
                }

                // write to a file;
                File.WriteAllLines(_filePath, randomWordList);

                // stop stopwatch and raise an event
                stopwatch.Stop();
                OnGenerateFinished(stopwatch.ElapsedMilliseconds);
            });
        }
    }
}
