namespace TwitterSentiment.NLP.Models
{
    public class TweetModel
    {
        public TweetModel(int tweetId, string originalText)
        {
            TweetId = tweetId;
            OriginalText = originalText;
            ProcessedText = originalText;
        }

        public int TweetId { get; set; }
        public string OriginalText { get; set; }
        public string ProcessedText { get; set; }
        public string FirstLetterCapitalizedWords { get; set; }
        public string Hashtags { get; set; }
        public string Mentions { get; set; }
        public string SentenceEndingCharacter { get; set; }
        public string PunctuationMarks { get; set; }
        public string MakeCodes { get; set; }
        public string ModelCodes { get; set; }
        public string StopWords { get; set; }
        public string Emoticons { get; set; }
        public string DomainSpecificWords { get; set; }
        public string WordUnigrams { get; set; }
        public string WordBigrams { get; set; }
        public string CharacterBigrams { get; set; }
    }
}
