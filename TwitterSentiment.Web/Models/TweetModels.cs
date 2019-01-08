using System;
using TwitterSentiment.Web.Helpers;

namespace TwitterSentiment.Web.Models
{
    public class TweetModel
    {
        public string Id { get; set; }
        private string _text;
        public string Text
        {
            get { return _text; }
            set
            {
                _text = TextHelper.Utf8Encoder.GetString(TextHelper.Utf8Encoder.GetBytes(value));
            }
        }
        private string _urlFreetext;
        public string URLFreeText
        {
            get { return _urlFreetext; }
            set
            {
                _urlFreetext = TextHelper.Utf8Encoder.GetString(TextHelper.Utf8Encoder.GetBytes(value));
            }
        }
        public string CreatedByAccountName { get; set; }
        public string Place { get; set; }
        public DateTime CreatedAt { get; set; }

        public int HashtagCount { get; set; }
        public int MediaCount { get; set; }
        public int SymbolCount { get; set; }
        public int UrlCount { get; set; }
        public int UserMentionCount { get; set; }
        public int IsRetweet { get; set; }
        public string RetweetRefId { get; set; }
        public int RetweetedCount { get; set; }
    }

    public class TweetSearchModel
    {
        public string Query { get; set; }
        public bool IsTurkishOnly { get; set; }
        public int MaxTweetCount { get; set; }
        public long MaxId { get; set; }
        public long SinceId { get; set; }
    }
}