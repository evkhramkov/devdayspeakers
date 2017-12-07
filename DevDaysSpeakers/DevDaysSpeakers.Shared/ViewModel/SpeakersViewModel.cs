using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using DevDaysSpeakers.Model;
using DevDaysSpeakers.Services;
using System.Net.Http;
using Newtonsoft.Json;
using System.Collections.ObjectModel;
using System.Runtime.CompilerServices;

namespace DevDaysSpeakers.ViewModel
{
    public class SpeakersViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged([CallerMemberName] string name = null) =>
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));

        private bool isBusy;
        public bool IsBusy
        {
            get { return isBusy; }
            set
            {
                isBusy = value;
                OnPropertyChanged();
                //Update the can execute
                GetSpeakersCommand.ChangeCanExecute();
            }
        }

        public Command GetSpeakersCommand { get; set; }


        public ObservableCollection<Speaker> Speakers { get; set; }
        public SpeakersViewModel()
        {
            Speakers = new ObservableCollection<Speaker>();
            GetSpeakersCommand = new Command(
                async () => await GetSpeakers(),
                () => !IsBusy);
        }

        private async Task GetSpeakers()
        {
            if (IsBusy)
                return;

            Exception error = null;
            try
            {
                IsBusy = true;
                using (var client = new HttpClient())
                {

                    var service = DependencyService.Get<AzureService>();
                    var items = await service.GetSpeakers();

                    //var json = await client.GetStringAsync("http://demo4404797.mockable.io/speakers");
                    //var items = JsonConvert.DeserializeObject<List<Speaker>>(json);
                    Speakers.Clear();
                    foreach (var item in items)
                        Speakers.Add(item);
                }
            }
            catch (Exception ex)
            {
                error = ex;
            }
            finally
            {
                IsBusy = false;
            }
            if (error != null)
                await Application.Current.MainPage.DisplayAlert("Error!", error.Message, "OK");
        }

        public async Task UpdateSpeaker(Speaker speaker)
        {
            var service = DependencyService.Get<AzureService>();
            service.UpdateSpeaker(speaker);
            await GetSpeakers();
        }
    }
}
