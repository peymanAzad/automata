using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace formal_language_automata
{
    class Vector : IVector
    {
        public Vector(IState state1, IState state2, string parameter)
        {
            this.State1 = state1;
            this.State2 = state2;
            this.Parameter = parameter;
        }
        public IState State1 { get; set; }
        public IState State2 { get; set; }
        public string Parameter { get; set; }
    }
}
