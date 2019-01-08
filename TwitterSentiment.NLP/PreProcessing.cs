using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using TwitterSentiment.NLP.Models;

namespace TwitterSentiment.NLP
{
    public static class PreProcessing
    {
        #region | Constants |
        private const string ExtractionDelimiter = ",";

        private static readonly string[] TurkishStopWords = { "acaba", "altmış", "altı", "ama", "ancak", "arada", "aslında", "ayrıca", "bana", "bazı", "belki", "ben", "benden", "beni", "benim", "beri", "beş", "bile", "bin", "bir", "birçok", "biri", "birkaç", "birkez", "birşey", "birşeyi", "biz", "bize", "bizden", "bizi", "bizim", "böyle", "böylece", "bu", "buna", "bunda", "bundan", "bunlar", "bunları", "bunların", "bunu", "bunun", "burada", "çok", "çünkü", "da", "daha", "dahi", "de", "defa", "değil", "diğer", "diye", "doksan", "dokuz", "dolayı", "dolayısıyla", "dört", "edecek", "eden", "ederek", "edilecek", "ediliyor", "edilmesi", "ediyor", "eğer", "elli", "en", "etmesi", "etti", "ettiği", "ettiğini", "gibi", "göre", "halen", "hangi", "hatta", "hem", "henüz", "hep", "hepsi", "her", "herhangi", "herkesin", "hiç", "hiçbir", "için", "iki", "ile", "ilgili", "ise", "işte", "itibaren", "itibariyle", "kadar", "karşın", "katrilyon", "kendi", "kendilerine", "kendini", "kendisi", "kendisine", "kendisini", "kez", "ki", "kim", "kimden", "kime", "kimi", "kimse", "kırk", "milyar", "milyon", "mu", "mü", "mı", "nasıl", "ne", "neden", "nedenle", "nerde", "nerede", "nereye", "niye", "niçin", "o", "olan", "olarak", "oldu", "olduğu", "olduğunu", "olduklarını", "olmadı", "olmadığı", "olmak", "olması", "olmayan", "olmaz", "olsa", "olsun", "olup", "olur", "olursa", "oluyor", "on", "ona", "ondan", "onlar", "onlardan", "onları", "onların", "onu", "onun", "otuz", "oysa", "öyle", "pek", "rağmen", "sadece", "sanki", "sekiz", "seksen", "sen", "senden", "seni", "senin", "siz", "sizden", "sizi", "sizin", "şey", "şeyden", "şeyi", "şeyler", "şöyle", "şu", "şuna", "şunda", "şundan", "şunları", "şunu", "tarafından", "trilyon", "tüm", "üç", "üzere", "var", "vardı", "ve", "veya", "ya", "yani", "yapacak", "yapılan", "yapılması", "yapıyor", "yapmak", "yaptı", "yaptığı", "yaptığını", "yaptıkları", "yedi", "yerine", "yetmiş", "yine", "yirmi", "yoksa", "yüz", "zaten" };

        public static readonly string[] DomainSpecificWords = { "ABS", "Adaptif", "Adaptive", "Alarm", "Anahtar", "Analog", "AWD", "Ayna", "Bagaj", "Benzin", "Boya", "Cabrio", "Cam", "Çekiş", "Coupe", "Crossover", "Cruise", "Dikiz", "Direksiyon", "Konsol", "Dizel", "Donanım", "Hasar", "Hasarsız", "Far", "Hatchback", "Hidrolik", "Hız", "Beygir", "Hybrid", "Hibrit", "Jant", "Kasa", "Klima", "LPG", "Motor", "Manuel", "Otomatik", "Panoramik", "Parça", "Park", "Plaka", "Roadster", "Römork", "Sedan", "Station", "Sunroof", "Torpido", "Vites", "Xenon", "Yakıt", "Yol", "Servis", "Sürüş", "Vasıta", "Araç", "Otomobil", "Arıza", "Tamir", "Gaz", "Gazlamak", "BMW" /*Marka*/, "Clubman" /*Model*/ };

        private static readonly string PunctuationMarks =
            "’'" +       // apostrophe
            "()[]{}<>" + // brackets
            ":" +        // colon
            "," +        // comma
            "‒–—―" +     // dashes
            "…" +        // ellipsis
            "!" +        // exclamation mark
            "." +        // full stop/period
            "«»" +       // guillemets
            "-‐" +       // hyphen
            "?" +        // question mark
            "‘’“”\"" +     // quotation marks
            ";" +        // semicolon
            "/" +        // slash/stroke
            "⁄" +        // solidus
            "␠" +        // space?   
            "·" +        // interpunct
            "&" +        // ampersand
            "@" +        // at sign
            "*" +        // asterisk
            "\\" +       // backslash
            "•" +        // bullet
            "^" +        // caret
            "¤¢$€£¥₩₪" + // currency
            "†‡" +       // dagger
            "°" +        // degree
            "¡" +        // inverted exclamation point
            "¿" +        // inverted question mark
            "¬" +        // negation
            "#" +        // number sign (hashtag)
            "№" +        // numero sign ()
            "%‰‱" +      // percent and related signs
            "¶" +        // pilcrow
            "′" +        // prime
            "§" +        // section sign
            "~" +        // tilde/swung dash
            "¨" +        // umlaut/diaeresis
            "_" +        // underscore/understrike
            "|¦" +       // vertical/pipe/broken bar
            "⁂" +        // asterism
            "☞" +        // index/fist
            "∴" +        // therefore sign
            "‽" +        // interrobang
            "※";         // reference mark

        private static readonly string[] Emoticons =
        {
            "☺%Smiley_face%1"
            , "☻%Black_smiley_face%1"
            , "☹%Frowning_face%-1"
            , "😁%Grinning_Face_With_Smiling_Eyes%1"
            , "😂%Face_With_Tears_Of_Joy%1"
            , "😃%Smiling_Face_With_Open_Mouth%1"
            , "😄%Smiling_Face_With_Open_Mouth_And_Smiling_Eyes%1"
            , "😅%Smiling_Face_With_Open_Mouth_And_Cold_Sweat%1"
            , "😆%Smiling_Face_With_Open_Mouth_And_Tightly_closed_Eyes%1"
            , "😇%Smiling_Face_With_Halo%1"
            , "😈%Smiling_Face_With_Horns%1"
            , "😉%Winking_Face%1"
            , "😊%Smiling_Face_With_Smiling_Eyes%1"
            , "😋%Face_Savouring_Delicious_Food%1"
            , "😌%Relieved_Face%0"
            , "😍%Smiling_Face_With_Heart_shaped_Eyes%1"
            , "😎%Smiling_Face_With_Sunglasses%1"
            , "😏%Smirking_Face%1"
            , "😐%Neutral_Face%0"
            , "😑%Expressionless_Face%0"
            , "😒%Unamused_Face%-1"
            , "😓%Face_With_Cold_Sweat%0"
            , "😔%Pensive_Face%-1"
            , "😕%Confused_Face%0"
            , "😖%Confounded_Face%-1"
            , "😗%Kissing_Face%1"
            , "😘%Face_Throwing_A_Kiss%1"
            , "😙%Kissing_Face_With_Smiling_Eyes%1"
            , "😚%Kissing_Face_With_Closed_Eyes%1"
            , "😛%Face_With_Stuck_out_Tongue%1"
            , "😜%Face_With_Stuck_out_Tongue_And_Winking_Eye%1"
            , "😝%Face_With_Stuck_out_Tongue_And_Tightly_closed_Eyes%1"
            , "😞%Disappointed_Face%-1"
            , "😟%Worried_Face%-1"
            , "😠%Angry_Face%-1"
            , "😡%Pouting_Face%-1"
            , "😢%Crying_Face%-1"
            , "😣%Persevering_Face%-1"
            , "😤%Face_With_Look_Of_Triumph%-1"
            , "😥%Disappointed_But_Relieved_Face%-1"
            , "😦%Frowning_Face_With_Open_Mouth%-1"
            , "😧%Anguished_Face%-1"
            , "😨%Fearful_Face%-1"
            , "😩%Weary_Face%-1"
            , "😪%Sleepy_Face%0"
            , "😫%Tired_Face%-1"
            , "😬%Grimacing_Face%-1"
            , "😭%Loudly_Crying_Face%-1"
            , "😮%Face_With_Open_Mouth%0"
            , "😯%Hushed_Face%0"
            , "😰%Face_With_Open_Mouth_And_Cold_Sweat%-1"
            , "😱%Face_Screaming_In_Fear%0"
            , "😲%Astonished_Face%0"
            , "😳%Flushed_Face%0"
            , "😴%Sleeping_Face%0"
            , "😵%Dizzy_Face%0"
            , "😶%Face_Without_Mouth%0"
            , "😷%Face_With_Medical_Mask%0"
            , "😸%Grinning_Cat_Face_With_Smiling_Eyes%1"
            , "😹%Cat_Face_With_Tears_Of_Joy%1"
            , "😺%Smiling_Cat_Face_With_Open_Mouth%1"
            , "😻%Smiling_Cat_Face_With_Heart_shaped_Eyes%1"
            , "😼%Cat_Face_With_Wry_Smile%1"
            , "😽%Kissing_Cat_Face_With_Closed_Eyes%1"
            , "😾%Pouting_Cat_Face%-1"
            , "😿%Crying_Cat_Face%-1"
            , "🙀%Weary_Cat_Face%-1"
            , "🙁%Slightly_Frowning_Face%-1"
            , "🙂%Slightly_Smiling_Face%1"
            , "🙃%Upside_down_Face%0"
            , "🙄%Face_With_Rolling_Eyes%0"
            , "🙅%Face_With_No_Good_Gesture%0"
            , "🙆%Face_With_Ok_Gesture%1"
            , "🙇%Person_Bowing_Deeply%0"
            , "🙈%See_no_evil_Monkey%0"
            , "🙉%Hear_no_evil_Monkey%0"
            , "🙊%Speak_no_evil_Monkey%0"
            , "🙋%Happy_Person_Raising_One_Hand%1"
            , "🙌%Person_Raising_Both_Hands_In_Celebration%1"
            , "🙍%Person_Frowning%-1"
            , "🙎%Person_With_Pouting_Face%-1"
            , "🙏%Person_With_Folded_Hands%0"
            , "☺%White_smiley_alt_code%1"
            , "☻%Black_smiley_alt_code%1"
            , "😀%lol_Smiley%1"
            , "🚗%car_Smiley%0"
            , "🤗%unknown_Smiley%0"
            , "🔞%mark_Smiley%0"
            , "👏🏻%clap_Smiley%1"
            , "💥%explosion_Smiley%0"
            , "👍%thumbs_up_Smiley%1"
            , "🔓%lock_Smiley%0"
            , "🌊%wave_Smiley%0"
            , "❤%heart_Smiley%1"
            , "👄%lkiss_Smiley%1"
            , "🏼%box_Smiley%0"
            , "🤪%unknown2_Smiley%0"
            , "👌%well_done_Smiley%1"
            , "💘%arrow_heart_Smiley%1"
            , "❣️%glass_Smiley%0"
            , "💨%vinn_Smiley%0"
            , "💪🏻%flex_Smiley%0"
            , "👑%crown_Smiley%0"
            , "🤠%unknown3_Smiley%0"
            , "🏎%f1_Smiley%0"
            , "💓%heart2_Smiley%1"
            , "📌%pin_Smiley%0"
            , "🗓%first_day_Smiley%0"
            , "👮%officer_Smiley%0"
            , "♂️%male_Smiley%0"
            , "🐻%bear_Smiley%0"
            , "📎%attach_Smiley%0"
            , "👉%horse_head_Smiley%0"
            , "🐘%elephant_Smiley%0"
            , "🔥%flame_Smiley%0"
            , "📍%pin2_Smiley%0"
            , "🚙%car2_Smiley%0"
            , "⏱%clock_Smiley%0"
            , "🌸%flower_Smiley%0"
            , "☕️%coffee_Smiley%0"
            , "🍀%clover_Smiley%0"
            , "🌲%tree_Smiley%0"
            , "🌺%flower2_Smiley%0"
            , "🌼%flower3_Smiley%0"
            , "💜%heart3_Smiley%1"
            , "💦%drops_Smiley%0"
            , "🇹🇷%lock_Smiley%0"
            , "🎾%tennis_Smiley%0"
            , "🏁%flag_Smiley%0"
            , "🔑%key2_Smiley%0"
            , "💣%bomb_Smiley%0"
            , "👳🏻%face_Smiley%0"
            , "🌹%flower4_Smiley%0"
            , "🚘%car3_Smiley%0"
            , "👥%people_Smiley%0"
            , "👐%hands_Smiley%0"
            , ":)%smile_text%1"
            , ":D%lol_text%1"
            , ":(%flower4_Smiley%-1"
            , "=)%flower4_Smiley%1"
            , "=D%flower4_Smiley%1"
            , "=(%flower4_Smiley%-1"
        };

        private static Dictionary<string, int> _emoticonDictionary;

        public static Dictionary<string, int> EmoticonDictionary
        {
            get
            {
                if (_emoticonDictionary == null || _emoticonDictionary.Count == 0)
                {
                    _emoticonDictionary = new Dictionary<string, int>();
                    foreach (var emoticon in Emoticons)
                    {
                        var emoticonSplit = emoticon.Split('%');
                        if (!_emoticonDictionary.ContainsKey(emoticonSplit[0]))
                            _emoticonDictionary.Add(emoticonSplit[0], Convert.ToInt32(emoticonSplit[2]));
                    }
                }

                return _emoticonDictionary;
            }
        }

        private static Dictionary<string, string> _makeDictionary;

        private static Dictionary<string, string> MakeDictionary
        {
            get
            {
                if (_makeDictionary == null || _makeDictionary.Count == 0)
                {
                    _makeDictionary = DataHelper.GetMakeCodes();
                }

                return _makeDictionary;
            }
        }

        private static Dictionary<string, string> _modelDictionary;

        private static Dictionary<string, string> ModelDictionary
        {
            get
            {
                if (_modelDictionary == null || _modelDictionary.Count == 0)
                {
                    _modelDictionary = DataHelper.GetModelCodes();
                }

                return _modelDictionary;
            }
        }

        //private static Dictionary<string, int> _emoticonDictionary;

        //private static Dictionary<string, int> EmoticonDictionary
        //{
        //    get
        //    {
        //        if (_emoticonDictionary == null || _emoticonDictionary.Count == 0)
        //        {
        //            _emoticonDictionary = DataHelper.GetEmoticons();
        //        }

        //        return _emoticonDictionary;
        //    }
        //}
        #endregion

        public static void ReplaceURLs(this TweetModel model, string replaceToken)
        {
            ReplaceAndExtractTexts(model, "http", " ", ExtractionDelimiter, replaceToken);
        }

        public static void WritePreProcessedTweets()
        {
            var rawTweets = DataHelper.GetRawTweets();
            var preprocessedTweets = DataHelper.GeneratePreProcessedDataTable();
            var preProcessDate = DateTime.Now;
            var batchSize = 1000;
            var counter = 0;

            foreach (var tweet in rawTweets)
            {
                counter++;
                var tweetModel = new TweetModel(tweet.Key, tweet.Value);

                tweetModel.ReplaceURLs("");
                tweetModel.ExtractFirstLetterCapitalizedWords(2); //With punctuation removal
                tweetModel.ProcessedText = tweetModel.ProcessedText.ToLower();
                tweetModel.ExtractAndReplaceMentions("");
                tweetModel.ExtractAndReplaceEmoticons("");
                tweetModel.ExtractHashtags();
                tweetModel.ReplaceCharacters("#", " "); //Hashtags will remain as single words
                tweetModel.ReplaceCharacters("&lt;", " ");
                tweetModel.ReplaceCharacters("&gt;", " ");
                tweetModel.SentenceEndingCharacter = !string.IsNullOrEmpty(tweetModel.ProcessedText.Trim()) ? tweetModel.ProcessedText.Trim()[tweetModel.ProcessedText.Trim().Length - 1].ToString() : string.Empty; //Take last character, for interrogation/exclamation analysis
                /*
                tweetModel.ExtractAndReplacePunctuationMarks(" ");
                tweetModel.ReplaceShortWords(2, "");
                tweetModel.ExtractAndReplaceMakeCodes("@@");
                tweetModel.ExtractAndReplaceModelCodes("##");
                tweetModel.ExtractAndReplaceStopWords("");
                tweetModel.ReplaceRetweetToken("");
                tweetModel.RemoveDoubleSpaces();
                */
                //tweetModel.ExtractDomainSpecificWords();
                //tweetModel.ExtractWordUnigrams();
                //tweetModel.ExtractWordBigrams();
                //tweetModel.ExtractCharacterBigrams();

                preprocessedTweets.Rows.Add(null, tweetModel.TweetId, tweetModel.ProcessedText, preProcessDate, tweetModel.FirstLetterCapitalizedWords, tweetModel.Hashtags, tweetModel.Mentions, tweetModel.PunctuationMarks, tweetModel.MakeCodes, tweetModel.ModelCodes, tweetModel.StopWords, tweetModel.Emoticons
                    //, tweetModel.WordUnigrams, tweetModel.WordBigrams, tweetModel.CharacterBigrams
                    , tweetModel.SentenceEndingCharacter
                    );

                if (counter % batchSize == 0 && preprocessedTweets.Rows.Count > 0)
                {
                    DataHelper.WritePreProcessedTweetsToDatabase(preprocessedTweets);
                    preprocessedTweets = DataHelper.GeneratePreProcessedDataTable();
                }
            }

            if (preprocessedTweets.Rows.Count > 0)
                DataHelper.WritePreProcessedTweetsToDatabase(preprocessedTweets);
        }

        public static void ExtractFirstLetterCapitalizedWords(this TweetModel model, int minWordLength)
        {
            var extracted = string.Empty;

            var modelCopy = new TweetModel(model.TweetId, model.ProcessedText);
            modelCopy.ExtractAndReplacePunctuationMarks(" ");

            var textSplit = modelCopy.ProcessedText.Split(new string[] { " " }, StringSplitOptions.RemoveEmptyEntries);
            foreach (var word in textSplit)
            {
                if (word.Length >= minWordLength
                    && char.IsUpper(word[0])) //Is capitalized
                    extracted += word + ExtractionDelimiter;
            }

            if (extracted.Length > 0)
                extracted = extracted.Remove(extracted.Length - ExtractionDelimiter.Length);

            model.FirstLetterCapitalizedWords = extracted.Trim();
        }

        public static void ExtractAndReplaceMentions(this TweetModel model, string replaceToken)
        {
            var extracted = ReplaceAndExtractTexts(model, "@", " ", ExtractionDelimiter, replaceToken);

            model.Mentions = extracted;
        }

        public static void ExtractHashtags(this TweetModel model)
        {
            var modelCopy = new TweetModel(model.TweetId, model.ProcessedText);
            var extracted = ReplaceAndExtractTexts(modelCopy, "#", " ", ExtractionDelimiter, " ");

            model.Hashtags = extracted;
        }

        public static void ReplaceCharacters(this TweetModel model, string replaceString, string replaceToken)
        {
            var result = string.Empty;

            result = model.ProcessedText.Replace(replaceString, replaceToken).Trim();

            model.ProcessedText = result;
        }

        public static void ExtractAndReplaceEmoticons(this TweetModel model, string replaceToken)
        {
            var extracted = ReplaceArrayValues(model, EmoticonDictionary.Keys.ToArray(), ExtractionDelimiter, replaceToken);

            model.Emoticons = extracted;
        }

        public static void ExtractAndReplacePunctuationMarks(this TweetModel model, string replaceToken)
        {
            var extracted = ReplaceArrayValues(model, PunctuationMarks.ToArray().Select(x => x.ToString()).ToArray(), " ", replaceToken);

            model.PunctuationMarks = extracted;
        }

        public static void ReplaceShortWords(this TweetModel model, int minWordLength, string replaceToken)
        {
            var result = string.Empty;

            var textSplit = model.ProcessedText.Split(new string[] { " " }, StringSplitOptions.RemoveEmptyEntries);
            foreach (var word in textSplit)
            {
                result += (word.Length >= minWordLength ? word : replaceToken) + " ";
            }
            result = result.Trim();

            model.ProcessedText = result;
        }

        public static void ExtractAndReplaceMakeCodes(this TweetModel model, string replaceToken)
        {
            var extracted = ReplaceArrayValues(model, MakeDictionary.Values.ToArray(), ExtractionDelimiter, replaceToken, true);

            model.MakeCodes = extracted;
        }

        public static void ExtractAndReplaceModelCodes(this TweetModel model, string replaceToken)
        {
            var extracted = ReplaceArrayValues(model, ModelDictionary.Values.ToArray(), ExtractionDelimiter, replaceToken, true);

            model.ModelCodes = extracted;
        }

        public static void ExtractAndReplaceStopWords(this TweetModel model, string replaceToken)
        {
            var extracted = ReplaceArrayValues(model, TurkishStopWords, ExtractionDelimiter, replaceToken, true);

            model.StopWords = extracted;
        }

        public static void ExtractDomainSpecificWords(this TweetModel model)
        {
            var extracted = ExtractArrayValues(model.ProcessedText, DomainSpecificWords, ExtractionDelimiter, true);

            model.DomainSpecificWords = extracted;
        }

        public static void ReplaceRetweetToken(this TweetModel model, string replaceToken)
        {
            var result = model.ProcessedText.IndexOf("RT ") == 0 ? (replaceToken.Length > 0 ? replaceToken + " " : replaceToken) + model.ProcessedText.Substring(0, 3) : model.ProcessedText;

            model.ProcessedText = result;
        }

        public static void RemoveDoubleSpaces(this TweetModel model)
        {
            var result = model.ProcessedText;

            RegexOptions options = RegexOptions.None;
            Regex regex = new Regex("[ ]{2,}", options);
            result = regex.Replace(result, " ");

            model.ProcessedText = result;
        }

        public static void ExtractWordUnigrams(this TweetModel model)
        {
            var extracted = ExtractWordNgrams(model, ExtractionDelimiter, 1);

            model.WordUnigrams = extracted;
        }

        public static void ExtractWordBigrams(this TweetModel model)
        {
            var extracted = ExtractWordNgrams(model, ExtractionDelimiter, 2);

            model.WordBigrams = extracted;
        }

        public static void ExtractCharacterBigrams(this TweetModel model)
        {
            var extracted = ExtractCharacterNgrams(model, ExtractionDelimiter, 2);

            model.CharacterBigrams = extracted;
        }

        public static void ReplaceHashtags(this TweetModel model, string replaceToken)
        {
            var result = ReplaceAndExtractTexts(model, "#", " ", ExtractionDelimiter, replaceToken);

            model.ProcessedText = result;
        }

        public static string NLPInputFormat(this NLPLine nlpLine)
        {
            nlpLine.LineText = nlpLine.LineText.Replace("?", "Guess");
            var result = nlpLine.TokenIndex.ToString() + "\t";
            var counter = 0;

            var split = nlpLine.LineText.Replace(" ", "\t").Replace("+", "\t").Split('\t');
            foreach (var item in split)
            {
                if (counter < 3)
                {
                    if (counter == 1 && string.IsNullOrEmpty(item))
                        result += split[0] + "\t";
                    else
                        result += item + "\t";
                    if (counter == 2)
                        result += item + "\t";
                }
                else
                {
                    result += item + "|";
                }

                counter++;
            }

            if (split.Count() <= 2)
                result += "Guess\tGuess\t-";
            else if (split.Count() == 3)
                result += "-";
            else
                result = result.Remove(result.Length - 1);

            return result;
        }

        #region | Private Methods |
        private static string ReplaceAndExtractTexts(TweetModel model, string prefixString, string stopString, string extractionDelimiter, string replaceToken)
        {
            var text = model.ProcessedText;
            var extracted = string.Empty;

            var separators = new string[] { prefixString };
            var splittedText = text.Split(separators, StringSplitOptions.RemoveEmptyEntries);

            if (splittedText.Count() > 0)
                foreach (var subText in splittedText)
                {
                    var splittedSubText = subText.Split(new string[] { stopString }, StringSplitOptions.RemoveEmptyEntries);
                    if (splittedSubText.Count() > 0)
                    {
                        var replaceText = prefixString + splittedSubText[0];
                        if (text.IndexOf(replaceText) >= 0)
                        {
                            text = text.Replace(replaceText, replaceToken);
                            extracted += replaceText + extractionDelimiter;
                        }
                    }
                }

            model.ProcessedText = text.Trim();

            if (extracted.Length > 0)
                extracted = extracted.Remove(extracted.Length - extractionDelimiter.Length);

            return extracted.Trim();
        }

        private static string ReplaceArrayValues(TweetModel model, string[] array, string extractionDelimiter, string replaceToken, bool wordsToBeReplaced = false)
        {
            var result = model.ProcessedText;
            var extracted = string.Empty;
            if (wordsToBeReplaced) result = " " + result + " ";

            foreach (var value in array)
            {
                var searchValue = value;
                var replaceText = replaceToken;
                if (wordsToBeReplaced) searchValue = " " + searchValue + " ";
                if (wordsToBeReplaced) replaceText = " " + replaceText + " ";

                if (result.IndexOf(searchValue) >= 0)
                    extracted += value + extractionDelimiter;

                result = result.Replace(searchValue, replaceText);
            }

            if (extracted.Length > 0)
                extracted = extracted.Remove(extracted.Length - extractionDelimiter.Length);

            model.ProcessedText = result.Trim();
            return extracted;
        }

        private static string ExtractArrayValues(string text, string[] array, string extractionDelimiter, bool wordsToBeExtracted = false)
        {
            var result = text;
            var extracted = string.Empty;
            if (wordsToBeExtracted) result = " " + result + " ";

            foreach (var value in array)
            {
                var searchValue = value;
                var replaceToken = "-";
                if (wordsToBeExtracted) searchValue = " " + searchValue + " ";
                if (wordsToBeExtracted) replaceToken = " " + replaceToken + " ";

                if (result.IndexOf(searchValue) >= 0)
                    extracted += value + extractionDelimiter;

                result = result.Replace(searchValue, replaceToken);
            }

            if (extracted.Length > 0)
                extracted = extracted.Remove(extracted.Length - extractionDelimiter.Length);

            return extracted;
        }

        private static string ExtractWordNgrams(this TweetModel model, string extractionDelimiter, int n)
        {
            var extracted = string.Empty;
            var ngram = string.Empty;
            var splitted = model.ProcessedText.Split(' ');
            var counter = 0;

            for (int i = 0; i < splitted.Length; i++)
            {
                if (string.IsNullOrEmpty(splitted[i]))
                    continue;

                counter++;
                ngram += splitted[i] + " ";

                if (counter % n == 0)
                {
                    ngram = ngram.Remove(ngram.Length - 1);
                    extracted += ngram + extractionDelimiter;

                    ngram = string.Empty;
                    counter = 0;
                    i -= n - 1;
                }
            }

            if (extracted.Length > 0)
                extracted = extracted.Remove(extracted.Length - extractionDelimiter.Length);

            return extracted;
        }

        public static string ExtractWordNgrams(this List<TweetTokenAttributeModel> wordsList, string extractionDelimiter, int n)
        {
            var extracted = string.Empty;
            var ngram = string.Empty;
            var counter = 0;

            for (int i = 0; i < wordsList.Count; i++)
            {
                var tokenAttribute = wordsList[i];
                var word = tokenAttribute.TokenStem;
                if (string.IsNullOrEmpty(word))
                    continue;

                word = (tokenAttribute.IsNegated == 1 ? "-" : "")
                       + tokenAttribute.POSTag == "Verb" ? NLPProcessing.AddVerbSuffix(word) : word;

                counter++;
                ngram += word + " ";

                if (counter % n == 0)
                {
                    ngram = ngram.Remove(ngram.Length - 1);
                    extracted += ngram + extractionDelimiter;

                    ngram = string.Empty;
                    counter = 0;
                    i -= n - 1;
                }
            }

            if (extracted.Length > 0)
                extracted = extracted.Remove(extracted.Length - extractionDelimiter.Length);

            return extracted;
        }

        public static string ExtractPOSNgrams(this List<TweetTokenAttributeModel> wordsList, string extractionDelimiter, int n)
        {
            var extracted = string.Empty;
            var ngram = string.Empty;
            var counter = 0;

            for (int i = 0; i < wordsList.Count; i++)
            {
                var tokenAttribute = wordsList[i];
                var word = tokenAttribute.POSTag;
                if (string.IsNullOrEmpty(word))
                    continue;

                counter++;
                ngram += word + " ";

                if (counter % n == 0)
                {
                    ngram = ngram.Remove(ngram.Length - 1);
                    extracted += ngram + extractionDelimiter;

                    ngram = string.Empty;
                    counter = 0;
                    i -= n - 1;
                }
            }

            if (extracted.Length > 0)
                extracted = extracted.Remove(extracted.Length - extractionDelimiter.Length);

            return extracted;
        }

        private static string ExtractCharacterNgrams(this TweetModel model, string extractionDelimiter, int n)
        {
            var extracted = string.Empty;
            var ngram = string.Empty;
            var counter = 0;

            for (int i = 0; i < model.ProcessedText.Length; i++)
            {
                var characterString = model.ProcessedText[i].ToString();

                counter++;
                ngram += characterString;

                if (counter % n == 0)
                {
                    extracted += ngram + extractionDelimiter;

                    ngram = string.Empty;
                    counter = 0;
                    i -= n - 1;
                }
            }

            if (extracted.Length > 0)
                extracted = extracted.Remove(extracted.Length - extractionDelimiter.Length);

            return extracted;
        }
        #endregion
    }
}
