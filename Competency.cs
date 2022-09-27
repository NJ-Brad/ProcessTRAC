using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProcessTRAC
{
    internal class Competency
    {
        public string Section = "";
        public string Category = "";
        public string Name = "";
        public string Text = "";
        public List<string> Examples = new List<string>();
        public int SelfRating = -1;
        public List<int> OtherRatings = new List<int>();
    }
}
