using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GenerateRandomText
{
    interface IRandomWordGenerator
    {
        char[] GetWord();
        string Name { get; } // Name of the Generator
        bool IsEnabled { get; }
    }
}
