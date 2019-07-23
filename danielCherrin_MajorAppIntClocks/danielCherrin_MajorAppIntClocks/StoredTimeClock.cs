using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;
using System.Net.Http;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace danielCherrin_MajorAppIntClocks
{
    [Serializable]
    public class StoredTimeClock : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public StoredTimeClock()
        {

        }

        [JsonProperty]
        private string _utc_offset;

        [JsonProperty]
        public string utc_offset
        {
            get { return _utc_offset; }
            set {
                    _utc_offset = value;
                    OnPropertyChanged("utc_offset");
                }
        }

        private string _utc_datetime;

        [JsonProperty]
        public string utc_datetime
        {
            get { return _utc_datetime; }
            set {
                    _utc_datetime = value;
                    OnPropertyChanged("utc_datetime");
                }
        }

        private string _datetime;

        [JsonProperty]
        public string datetime
        {
            get { return _datetime; }
            set
                {
                    _datetime = value;
                    OnPropertyChanged("datetime");
                }
        }

        private string _timezone;

        [JsonProperty]
        public string timezone
        {
            get { return _timezone; }
            set {
                    _timezone = value;
                    OnPropertyChanged("timezone");
                }
        }

        private string _abbreviation;

        [JsonProperty]
        public string abbreviation {
            get { return _abbreviation; }
            set {
                    _abbreviation = value;
                    OnPropertyChanged("abbreviation");
                }
        }

        private DateTime _currentDateTime;

        [JsonProperty]
        public DateTime currentDateTime
        {
            get { return _currentDateTime; }
            set {
                    _currentDateTime = value;
                    OnPropertyChanged("currentDateTime");
                }
        }

        // Create the OnPropertyChanged method to raise the event
        protected void OnPropertyChanged(string name)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(name));
            }
        }

        public static async Task<StoredTimeClock> GetTimezoneClock(string zone)
        {
            var client = new HttpClient();
            var response = await client.GetAsync("http://worldtimeapi.org/api/timezone/"+zone);
            var responseString = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<StoredTimeClock>(responseString);
        }

        public void SetCurrentDateTimeClock()
        {
            currentDateTime = DateTime.Parse(datetime.Substring(0, datetime.Length - 6));
        }

    }
}
