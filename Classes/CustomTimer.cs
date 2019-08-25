using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using BottomNavigationViewPager.Fragments;

namespace BottomNavigationViewPager.Classes
{
    public class CustomTimer
    {
        public static double[] _timerSettings;
        public static System.Timers.Timer _p1Timer = new System.Timers.Timer();
        public static System.Timers.Timer _p2Timer = new System.Timers.Timer();
        public static bool _p1Sent;
        public static double _p1Time { get; set; }
        public static double _p2Time { get; set; }
        public static double _p1TimeSetting { get; set; }
        public static double _p2TimeSetting { get; set; }

        public static double _timeIntervalDouble;
 
        public static double _addInterval;

        private string _p1DisplayOut;
        private string _p2DisplayOut;
        public static string _timeDisplayOut;

        public static TheFragment1 _fm1 = new TheFragment1();

        /// <summary>
        /// called OnCreate,
        /// sets the timer settings,
        /// will eventually be tied into a static settings file,
        ///  but for now just sets the game length to 5 minutes
        /// </summary>
        public void OnAppLoaded()
        {
            // 5 minutes * 60 seconds * 1000ms/sec = 300,000 ms / 10ms interval
            _p1TimeSetting = _fm1.GetP1TimeIntervalFromText();
            _p2TimeSetting = _fm1.GetP2TimeIntervalFromText();

            _p1Time = _p1TimeSetting;
            _p2Time = _p2TimeSetting;

            //300 = 3000ms
            _addInterval = 300;
            
            //10ms interval for now, don't know if we need more precision
            _p1Timer.Interval = 10;
            _p2Timer.Interval = 10;

            _p1Timer.Elapsed += P1TimerElapse;
            _p2Timer.Elapsed += P2TimerElapse;

            _fm1.SetP1ButtonText(GetTimeStringFromDouble(_p1Time));
            _fm1.SetP2ButtonText(GetTimeStringFromDouble(_p2Time));

            GameState._gameIsPaused = false;
        }

        public void StartTimer(bool p1Sent)
        {
            if (!GameState._gameInProgress)
            {
                GameState._gameInProgress = true;
            }

            _fm1.SetP1ButtonText(GetTimeStringFromDouble(_fm1.GetP1TimeIntervalFromText()));
            _fm1.SetP2ButtonText(GetTimeStringFromDouble(_fm1.GetP2TimeIntervalFromText()));

            _p1Time = _p1TimeSetting;
            _p2Time = _p2TimeSetting;

            if (p1Sent)
            {
                GameState._p1HasControl = false;
                _p2Timer.Start();
            }
            else
            {
                GameState._p1HasControl = true;
                _p1Timer.Start();
            }
        }

        private async void P1TimerElapse(object sender, EventArgs eventArgs)
        {
            if (GameState._p1HasControl)
            {
                await Task.Run(() =>
                {
                    _p1Time--;
                    _fm1.SetP1ButtonText(GetTimeStringFromDouble(_p1Time));
                });


                if (_p1Time <= 0)
                {
                    GameState._p1HasWon = false;
                    _fm1.SetP1ButtonText("You lost!");
                    _p1Timer.Stop();
                    GameState._gameInProgress = false;
                }
            }
        }

        private async void P2TimerElapse(object sender, EventArgs eventArgs)
        {
            if (!GameState._p1HasControl)
            {
                await Task.Run(() =>
                {
                    _p2Time--;
                    _fm1.SetP2ButtonText(GetTimeStringFromDouble(_p2Time));
                });

                if (_p2Time <= 0)
                {
                    GameState._p1HasWon = true;
                    _fm1.SetP2ButtonText("You lost!");
                    _p2Timer.Stop();
                    GameState._gameInProgress = false;
                }
            }
        }

        public async void TimerButtonOnClick(bool p1Sent)
        {
            if (!GameState._gameIsPaused || !GameState._p1HasWon || GameState._p1HasWon)
            {
                if (p1Sent)
                {
                    await Task.Run(() => {
                        _p1Timer.Stop();
                        _p1Time += _addInterval;
                        _fm1.SetP1ButtonText(GetTimeStringFromDouble(_p1Time));
                        GameState._p1HasControl = false;
                        _p2Timer.Start();
                        });
                }
                else
                {
                    await Task.Run(() =>
                    {
                       _p2Timer.Stop();
                       _p2Time += _addInterval;
                       _fm1.SetP2ButtonText(GetTimeStringFromDouble(_p2Time));
                       GameState._p1HasControl = true;
                       _p1Timer.Start();
                    });
                }
            }
            else
            {
                if (!GameState._p1HasControl)
                {
                    _p1Timer.Start();
                }
                else
                {
                    _p2Timer.Start();
                }
                GameState._gameIsPaused = false;
            }
        }

        /// <summary>
        /// resets the timers when called
        /// </summary>
        public void ResetTimers()
        {
            _p1Timer.Stop();
            _p2Timer.Stop();
            _p1Time = _p1TimeSetting;
            _p2Time = _p2TimeSetting;
            _fm1.SetP1ButtonText(GetTimeStringFromDouble(_p1TimeSetting));
            _fm1.SetP2ButtonText(GetTimeStringFromDouble(_p2TimeSetting));

            GameState._gameInProgress = false;
        }

        public void PauseGame()
        {
            GameState._gameIsPaused = true;
            _p1Timer.Stop();
            _p2Timer.Stop();
        }

        public string GetTimeStringFromDouble (double timeIn)
        {
            //_timeDisplayOut = (timeIn / 60 / 100).ToString().Substring(0, 4);
            TimeSpan t = TimeSpan.FromMilliseconds((timeIn * 10));
            _timeDisplayOut = string.Format("{0:D2}h:{1:D2}m:{2:D2}s:{3:D3}ms",
                                    t.Hours,
                                    t.Minutes,
                                    t.Seconds,
                                    t.Milliseconds);
            return _timeDisplayOut;
        }

        /// <summary>
        /// gets the time interval, input is in minutes
        /// the game time interval is 10ms.  
        /// 1ms interval seems overly resource intensive
        /// I don't think we need that kind of precision.
        /// </summary>
        /// <returns>10ms double</returns>
        public double GetTimeInterval(double timeIn)
        {
            // time interval = (minutes * 60 seconds * 1000 ms) / 10ms intervals
            _timeIntervalDouble = ((timeIn * 60 * 1000) / 10);

            return _timeIntervalDouble;
        }
    }
}