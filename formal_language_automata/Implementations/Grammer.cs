using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace formal_language_automata
{
    class Grammer : IGrammer
    {
        public Grammer()
        {
            
        }

        public Grammer(List<IVector> rules)
        {
            this.Vectors = rules;
        }
        public List<IVector> Vectors { get; set; }
        public bool AddRule(IVector rule)
        {
            throw new NotImplementedException();
        }

        public bool RemoveRule(IVector rule)
        {
            throw new NotImplementedException();
        }

        public override string ToString()
        {
            string result = String.Empty;
            IVector start;
            start = Vectors.Single(t => t.State1.IsStart);
            Vectors.Remove(start);
            Vectors.Insert(0, start);
            var groups = Vectors.GroupBy(t => t.State1);
            foreach (var group in groups)
            {
                foreach (var vector in group)
                {
                    result += String.Format("{0} -> {1} {2}\n", vector.State1.Name, vector.Parameter, vector.State2.Name);
                }
            }
            var finals = Vectors.Where(t => t.State2.IsFinal).Select(t => t.State2).Distinct();
            foreach (var final in finals)
            {
                result += String.Format("{0} -> λ\n", final.Name);
            }
            return result;
        }
    }
}
