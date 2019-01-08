using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TwitterSentiment.Web.Services;
using TwitterSentiment.Web.Models;
using System.IO;
using System.Collections.Generic;

namespace TwitterSentiment.Tests.Services
{
    [TestClass]
    public class TwitterServiceTest
    {
        [TestMethod]
        public void Authenticate()
        {
            var twitterService = new TwitterService();

            twitterService.Authenticate();
        }

        [TestMethod]
        public void GetTweets()
        {
            var twitterService = new TwitterService();

            twitterService.Authenticate();

            var tweetSearchList = new List<TweetSearchModel>();
            //tweetSearchList.Add(new TweetSearchModel { Query = "boyner", IsTurkishOnly = true, MaxTweetCount = 12000, IsUrlFree = true });

            //tweetSearchList.Add(new TweetSearchModel { Query = "borusanoto%20OR%20borusanotomotiv%20OR%20kosifleroto", IsTurkishOnly = false, MaxTweetCount = 1000 });
            //tweetSearchList.Add(new TweetSearchModel { Query = "borusan%20oto%20OR%20otomotiv%20OR%20bmw%20OR%20mini%20OR%20cooper%20OR%20minicooper%20OR%20clubman%20OR%20motor%20OR%20motorrad%20OR%20land%20OR%20range%20OR%20rover%20OR%20jaguar%20OR%20avcılar%20OR%20istinye%20OR%20esenboğa%20OR%20balgat%20OR%20birlik%20OR%20adana%20OR%20mersin%20OR%20bodrum%20OR%20samandıra%20OR%20vadi%20OR%20istanbul%20OR%20dolmabahçe%20OR%20çatalca%20OR%20ataşehir%20OR%20kıbrıs%20OR%20çorlu%20OR%20ankara", IsTurkishOnly = false, MaxTweetCount = 1000 });
            //tweetSearchList.Add(new TweetSearchModel { Query = "kosifler%20oto%20OR%20otomotiv%20OR%20bmw%20OR%20mini%20OR%20cooper%20OR%20minicooper%20OR%20clubman%20OR%20motor%20OR%20motorrad%20OR%20land%20OR%20range%20OR%20rover%20OR%20jaguar%20OR%20bostancı%20OR%20kavacık%20OR%20antalya%20OR%20samsun%20OR%20istanbul", IsTurkishOnly = false, MaxTweetCount = 1000 });
            //tweetSearchList.Add(new TweetSearchModel { Query = "bmw%20OR%20bmwmotor%20OR%20minicooper%20OR%20motorrad%20OR%20landrover%20OR%20rangerover", IsTurkishOnly = true, MaxTweetCount = 1000 });
            //tweetSearchList.Add(new TweetSearchModel { Query = "land%20rover", IsTurkishOnly = true, MaxTweetCount = 1000 });
            //tweetSearchList.Add(new TweetSearchModel { Query = "range%20rover", IsTurkishOnly = true, MaxTweetCount = 1000 });
            //tweetSearchList.Add(new TweetSearchModel { Query = "mini%20cooper", IsTurkishOnly = true, MaxTweetCount = 1000 });
            //tweetSearchList.Add(new TweetSearchModel { Query = "jaguar%20fpace%20OR%20ftype%20OR%20araç%20OR%20araçlar%20OR%20oto%20OR%20otomobil%20OR%20otomobiller", IsTurkishOnly = true, MaxTweetCount = 1000 });
            //tweetSearchList.Add(new TweetSearchModel { Query = "to%3Aborusanoto%20OR%20to%3Aborusanotomotiv%20OR%20to%3Aborusanpremium%20OR%20to%3Abotoistinye%20OR%20to%3Abotoavcilar%20OR%20to%3Abotoankara%20OR%20to%3Abotoadana%20OR%20to%3Abotodbahce%20OR%20to%3Akosifleroto", IsTurkishOnly = false, MaxTweetCount = 1000 });
            //tweetSearchList.Add(new TweetSearchModel { Query = "%40borusanoto%20OR%20%40borusanotomotiv%20OR%20%40borusanpremium%20OR%20%40botoistinye%20OR%20%40botoavcilar%20OR%20%40botoankara%20OR%20%40botoadana%20OR%20%40botodbahce%20OR%20%40kosifleroto", IsTurkishOnly = false, MaxTweetCount = 1000 });

            tweetSearchList.Add(new TweetSearchModel { Query = "Renault%20OR%20Volkswagen%20OR%20Ford%20OR%20BMW%20OR%20Mercedes%20OR%20Hyundai%20OR%20Peugeot%20OR%20Audi%20OR%20Honda%20OR%20Citroen%20OR%20Skoda%20OR%20Seat%20OR%20Chevrolet%20OR%20Dacia%20OR%20Volvo%20OR%20Opel%20OR%20Nissan%20OR%20Toyota%20-maç%20-basketbol%20-arena%20-sanat%20-fashion%20-sahne%20-futbol%20-spor%20-basket", IsTurkishOnly = true, MaxTweetCount = 40000, MaxId = 0, SinceId = 980514266611675137 });

            //tweetSearchList.Add(new TweetSearchModel { Query = "doğuş%20oto%20OR%20otomotiv%20OR%20vw%20OR%20volkswagen%20OR%20audi%20OR%20seat%20OR%20skoda", IsTurkishOnly = true, MaxTweetCount = 40000, MaxId = 0, SinceId = 0 });
            //tweetSearchList.Add(new TweetSearchModel { Query = "borusan%20oto%20OR%20otomotiv%20OR%20bmw%20OR%20mini%20OR%20cooper%20OR%20land%20OR%20range%20OR%20rover%20OR%20jaguar", IsTurkishOnly = true, MaxTweetCount = 40000, MaxId = 0, SinceId = 0 });
            //tweetSearchList.Add(new TweetSearchModel { Query = "kosifler%20oto%20OR%20otomotiv%20OR%20bmw%20OR%20mini%20OR%20cooper%20OR%20land%20OR%20range%20OR%20rover%20OR%20jaguar", IsTurkishOnly = true, MaxTweetCount = 40000, MaxId = 0, SinceId = 0 });
            //tweetSearchList.Add(new TweetSearchModel { Query = "koç%20oto%20OR%20otomotiv%20OR%20otosan%20OR%20ford%20OR%20volvo%20-ali", IsTurkishOnly = true, MaxTweetCount = 40000, MaxId = 0, SinceId = 0 });
            //tweetSearchList.Add(new TweetSearchModel { Query = "tofaş%20oto%20OR%20otomotiv", IsTurkishOnly = true, MaxTweetCount = 40000, MaxId = 0, SinceId = 0 });
            //tweetSearchList.Add(new TweetSearchModel { Query = "hyundai%20oto%20OR%20otomotiv", IsTurkishOnly = true, MaxTweetCount = 40000, MaxId = 0, SinceId = 0 });
            //tweetSearchList.Add(new TweetSearchModel { Query = "honda%20oto%20OR%20otomotiv", IsTurkishOnly = true, MaxTweetCount = 40000, MaxId = 0, SinceId = 0 });


            var tweets = new List<TweetModel>();
            foreach (var tweetSearch in tweetSearchList)
            {
                tweets.AddRange(twitterService.GetTweets(tweetSearch));
            }

            var excelData = ExcelExportService.ExportExcel(tweets);

            File.WriteAllBytes(Guid.NewGuid().ToString() + ".xlsx", excelData);
        }

        [TestMethod]
        public void GetUrlFreeText()
        {
            var twitterService = new TwitterService();

            var text = @"https://t.co/wuw57RB6Ds BAU İlkbahar trofesi ilk ayak ikinci https://t.co/wuw57RB6Ds gün fotoğrafları haberimize eklenmiştir...
""BAU Trofesi’nde ilk ayağın kazananı Ford Otosan""
https://t.co/3H5VwiTOht";
            var text2 = @"Ich mag das @YouTube-Video: https://t.co/Omyjuew9ik CİTY CAR DRİVİNG BMW M5 (MOD NASIL YAPILIR)*LİNK AÇIKLAMADA*https://t.co/wuw57RB6Ds";
            var text3 = @"https://t.co/wuw57RB6Ds Kocasının hatırasını 1968 https://t.co/wuw57RB6Ds model Mercedes'te yaşatıyor haberin detayı için tıklayınız...";
            var text4 = @"Kocasının hatırasını 1968 model Mercedes'te yaşatıyor haberin detayı için tıklayınız...";

            var urlFreeText = twitterService.GetUrlFreeText(text);
            var urlFreeText2 = twitterService.GetUrlFreeText(text2);
            var urlFreeText3 = twitterService.GetUrlFreeText(text3);
            var urlFreeText4 = twitterService.GetUrlFreeText(text4);
        }

        [TestMethod]
        public void GetStringOccuranceCount()
        {
            var twitterService = new TwitterService();

            var text = @"https://t.co/wuw57RB6Ds BAU İlkbahar trofesi ilk ayak ikinci https://t.co/wuw57RB6Ds gün fotoğrafları haberimize eklenmiştir...
""BAU Trofesi’nde ilk ayağın kazananı Ford Otosan""
https://t.co/3H5VwiTOht";
            var text2 = @"Ich mag das @YouTube-Video: https://t.co/Omyjuew9ik CİTY CAR DRİVİNG BMW M5 (MOD NASIL YAPILIR)*LİNK AÇIKLAMADA*https://t.co/wuw57RB6Ds";
            var text3 = @"https://t.co/wuw57RB6Ds Kocasının hatırasını 1968 https://t.co/wuw57RB6Ds model Mercedes'te yaşatıyor haberin detayı için tıklayınız...";
            var text4 = @"Kocasının hatırasını 1968 model Mercedes'te yaşatıyor haberin detayı için tıklayınız...";

            var count = twitterService.GetUrlOccuranceCount(text);
            var count2 = twitterService.GetUrlOccuranceCount(text2);
            var count3 = twitterService.GetUrlOccuranceCount(text3);
            var count4 = twitterService.GetUrlOccuranceCount(text4);
        }
    }
}
