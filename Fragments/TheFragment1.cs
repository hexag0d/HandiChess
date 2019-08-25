using Android.Graphics.Drawables;
using Android.OS;
using Android.Support.V4.App;
using Android.Text;
using Android.Views;
using Android.Webkit;
using Android.Widget;
using BottomNavigationViewPager.Classes;
using System;
using System.Threading.Tasks;
using static Android.Views.View;

namespace BottomNavigationViewPager.Fragments
{
    [Android.Runtime.Register("onWindowVisibilityChanged", "(I)V", "GetOnWindowVisibilityChanged_IHandler")]
    public class TheFragment1 : Fragment
    {
        string _title;
        string _icon;

        protected static WebView _wv;
        protected static View _view;

        readonly ExtWebViewClient _wvc = new ExtWebViewClient();

        public static CustomTimer _timer = new CustomTimer();

        public static Button _p1TimerButton;
        public static Button _p2TimerButton;
        public static Button _resetButton;
        public static EditText _p1TimerText;
        public static EditText _p2TimerText;
        public static EditText _timeAdderText;

        public static GameState _gameState = new GameState();
        public static CustomTimer _customTimer = new CustomTimer();

        /// <summary>
        /// this bool is set true when player 1 is sender of 
        /// event object
        /// </summary>
        public static bool _p1IsSender = false;        

        public static TheFragment1 NewInstance(string title, string icon) {
            var fragment = new TheFragment1();
            fragment.Arguments = new Bundle();
            fragment.Arguments.PutString("title", title);
            fragment.Arguments.PutString("icon", icon);
            return fragment;
        }

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            if (Arguments != null)
            {
                if (Arguments.ContainsKey("title"))
                    _title = (string)Arguments.Get("title");

                if (Arguments.ContainsKey("icon"))
                    _icon = (string)Arguments.Get("icon");
            }
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            _view = inflater.Inflate(Resource.Layout.TheFragmentLayout1, container, false);
            
            _p1TimerButton = _view.FindViewById<Button>(Resource.Id.p1TimerButton);
            _p2TimerButton = _view.FindViewById<Button>(Resource.Id.p2TimerButton);
            _timeAdderText = _view.FindViewById<EditText>(Resource.Id.timeAdderText);
            _p1TimerText = _view.FindViewById<EditText>(Resource.Id.timeSetterP1Text);
            _p2TimerText = _view.FindViewById<EditText>(Resource.Id.timeSetterP2Text);
            _resetButton = _view.FindViewById<Button>(Resource.Id.resetButton);

            _p1TimerButton.Click += P1OnClick;
            _p2TimerButton.Click += P2OnClick;
            _resetButton.Click += ResetButtonOnClick;
            _p1TimerText.TextChanged += P1TimerChanged;
            _p2TimerText.TextChanged += P2TimerChanged;
            _timeAdderText.TextChanged += TimerAdderChanged;

            GameState._gameInProgress = false;
            CustomTimer._p1Time = 5000000;
            CustomTimer._p2Time = 5000000;
            
            return _view;
        }

        private void P1OnClick(object sender, EventArgs eventArgs)
        {
            _p1IsSender = true;

            if (GameState._gameInProgress == false)
            {
                _timer.StartTimer(_p1IsSender);
            }
            else
            {
                if (GameState._p1HasControl)
                {
                    _customTimer.TimerButtonOnClick(_p1IsSender);
                }
                else
                {
                    //if P1 isn't in control, do nothing.
                }
            }
        }

        private void P2OnClick(object sender, EventArgs eventArgs)
        {
            _p1IsSender = false;

            if (GameState._gameInProgress == false)
            {
                _timer.StartTimer(_p1IsSender);
            }
            else
            {
                if (!GameState._p1HasControl)
                {
                    _customTimer.TimerButtonOnClick(_p1IsSender);
                }
                else
                {
                    //if P2 isn't in control, do nothing.
                }
            }
        }

        public async void SetP1ButtonText(string btnTxt)
        {
            await Task.Run ( () =>
            _p1TimerButton.Text = btnTxt);
        }

        public async void SetP2ButtonText(string btnTxt)
        {
                await Task.Run(() =>
             _p2TimerButton.Text = btnTxt);
        }

        private void P1TimerChanged(object sender, TextChangedEventArgs eventArgs)
        {
            if (GameState._gameInProgress == false)
            {
                _customTimer.TimerSettings(Convert.ToDouble(_p1TimerText.Text),
                    Convert.ToDouble(_p2TimerText.Text),
                    Convert.ToDouble(_timeAdderText));
            }
        }

        private void P2TimerChanged(object sender, TextChangedEventArgs eventArgs)
        {
            if (GameState._gameInProgress == false)
            {
                _customTimer.TimerSettings(Convert.ToDouble(_p1TimerText.Text),
                    Convert.ToDouble(_p2TimerText.Text),
                    Convert.ToDouble(_timeAdderText.Text));
            }
        }

        private void TimerAdderChanged(object sender, TextChangedEventArgs eventArgs)
        {
            if (GameState._gameInProgress == false)
            {
                _customTimer.TimerSettings(Convert.ToDouble(_p1TimerText.Text),
                    Convert.ToDouble(_p2TimerText.Text),
                    Convert.ToDouble(_timeAdderText.Text));
            }
        }

        private void ResetButtonOnClick(object sender, EventArgs eventArgs)
        {
            _customTimer.ResetTimers();
            _gameState.ResetGame();
        }
        
        /// <summary>
        /// tells the webview to GoBack, if it can
        /// </summary>
        public void WebViewGoBack()
        {

        }

        static bool _wvRl = true;

        /// <summary>
        /// one press refreshes the page; two presses pops back to the root
        /// </summary>
        public void Pop2Root()
        {
            if (_wvRl)
            {
                _wv.Reload();
                _wvRl = false;
            }
            else
            {
                
            }
        }

        public static bool _wvRling = false;

        /// <summary>
        /// this is to allow faster phones and connections the ability to Pop2Root
        /// used to be set without delay inside OnPageFinished but I don't think 
        /// that would work on faster phones
        /// </summary>
        public static async void SetReload()
        {
            if (!_wvRling)
            {
                _wvRling = true;

                await Task.Delay(500);

                _wvRl = true;

                _wvRling = false;
            }
        }
        

        private class ExtWebViewClient : WebViewClient
        {
            public override void OnPageFinished(WebView _view, string url)
            {
                base.OnPageFinished(_view, url);
            }
        }
    }
}
