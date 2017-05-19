using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace formal_language_automata
{
    class Machine : IMachine
    {
        public Machine(string path)
        {
            string[] lines = System.IO.File.ReadAllLines(path);
            foreach (var line in lines)
            {
                var parts = line.Split(' ');
                var command = parts[0];
                switch (command)
                {
                    case "state":
                        var state = new State() {Name = parts[1], IsStart = line.Contains("-start"), IsFinal = line.Contains("-final")};
                        this.AddState(state);
                        break;
                    case "vector":
                        var name1 = parts[1];
                        var name2 = parts[2];
                        var parameter = parts[3];
                        var state1 = States.SingleOrDefault(u => u.Name == name1);
                        var state2 = States.SingleOrDefault(o => o.Name == name2);
                        if (state1 != null && state2 != null &&
                            Alphabet.Contains(parameter))
                        {
                            this.AddVector(state1, state2, parameter);
                        }
                        break;
                    case "alphabet":
                        Alphabet.Add(parts[1]);
                        break;
                }
            }
        }
        public List<IState> States { get; set; }
        public List<IVector> Vectors { get; set; }
        public List<string> Alphabet { get; set; }
        public bool AddState(IState state)
        {
            try
            {
                States.Add(state);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
            
        }

        public bool AddVector(IState state1, IState state2, string parameter)
        {
            if (States.Contains(state1) && States.Contains(state2))
            {
                if (Vectors.Any(
                    t => t.State1.Name == state1.Name && t.State2.Name == state2.Name && t.Parameter == parameter))
                {
                    return true;
                }
                
                var vec = new Vector(state1, state2, parameter);
                this.Vectors.Add(vec);
                return true;
            }
            else
            {
                return false;
            }
        }

        public bool RemoveState(IState state)
        {
            try
            {
                if (States.Contains(state))
                {
                    return States.Remove(state);
                }
                else
                {
                    return false;
                }
            }
            catch (Exception)
            {
                return false;
            }
        }

        public List<IVector> RemoveDStates()
        {
            var vectors = new List<IVector>(Vectors);
            try
            {
                foreach (var state in States)
                {
                    if (vectors.Where(t => t.State1 == state).All(a => a.State2 == state))
                    {
                        States.Remove(state);
                    }
                }
                return vectors;
            }
            catch
            {
                throw new Exception("Removing D stats failed");
            }
        }

        public IGrammer ToGrammer()
        {
            var rules = this.RemoveDStates();
            var grammer = new Grammer(rules);
            return grammer;
        }
    }
}
