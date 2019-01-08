namespace TwitterSentiment.NLP.Models
{
    public class SentiTurkNetModel
    {
        public string Synonyms { get; set; }
        public string Gloss { get; set; }
        public string PolarityLabel { get; set; }
        public string POSTag { get; set; }
        public decimal? NegPolarity { get; set; }
        public decimal? ObjPolarity { get; set; }
        public decimal? PosPolarity { get; set; }
    }
}
