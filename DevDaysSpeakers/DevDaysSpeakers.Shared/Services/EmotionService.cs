using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.ProjectOxford.Emotion;
using Microsoft.ProjectOxford.Emotion.Contract;
using Xamarin.Forms;

namespace DevDaysSpeakers.Services
{
    public class EmotionService
    {
        private static async Task<Emotion[]> GetHappinessAsync(string url)
        {
            var emotionClient = new EmotionServiceClient("c9d104dc21634f728e78a416d346263d");

            var emotionResults = await emotionClient.RecognizeAsync(url);

            if (emotionResults == null || emotionResults.Count() == 0)
            {
                await Application.Current.MainPage.DisplayAlert("Warning", "Can't detect face", "OK");
                //throw new Exception("Can't detect face");
            }

            return emotionResults;
        }

        //Average happiness calculation in case of multiple people
        public static async Task<float> GetAverageHappinessScoreAsync(string url)
        {
            Emotion[] emotionResults = await GetHappinessAsync(url);

            float score = 0;
            foreach (var emotionResult in emotionResults)
            {
                score = score + emotionResult.Scores.Happiness;
            }

            return score / emotionResults.Count();
        }

        public static string GetHappinessMessage(float score)
        {
            score = score * 100;
            double result = Math.Round(score, 2);

            if (score >= 50)
                return result + " % :-)";
            else
                return result + "% :-(";
        }
    }
}
