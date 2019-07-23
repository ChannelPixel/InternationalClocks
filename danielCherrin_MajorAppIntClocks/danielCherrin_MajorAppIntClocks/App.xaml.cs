using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

[assembly: XamlCompilation (XamlCompilationOptions.Compile)]
namespace danielCherrin_MajorAppIntClocks
{
	public partial class App : Application
	{
        MainPage page;

		public App ()
		{
			InitializeComponent();

			MainPage = new MainPage();
            page = (MainPage)MainPage;
		}

		protected override void OnStart ()
		{
			// Handle when your app starts
		}

		protected override void OnSleep ()
		{
            page.OnClose();
		}

		protected override void OnResume ()
		{
			// Handle when your app resumes
		}
	}
}
