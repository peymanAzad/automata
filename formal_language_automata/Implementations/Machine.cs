using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace formal_language_automata
{
    class Machine : IMachine
    {
        public Machine()
        {
            Alphabet = new List<string>();
            States = new List<IState>();
            Vectors = new List<IVector>();
        }
        public Machine(string path):this()
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
                        var alphabet = parts[1].Split(',');
                        Alphabet.AddRange(alphabet);
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
                    var relations = Vectors.Where(t => t.State1 == state || t.State2 == state).ToList();
                    foreach (var relation in relations)
                    {
                        Vectors.Remove(relation);
                    }
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
                var dstates = vectors.Where(t => t.State1 == t.State2 && !t.State2.IsFinal && !t.State2.IsStart).Select(s => s.State1).Distinct().ToList();
                for(var i = 0; i < dstates.Count(); i++)
                {
                    var d = dstates[i];
                    var relations = vectors.RemoveAll(t => t.State1 == d || t.State2 == d);
                    States.Remove(d);
                }
                
                return vectors;
            }
            catch(Exception ex)
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

        public static IMachine Nfa2Dfa(IMachine nfa)
        {
            var dfa = new Machine();
            dfa.Alphabet = nfa.Alphabet;
            var startstates = nfa.States.Where(t => t.IsStart).ToList();
            ProcessVectors(dfa, startstates, nfa);
            AddDState(dfa);
            return dfa;
        }

        private static IState ProcessVectors(IMachine machine, List<IState> currentStates, IMachine nfa)
        {
            var state = new State()
            {
                IsStart = currentStates.All(t => t.IsStart),
                IsFinal = currentStates.Any(t => t.IsFinal),
                Name = String.Concat(currentStates.Select(s => s.Name).Distinct())
            };
            machine.AddState(state);
            var vectorsG = nfa.Vectors.Where(t => currentStates.Contains(t.State1)).GroupBy(g => g.Parameter);
            foreach (var vectors in vectorsG)
            {
                var name = String.Concat(vectors.Select(s => s.State2.Name).Distinct());
                var newstate = machine.States.SingleOrDefault(a => a.Name == name);
                if (newstate == null)
                {
                    newstate = ProcessVectors(machine, vectors.Select(t => t.State2).ToList(), nfa);
                }
                machine.AddVector(state, newstate, vectors.Key);
            }
            return state;
        }

        private static void AddDState(IMachine machine)
        {
            var numStates = machine.States.Count;
            var numVectors = machine.Vectors.Count;
            if (numVectors < numStates*machine.Alphabet.Count)
            {
                var dState = new State() {IsFinal = false, IsStart = false, Name = "D"};
                machine.AddState(dState);
                foreach (var alpha in machine.Alphabet)
                {
                    machine.AddVector(dState, dState, alpha);
                }
                var vecG = machine.Vectors.GroupBy(g => g.State1);
                foreach (var vectors in vecG)
                {
                    if (vectors.Count() < machine.Alphabet.Count)
                    {
                        var inputs = vectors.Select(s => s.Parameter).Distinct();
                        var dInputs = machine.Alphabet.Where(t => !inputs.Contains(t)).ToList().Distinct();
                        foreach (var dInput in dInputs)
                        {
                            machine.AddVector(vectors.Key, dState, dInput);
                        }
                    }
                }
            }
        }
    }
}
