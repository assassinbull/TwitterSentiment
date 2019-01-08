namespace TwitterSentiment.NLP.Models
{
    public class DependencyParsedTweet
    {
        public int TweetId { get; set; }
        public int TokenIndex { get; set; }
        public string Token { get; set; }
        public string TokenStem { get; set; }
        public string POSTag { get; set; }
        public string DefinitiveTags { get; set; }
        public int RelatedToIndex { get; set; }
        public string TokenRelation { get; set; }
        public int IsNegated { get; set; }
        public string TokenEng { get; set; }
        public decimal PolarityScore { get; set; }
    }
}
