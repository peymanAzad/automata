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

        public void RemoveDStates()
        {
            try
            {
                var loop = Vectors.Where(t => t.State1 == t.State2 && !t.State2.IsFinal && !t.State2.IsStart).Select(s => s.State1).Distinct().ToList();
                var dlist = new List<IState>();
                foreach (var state in loop)
                {
                    var flag = Alphabet.All(alpha => Vectors.Any(a => a.State1 == state && a.State2 == state && a.Parameter == alpha));
                    if (flag)
                    {
                        if (Vectors.Where(t => t.State1 == state).All(a => a.State1 == a.State2))
                        {
                            dlist.Add(state);
                        }
                    }
                }
                for(var i = 0; i < dlist.Count(); i++)
                {
                    var d = dlist[i];
                    var relations = Vectors.RemoveAll(t => t.State1 == d || t.State2 == d);
                    States.Remove(d);
                }
            }
            catch(Exception ex)
            {
                throw new Exception("Removing D stats failed");
            }
        }

        public override string ToString()
        {
            var output = String.Format("alphabet {0}\n", String.Join(",", Alphabet));
            foreach (var state in States)
            {
                output += String.Format("state {0} {1} {2}\n", state.Name, state.IsStart ? "-start" : String.Empty,
                    state.IsFinal ? "-final" : String.Empty);
            }
            foreach (var vector in Vectors)
            {
                output += String.Format("vector {0} {1} {2}\n", vector.State1.Name, vector.State2.Name, vector.Parameter);
            }
            return output;
        }

        public IGrammer ToGrammer()
        {
            this.RemoveDStates();
            var grammer = new Grammer(Vectors);
            return grammer;
        }

        public string ToRegX()
        {
            RemoveDStates();
            var finishStates = States.Where(t => t.IsFinal).ToList();
            var result = String.Empty;
            if (finishStates.Count > 1)
            {
                foreach (var finishState in finishStates)
                {
                    var machnie = new Machine
                    {
                        Vectors = new List<IVector>(Vectors),
                        States = new List<IState>(States)
                    };
                    machnie.States.ForEach(f => f.IsFinal = false);
                    machnie.States.Single(s => s.Name == finishState.Name).IsFinal = true;
                    var regx = machnie.ToRegX();
                    result += (regx == String.Empty ? "?" : regx) + "+";
                }
                result = result.Remove(result.Length - 1, 1);
            }
            else if (finishStates.Count == 1)
            {
                while (States.Count(t => !t.IsFinal && !t.IsStart) > 0)
                {
                    var middleState = States.FirstOrDefault(t => !t.IsFinal && !t.IsStart);
                    if (middleState != null)
                    {
                        var vectors1 = Vectors.Where(t => t.State1 == middleState && t.State2 != middleState).ToList();
                        var vectors2 = Vectors.Where(t => t.State2 == middleState && t.State1 != middleState).ToList();
                        foreach (var v2 in vectors2)
                        {
                            foreach (var v1 in vectors1)
                            {
                                var parameter = v2.Parameter;
                                var loop =
                                    Vectors.Where(t => t.State1 == middleState && t.State2 == middleState).ToList();
                                parameter = loop.Aggregate(parameter, (current, vl) => current + String.Format("({0})*", vl.Parameter));
                                parameter += v1.Parameter;

                                AddVector(v2.State1, v1.State2, parameter);
                            }
                        }
                        RemoveState(middleState);
                    }
                }
                var startState = States.Single(t => t.IsStart);
                var finishState = States.Single(t => t.IsFinal);

                var startStart = Vectors.Where(t => t.State1 == startState && t.State2 == startState).ToList();
                var startFinish = Vectors.Where(t => t.State1 == startState && t.State2 == finishState).ToList();
                var finishStart = Vectors.Where(t => t.State1 == finishState && t.State2 == startState).ToList();
                var finishFinish = Vectors.Where(t => t.State1 == finishState && t.State2 == finishState).ToList();

                if (startStart.Count > 0)
                {
                    result += "(";
                    result = startStart.Aggregate(result, (current, ss) => current + (ss.Parameter + "+"));
                    result = result.Remove(result.Length - 1, 1);
                    result += ")*";
                }

                if (startState == finishState) return result;

                if (startFinish.Count > 0)
                {
                    result += "(";
                    result = startFinish.Aggregate(result, (current, ss) => current + (ss.Parameter + "+"));
                    result = result.Remove(result.Length - 1, 1);
                    result += ")";
                }

                if (finishFinish.Count > 0)
                {
                    result += "(";
                    result = finishFinish.Aggregate(result, (current, ss) => current + (ss.Parameter + "+"));
                    result = result.Remove(result.Length - 1, 1);
                    result += ")*";
                }

                if (finishStart.Count > 0)
                {
                    var temp = result;
                    result += "((";
                    result = finishStart.Aggregate(result, (current, ss) => current + (ss.Parameter + "+"));
                    result = result.Remove(result.Length - 1, 1);
                    result += ")";
                    result += temp + ")*";
                }
            }
            return result;
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
                Name = String.Concat(currentStates.Select(s => s.Name).OrderBy(o => o).Distinct())
            };
            machine.AddState(state);
            var vectorsG = nfa.Vectors.Where(t => currentStates.Contains(t.State1)).GroupBy(g => g.Parameter);
            foreach (var vectors in vectorsG)
            {
                var name = String.Concat(vectors.Select(s => s.State2.Name).OrderBy(o => o).Distinct());
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
