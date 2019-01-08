using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TwitterSentiment.NLP;
using System.Collections.Generic;
using System.Data;

namespace TwitterSentiment.Tests.NLPMethods
{
    [TestClass]
    public class NLPMethodTest
    {
        [TestMethod]
        public void PreProcessTweets()
        {
            PreProcessing.WritePreProcessedTweets();
        }

        [TestMethod]
        public void NLPProcessTweets()
        {
            return;

            var featureExtraction = new NLPProcessing();

            featureExtraction.WriteNormalizedData();
            featureExtraction.WriteMorphGeneratedData();
            featureExtraction.WriteMorphDisambiguatedData();
            featureExtraction.WriteNamedEntityRecognizedData();
            featureExtraction.WriteParsedDependencyData();
        }

        [TestMethod]
        public void DependencyParseTweets()
        {
            return;

            var featureExtraction = new NLPProcessing();

            featureExtraction.WriteDependencyParsedTweets();
        }

        [TestMethod]
        public void TweetTokenExtraInfo()
        {
            var featureExtraction = new NLPProcessing();

            featureExtraction.WriteTweetTokenExtraInfo();
        }

        [TestMethod]
        public void NgramExtractTweets()
        {
            var featureExtraction = new NLPProcessing();

            featureExtraction.WriteNgramExtractedTweetsData();
        }

        [TestMethod]
        public void ExtractFeatures()
        {
            var featureExtraction = new FeatureExtraction();

            featureExtraction.ExtractFeatures(); //Insert
            //featureExtraction.ExtractWordBigramFeatures(); //Update
        }
    }
}
