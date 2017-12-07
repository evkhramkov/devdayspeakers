using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;

using DevDaysSpeakers.Model;
using Plugin.TextToSpeech;

using DevDaysSpeakers.ViewModel;
using DevDaysSpeakers.Services;

namespace DevDaysSpeakers.View
{
    public partial class DetailsPage : ContentPage
    {
        Speaker speaker;
        SpeakersViewModel vm;
        public DetailsPage(Speaker item, SpeakersViewModel viewModel)
        {
            InitializeComponent();
            
            //Set local instance of speaker and set BindingContext
            this.speaker = item;
            this.vm = viewModel;
            BindingContext = speaker;
            ButtonSpeak.Clicked += ButtonSpeak_Clicked;
            ButtonWebsite.Clicked += ButtonWebsite_Clicked;
            ButtonAnalyze.Clicked += ButtonAnalyze_Clicked;
            ButtonSave.Clicked += ButtonSave_Clicked;

        }

        private async void ButtonSave_Clicked(object sender, EventArgs e)
        {
            speaker.Title = EntryTitle.Text;
            await vm.UpdateSpeaker(speaker);
            await Navigation.PopAsync();
        }

        private async void ButtonAnalyze_Clicked(object sender, EventArgs e)
        {
            var level = await EmotionService.GetAverageHappinessScoreAsync(this.speaker.Avatar);
            await DisplayAlert("Happiness Level", EmotionService.GetHappinessMessage(level), "OK");
        }

        private void ButtonSpeak_Clicked(object sender, EventArgs e)
        {
            CrossTextToSpeech.Current.Speak(this.speaker.Description);
        }

        private void ButtonWebsite_Clicked(object sender, EventArgs e)
        {
            if (speaker.Website.StartsWith("http"))
                Device.OpenUri(new Uri(speaker.Website));
        }

    }
}
