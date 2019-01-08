using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TwitterSentiment.NLP.Models
{
    class TweetFeatures
    {
        public int TweetId { get; set; }
        public string WordUnigrams { get; set; }
        public string PunctuationMarks { get; set; }
        public string Emoticons { get; set; }
        public string CharacterBigrams { get; set; }
        public string WordBigrams { get; set; }
        public string Hashtags { get; set; }
        public string NumberOfEmoticons { get; set; }
        public string NumberOfFirstLetterCapitalizedWords { get; set; }
        public string NumberOfDomainSpecificWords { get; set; }
        public string LengthOfSentence { get; set; }
        public string NegationSuffixes { get; set; }
        public string POSTags { get; set; }
        public string AveragePolarity { get; set; }
        public string Polarities { get; set; }
        public string NumberOfPolarizedWords { get; set; }
        public string NumberOfPOSTags { get; set; }
        public string SentenceStructure { get; set; }
    }
}
