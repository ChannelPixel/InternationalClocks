using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using PCLStorage;
using Newtonsoft.Json;
using System.Threading;
using System.Collections.ObjectModel;

namespace danielCherrin_MajorAppIntClocks
{


    public partial class MainPage : ContentPage
    {
        public ObservableCollection<string> localTimezones = new ObservableCollection<string>();
        public ObservableCollection<TimezonesUI> timezonesUIs = new ObservableCollection<TimezonesUI>();
        public ObservableCollection<StoredTimeClock> userClocks = new ObservableCollection<StoredTimeClock>();
        public bool counting = true;

        public MainPage()
		{
            InitializeComponent();
            SetTimezonePickerUI();
            SetUserClocksUI();
            RunClocks();
            
        }

        public async void OnClose()
        {
            counting = false;
            Debug.WriteLine("disappear");
            if (userClocks.Count > 0)
            {
                Debug.WriteLine("saving");
                IFolder folder = FileSystem.Current.LocalStorage;
                folder = await folder.CreateFolderAsync(IntClockStorage.FolderName,
                                                        CreationCollisionOption.OpenIfExists);
                IFile file = await folder.CreateFileAsync(IntClockStorage.UserClocksFileName,
                                                          CreationCollisionOption.ReplaceExisting);

                string clocksJson = JsonConvert.SerializeObject(userClocks, Formatting.Indented);
                await file.WriteAllTextAsync(clocksJson);
            }
            else
            {
                
                IFolder folder = FileSystem.Current.LocalStorage;
                folder = await folder.CreateFolderAsync(IntClockStorage.FolderName,
                                                        CreationCollisionOption.OpenIfExists);

                ExistenceCheckResult fileExists = await folder.CheckExistsAsync(IntClockStorage.UserClocksFileName);
                //Check if clocks file exists
                if (fileExists == ExistenceCheckResult.FileExists)
                {
                    Debug.WriteLine("deleting");
                    IFile file = await folder.GetFileAsync(IntClockStorage.UserClocksFileName);

                    await file.DeleteAsync();
                }
            }
        }

        async void SetTimezonePickerUI()
        {
            await TimezoneStartup();
            foreach(string zone in localTimezones)
            {
                timezonesUIs.Add(new TimezonesUI(zone.Trim('\"')));
            }

            lstvw_timezones.ItemsSource = timezonesUIs;
        }

        async void SetUserClocksUI()
        {
            await ClocksStartup();

            lstvw_clocks.ItemsSource = userClocks;
        }

        public async Task ClocksStartup()
        {
            IFolder folder = FileSystem.Current.LocalStorage;
            folder = await folder.CreateFolderAsync(IntClockStorage.FolderName,
                                                    CreationCollisionOption.OpenIfExists);
            ExistenceCheckResult fileExists = await folder.CheckExistsAsync(IntClockStorage.UserClocksFileName);
            //Check if clocks file exists
            if(fileExists == ExistenceCheckResult.FileExists)
            {
                IFile file = await folder.GetFileAsync(IntClockStorage.UserClocksFileName);
                string userclocksJson = await file.ReadAllTextAsync();
                List<StoredTimeClock> clockInputList = JsonConvert.DeserializeObject<List<StoredTimeClock>>(userclocksJson);
                foreach(StoredTimeClock clock in clockInputList)
                {
                    StoredTimeClock tempClock = await StoredTimeClock.GetTimezoneClock(clock.timezone);
                    tempClock.SetCurrentDateTimeClock();
                    userClocks.Add(tempClock);
                }
                Debug.WriteLine("Found user clocks");
            }
        }

        public void RunClocks()
        {
            Device.StartTimer(TimeSpan.FromSeconds(1), () => {
                while (userClocks.Count > 0)
                {
                    if (counting)
                    {
                        foreach (StoredTimeClock clock in userClocks)
                        {
                            clock.currentDateTime = clock.currentDateTime.AddSeconds(1);
                        }
                    }
                    return true;
                }
                return true;
            });
        }

        public async Task TimezoneStartup()
        {
            IFolder folder = FileSystem.Current.LocalStorage;
            folder = await folder.CreateFolderAsync(IntClockStorage.FolderName,
                                                    CreationCollisionOption.OpenIfExists);
            //Check if timezones file exists, otherwise create.
            ExistenceCheckResult fileExists = await folder.CheckExistsAsync(IntClockStorage.TimezonesFileName);
            if(fileExists == ExistenceCheckResult.FileExists)
            {
                IFile file = await folder.GetFileAsync(IntClockStorage.TimezonesFileName);
                string timezoneJson = await file.ReadAllTextAsync();
                localTimezones = new ObservableCollection<string>(JsonConvert.DeserializeObject<List<string>>(timezoneJson));
                Debug.WriteLine("Found Timezones");
            }
            else
            {
                IFile file = await folder.CreateFileAsync(IntClockStorage.TimezonesFileName,
                                                            CreationCollisionOption.OpenIfExists);
                localTimezones = new ObservableCollection<string>(await IntTimezones.GetTimezones());
                string timeJson = JsonConvert.SerializeObject(localTimezones, Formatting.Indented);
                await file.WriteAllTextAsync(timeJson);
                Debug.WriteLine("Not Found - Timezones");
            }
            //Check if userTimezones file exists, otherwise create.
        }

        private async void btn_addClock_Clicked(object sender, EventArgs e)
        {
            if(lstvw_timezones.SelectedItem != null)
            {
                Debug.WriteLine((lstvw_timezones.SelectedItem as TimezonesUI).timezone);
                string tzone = (lstvw_timezones.SelectedItem as TimezonesUI).timezone;
                StoredTimeClock tempClock = await StoredTimeClock.GetTimezoneClock(tzone);
                tempClock.SetCurrentDateTimeClock();
                counting = false;
                userClocks.Add(tempClock);
                counting = true;
            }
            else
            {
                await DisplayAlert("Select a timezone to add.", "Select a timezone to add.", "OK");
            }
        }

        private void lstvw_timezones_ItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            if (lstvw_timezones.SelectedItem != null)
            {
                btn_addClock.IsEnabled = true;
            }
            else
            {
                btn_addClock.IsEnabled = false;
            }
        }

        private void MenuItem_Clicked(object sender, EventArgs e)
        {
            MenuItem itemTap = sender as MenuItem;
            StoredTimeClock tappedClock = itemTap.CommandParameter as StoredTimeClock;
            var index = userClocks.IndexOf(tappedClock);
            userClocks.RemoveAt(index);
        }
    }
}
