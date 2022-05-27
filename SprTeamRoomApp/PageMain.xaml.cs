using System;
using System.Globalization;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using Windows.UI.Core;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Emmellsoft.IoT.Rpi.SenseHat;

namespace SprTeamRoomApp
{
    public sealed partial class PageMain : Page
    {
        private ManualResetEventSlim clockWaitHandle;
        private ManualResetEventSlim senseHatJoystickWaitHandle;
        private CultureInfo cultureInfo;
        private int index;

        private readonly string[] headers =
        {
            "Hello World !!!",
            "PSE Innowacje",
            "Projekt SPR - Sprint 4"
        };

        public PageMain()
        {
            this.InitializeComponent();
            this.clockWaitHandle = new ManualResetEventSlim(false);
            this.senseHatJoystickWaitHandle = new ManualResetEventSlim(false);
            this.cultureInfo = CultureInfo.GetCultureInfo("pl-PL");
            this.index = 0;
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            ApplicationView.GetForCurrentView().TryEnterFullScreenMode();
            this.StartSenseHatJoystick();
            this.StartClock();
            this.StartAirConditionCheck();
        }

        private void StartSenseHatJoystick()
        {
            Task.Run(async () =>
            {
                var senseHat = await SenseHatFactory.GetSenseHat();
                senseHat.Display.Clear();
                senseHat.Display.Update();

                await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                {
                    this.tbHeader.Text = this.GetNextHeader();
                });

                while (true)
                {
                    if (senseHat.Joystick.Update() && senseHat.Joystick.EnterKey == KeyState.Pressed)
                    {
                        await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                        {
                            this.tbHeader.Text = this.GetNextHeader();
                        });
                    }
                    this.senseHatJoystickWaitHandle.Wait(100);
                }
            });
        }

        private string GetNextHeader()
        {
            if (this.index == this.headers.Length - 1)
            {
                this.index = 0;
            }
            else
            {
                ++this.index;
            }

            return headers[index];
        }

        private void StartClock()
        {
            Task.Run(async () =>
            {
                while (true)
                {
                    await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                    {
                        try
                        {
                            var now = this.GetDateTimeNow();
                            this.tbTime.Text = now.ToString("HH:mm", cultureInfo);
                            this.tbDate.Text = now.ToString("dddd, d MMMM yyyy", cultureInfo);
                            this.tbError.Text = string.Empty;
                        }
                        catch (Exception ex)
                        {
                            this.tbError.Text = ex.Message;
                        }
                    });

                    this.clockWaitHandle.Wait(2000);
                }
            });
        }

        private DateTime GetDateTimeNow()
        {
            using (var webClient = new WebClient())
            {
                var response = webClient.DownloadString("http://worldclockapi.com/api/json/cet/now");
                var match = Regex.Match(response, @"""currentDateTime"":""(\d{4}-\d{2}-\d{2}T\d{2}:\d{2})");
                return DateTime.Parse(match.Groups[1].Value);
            }
        }

        private void StartAirConditionCheck()
        {
            Task.Run(async () =>
            {
                while (true)
                {
                    await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                    {
                        try
                        {
                            this.tbAir.Text = $"jakość powietrza: {this.GetAirCondition()}";
                            this.tbError.Text = string.Empty;
                        }
                        catch (Exception ex)
                        {
                            this.tbError.Text = ex.Message;
                        }
                    });

                    this.clockWaitHandle.Wait(1000 * 60 * 10);
                }
            });
        }

        private string GetAirCondition()
        {
            using (var webClient = new WebClient())
            {
                var response = webClient.DownloadString("https://www.wroclaw.pl/srodowisko/jakosc-powietrza-we-wroclawiu");
                var match = Regex.Match(response, @"<span class=""txtValue wip"">(.+)</span>");
                return match.Groups[1].Value;
            }
        }
    }
}