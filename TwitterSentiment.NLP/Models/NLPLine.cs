namespace TwitterSentiment.NLP.Models
{
    public class NLPLine
    {
        public int TweetId { get; set; }
        public int TokenIndex { get; set; }
        public string LineText { get; set; }
    }
}
