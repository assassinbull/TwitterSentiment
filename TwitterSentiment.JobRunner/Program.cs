using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TwitterSentiment.NLP;

namespace TwitterSentiment.JobRunner
{
    class Program
    {
        private static Dictionary<string, string> _jobList;

        static void Main(string[] args)
        {
            //Job list
            _jobList = new Dictionary<string, string>();
            _jobList.Add("FE", "Feature Extraction Job");

            Console.WriteLine("Welcome you!");
            Console.WriteLine();

            var jobCode = string.Empty;
            do
            {
                jobCode = ExpectJobCode();
            }
            while (string.IsNullOrEmpty(jobCode));

            RunJob(jobCode);

            Console.WriteLine("Press any key to exit...");
            Console.ReadKey();
        }

        private static bool RunJob(string jobCode)
        {
            var result = false;
            switch (jobCode)
            {
                case "FE":
                    Console.WriteLine(string.Format("[{0}] {1} is starting...", DateTime.Now, _jobList[jobCode]));

                    var stopWatch = new Stopwatch();
                    stopWatch.Start();

                    try
                    {
                        var featureExtraction = new FeatureExtraction();
                        featureExtraction.ExtractFeatures();

                        stopWatch.Stop();
                        var ts = stopWatch.Elapsed;
                        var elapsedTime = string.Format("{0:00}:{1:00}:{2:00}.{3:00}", ts.Hours, ts.Minutes, ts.Seconds, ts.Milliseconds / 10);

                        Console.WriteLine(string.Format("[{0}] {1} has finished, in {2}.", DateTime.Now, _jobList[jobCode], elapsedTime));

                        result = true;
                    }
                    catch (Exception ex)
                    {
                        stopWatch.Stop();
                        var ts = stopWatch.Elapsed;
                        var elapsedTime = string.Format("{0:00}:{1:00}:{2:00}.{3:00}", ts.Hours, ts.Minutes, ts.Seconds, ts.Milliseconds / 10);

                        Console.WriteLine(string.Format("[{0}] {1} has stopped, in {2}, with error: \n{3}", DateTime.Now, _jobList[jobCode], elapsedTime, ex.ToString()));

                        result = false;
                    }

                    Console.WriteLine();
                    break;
            }

            return result;
        }

        private static string ExpectJobCode()
        {
            var jobCode = string.Empty;

            Console.WriteLine("Choose your job to run. Type job code and press Enter.");
            Console.WriteLine();
            Console.WriteLine("Code\tDescription");
            Console.WriteLine("----\t-----------");
            foreach (var job in _jobList)
            {
                Console.WriteLine(string.Format("{0}\t{1}", job.Key, job.Value));
            }
            Console.WriteLine();

            var input = Console.ReadLine();

            if (_jobList.Any(x => x.Key == input))
                jobCode = input;

            if (!string.IsNullOrEmpty(jobCode))
            {
                if (!Confirm(string.Format("{0} is about to run!", _jobList[jobCode])))
                    jobCode = string.Empty;
            }

            return jobCode;
        }

        private static bool Confirm(string message = "")
        {
            var result = false;

            Console.WriteLine();
            Console.WriteLine("{0} Do you confirm?", message);
            var input = Console.ReadLine();
            if (input.ToLower() == "y" || input.ToLower() == "yes")
            {
                Console.WriteLine("You have confirmed.");
                result = true;
            }
            else
                Console.WriteLine("You have not confirmed!");
            Console.WriteLine();

            return result;
        }
    }
}
