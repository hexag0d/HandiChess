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
        //public static double _p1TimeSetting { get; set; }
        //public static double _p2TimeSetting { get; set; }

        public event System.EventHandler _p1TimeSettingChanged;
        public event System.EventHandler _p2TimeSettingChanged;
 
        public static double _addInterval;

        private string _p1DisplayOut;
        private string _p2DisplayOut;
        public static string _timeDisplayOut;

        /// <summary>
        /// called OnCreate,
        /// sets the timer settings,
        /// will eventually be tied into a static settings file,
        ///  but for now just sets the game length to 5 minutes
        /// </summary>
        public void OnAppLoaded()
        {
            // 5 minutes * 60 seconds * 1000ms/sec = 300,000 ms / 10ms interval
            _p1Time = 30000;
            _p2Time = 30000;
            _p1TimeSetting = 30000;
            _p2TimeSetting = 30000;
            //300 = 3000ms
            _addInterval = 300;

            GameState._gameIsPaused = false;
        }

        public double _p1TimeSetting
        {
            get
            {
                return _p1Time;
            }
            set
            {
                _p1TimeSetting = value;
                OnP1TimeSettingChanged();
            }
        }

        public double _p2TimeSetting
        {
            get
            {
                return _p2Time;
            }
            set
            {
                _p2TimeSetting = value;
                OnP2TimeSettingChanged();
            }
        }

        protected virtual void OnP1TimeSettingChanged()
        {
            if (_p1TimeSettingChanged != null)
                _p1TimeSettingChanged(this, EventArgs.Empty);

            if (!GameState._gameInProgress)
            {
                _p1Time = _p1TimeSetting;
            }
        }

        protected virtual void OnP2TimeSettingChanged()
        {
            if (_p2TimeSettingChanged != null)
                _p2TimeSettingChanged(this, EventArgs.Empty);

            if (!GameState._gameInProgress)
            {
                _p2Time = _p2TimeSetting;
            }
        }

        public double[] TimerSettings(double p1Time, double p2Time, double timeAdd)
        {
            _p1TimeSetting = p1Time;
            _p2TimeSetting = p2Time;
            _addInterval = timeAdd;

            return _timerSettings;
        }

        public void StartTimer(bool p1Sent)
        {
            if (!GameState._gameInProgress)
            {
                GameState._gameInProgress = true;

                //10ms interval for now, don't know if we need more precision
                _p1Timer.Interval = 10;
                _p2Timer.Interval = 10;

                _p1Timer.Elapsed += P1TimerElapse;
                _p2Timer.Elapsed += P2TimerElapse;
            }

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

        public static TheFragment1 _fm1 = new TheFragment1();

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

        public void TimerButtonOnClick(bool p1Sent)
        {
            if (!GameState._gameIsPaused || !GameState._p1HasWon || GameState._p1HasWon)
            {
                if (p1Sent)
                {
                    _p1Timer.Stop();
                    _p1Time += _addInterval;
                    _fm1.SetP1ButtonText(GetTimeStringFromDouble(_p1Time));
                    GameState._p1HasControl = false;
                    _p2Timer.Start();
                }
                else
                {
                    _p2Timer.Stop();
                    _p2Time += _addInterval;
                    _fm1.SetP2ButtonText(GetTimeStringFromDouble(_p2Time));
                    GameState._p1HasControl = true;
                    _p1Timer.Start();
                }
            }
            else
            {
                if (GameState._p1HasControl)
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

        public void ResetTimers()
        {
            _p1Timer.Stop();
            _p2Timer.Stop();
            _p1Time = _p1TimeSetting;
            _p2Time = _p2TimeSetting;
            _fm1.SetP1ButtonText(GetTimeStringFromDouble(_p1Time));
            _fm1.SetP2ButtonText(GetTimeStringFromDouble(_p2Time));

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
            _timeDisplayOut = (timeIn / 60 / 100).ToString().Substring(0, 4);
            return _timeDisplayOut;
        }
    }
}