using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace formal_language_automata
{
    interface IGrammer
    {
        List<IVector> Vectors { get; set; }
        bool AddRule(IVector rule);
        bool RemoveRule(IVector rule);
    }
}
