using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using TwitterSentiment.NLP.Models;

namespace TwitterSentiment.NLP
{
    public static class DataHelper
    {
        #region | Constants |
        private const string Connectionstring = "Data Source=.\\SQLEXPRESS01;initial catalog=MachineMentor;Integrated Security=SSPI"; //;uid=.\\administrator;pwd=H3lpd3sk!";
        private const string NLPRootFolderPath = @"C:\Users\erdemc.BORINTERNAL\Downloads\SA in Turkish\";

        private const string GetRawTweetsQuery =
            @"SELECT Id, Text FROM AutomotiveTweetsData";
        private const string GetMakeCodesQuery =
            @"SELECT DISTINCT Code, DisplayName FROM AutomotiveMake";
        private const string GetModelCodesQuery =
            @"SELECT DISTINCT Code, DisplayName FROM AutomotiveModel";
        private const string GetEmoticonsQuery =
            @"SELECT Emoticon, Sentiment FROM Emoticons";
        private const string GetPreProcessedTweetsQuery =
            @"SELECT
                TweetId
                ,FirstLetterCapitalizedWords
                ,Hashtags
                ,PunctuationMarks
                ,StopWords
                ,Emoticons
                ,WordUnigrams
                ,WordBigrams
                ,CharacterBigrams
            FROM PreProcessedTweetsData";
        private const string GetParsedDependenciesQuery =
            @"SELECT
	            TweetId
	            , TokenIndex
	            , [Text]
            FROM NLPProcessedTweetsData
            WHERE [Type] = 'Dependency Parser'
	            AND TokenIndex > 0";
        private const string GetNamedEntitiesQuery =
            @"SELECT
	            TweetId
	            , TokenIndex
	            , [Text]
            FROM NLPProcessedTweetsData
            WHERE [Type] = 'Named Entity Recognizer'
	            AND TokenIndex > 0";
        private const string GetDependencyParsedTweetsQuery =
            @"SELECT 
                dp.TweetId
                , dp.TokenIndex
                , Token
                , TokenStem
                , POSTag
                , DefinitiveTags
                , RelatedToIndex
                , TokenRelation 
                , ISNULL(IsNegated, 0) IsNegated
                , ISNULL(TokenEng, '') TokenEng 
                , ISNULL(SenticNetPolarity, 0) PolarityScore
            FROM DependencyParsedTweetsData dp
            LEFT JOIN TweetTokenExtraInfoData tt ON tt.TweetId = dp.TweetId AND tt.TokenIndex = dp.TokenIndex
            WHERE dp.TweetId > 0";
        private const string GetTweetAttributesQuery =
            @"SELECT 
	            pp.TweetId
	            , ProcessedText
	            , FirstLetterCapitalizedWords
	            , Hashtags
	            , PunctuationMarks
	            , StopWords
	            , Emoticons
	            , WordUnigrams
	            , WordBigrams
	            , CharacterBigrams
	            , EndingCharacter
                , Mentions
            FROM PreProcessedTweetsData pp
            INNER JOIN NgramExtractedTweetsData ng ON ng.TweetId = pp.TweetId
            ORDER BY pp.TweetId";
        private const string GetTweetTokenAttributesQuery =
            @"SELECT 
	            dp.TweetId
	            , dp.TokenIndex
	            , Token
	            , TokenStem
	            , POSTag
	            , DefinitiveTags
	            , IsNegated
	            , SenticNetPolarityLabel
	            , SenticNetPolarity
	            , IsDomainSpecific
	            , SentiTurkNetPolarityLabel
	            , SentiTurkNetNegPolarity
	            , SentiTurkNetObjPolarity
	            , SentiTurkNetPosPolarity
	            , RelatedToIndex
	            , TokenRelation
	            , IsTurkish
                , NamedEntity
            FROM DependencyParsedTweetsData dp
            INNER JOIN TweetTokenExtraInfoData tt ON tt.TweetId = dp.TweetId AND tt.TokenIndex = dp.TokenIndex
            ORDER BY dp.TweetId, dp.TokenIndex";
        #endregion

        public static Dictionary<int, string> GetRawTweets()
        {
            var tweets = new Dictionary<int, string>();

            using (SqlConnection connection = new SqlConnection(Connectionstring))
            {
                connection.Open();

                using (SqlCommand command = new SqlCommand(GetRawTweetsQuery, connection))
                {
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            tweets.Add(reader.GetInt32(0), reader.GetString(1));
                        }
                    }
                }
            }

            return tweets;
        }

        public static Dictionary<string, string> GetMakeCodes()
        {
            var result = new Dictionary<string, string>();

            using (SqlConnection connection = new SqlConnection(Connectionstring))
            {
                connection.Open();

                using (SqlCommand command = new SqlCommand(GetMakeCodesQuery, connection))
                {
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            result.Add(reader.GetString(0), reader.GetString(1));
                        }
                    }
                }
            }

            return result;
        }

        public static Dictionary<string, string> GetModelCodes()
        {
            var result = new Dictionary<string, string>();

            using (SqlConnection connection = new SqlConnection(Connectionstring))
            {
                connection.Open();

                using (SqlCommand command = new SqlCommand(GetModelCodesQuery, connection))
                {
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            result.Add(reader.GetString(0), reader.GetString(1));
                        }
                    }
                }
            }

            return result;
        }

        public static Dictionary<string, int> GetEmoticons()
        {
            var result = new Dictionary<string, int>();

            using (SqlConnection connection = new SqlConnection(Connectionstring))
            {
                connection.Open();

                using (SqlCommand command = new SqlCommand(GetEmoticonsQuery, connection))
                {
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            result.Add(reader.GetString(0), reader.GetInt32(1));
                        }
                    }
                }
            }

            return result;
        }

        public static List<PreProcessedTweet> GetPreProcessedTweets()
        {
            var tweets = new List<PreProcessedTweet>();

            using (SqlConnection connection = new SqlConnection(Connectionstring))
            {
                connection.Open();

                using (SqlCommand command = new SqlCommand(GetPreProcessedTweetsQuery, connection))
                {
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            tweets.Add(new PreProcessedTweet
                            {
                                TweetId = reader.GetInt32(0),
                                FirstLetterCapitalizedWords = reader.GetString(1),
                                Hashtags = reader.GetString(2),
                                PunctuationMarks = reader.GetString(3),
                                StopWords = reader.GetString(4),
                                Emoticons = reader.GetString(5),
                                WordUnigrams = reader.GetString(6),
                                WordBigrams = reader.GetString(7),
                                CharacterBigrams = reader.GetString(8)
                            });
                        }
                    }
                }
            }

            return tweets;
        }

        public static List<NLPLine> GetNLPFile(string fileName, bool isSingleLine, int startingTweetId = 1)
        {
            var result = new List<NLPLine>();
            var tweetId = startingTweetId;
            var tokenIndex = 1;

            string line;

            var file = new StreamReader(NLPRootFolderPath + fileName);
            while ((line = file.ReadLine()) != null)
            {
                if (!isSingleLine && (line.IndexOf("<S> <S>+BSTag") >= 0 || line.IndexOf("</S> </S>+ESTag") >= 0))
                {
                    if (line.IndexOf("</S> </S>+ESTag") >= 0)
                    {
                        tweetId++;
                        tokenIndex = 1;
                    }
                    continue;
                }

                result.Add(new NLPLine { TweetId = tweetId, TokenIndex = tokenIndex, LineText = line });

                tokenIndex++;
                if (isSingleLine)
                    tweetId++;
            }

            file.Close();

            return result;
        }

        public static List<SenticNetModel> GetSenticNetFile()
        {
            var result = new List<SenticNetModel>();

            string line;

            var file = new StreamReader(NLPRootFolderPath + "senticnet5.txt");
            while ((line = file.ReadLine()) != null)
            {
                var split = line.Split(new[] { "[", "]" }, StringSplitOptions.None);

                var concept = split[1].Replace("'", "").Replace("_", " ").Trim();
                var attributes = split[3].Replace("'", "").Replace("_", " ").Replace("#", "").Split(',');

                var pleasantness = Convert.ToDecimal(attributes[0].Replace(".", ",").Trim());
                var attention = Convert.ToDecimal(attributes[1].Replace(".", ",").Trim());
                var sensitivity = Convert.ToDecimal(attributes[2].Replace(".", ",").Trim());
                var aptitude = Convert.ToDecimal(attributes[3].Replace(".", ",").Trim());
                var primaryMood = attributes[4].Trim();
                var secondaryMood = attributes[5].Trim();
                var polarityLabel = attributes[6].Trim();
                var polarity = Convert.ToDecimal(attributes[7].Replace(".", ","));
                var semantics1 = attributes.Length > 8 ? attributes[8].Trim() : string.Empty;
                var semantics2 = attributes.Length > 9 ? attributes[9].Trim() : string.Empty;
                var semantics3 = attributes.Length > 10 ? attributes[10].Trim() : string.Empty;
                var semantics4 = attributes.Length > 11 ? attributes[11].Trim() : string.Empty;
                var semantics5 = attributes.Length > 12 ? attributes[12].Trim() : string.Empty;

                result.Add(new SenticNetModel { Concept = concept, Pleasantness = pleasantness, Attention = attention, Sensitivity = sensitivity, Aptitude = aptitude, PrimaryMood = primaryMood, SecondaryMood = secondaryMood, PolarityLabel = polarityLabel, Polarity = polarity, Semantics1 = semantics1, Semantics2 = semantics2, Semantics3 = semantics3, Semantics4 = semantics4, Semantics5 = semantics5 });
            }

            file.Close();

            return result;
        }

        public static List<SentiTurkNetModel> GetSentiTurkNetFile()
        {
            var result = new List<SentiTurkNetModel>();

            string line;

            var file = new StreamReader(NLPRootFolderPath + "sentiturknet.txt");
            while ((line = file.ReadLine()) != null)
            {
                var split = line.Split('\t');

                var synonyms = split[0].Replace("  ", " ").Replace(";", ",").Replace(" , ", ",").Replace(", ", ",").Replace(" ,", ",").Trim();

                if (string.IsNullOrEmpty(synonyms))
                    continue;

                var gloss = split[1].Replace("  ", " ").Replace(" , ", ",").Replace(";", ",").Replace(", ", ",").Replace(" ,", ",").Trim();
                var polarityLabel = split[2].Trim();
                var posTag = split[3].Trim();
                var negPolarity = Convert.ToDecimal(split[4].Replace(".", ",").Trim());
                var objPolarity = Convert.ToDecimal(split[5].Replace(".", ",").Trim());
                var posPolarity = Convert.ToDecimal(split[6].Replace(".", ",").Trim());

                result.Add(new SentiTurkNetModel { Synonyms = synonyms, Gloss = gloss, PolarityLabel = polarityLabel, POSTag = posTag, NegPolarity = negPolarity, ObjPolarity = objPolarity, PosPolarity = posPolarity });
            }

            file.Close();

            return result;
        }

        public static List<NLPLine> GetParsedDependencies()
        {
            var tweets = new List<NLPLine>();

            using (SqlConnection connection = new SqlConnection(Connectionstring))
            {
                connection.Open();

                using (SqlCommand command = new SqlCommand(GetParsedDependenciesQuery, connection))
                {
                    command.CommandTimeout = 3600;
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            tweets.Add(new NLPLine
                            {
                                TweetId = reader.GetInt32(0),
                                TokenIndex = reader.GetInt32(1),
                                LineText = reader.GetString(2)
                            });
                        }
                    }
                }
            }

            return tweets;
        }

        public static List<DependencyParsedTweet> GetDependencyParsedTweets()
        {
            var tweets = new List<DependencyParsedTweet>();

            using (SqlConnection connection = new SqlConnection(Connectionstring))
            {
                connection.Open();

                using (SqlCommand command = new SqlCommand(GetDependencyParsedTweetsQuery, connection))
                {
                    command.CommandTimeout = 3600;
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            tweets.Add(new DependencyParsedTweet
                            {
                                TweetId = reader.GetInt32(0),
                                TokenIndex = reader.GetInt32(1),
                                Token = reader.GetString(2),
                                TokenStem = reader.GetString(3),
                                POSTag = reader.GetString(4),
                                DefinitiveTags = reader.GetString(5),
                                RelatedToIndex = reader.GetInt32(6),
                                TokenRelation = reader.GetString(7),
                                IsNegated = reader.GetInt32(8),
                                TokenEng = reader.GetString(9),
                                PolarityScore = reader.GetDecimal(10)
                            });
                        }
                    }
                }
            }

            return tweets;
        }
        public static List<TweetAttributeModel> GetTweetAttributes()
        {
            var tweets = new List<TweetAttributeModel>();

            using (SqlConnection connection = new SqlConnection(Connectionstring))
            {
                connection.Open();

                using (SqlCommand command = new SqlCommand(GetTweetAttributesQuery, connection))
                {
                    command.CommandTimeout = 3600;
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            tweets.Add(new TweetAttributeModel
                            {
                                TweetId = reader.GetInt32(0),
                                ProcessedText = reader.GetString(1),
                                FirstLetterCapitalizedWords = reader.GetString(2),
                                Hashtags = reader.GetString(3),
                                PunctuationMarks = reader.GetString(4),
                                StopWords = reader.GetString(5),
                                Emoticons = reader.GetString(6),
                                WordUnigrams = reader.GetString(7),
                                WordBigrams = reader.GetString(8),
                                CharacterBigrams = reader.GetString(9),
                                EndingCharacter = reader.GetString(10),
                                Mentions = reader.GetString(11)
                            });
                        }
                    }
                }
            }

            return tweets;
        }

        public static List<TweetTokenAttributeModel> GetTweetTokenAttributes()
        {
            var tweets = new List<TweetTokenAttributeModel>();

            using (SqlConnection connection = new SqlConnection(Connectionstring))
            {
                connection.Open();

                using (SqlCommand command = new SqlCommand(GetTweetTokenAttributesQuery, connection))
                {
                    command.CommandTimeout = 3600;
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var tokenAttributeModel = new TweetTokenAttributeModel();
                            tokenAttributeModel.TweetId = reader.GetInt32(0);
                            tokenAttributeModel.TokenIndex = reader.GetInt32(1);
                            tokenAttributeModel.Token = reader.GetString(2);
                            tokenAttributeModel.TokenStem = reader.GetString(3);
                            tokenAttributeModel.POSTag = reader.GetString(4);
                            tokenAttributeModel.DefinitiveTags = reader.GetString(5);
                            tokenAttributeModel.IsNegated = reader.GetInt32(6);
                            if (!reader.IsDBNull(7))
                                tokenAttributeModel.SenticNetPolarityLabel = reader.GetString(7);
                            if (!reader.IsDBNull(8))
                                tokenAttributeModel.SenticNetPolarity = reader.GetDecimal(8);
                            if (!reader.IsDBNull(9))
                                tokenAttributeModel.IsDomainSpecific = reader.GetInt32(9);
                            if (!reader.IsDBNull(10))
                                tokenAttributeModel.SentiTurkNetPolarityLabel = reader.GetString(10);
                            if (!reader.IsDBNull(11))
                                tokenAttributeModel.SentiTurkNetNegPolarity = reader.GetDecimal(11);
                            if (!reader.IsDBNull(12))
                                tokenAttributeModel.SentiTurkNetObjPolarity = reader.GetDecimal(12);
                            if (!reader.IsDBNull(13))
                                tokenAttributeModel.SentiTurkNetPosPolarity = reader.GetDecimal(13);
                            tokenAttributeModel.RelatedToIndex = reader.GetInt32(14);
                            tokenAttributeModel.TokenRelation = reader.GetString(15);
                            tokenAttributeModel.IsTurkish = reader.GetInt32(16);
                            if (!reader.IsDBNull(17))
                                tokenAttributeModel.NamedEntity = reader.GetString(17);

                            tweets.Add(tokenAttributeModel);
                        }
                    }
                }
            }

            return tweets;
        }

        public static DataTable GenerateTweetTokenExtraInfoDataTable()
        {
            var dt = new DataTable();

            dt.Columns.Add("Id", typeof(int));
            dt.Columns.Add("TweetId", typeof(int));
            dt.Columns.Add("TokenIndex", typeof(int));
            dt.Columns.Add("IsNegated", typeof(int));
            dt.Columns.Add("TokenEng", typeof(string));
            var dc = dt.Columns.Add("SenticNetPleasantness", typeof(decimal));
            dc.AllowDBNull = true;
            dc = dt.Columns.Add("SenticNetAttention", typeof(decimal));
            dc.AllowDBNull = true;
            dc = dt.Columns.Add("SenticNetsensitivity", typeof(decimal));
            dc.AllowDBNull = true;
            dc = dt.Columns.Add("SenticNetAptitude", typeof(decimal));
            dc.AllowDBNull = true;
            dt.Columns.Add("SenticNetPrimaryMood", typeof(string));
            dt.Columns.Add("SenticNetSecondaryMood", typeof(string));
            dt.Columns.Add("SenticNetPolarityLabel", typeof(string));
            dc = dt.Columns.Add("SenticNetPolarity", typeof(decimal));
            dc.AllowDBNull = true;
            dt.Columns.Add("SenticNetSemantics1", typeof(string));
            dt.Columns.Add("SenticNetSemantics2", typeof(string));
            dt.Columns.Add("SenticNetSemantics3", typeof(string));
            dt.Columns.Add("SenticNetSemantics4", typeof(string));
            dt.Columns.Add("SenticNetSemantics5", typeof(string));
            dt.Columns.Add("IsDomainSpecific", typeof(int));
            dt.Columns.Add("SentiTurkNetSynonyms", typeof(string));
            dt.Columns.Add("SentiTurkNetGloss", typeof(string));
            dt.Columns.Add("SentiTurkNetPolarityLabel", typeof(string));
            dt.Columns.Add("SentiTurkNetPOSTag", typeof(string));
            dc = dt.Columns.Add("SentiTurkNetNegPolarity", typeof(decimal));
            dc.AllowDBNull = true;
            dc = dt.Columns.Add("SentiTurkNetObjPolarity", typeof(decimal));
            dc.AllowDBNull = true;
            dc = dt.Columns.Add("SentiTurkNetPosPolarity", typeof(decimal));
            dc.AllowDBNull = true;

            return dt;
        }

        public static void WriteTweetTokenExtraToDatabase(DataTable dt)
        {
            WriteToDatabase(dt, "TweetTokenExtraInfoData");
        }

        public static DataTable GenerateNgramExtractedTweetsDataTable()
        {
            var dt = new DataTable();

            dt.Columns.Add("Id", typeof(int));
            dt.Columns.Add("TweetId", typeof(int));
            dt.Columns.Add("ExtractedText", typeof(string));
            dt.Columns.Add("WordUnigrams", typeof(string));
            dt.Columns.Add("WordBigrams", typeof(string));
            dt.Columns.Add("CharacterBigrams", typeof(string));

            return dt;
        }

        public static void WriteNgramExtractedTweetsToDatabase(DataTable dt)
        {
            WriteToDatabase(dt, "NgramExtractedTweetsData");
        }

        public static DataTable GenerateDependencyParsedTweetsDataTable()
        {
            var dt = new DataTable();

            dt.Columns.Add("Id", typeof(int));
            dt.Columns.Add("TweetId", typeof(int));
            dt.Columns.Add("TokenIndex", typeof(int));
            dt.Columns.Add("Token", typeof(string));
            dt.Columns.Add("TokenStem", typeof(string));
            dt.Columns.Add("POSTag", typeof(string));
            dt.Columns.Add("SecondPosTag", typeof(string));
            dt.Columns.Add("DefinitiveTags", typeof(string));
            dt.Columns.Add("RelatedToIndex", typeof(string));
            dt.Columns.Add("TokenRelation", typeof(string));

            return dt;
        }

        public static void WriteDependencyParsedTweetsToDatabase(DataTable dt)
        {
            WriteToDatabase(dt, "DependencyParsedTweetsData");
        }

        public static DataTable GeneratePreProcessedDataTable()
        {
            var dt = new DataTable();

            dt.Columns.Add("Id", typeof(int));
            dt.Columns.Add("TweetId", typeof(int));
            dt.Columns.Add("ProcessedText", typeof(string));
            dt.Columns.Add("ProcessedDate", typeof(DateTime));
            dt.Columns.Add("FirstLetterCapitalizedWords", typeof(string));
            dt.Columns.Add("Hashtags", typeof(string));
            dt.Columns.Add("Mentions", typeof(string));
            dt.Columns.Add("PunctuationMarks", typeof(string));
            dt.Columns.Add("MakeCodes", typeof(string));
            dt.Columns.Add("ModelCodes", typeof(string));
            dt.Columns.Add("StopWords", typeof(string));
            dt.Columns.Add("Emoticons", typeof(string));
            //dt.Columns.Add("WordUnigrams", typeof(string));
            //dt.Columns.Add("WordBigrams", typeof(string));
            //dt.Columns.Add("CharacterBigrams", typeof(string));
            dt.Columns.Add("EndingCharacter", typeof(string));

            return dt;
        }

        public static void WritePreProcessedTweetsToDatabase(DataTable dt)
        {
            WriteToDatabase(dt, "PreProcessedTweetsData");
        }

        public static DataTable GenerateTweetFeaturesDataTable()
        {
            var dt = new DataTable();

            dt.Columns.Add("TweetId", typeof(int));
            dt.Columns.Add("WordUnigrams", typeof(string));
            dt.Columns.Add("PunctuationMarks", typeof(string));
            dt.Columns.Add("Emoticons", typeof(string));
            dt.Columns.Add("CharacterBigrams", typeof(string));
            dt.Columns.Add("WordBigrams", typeof(string));
            dt.Columns.Add("Hashtags", typeof(string));
            dt.Columns.Add("NumberOfEmoticons", typeof(string));
            dt.Columns.Add("NumberOfFirstLetterCapitalizedWords", typeof(string));
            dt.Columns.Add("NumberOfDomainSpecificWords", typeof(string));
            dt.Columns.Add("LengthOfSentence", typeof(string));
            dt.Columns.Add("NegationSuffixes", typeof(string));
            dt.Columns.Add("POSTags", typeof(string));
            dt.Columns.Add("AveragePolarity", typeof(string));
            dt.Columns.Add("Polarities", typeof(string));
            dt.Columns.Add("NumberOfPolarizedWords", typeof(string));
            dt.Columns.Add("NumberOfPOSTags", typeof(string));
            dt.Columns.Add("SentenceStructure", typeof(string));
            dt.Columns.Add("ProposedFeatures", typeof(string));
            dt.Columns.Add("ProposedFeatureNgrams", typeof(string));

            return dt;
        }

        public static DataTable GenerateTweetFeatureWordBigramsDataTable()
        {
            var dt = new DataTable();

            dt.Columns.Add("TweetId", typeof(int));
            dt.Columns.Add("WordBigrams", typeof(string));

            return dt;
        }

        public static void WriteTweetFeaturesToDatabase(DataTable dt)
        {
            WriteToDatabase(dt, "TweetFeaturesCompressed");
        }

        public static void WriteTweetFeatureWordBigramsToDatabase(DataTable dt)
        {
            UpdateTweetFeatureWordBigrams(dt);
        }

        public static DataTable GenerateNLPResultDataTable()
        {
            var dt = new DataTable();

            dt.Columns.Add("Id", typeof(int));
            dt.Columns.Add("TweetId", typeof(int));
            dt.Columns.Add("Type", typeof(string));
            dt.Columns.Add("TokenIndex", typeof(int));
            dt.Columns.Add("Text", typeof(string));

            return dt;
        }

        public static void WriteNLPResultToDatabase(DataTable dt)
        {
            WriteToDatabase(dt, "NLPProcessedTweetsData");
        }

        private static void WriteToDatabase(DataTable dataTable, string tableName)
        {
            using (SqlConnection connection = new SqlConnection(Connectionstring))
            {
                SqlBulkCopy bulkCopy =
                    new SqlBulkCopy
                    (
                    connection,
                    SqlBulkCopyOptions.TableLock |
                    SqlBulkCopyOptions.FireTriggers |
                    SqlBulkCopyOptions.UseInternalTransaction,
                    null
                    );
                bulkCopy.BulkCopyTimeout = 3600;
                bulkCopy.DestinationTableName = tableName;
                connection.Open();

                bulkCopy.WriteToServer(dataTable);

                connection.Close();
            }
        }

        public static void UpdateTweetFeatureWordBigrams(DataTable dt)
        {
            using (SqlConnection conn = new SqlConnection(Connectionstring))
            {
                using (SqlCommand command = new SqlCommand("", conn))
                {
                    try
                    {
                        conn.Open();

                        //Creating temp table on database
                        command.CommandText = "CREATE TABLE #TmpTable(TweetId int, WordBigrams nvarchar(max))";
                        command.ExecuteNonQuery();

                        //Bulk insert into temp table
                        using (SqlBulkCopy bulkcopy = new SqlBulkCopy(conn))
                        {
                            bulkcopy.BulkCopyTimeout = 660;
                            bulkcopy.DestinationTableName = "#TmpTable";
                            bulkcopy.WriteToServer(dt);
                            bulkcopy.Close();
                        }

                        // Updating destination table, and dropping temp table
                        command.CommandTimeout = 300;
                        command.CommandText = "UPDATE T SET WordBigrams = Temp.WordBigrams FROM TweetFeatures T INNER JOIN #TmpTable Temp ON T.TweetId = Temp.TweetId; DROP TABLE #TmpTable;";
                        command.ExecuteNonQuery();
                    }
                    catch (Exception ex)
                    {
                        // Handle exception properly
                    }
                    finally
                    {
                        conn.Close();
                    }
                }
            }
        }
    }
}
