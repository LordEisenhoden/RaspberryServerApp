using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// Die Elementvorlage "Leere Seite" wird unter https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x407 dokumentiert.

namespace RaspberryServerApp
{
    /// <summary>
    /// Eine leere Seite, die eigenständig verwendet oder zu der innerhalb eines Rahmens navigiert werden kann.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        private static TCPConnection TCPListener;
        public static EventManager EventManager;
        public static UserManager UserManager;
        public static TextBox LoggerTextBox;

        public MainPage()
        {
            this.InitializeComponent();
            EventManager = new EventManager(this);
            TCPListener = new TCPConnection();
            TCPListener.Init(EventManager);
            UserManager = new UserManager();
            LoggerTextBox = DebugTextBox;

            //Startet eine Methode in einem eigenen Task -perfekt wenn man was Asynchrones will!
            Task.Factory.StartNew(() => { EndlessLoopAsync(); });
        }


        //Asynchrone Endlosschleife welche in Regelmäßigen Abständen den EventManager Triggert
        private async Task EndlessLoopAsync()
        {
            //Da der Aufruf von Connection.Init async ist und darauf gewartet werden muss ist sie hier und nicht in MainPage() 

            while (true)
            {
                //Eventmanager soll seine Events Triggern
                EventManager.Run();

                //Falls keine Events vorhanden sind warte kurz und schau erneut nach
                if (EventManager.EventsPending() == false)
                {
                    await Task.Delay(TimeSpan.FromMilliseconds(100));
                }
            }
        }


        public async Task WriteToLogAsync(string input)
        {
            await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                LoggerTextBox.Text = LoggerTextBox.Text + Environment.NewLine + DateTime.Now + " " + input;
            });
        }
    }
}

