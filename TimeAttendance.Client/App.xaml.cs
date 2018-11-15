// 
// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license.
// 
// Microsoft Cognitive Services: http://www.microsoft.com/cognitive
// 
// Microsoft Cognitive Services Github:
// https://github.com/Microsoft/Cognitive
// 
// Copyright (c) Microsoft Corporation
// All rights reserved.
// 
// MIT License:
// Permission is hereby granted, free of charge, to any person obtaining
// a copy of this software and associated documentation files (the
// "Software"), to deal in the Software without restriction, including
// without limitation the rights to use, copy, modify, merge, publish,
// distribute, sublicense, and/or sell copies of the Software, and to
// permit persons to whom the Software is furnished to do so, subject to
// the following conditions:
// 
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED ""AS IS"", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE
// LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION
// OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION
// WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
// 

using System;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Navigation;
using Microsoft.Data.Sqlite;
using Microsoft.Data.Sqlite.Internal;

// The Blank Application template is documented at http://go.microsoft.com/fwlink/?LinkId=402347&clcid=0x409

namespace TimeAttendance.Client
{
    using System.Diagnostics;
    using System.Threading.Tasks;
    using Views;
    using Windows.Data.Xml.Dom;
    using Windows.UI.Notifications;
    using Windows.ApplicationModel.ExtendedExecution.Foreground;
    using TimeAttendance.Client.Model;
    using TimeAttendance.Client.Util;
    using TimeAttendance.Client.ServiceBus;
    using TimeAttendance.Client.AzureStorage;
    using Windows.UI.Popups;

    /// <summary>
    /// Provides application-specific behavior to supplement the default Application class.
    /// </summary>
    sealed partial class App : Application
    {
        /// <summary>
        /// Initializes the singleton application object.  This is the first line of authored code
        /// executed, and as such is the logical equivalent of main() or WinMain().
        /// </summary>
        public App()
        {
            this.InitializeComponent();
            this.Suspending += OnSuspending;

            SqliteEngine.UseWinSqlite3(); //Configuring library to use SDK version of SQLite
            using (SqliteConnection db = new SqliteConnection(Constants.PathDatabase))
            {

                db.Open();
                String tblConfigServer = "CREATE TABLE IF NOT EXISTS ConfigServer (ServerId INTEGER PRIMARY KEY AUTOINCREMENT, ServiceBase NVARCHAR(300) NULL, TotalFrame INTEGER NULL, QueueURL NVARCHAR(300) NULL, AccessKeyName NVARCHAR(300) NULL, AccessKeyValue NVARCHAR(300) NULL, BusTopicName NVARCHAR(300) NULL, BusKeySend NVARCHAR(300) NULL, AzureName NVARCHAR(300) NULL, AzureKey NVARCHAR(300) NULL, AzureContainer NVARCHAR(300) NULL, AzureUrlHost NVARCHAR(300) NULL)";
                String tblConfigCamera = "CREATE TABLE IF NOT EXISTS ConfigCamera (CameraId INTEGER PRIMARY KEY AUTOINCREMENT, CameraIP NVARCHAR(300) NULL, CameraType NVARCHAR(300) NULL, CameraUser NVARCHAR(300) NULL, CameraPass NVARCHAR(300) NULL, StreaURI NVARCHAR(500) NULL, IndexView INTEGER NULL, BoxWidth INTEGER NULL, BoxHeight INTEGER NULL, BoxPointX INTEGER NULL, BoxPointY INTEGER NULL)";

                SqliteCommand createConfigServer = new SqliteCommand(tblConfigServer, db);
                SqliteCommand createConfigCamera = new SqliteCommand(tblConfigCamera, db);
                try
                {
                    createConfigServer.ExecuteReader();
                    createConfigCamera.ExecuteReader();
                    createConfigCamera.Dispose();
                    createConfigServer.Dispose();
                    db.Close();
                }
                catch (SqliteException e)
                {
                    db.Close();
                }
            }

            try
            {
                InfoSettingFix.InfoSetting = new ServiceUtil().GetCameraAPI();
                //Load cấu hình service bus
                //"https://vcb-poc-stag.servicebus.windows.net/face-detection"
                ServiceBusSetting.QueueURL = InfoSettingFix.InfoSetting.QueueURL;
                //"RootManageSharedAccessKey"
                ServiceBusSetting.AccessKeyName = InfoSettingFix.InfoSetting.AccessKeyName;
                //"DiW5SEtn5tr6+LM/Jji52KIUyjUNCA2EcFle3USKxYg="
                ServiceBusSetting.AccessKeyValue = InfoSettingFix.InfoSetting.AccessKeyValue;

                //Load cấu hình Azure Storage
                //"ntsstorageaccount"
                AzureStorageUtils.StorageAccountName = InfoSettingFix.InfoSetting.AzureName;
                //"8j0q5p8UbWjnVcBw6ht/Du3rZ4IXjEHeaQyhkynLOcQUfs5fdcmkeOq2ooLNz2uKtSQUT0sidUh3YbQ4hbyU2A=="
                AzureStorageUtils.StorageAccountKey = InfoSettingFix.InfoSetting.AzureKey;
                //"TimeAttendance.Client"
                AzureStorageUtils.StorageContainer = InfoSettingFix.InfoSetting.AzureContainer;
                //"https://ntsstorageaccount.blob.core.windows.net/"
                AzureStorageUtils.UrlHostImage = InfoSettingFix.InfoSetting.AzureUrlHost;
            }
            catch (Exception ex)
            {
                InfoSettingFix.InfoSetting = new InfoSettingModel();
            }
        }

        private Windows.System.Display.DisplayRequest _displayRequest;
        ExtendedExecutionForegroundSession _session;


        public void ActivateDisplay()
        {
            //create the request instance if needed
            if (_displayRequest == null)
                _displayRequest = new Windows.System.Display.DisplayRequest();

            //make request to put in active state
            _displayRequest.RequestActive();
        }

        /// <summary>
        /// Invoked when the application is launched normally by the end user.  Other entry points
        /// will be used such as when the application is launched to open a specific file.
        /// </summary>
        /// <param name="e">Details about the launch request and process.</param>
        protected override async void OnLaunched(LaunchActivatedEventArgs e)
        {
#if DEBUG
            if (System.Diagnostics.Debugger.IsAttached)
            {
                // This just gets in the way.
                //this.DebugSettings.EnableFrameRateCounter = true;
            }
#endif
            ActivateDisplay();

            AppShell shell = Window.Current.Content as AppShell;

            // Do not repeat app initialization when the Window already has content,
            // just ensure that the window is active
            if (shell == null)
            {
                // callbacks for core library
                ErrorTrackingHelper.TrackException = (ex, msg) => LogException(ex, msg);
                ErrorTrackingHelper.GenericApiCallExceptionHandler = CoreUtil.GenericApiCallExceptionHandler;

                // Create a AppShell to act as the navigation context and navigate to the first page
                shell = new AppShell();

                // Set the default language
                shell.Language = Windows.Globalization.ApplicationLanguages.Languages[0];

                shell.AppFrame.NavigationFailed += OnNavigationFailed;

                if (e.PreviousExecutionState == ApplicationExecutionState.Terminated)
                {
                    //TODO: Load state from previously suspended application
                }

                // Set the TitleBar to Dark Theme
                var appView = Windows.UI.ViewManagement.ApplicationView.GetForCurrentView();
                var titleBar = appView.TitleBar;
                titleBar.BackgroundColor = Windows.UI.Colors.Black;
                titleBar.ForegroundColor = Windows.UI.Colors.White;
                titleBar.ButtonBackgroundColor = Windows.UI.Colors.Black;
                titleBar.ButtonForegroundColor = Windows.UI.Colors.White;
            }

            // Place our app shell in the current Window
            Window.Current.Content = shell;

            if (e.PrelaunchActivated == false)
            {
                if (shell.AppFrame.Content == null)
                {
                    shell.AppFrame.Navigate(typeof(SettingConfig), e.Arguments, new Windows.UI.Xaml.Media.Animation.SuppressNavigationTransitionInfo());
                }
                Window.Current.Activate();
            }

            if (_session == null)
            {
                await PreventFromSuspending();
            }

            // Ensure the current window is active
            Window.Current.Activate();
        }

        private async Task PreventFromSuspending()
        {
            ExtendedExecutionForegroundSession newSession = new ExtendedExecutionForegroundSession();
            newSession.Reason = ExtendedExecutionForegroundReason.Unconstrained;
            newSession.Revoked += SessionRevoked;

            ExtendedExecutionForegroundResult result = await newSession.RequestExtensionAsync();
            switch (result)
            {
                case ExtendedExecutionForegroundResult.Allowed:
                    _session = newSession;
                    break;
                default:
                case ExtendedExecutionForegroundResult.Denied:
                    newSession.Dispose();
                    break;
            }
        }

        private void SessionRevoked(object sender, ExtendedExecutionForegroundRevokedEventArgs args)
        {
            if (_session != null)
            {
                _session.Dispose();
                _session = null;
            }
        }

        private static void ShowThrottlingToast(string api)
        {
            ToastTemplateType toastTemplate = ToastTemplateType.ToastText02;
            XmlDocument toastXml = ToastNotificationManager.GetTemplateContent(toastTemplate);
            XmlNodeList toastTextElements = toastXml.GetElementsByTagName("text");
            toastTextElements[0].AppendChild(toastXml.CreateTextNode("Intelligent Kiosk"));
            toastTextElements[1].AppendChild(toastXml.CreateTextNode("The " + api + " API is throttling your requests. Consider upgrading to a Premium Key."));

            ToastNotification toast = new ToastNotification(toastXml);
            ToastNotificationManager.CreateToastNotifier().Show(toast);
        }

        private static void LogException(Exception ex, string message)
        {
            Debug.WriteLine("Error detected! Exception: \"{0}\", More info: \"{1}\".", ex.Message, message);
        }

        /// <summary>
        /// Invoked when Navigation to a certain page fails
        /// </summary>
        /// <param name="sender">The Frame which failed navigation</param>
        /// <param name="e">Details about the navigation failure</param>
        private void OnNavigationFailed(object sender, NavigationFailedEventArgs e)
        {
            throw new Exception("Failed to load Page " + e.SourcePageType.FullName);
        }

        /// <summary>
        /// Invoked when application execution is being suspended.  Application state is saved
        /// without knowing whether the application will be terminated or resumed with the contents
        /// of memory still intact.
        /// </summary>
        /// <param name="sender">The source of the suspend request.</param>
        /// <param name="e">Details about the suspend request.</param>
        private async void OnSuspending(object sender, SuspendingEventArgs e)
        {
            var deferral = e.SuspendingOperation.GetDeferral();

            //Save application state and stop any background activity
            var currentView = (Window.Current.Content as AppShell)?.AppFrame?.Content;

            if (currentView != null && currentView.GetType() == typeof(RealTimeDemo))
            {
                await (currentView as RealTimeDemo).HandleApplicationShutdownAsync();
            }
            deferral.Complete();
        }
    }
}
