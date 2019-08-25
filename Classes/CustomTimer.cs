using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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

        public static Timer _p1Timer = new Timer();

        public static Timer _p2Timer = new Timer();

        public static bool _p1Sent;
        
        public static double _p1Time { get; set; }

        public static double _p2Time { get; set; }

        public static double _p1TimeSetting { get; set; }

        public static double _p2TimeSetting { get; set; }
        
        public static bool _p1HasControl = false;

        public static double _addInterval = 3000;
        
        
        public double[] TimerSettings(double p1Time, double p2Time, double timeAdd)
        {
            _p1Time = _p1Time;
            _p2Time = _p2Time;
            _addInterval = timeAdd;

            return _timerSettings;
        }
        
        public void StartTimer(bool p1Sent)
        {
            if (!GameState._gameInProgress)
            {
                GameState._gameInProgress = true;

                _p1Timer.Interval = 1;
                _p2Timer.Interval = 1;
                
                _p1Timer.Elapsed += P1TimerElapse;
                _p2Timer.Elapsed += P2TimerElapse;
            }

            if (p1Sent)
            {
                _p2Timer.Stop();
                _p1Timer.Start();
            }
            else
            {
                _p1Timer.Stop();
                _p2Timer.Start();
            }
        }

        public static TheFragment1 _fm1 = new TheFragment1();

        private async void P1TimerElapse(object sender, EventArgs eventArgs)
        {
            while (_p1HasControl)
            {
                await Task.Run(() => _p1Time--);

                await Task.Run(() => 
                _fm1.SetP1ButtonText(_p1Time.ToString()));
                
                if (_p1Time == 0)
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
            while (!_p1HasControl)
            {
                await Task.Run(() => _p2Time--);

                await Task.Run(() =>
                _fm1.SetP2ButtonText(_p1Time.ToString()));
                
                if (_p2Time == 0)
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
            if (p1Sent)
            {
                _p1Timer.Stop();
                _p1Time += _addInterval;
                _fm1.SetP1ButtonText(_p1Time.ToString());
                _p1HasControl = false;
                _p2Timer.Start();
            }
            else
            {
                _p2Timer.Stop();
                _p2Time += _addInterval;
                _fm1.SetP2ButtonText(_p2Time.ToString());
                _p1HasControl = true;
                _p2Timer.Start();
            }
        }

        public void ResetTimers()
        {
            _p1Timer.Stop();
            _p2Timer.Stop();
            _p1Time = _p1TimeSetting;
            _p2Time = _p2TimeSetting;
            _fm1.SetP1ButtonText(_p1Time.ToString());
            _fm1.SetP2ButtonText(_p2Time.ToString());
        }
    }
}