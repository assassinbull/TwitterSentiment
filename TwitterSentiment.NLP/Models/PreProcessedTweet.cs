using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TwitterSentiment.NLP.Models
{
    public class PreProcessedTweet
    {
        public int TweetId { get; set; }
        public string FirstLetterCapitalizedWords { get; set; }
        public string Hashtags { get; set; }
        public string PunctuationMarks { get; set; }
        public string StopWords { get; set; }
        public string Emoticons { get; set; }
        public string WordUnigrams { get; set; }
        public string WordBigrams { get; set; }
        public string CharacterBigrams { get; set; }
    }
}
