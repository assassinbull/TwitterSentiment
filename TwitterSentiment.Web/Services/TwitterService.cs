using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using Tweetinvi;
using Tweetinvi.Models;
using Tweetinvi.Models.DTO;
using Tweetinvi.Parameters;
using TwitterSentiment.Web.Models;

namespace TwitterSentiment.Web.Services
{
    public class TwitterService
    {
        public static string ConsumerKey = "l7iGCoCG5L9MBoVX4gCe0Ga4d";
        public static string ConsumerSecret = "EryXfKRUOAgZmO7gHU6JKk0oL0u5R25KC48DDWvY7mRePrN6e4";
        public static string AccessToken = "56163730-bvOlRhDBkgMeT1qcP2csvvpgg1bInRa1Y3VewnnCw";
        public static string AccessTokenSecret = "uvmcbAp2CrVGRUTyy1siF5djRXmNZgMw9aVJsXgZoLc0S";

        public TwitterService()
        {
            TweetinviConfig.ApplicationSettings.TweetMode = TweetMode.Extended;
            TweetinviConfig.CurrentThreadSettings.InitialiseFrom(TweetinviConfig.ApplicationSettings);
        }

        public void Authenticate()
        {
            var authResult = Auth.SetUserCredentials(ConsumerKey, ConsumerSecret, AccessToken, AccessTokenSecret);
        }

        public List<TweetModel> GetTweets(TweetSearchModel searchModel)
        {
            var searchParameter = new SearchTweetsParameters(searchModel.Query)
            {
                MaximumNumberOfResults = searchModel.MaxTweetCount
            };
            if (searchModel.IsTurkishOnly)
                searchParameter.Lang = LanguageFilter.Turkish;
            if (searchModel.MaxId > 0)
                searchParameter.MaxId = searchModel.MaxId;
            if (searchModel.SinceId > 0)
                searchParameter.SinceId = searchModel.SinceId;

            var matchingTweets = Search.SearchTweets(searchParameter);

            var tweetModels = new List<TweetModel>();
            if (matchingTweets != null)
                foreach (var tweet in matchingTweets)
                {
                    var tweetText = tweet.RetweetCount > 0 && tweet.RetweetedTweet != null ? "RT " + tweet.RetweetedTweet.FullText : tweet.FullText;
                    var tweetModel = new TweetModel
                    {
                        Id = tweet.Id.ToString(),
                        Text = tweetText,
                        URLFreeText = GetUrlFreeText(tweetText),
                        CreatedByAccountName = tweet.CreatedBy.ScreenName,
                        CreatedAt = tweet.CreatedAt,
                        Place = tweet.Place != null ? tweet.Place.FullName : string.Empty,

                        HashtagCount = tweet.Entities.Hashtags == null ? 0 : tweet.Entities.Hashtags.Count(),
                        MediaCount = tweet.Entities.Medias == null ? 0 : tweet.Entities.Medias.Count(),
                        SymbolCount = tweet.Entities.Symbols == null ? 0 : tweet.Entities.Symbols.Count(),
                        UrlCount = GetUrlOccuranceCount(tweetText),
                        UserMentionCount = tweet.UserMentions == null ? 0 : tweet.UserMentions.Count(),
                        IsRetweet = tweet.RetweetCount > 0 && tweet.RetweetedTweet != null ? 1 : 0,
                        RetweetRefId = tweet.RetweetCount > 0 && tweet.RetweetedTweet != null ? tweet.RetweetedTweet.Id.ToString() : string.Empty,
                        RetweetedCount = tweet.RetweetCount > 0 && tweet.RetweetedTweet == null ? tweet.RetweetCount : 0
                    };
                    tweetModels.Add(tweetModel);
                }

            return tweetModels;
        }

        public string GetUrlFreeText(string text)
        {
            var result = text;
            var urlKeyword = "http";

            var separators = new string[] { urlKeyword };
            var splittedText = text.Split(separators, StringSplitOptions.RemoveEmptyEntries);

            if (splittedText.Count() > 0)
                foreach (var subText in splittedText)
                {
                    var splittedSubText = subText.Split(new string[] { " " }, StringSplitOptions.RemoveEmptyEntries);
                    if (splittedSubText.Count() > 0)
                    {
                        var urlText = urlKeyword + splittedSubText[0];
                        result = result.Replace(urlText, "@{{URL}}");
                    }
                }

            return result;
        }

        public int GetUrlOccuranceCount(string text)
        {
            var occuranceCount = 0;

            occuranceCount = Regex.Matches(text, "http").Count;

            return occuranceCount;
        }
    }
}