using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace formal_language_automata
{
    interface IMachine
    {
        List<IState> States { get; set; }
        List<IVector> Vectors { get; set; }
        List<string> Alphabet { get; set; }
        bool AddState(IState state);
        bool AddVector(IState state1, IState state2, string parameter);
        bool RemoveState(IState state);
        void RemoveDStates();
        IGrammer ToGrammer();
        string ToRegX();
    }
}
