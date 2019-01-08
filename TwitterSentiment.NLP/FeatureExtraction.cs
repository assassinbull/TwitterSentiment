using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TwitterSentiment.NLP.Models;

namespace TwitterSentiment.NLP
{
    public class FeatureExtraction
    {
        public Dictionary<string, int> FirstLetterCapitalizedWordDict { get; set; }
        public Dictionary<string, int> HashtagDict { get; set; }
        public Dictionary<string, int> PunctuationMarkDict { get; set; }
        public Dictionary<string, int> StopWordDict { get; set; }
        public Dictionary<string, int> EmoticonDict { get; set; }
        public Dictionary<string, int> WordUnigramDict { get; set; }
        public Dictionary<string, int> WordBigramDict { get; set; }
        public Dictionary<string, int> CharacterBigramDict { get; set; }
        public Dictionary<string, int> NegationSuffixDict { get; set; }
        public Dictionary<string, int> POSTagDict { get; set; }
        public Dictionary<string, int> DomainRelationPOSTrigramDict { get; set; }
        public Dictionary<string, int> DomainRelationPOSFourgramDict { get; set; }

        public List<TweetAttributeModel> TweetAttributeList { get; set; }
        public List<TweetTokenAttributeModel> TweetTokenAttributeList { get; set; }

        public FeatureExtraction()
        {
            FirstLetterCapitalizedWordDict = new Dictionary<string, int>();
            HashtagDict = new Dictionary<string, int>();
            PunctuationMarkDict = new Dictionary<string, int>();
            StopWordDict = new Dictionary<string, int>();
            EmoticonDict = new Dictionary<string, int>();
            WordUnigramDict = new Dictionary<string, int>();
            WordBigramDict = new Dictionary<string, int>();
            CharacterBigramDict = new Dictionary<string, int>();
            NegationSuffixDict = new Dictionary<string, int>();
            POSTagDict = new Dictionary<string, int>();
            DomainRelationPOSTrigramDict = new Dictionary<string, int>();
            DomainRelationPOSFourgramDict = new Dictionary<string, int>();
        }

        public void ExtractFeatures()
        {
            if (TweetAttributeList == null)
                TweetAttributeList = DataHelper.GetTweetAttributes();
            if (TweetTokenAttributeList == null)
                TweetTokenAttributeList = DataHelper.GetTweetTokenAttributes();

            var tweetFeaturesDt = DataHelper.GenerateTweetFeaturesDataTable();
            var batchSize = 1000;
            var counter = 0;

            //Generate dictionaries
            PunctuationMarkDict.Add("?", 0);
            PunctuationMarkDict.Add("!", 0);
            foreach (var tweet in TweetAttributeList)
            {
                tweet.TweetTokens = TweetTokenAttributeList.Where(x => x.TweetId == tweet.TweetId).ToList();

                FillDictionary(tweet.WordUnigrams, ',', WordUnigramDict);
                FillDictionary(tweet.Emoticons, ',', EmoticonDict);
                FillDictionary(tweet.CharacterBigrams, ',', CharacterBigramDict);
                FillDictionary(tweet.WordBigrams, ',', WordBigramDict);
                FillDictionary(tweet.Hashtags, ',', HashtagDict);
                FillDictionary(tweet.FirstLetterCapitalizedWords, ',', FirstLetterCapitalizedWordDict);

                //Select negation suffixes
                var negatedTokens = tweet.TweetTokens.Where(x => x.DefinitiveTags.Split('|').Any(y => y.IndexOf("Neg") >= 0 || y.IndexOf("Without") >= 0));
                foreach (var token in negatedTokens)
                {
                    tweet.NegationSuffixes = string.Join(",", token.DefinitiveTags.Split('|').Where(y => y.IndexOf("Neg") >= 0 || y.IndexOf("Without") >= 0));
                }
                FillDictionary(tweet.NegationSuffixes, ',', NegationSuffixDict);

                //Select pos tags
                tweet.POSTags = string.Join(",", tweet.TweetTokens.Select(x => x.POSTag));
                FillDictionary(tweet.POSTags, ',', POSTagDict);

                FillDictionary(tweet.StopWords, ',', StopWordDict);

                //Proposed feature ngrams
                var domainSpecificWords = tweet.TweetTokens.Where(y => y.IsDomainSpecific == 1).ToList();
                var domainWordPredecessors = new List<TweetTokenAttributeModel>();
                var domainWordSuccessors = new List<TweetTokenAttributeModel>();
                domainWordPredecessors = GetWordRelations(tweet.TweetTokens, domainSpecificWords, domainWordPredecessors, true);
                domainWordSuccessors = GetWordRelations(tweet.TweetTokens, domainSpecificWords, domainWordPredecessors, false);
                var domainWordRelations = domainWordPredecessors.Union(domainWordSuccessors);
                FillDictionary(domainWordRelations.ToList().ExtractPOSNgrams(",", 3), ',', DomainRelationPOSTrigramDict);
                FillDictionary(domainWordRelations.ToList().ExtractPOSNgrams(",", 4), ',', DomainRelationPOSFourgramDict);
            }

            //Shrink dictionary size
            WordUnigramDict = WordUnigramDict.OrderByDescending(x => x.Value).Take(500).ToDictionary(x => x.Key, x => x.Value);
            CharacterBigramDict = CharacterBigramDict.OrderByDescending(x => x.Value).Take(500).ToDictionary(x => x.Key, x => x.Value);
            WordBigramDict = WordBigramDict.OrderByDescending(x => x.Value).Take(500).ToDictionary(x => x.Key, x => x.Value);
            HashtagDict = HashtagDict.OrderByDescending(x => x.Value).Take(100).ToDictionary(x => x.Key, x => x.Value);
            DomainRelationPOSTrigramDict = DomainRelationPOSTrigramDict.OrderByDescending(x => x.Value).Take(500).ToDictionary(x => x.Key, x => x.Value);
            DomainRelationPOSFourgramDict = DomainRelationPOSFourgramDict.OrderByDescending(x => x.Value).Take(500).ToDictionary(x => x.Key, x => x.Value);

            //Insert feature headings
            tweetFeaturesDt.Rows.Add(
                0
                , GetFeatureHeadingsString(WordUnigramDict, "1-2-wu_")
                , GetFeatureHeadingsString(PunctuationMarkDict, "1-")
                , "1-PositiveEmoticon,1-NegativeEmoticon"
                , GetFeatureHeadingsString(CharacterBigramDict, "2-cu_")
                , GetFeatureHeadingsString(WordBigramDict, "1-2-wb_")
                , GetFeatureHeadingsString(HashtagDict, "2-")
                , "3-NumberOfPositiveEmoticon,3-NumberOfNegativeEmoticon"
                , "3-NumberOfFirstLetterCapitalizedWords"
                , "3-NumberOfDomainSpecificWords"
                , "3-LengthOfSentence"
                , GetFeatureHeadingsString(NegationSuffixDict, "1-")
                , GetFeatureHeadingsString(POSTagDict, "1-")
                , "1-AveragePolarity"
                , "1-3-SenticNetPosSum,1-3-SenticNetNegSum,1-3-SentiTurkNetPosSum,1-3-SentiTurkNetNegSum"
                , "3-NumberOfPositiveWords,3-NumberOfNegativeWords"
                , "3-NumberOfAdjectives,3-NumberOfAdverbs"
                , "3-IsConditional,3-IsInterrogative,3-IsNegated,3-IsExclamative"
                //ProposedFeatures
                , "0-prop1_AveragePolarityOfDomainWordPredecessors,0-prop2_AveragePolarityOfDomainWordSuccessors,0-prop0_NumberOfFirstPersonWords,0-prop0_NumberOfSecondPersonWords,0-prop0_NumberOfThirdPersonWords,0-prop0_NumberOfMentions,0-prop0_NumberOfNonTurkishWords,0-prop0_NumberOfPersonEntities,0-prop3_NumberOfNegativeWordsInDomainPredecessors,0-prop4_NumberOfNegativeWordsInDomainSuccessors,0-prop5_NumberOfPositiveWordsInDomainPredecessors,0-prop6_NumberOfPositiveWordsInDomainSuccessors"
                , GetFeatureHeadingsString(DomainRelationPOSTrigramDict, "0-prop7_") + "," + GetFeatureHeadingsString(DomainRelationPOSFourgramDict, "0-prop8_")
            );

            //Insert tweet features
            foreach (var tweet in TweetAttributeList)
            {
                counter++;

                SetItemPresence(tweet.WordUnigrams, ',', WordUnigramDict);
                var wordUnigramsPresence = GetPresenceString(WordUnigramDict).ToString();

                SetItemPresence(tweet.PunctuationMarks, ' ', PunctuationMarkDict);
                var punctuationMarksPresence = GetPresenceString(PunctuationMarkDict).ToString();

                //Positive and negative emoticon presence
                var hasPositiveEmoticon = GetBooleanString(false);
                var hasNegativeEmoticon = GetBooleanString(false);
                var numberOfPositiveEmoticon = 0;
                var numberOfNegativeEmoticon = 0;
                foreach (var emot in tweet.Emoticons.Split(','))
                {
                    if (PreProcessing.EmoticonDictionary.Any(x => x.Key == emot && x.Value > 0))
                    {
                        hasPositiveEmoticon = GetBooleanString(true);
                        numberOfPositiveEmoticon++;
                    }
                    if (PreProcessing.EmoticonDictionary.Any(x => x.Key == emot && x.Value < 0))
                    {
                        hasNegativeEmoticon = GetBooleanString(true);
                        numberOfNegativeEmoticon++;
                    }
                }
                var emoticonsPresence = string.Format("{0},{1}", hasPositiveEmoticon, hasNegativeEmoticon);

                SetItemPresence(tweet.CharacterBigrams, ',', CharacterBigramDict);
                var characterBigramsPresence = GetPresenceString(CharacterBigramDict).ToString();

                SetItemPresence(tweet.WordBigrams, ',', WordBigramDict);
                var wordBigramsPresence = GetPresenceString(WordBigramDict).ToString();

                SetItemPresence(tweet.Hashtags, ',', HashtagDict);
                var hashtagsPresence = GetPresenceString(HashtagDict).ToString();

                //Positive and negative emoticon counts
                var numberOfEmoticons = string.Format("{0},{1}", numberOfPositiveEmoticon, numberOfNegativeEmoticon);

                var numberOfFirstLetterCapitalizedWords = FirstLetterCapitalizedWordDict.Count(x => x.Value == 1).ToString();

                var numberOfDomainSpecificWords = tweet.TweetTokens.Count(x => x.IsDomainSpecific == 1).ToString();

                var lengthOfSentence = tweet.WordUnigrams.Split(',').Count().ToString();

                SetItemPresence(tweet.NegationSuffixes, ',', NegationSuffixDict);
                var negationSuffixesPresence = GetPresenceString(NegationSuffixDict).ToString();

                SetItemPresence(tweet.POSTags, ',', POSTagDict);
                var posTagsPresence = GetPresenceString(POSTagDict).ToString();

                var senticNetPolarityTreshold = (decimal)0.1;

                //Negation effect on polarities
                foreach (var token in tweet.TweetTokens)
                {
                    if (token.SenticNetPolarity != null && token.SenticNetPolarity.Value >= -senticNetPolarityTreshold && token.SenticNetPolarity.Value <= senticNetPolarityTreshold)
                    {
                        token.SenticNetPolarityLabel = "objective";
                        token.SenticNetPolarity = 0;
                    }

                    if (token.IsNegated == 1)
                    {
                        if (token.SenticNetPolarityLabel == "positive") token.SenticNetPolarityLabel = "negative";
                        if (token.SenticNetPolarityLabel == "negative") token.SenticNetPolarityLabel = "positive";
                        token.SenticNetPolarity = -1 * token.SenticNetPolarity;
                        if (token.SentiTurkNetPolarityLabel == "p") token.SentiTurkNetPolarityLabel = "n";
                        if (token.SentiTurkNetPolarityLabel == "n") token.SentiTurkNetPolarityLabel = "p";
                        token.SentiTurkNetNegPolarity = token.SentiTurkNetPosPolarity;
                        token.SentiTurkNetPosPolarity = token.SentiTurkNetNegPolarity;
                    }
                }

                var averagePolarity = tweet.TweetTokens.Average(x => x.SenticNetPolarity);

                var senticNetPosSum = tweet.TweetTokens.Where(x => x.SenticNetPolarity > 0).Sum(x => x.SenticNetPolarity);
                var senticNetNegSum = tweet.TweetTokens.Where(x => x.SenticNetPolarity < 0).Sum(x => x.SenticNetPolarity);
                var sentiTurkNetPosSum = tweet.TweetTokens.Sum(x => x.SentiTurkNetPosPolarity);
                var SentiTurkNetNegSum = tweet.TweetTokens.Sum(x => x.SentiTurkNetNegPolarity) * -1;

                var numberOfPositiveWords = tweet.TweetTokens.Count(x => x.SentiTurkNetPolarityLabel == "p"); //From SentiTurkNet
                var numberOfNegativeWords = tweet.TweetTokens.Count(x => x.SentiTurkNetPolarityLabel == "n"); //From SentiTurkNet

                var numberOfAdjectives = tweet.TweetTokens.Count(x => x.POSTag == "Adj");
                var numberOfAdverbs = tweet.TweetTokens.Count(x => x.POSTag == "Adverb");

                SetItemPresence(tweet.StopWords, ',', StopWordDict);
                var stopWordsPresence = GetPresenceString(StopWordDict).ToString();
                var isConditionalSentence = GetBooleanString(tweet.TweetTokens.Any(x => x.DefinitiveTags.Split('|').Any(y => y == "Desr"))); //-se -sa
                var isInterrogativeSentence = GetBooleanString(tweet.EndingCharacter == "?");
                var isNegatedSentence = GetBooleanString(NegationSuffixDict.Any(x => x.Value == 1));
                var isExclamativeSentence = GetBooleanString(tweet.EndingCharacter == "!");

                /* ProposedFeatures */
                //Predecessor average polarity
                var domainSpecificWords = tweet.TweetTokens.Where(y => y.IsDomainSpecific == 1).ToList();
                var domainWordPredecessors = new List<TweetTokenAttributeModel>();
                domainWordPredecessors = GetWordRelations(tweet.TweetTokens, domainSpecificWords, domainWordPredecessors, true);
                var averagePolarityOfDomainWordPredecessors = domainWordPredecessors.Average(x => x.SenticNetPolarity);
                //Successor average polarity
                var domainWordSuccessors = new List<TweetTokenAttributeModel>();
                domainWordSuccessors = GetWordRelations(tweet.TweetTokens, domainSpecificWords, domainWordSuccessors, false);
                var averagePolarityOfDomainWordSuccessors = domainWordSuccessors.Average(x => x.SenticNetPolarity);
                //First person words
                var numberOfFristPersonWords = tweet.TweetTokens.Count(x => x.DefinitiveTags.Contains("A1sg"));
                var numberOfSecondPersonWords = tweet.TweetTokens.Count(x => x.DefinitiveTags.Contains("A2sg"));
                var numberOfThirdPersonWords = tweet.TweetTokens.Count(x => x.DefinitiveTags.Contains("A3sg") && x.POSTag == "Verb");
                //Number of mentions
                var numberOfMentions = !string.IsNullOrEmpty(tweet.Mentions) ? tweet.Mentions.Length - tweet.Mentions.Replace("@", "").Length : 0;
                //Number of non-Turkish words
                var numberOfNonTurkishWords = tweet.TweetTokens.Count(x => x.IsTurkish == 0);
                //Number of person named entities
                var numberOfPersonEntities = tweet.TweetTokens.Count(x => x.NamedEntity == "PERSON");
                //Number of negative words in domain word relations
                var numberOfNegativeWordsInDomainPredecessors = domainWordPredecessors.Count(x => x.SenticNetPolarity < 0);
                var numberOfNegativeWordsInDomainSuccessors = domainWordSuccessors.Count(x => x.SenticNetPolarity < 0);
                //Number of positive words in domain word relations
                var numberOfPositiveWordsInDomainPredecessors = domainWordPredecessors.Count(x => x.SenticNetPolarity > 0);
                var numberOfPositiveWordsInDomainSuccessors = domainWordSuccessors.Count(x => x.SenticNetPolarity > 0);
                //Domain relation ngrams
                var domainWordRelations = domainWordPredecessors.Union(domainWordSuccessors);
                SetItemPresence(domainWordRelations.ToList().ExtractPOSNgrams(",", 3), ',', DomainRelationPOSTrigramDict);
                var domainRelationPOSTrigramsPresence = GetPresenceString(DomainRelationPOSTrigramDict).ToString();
                SetItemPresence(domainWordRelations.ToList().ExtractPOSNgrams(",", 4), ',', DomainRelationPOSFourgramDict);
                var domainRelationPOSFourgramsPresence = GetPresenceString(DomainRelationPOSFourgramDict).ToString();

                tweetFeaturesDt.Rows.Add(
                    tweet.TweetId
                    , wordUnigramsPresence
                    , punctuationMarksPresence
                    , emoticonsPresence
                    , characterBigramsPresence
                    , wordBigramsPresence //Will be updated later
                    , hashtagsPresence
                    , numberOfEmoticons
                    , numberOfFirstLetterCapitalizedWords
                    , numberOfDomainSpecificWords
                    , lengthOfSentence
                    , negationSuffixesPresence
                    , posTagsPresence
                    , averagePolarity.ToString().Replace(",", ".")
                    , string.Format("{0},{1},{2},{3}", senticNetPosSum.ToString().Replace(",", "."), senticNetNegSum.ToString().Replace(",", "."), sentiTurkNetPosSum.ToString().Replace(",", "."), SentiTurkNetNegSum.ToString().Replace(",", "."))
                    , string.Format("{0},{1}", numberOfPositiveWords, numberOfNegativeWords)
                    , string.Format("{0},{1}", numberOfAdjectives, numberOfAdverbs)
                    , string.Format("{0},{1},{2},{3}", isConditionalSentence, isInterrogativeSentence, isNegatedSentence, isExclamativeSentence)
                    //ProposedFeatures
                    , string.Format("{0},{1},{2},{3},{4},{5},{6},{7},{8},{9},{10},{11}", averagePolarityOfDomainWordPredecessors.ToString().Replace(",", "."), averagePolarityOfDomainWordSuccessors.ToString().Replace(",", "."), numberOfFristPersonWords, numberOfSecondPersonWords, numberOfThirdPersonWords, numberOfMentions, numberOfNonTurkishWords, numberOfPersonEntities, numberOfNegativeWordsInDomainPredecessors, numberOfNegativeWordsInDomainSuccessors, numberOfPositiveWordsInDomainPredecessors, numberOfPositiveWordsInDomainSuccessors)
                    , string.Format("{0},{1}", domainRelationPOSTrigramsPresence, domainRelationPOSFourgramsPresence)
                );

                if (counter % batchSize == 0 && tweetFeaturesDt.Rows.Count > 0)
                {
                    DataHelper.WriteTweetFeaturesToDatabase(tweetFeaturesDt);
                    tweetFeaturesDt = DataHelper.GenerateTweetFeaturesDataTable();
                }
            }

            if (tweetFeaturesDt.Rows.Count > 0)
                DataHelper.WriteTweetFeaturesToDatabase(tweetFeaturesDt);
        }

        public void ExtractWordBigramFeatures()
        {
            if (TweetAttributeList == null)
                TweetAttributeList = DataHelper.GetTweetAttributes();
            if (TweetTokenAttributeList == null)
                TweetTokenAttributeList = DataHelper.GetTweetTokenAttributes();

            var tweetFeaturesDt = DataHelper.GenerateTweetFeatureWordBigramsDataTable();
            var batchSize = 10;
            var counter = 0;

            if (WordBigramDict == null || WordBigramDict.Count == 0)
                foreach (var tweet in TweetAttributeList)
                {
                    FillDictionary(tweet.WordBigrams, ',', WordBigramDict);
                }

            //Insert tweet features
            foreach (var tweet in TweetAttributeList)
            {
                counter++;

                SetItemPresence(tweet.WordBigrams, ',', WordBigramDict);
                var wordBigramsPresence = GetPresenceString(WordBigramDict).ToString();

                tweetFeaturesDt.Rows.Add(
                    tweet.TweetId
                    , wordBigramsPresence
                );

                if (counter % batchSize == 0 && tweetFeaturesDt.Rows.Count > 0)
                {
                    DataHelper.WriteTweetFeatureWordBigramsToDatabase(tweetFeaturesDt);
                    tweetFeaturesDt = DataHelper.GenerateTweetFeatureWordBigramsDataTable();
                }
            }

            if (tweetFeaturesDt.Rows.Count > 0)
                DataHelper.WriteTweetFeatureWordBigramsToDatabase(tweetFeaturesDt);
        }

        private List<TweetTokenAttributeModel> GetWordRelations(List<TweetTokenAttributeModel> tweetTokens, List<TweetTokenAttributeModel> wordslList, List<TweetTokenAttributeModel> relationList, bool predecessor)
        {
            if (relationList == null) relationList = new List<TweetTokenAttributeModel>();
            IEnumerable<TweetTokenAttributeModel> relatedWords;

            if (predecessor)
                relatedWords = tweetTokens.Where(x => wordslList.Any(y => y.TokenIndex == x.RelatedToIndex));
            else
                relatedWords = tweetTokens.Where(x => wordslList.Any(y => y.RelatedToIndex == x.TokenIndex));

            if (relatedWords != null && relatedWords.Count() > 0 && relatedWords.Any(x => !relationList.Any(y => y.TokenIndex == x.TokenIndex)))
            {
                relationList.AddRange(relatedWords);
                relationList = relationList.Distinct().ToList();
                relationList = GetWordRelations(tweetTokens, relatedWords.ToList(), relationList, predecessor);
            }

            return relationList;
        }

        private void FillDictionary(string items, char delimiter, Dictionary<string, int> dict)
        {
            if (!string.IsNullOrEmpty(items))
            {
                items = items.ToLower();
                var split = items.Split(delimiter);
                foreach (var item in split)
                {
                    if (!string.IsNullOrEmpty(item.Trim()))
                        if (!dict.Any(x => x.Key == item))
                            dict.Add(item, 0);
                        else
                            dict[item]++;
                }
            }
        }

        private void SetItemPresence(string items, char delimiter, Dictionary<string, int> dict)
        {
            ResetDictionary(dict);
            if (!string.IsNullOrEmpty(items))
            {
                items = items.ToLower();
                var split = items.Split(delimiter);
                foreach (var item in split)
                {
                    if (!string.IsNullOrEmpty(item.Trim()))
                        if (dict.Any(x => x.Key == item))
                            dict[item] = 1;
                }
            }
        }

        private string GetPresenceString(Dictionary<string, int> dict)
        {
            var result = string.Empty;

            result = string.Join("','", dict.Values);
            if (!string.IsNullOrEmpty(result))
            {
                result = "'" + result + "'";
                result = result.Replace("'1'", GetBooleanString(true));
                result = result.Replace("'0'", GetBooleanString(false));
            }

            return result;
        }

        private string GetFeatureHeadingsString(Dictionary<string, int> dict, string featureSetPrefix)
        {
            var result = string.Empty;

            result = string.Join("," + featureSetPrefix + "", dict.Keys);
            if (!string.IsNullOrEmpty(result)) result = featureSetPrefix + result;

            return result;
        }

        private void ResetDictionary(Dictionary<string, int> dict)
        {
            foreach (var key in dict.Keys.ToList())
            {
                dict[key] = 0;
            }
        }

        private string GetBooleanString(bool value)
        {
            var result = string.Empty;

            //result = value ? "y" : "n";
            result = value ? "1" : "0";

            return result;
        }
    }
}
