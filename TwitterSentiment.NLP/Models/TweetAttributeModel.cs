using System.Collections.Generic;

namespace TwitterSentiment.NLP.Models
{
    public class TweetAttributeModel
    {
        public int TweetId { get; set; }
        public string ProcessedText { get; set; }
        public string FirstLetterCapitalizedWords { get; set; }
        public string Hashtags { get; set; }
        public string PunctuationMarks { get; set; }
        public string StopWords { get; set; }
        public string Emoticons { get; set; }
        public string WordUnigrams { get; set; }
        public string WordBigrams { get; set; }
        public string CharacterBigrams { get; set; }
        public string EndingCharacter { get; set; }
        public string Mentions { get; set; }

        public List<TweetTokenAttributeModel> TweetTokens { get; set; }
        public string NegationSuffixes { get; set; }
        public string POSTags { get; set; }
    }
}
