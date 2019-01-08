using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TwitterSentiment.NLP.Models
{
    public class SenticNetModel
    {
        public string Concept { get; set; }
        public decimal? Pleasantness { get; set; }
        public decimal? Attention { get; set; }
        public decimal? Sensitivity { get; set; }
        public decimal? Aptitude { get; set; }
        public string PrimaryMood { get; set; }
        public string SecondaryMood { get; set; }
        public string PolarityLabel { get; set; }
        public decimal? Polarity { get; set; }
        public string Semantics1 { get; set; }
        public string Semantics2 { get; set; }
        public string Semantics3 { get; set; }
        public string Semantics4 { get; set; }
        public string Semantics5 { get; set; }
    }
}
