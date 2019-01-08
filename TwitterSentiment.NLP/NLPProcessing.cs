using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Web;
using TwitterSentiment.NLP.Models;

namespace TwitterSentiment.NLP
{
    public class NLPProcessing
    {
        private static readonly int BatchSize = 1000;
        private List<SenticNetModel> _senticNetData;
        public List<SenticNetModel> SenticNetData
        {
            get
            {
                if (_senticNetData == null || _senticNetData.Count == 0)
                    _senticNetData = DataHelper.GetSenticNetFile();

                return _senticNetData;
            }
        }
        private List<SentiTurkNetModel> _sentiTurkNetData;
        public List<SentiTurkNetModel> SentiTurkNetData
        {
            get
            {
                if (_sentiTurkNetData == null || _sentiTurkNetData.Count == 0)
                    _sentiTurkNetData = DataHelper.GetSentiTurkNetFile();

                return _sentiTurkNetData;
            }
        }

        public void WriteNormalizedData()
        {
            var normalizedList = DataHelper.GetNLPFile("Automotive Tweets - NLP Normalized.txt", true);

            var dt = DataHelper.GenerateNLPResultDataTable();
            var batchCounter = 0;

            foreach (var item in normalizedList)
            {
                batchCounter++;
                dt.Rows.Add(null, item.TweetId, "Normalization", 0, item.LineText);

                if (batchCounter % BatchSize == 0 && dt.Rows.Count > 0)
                {
                    DataHelper.WriteNLPResultToDatabase(dt);
                    dt = DataHelper.GenerateNLPResultDataTable();
                }
            }

            if (dt.Rows.Count > 0)
                DataHelper.WriteNLPResultToDatabase(dt);
        }

        public void WriteMorphGeneratedData()
        {
            WriteNLPData("Automotive Tweets - NLP Morph Generated.txt", "Morphological Analyzer");
        }

        public void WriteMorphDisambiguatedData()
        {
            WriteNLPData("Automotive Tweets - NLP Morph Disambiguated.txt", "Morphological Disambiguator");
        }

        public void WriteNamedEntityRecognizedData()
        {
            WriteNLPData("Automotive Tweets - NLP Named Entity Recognized.txt", "Named Entity Recognizer");
        }

        public void WriteParsedDependencyData()
        {
            var result = new List<NLPLine>();
            var nlpList = DataHelper.GetNLPFile("Automotive Tweets - NLP Morph Disambiguated.txt", false);
            var inputString = string.Empty;
            var tweetId = 1;

            foreach (var item in nlpList)
            {
                if (tweetId != item.TweetId)
                {
                    var nlpString = NLPApiRequest("DepParserFormal", inputString);

                    foreach (var nlpLine in nlpString.Split('\n'))
                    {
                        if (!string.IsNullOrEmpty(nlpLine))
                        {
                            var index = 0;
                            int.TryParse(nlpLine.Split('\t')[0], out index);
                            result.Add(new NLPLine { TweetId = tweetId, TokenIndex = index > 0 ? index : -1, LineText = nlpLine });
                        }
                    }

                    inputString = string.Empty;
                    tweetId = item.TweetId;
                }

                inputString += item.NLPInputFormat() + "\n";
            }

            if (!string.IsNullOrEmpty(inputString))
            {
                var nlpString = NLPApiRequest("DepParserFormal", inputString);

                foreach (var nlpLine in nlpString.Split('\n'))
                {
                    if (!string.IsNullOrEmpty(nlpLine))
                    {
                        var index = 0;
                        int.TryParse(nlpLine.Split('\t')[0], out index);
                        result.Add(new NLPLine { TweetId = tweetId, TokenIndex = index > 0 ? index : -1, LineText = nlpLine });
                    }
                }
            }

            WriteNLPDataMain(result, "Dependency Parser");
        }

        public void WriteDependencyParsedTweets()
        {
            var lines = DataHelper.GetParsedDependencies();
            var tweets = DataHelper.GenerateDependencyParsedTweetsDataTable();
            var counter = 0;

            foreach (var line in lines)
            {
                counter++;
                var split = line.LineText.Split('\t');

                var posTag = split[3];
                posTag = posTag.Replace("^DB", "");

                var secondPosTag = split[4];
                secondPosTag = secondPosTag.Replace("^DB", "");

                var tweetId = line.TweetId;
                var availablePosTags = new string[] { "Verb", "Det", "Noun", "Adj", "Interj", "Conj", "Pron", "Dup", "Guess", "Adverb", "Postp" };
                if (!availablePosTags.Contains(posTag))
                    tweetId = -1 * tweetId;

                tweets.Rows.Add(null, tweetId, line.TokenIndex, split[1], split[2], posTag, secondPosTag, split[5], Convert.ToUInt32(split[6]), split[7]);

                if (counter % BatchSize == 0 && tweets.Rows.Count > 0)
                {
                    DataHelper.WriteDependencyParsedTweetsToDatabase(tweets);
                    tweets = DataHelper.GenerateDependencyParsedTweetsDataTable();
                }
            }

            if (tweets.Rows.Count > 0)
                DataHelper.WriteDependencyParsedTweetsToDatabase(tweets);
        }

        public void WriteTweetTokenExtraInfo()
        {
            var items = DataHelper.GetDependencyParsedTweets();
            var dt = DataHelper.GenerateTweetTokenExtraInfoDataTable();
            var batchCounter = 0;

            foreach (var item in items)
            {
                batchCounter++;

                //IsNegated
                //var isNegated = IsNegated(item);

                //TokenEng
                //var tokenEng = TranslateToken(item);

                //SenticNet
                //var senticNet = GetSenticNetResult(tokenEng);

                var token = item.TokenStem;
                if (item.POSTag == "Verb")
                    token = AddVerbSuffix(token);

                //IsDomainSpecific
                var isDomainSpecific = PreProcessing.DomainSpecificWords.ToList().Any(x => x.ToLower() == token.ToLower());

                //SentiTurkNet
                var sentiTurkNet = GetSentiTurkNetResult(token);

                dt.Rows.Add(null, item.TweetId, item.TokenIndex
                    //, isNegated, tokenEng, senticNet.Pleasantness, senticNet.Attention, senticNet.Sensitivity, senticNet.Aptitude, senticNet.PrimaryMood, senticNet.SecondaryMood, senticNet.PolarityLabel, senticNet.Polarity, senticNet.Semantics1, senticNet.Semantics2, senticNet.Semantics3, senticNet.Semantics4, senticNet.Semantics5
                    , 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0
                    , isDomainSpecific ? 1 : 0, sentiTurkNet.Synonyms, sentiTurkNet.Gloss, sentiTurkNet.PolarityLabel, sentiTurkNet.POSTag, sentiTurkNet.NegPolarity, sentiTurkNet.ObjPolarity, sentiTurkNet.PosPolarity);

                if (batchCounter % BatchSize == 0 && dt.Rows.Count > 0)
                {
                    DataHelper.WriteTweetTokenExtraToDatabase(dt);
                    dt = DataHelper.GenerateTweetTokenExtraInfoDataTable();
                }
            }

            if (dt.Rows.Count > 0)
                DataHelper.WriteTweetTokenExtraToDatabase(dt);
        }

        public void WriteNgramExtractedTweetsData()
        {
            var items = DataHelper.GetDependencyParsedTweets();
            var ngramTweets = new List<TweetModel>();
            var tweetId = 1;
            var extractedText = string.Empty;
            var extractedOriginalText = string.Empty;

            foreach (var item in items)
            {
                if (tweetId != item.TweetId)
                {
                    var tweetModel = new TweetModel(tweetId, extractedOriginalText.Trim());
                    tweetModel.ExtractCharacterBigrams();
                    tweetModel.ProcessedText = extractedText.Trim();
                    tweetModel.ExtractWordUnigrams();
                    tweetModel.ExtractWordBigrams();

                    ngramTweets.Add(tweetModel);

                    extractedText = string.Empty;
                    extractedOriginalText = string.Empty;
                    tweetId = item.TweetId;
                }

                var tokenStem = item.POSTag == "Verb" ? AddVerbSuffix(item.TokenStem) : item.TokenStem;

                extractedText += (item.IsNegated == 1 ? "-" : "") + tokenStem + " ";
                extractedOriginalText += item.Token + " ";
            }

            if (!string.IsNullOrEmpty(extractedText))
            {
                var tweetModel = new TweetModel(tweetId, extractedOriginalText.Trim());
                tweetModel.ExtractCharacterBigrams();
                tweetModel.ProcessedText = extractedText.Trim();
                tweetModel.ExtractWordUnigrams();
                tweetModel.ExtractWordBigrams();

                ngramTweets.Add(tweetModel);
            }

            WriteNgramData(ngramTweets);
        }

        private bool IsNegated(DependencyParsedTweet model)
        {
            var result = false;

            if (model.DefinitiveTags.Contains("Neg") || model.DefinitiveTags.Contains("Without"))
                result = true;

            return result;
        }

        private string NLPApiRequest(string method, string input)
        {
            var url = "http://tools.nlp.itu.edu.tr/SimpleApi";
            var result = string.Empty;

            using (var client = new WebClient())
            {
                var parameters = new NameValueCollection();
                parameters.Add("tool", method);
                parameters.Add("input", input);
                parameters.Add("token", "RpnDr8DytDbBR3tRinm1VLhhmOvqhGmK");

                var responseData = client.UploadValues(url, "POST", parameters);

                result = Encoding.UTF8.GetString(responseData);
            }

            return result;
        }

        private void WriteNLPData(string fileName, string nlpType)
        {
            var nlpList = DataHelper.GetNLPFile(fileName, false);
            WriteNLPDataMain(nlpList, nlpType);
        }

        private void WriteNLPDataMain(List<NLPLine> nlpList, string nlpType)
        {
            var dt = DataHelper.GenerateNLPResultDataTable();
            var tweetId = 0;
            var batchCounter = 0;

            foreach (var item in nlpList)
            {
                if (tweetId != item.TweetId)
                {
                    tweetId = item.TweetId;
                    batchCounter++;
                }

                if (item.TokenIndex == 1 && batchCounter % BatchSize == 0 && dt.Rows.Count > 0)
                {
                    DataHelper.WriteNLPResultToDatabase(dt);
                    dt = DataHelper.GenerateNLPResultDataTable();
                }

                dt.Rows.Add(null, item.TweetId, nlpType, item.TokenIndex, item.LineText);
            }

            if (dt.Rows.Count > 0)
                DataHelper.WriteNLPResultToDatabase(dt);
        }

        private void WriteNgramData(List<TweetModel> modelList)
        {
            var result = DataHelper.GenerateNgramExtractedTweetsDataTable();
            var batchCounter = 0;

            foreach (var model in modelList)
            {
                batchCounter++;

                if (batchCounter % BatchSize == 0 && result.Rows.Count > 0)
                {
                    DataHelper.WriteNgramExtractedTweetsToDatabase(result);
                    result = DataHelper.GenerateNgramExtractedTweetsDataTable();
                }

                result.Rows.Add(null, model.TweetId, model.ProcessedText, model.WordUnigrams, model.WordBigrams, model.CharacterBigrams);
            }

            if (result.Rows.Count > 0)
                DataHelper.WriteNgramExtractedTweetsToDatabase(result);
        }

        private string TranslateToken(DependencyParsedTweet model)
        {
            var translateResult = model.TokenStem;
            var token = model.TokenStem;
            if (model.POSTag == "Verb")
                token = AddVerbSuffix(token);

            //translate
            var url = "https://translate.yandex.net/api/v1.5/tr/translate";
            var result = string.Empty;

            using (var client = new WebClient())
            {
                var parameters = new NameValueCollection();
                parameters.Add("key", "trnsl.1.1.20180410T171649Z.a8f4616c10af05b1.818c09bc5bff762ce5508dab5d83f1b7ecda0cd6");
                parameters.Add("text", token);
                parameters.Add("lang", "tr-en");

                var responseData = client.UploadValues(url, "POST", parameters);

                result = Encoding.UTF8.GetString(responseData);
            }

            if (!string.IsNullOrEmpty(result) && result.Contains("<text>") && result.Contains("</text>"))
                translateResult = result.Split(new[] { "<text>", "</text>" }, StringSplitOptions.None)[1];

            return translateResult;
        }

        private SenticNetModel GetSenticNetResult(string tokenEng)
        {
            /*
            //get polarity score
            var url = "http://sentic.net/api/en/concept/CONCEPT_NAME/polarity/intensity";
            var result = string.Empty;

            url = url.Replace("CONCEPT_NAME", tokenEng);

            using (var client = new WebClient())
            {
                result = client.DownloadString(url);
            }

            if (!string.IsNullOrEmpty(result) && result.Contains("<intensity") && result.Contains("</intensity>"))
                polarityResult = Convert.ToDecimal(result.Split(new[] { "<intensity", "</intensity>" }, StringSplitOptions.None)[1].Split('>')[1].Replace(".", ","));
            */

            var sentic = SenticNetData.FirstOrDefault(x => x.Concept == tokenEng);
            if (sentic == null)
                sentic = new SenticNetModel();

            return sentic;
        }

        private SentiTurkNetModel GetSentiTurkNetResult(string token)
        {
            var sentic = SentiTurkNetData.FirstOrDefault(x => ("," + x.Synonyms.ToLower() + ",").Contains("," + token.ToLower() + ","));
            if (sentic == null)
                sentic = SentiTurkNetData.FirstOrDefault(x => ("," + x.Gloss.ToLower() + ",").Contains("," + token.ToLower() + ","));
            if (sentic == null)
                sentic = new SentiTurkNetModel();

            return sentic;
        }

        public static string AddVerbSuffix(string verb)
        {
            var result = verb;

            var thickCharacterIndex = verb.LastIndexOfAny(new char[] { 'a', 'ı', 'o', 'u' });
            var thinCharacterIndex = verb.LastIndexOfAny(new char[] { 'e', 'i', 'ö', 'ü' });
            result = result + (thickCharacterIndex > thinCharacterIndex ? "mak" : "mek");

            return result;
        }
    }
}
