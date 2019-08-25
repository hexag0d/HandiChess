using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

namespace BottomNavigationViewPager.Classes
{
    public class GameState
    {
        public static bool _gameInProgress { get; set; }

        public static bool _p1HasControl { get; set; }

        public static bool _p1HasWon = false;

        public void ResetGame()
        {
            _gameInProgress = false;
            _p1HasWon = false;
            _p1HasControl = false;
        }
    }
}