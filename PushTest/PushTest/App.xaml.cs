using PushTest.Services;
using PushTest.Models;
using System;
using System.ComponentModel.Design;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Xamarin.Essentials;

namespace PushTest
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();
            Services.ServiceContainer.Resolve<IPushDemoNotificationActionService>()
                .ActionTriggered += NotificationActionTriggered;
            MainPage = new MainPage();
        }

        void NotificationActionTriggered(object sender, PushDemoAction e)
            => ShowActionAlert(e);

        void ShowActionAlert(PushDemoAction action)
            => MainThread.BeginInvokeOnMainThread(()
                => MainPage?.DisplayAlert("PushDemo", $"{action} action received", "OK")
                    .ContinueWith((task) => { if (task.IsFaulted) throw task.Exception; }));

        protected override void OnStart()
        {
        }

        protected override void OnSleep()
        {
        }

        protected override void OnResume()
        {
        }
    }
}
