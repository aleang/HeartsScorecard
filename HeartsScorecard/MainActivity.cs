using Android.App;
using Android.Content.PM;
using Android.Content.Res;
using Android.Graphics;
using Android.Graphics.Drawables;
using Android.OS;
using Android.Views;
using Android.Widget;
using HeartsScorecard.Helpers;
using System.Collections.Generic;
using System.Linq;

namespace HeartsScorecard
{
    [Activity(Label = "Hearts Scorecard",
        MainLauncher = true,
        ScreenOrientation = ScreenOrientation.Portrait,
        Icon = "@drawable/icon")]
    public class MainActivity : Activity
    {
        private List<EditText> _playerNameEditTexts;
        private List<ProgressBar> _displayScoreProgress;
        private List<TextView> _displayScoreViews;
        private List<RowOfScore> _scoreViews;
        private LinearLayout _rowScoreProgressBar;
        private LinearLayout _rowScoreTextView;
        private string[] _playerNameSuggestions;
        private ArrayAdapter _autoCompleteAdapter;
        private Color _babyBlueColor;
        private Color _invalidScoreColour;
        private string _lastValue;

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.Main);
            InitialiseStuff();
            FindViews();
            AddEventHandler();

            if (bundle != null && bundle.ContainsKey("PlayerNames"))
            {
                var savedNameList = bundle.GetStringArrayList("PlayerNames");
                if (savedNameList.Count == 5)
                {
                    for (int i = 0; i < savedNameList.Count; i++)
                    {
                        _playerNameEditTexts[i].Text = savedNameList[i];
                    }
                }
            }
        }

        private void InitialiseStuff()
        {

            _invalidScoreColour = Color.LightPink;
            _playerNameSuggestions = new[]
                           {
                               "Pheng", "Matty", "Paul", "Anna", "Diego", "Irene", "Wiremu", "Marcos", "Clarita", "Michael", "Justin"
                           };
            _autoCompleteAdapter = new ArrayAdapter(this, Android.Resource.Layout.SimpleDropDownItem1Line, _playerNameSuggestions);
            _babyBlueColor = Resources.GetColor(Resource.Color.babyblue);
        }

        private void FindViews()
        {
            _rowScoreProgressBar = FindViewById<LinearLayout>(Resource.Id.scoreProgressBarRow);
            _rowScoreTextView = FindViewById<LinearLayout>(Resource.Id.scoreTextViewRow);
            _scoreViews = new List<RowOfScore>();
            var allScoreEditTextId = new int[Resources.GetInteger(Resource.Integer.rowCount)][];

            GetAllScoreTextFields(allScoreEditTextId);

            foreach (var view in ResourceIdentifiers.PlayerNameAutoComplete
                .Select(id => FindViewById<AutoCompleteTextView>(id)))
            {
                view.Adapter = _autoCompleteAdapter;
                view.SetDropDownBackgroundDrawable(new ColorDrawable(Color.DeepSkyBlue));
            }

            foreach (var rowIds in allScoreEditTextId)
            {
                _scoreViews.Add(
                    new RowOfScore
                    {
                        EditTexts = rowIds
                            .Select(id => FindViewById<EditText>(id))
                            .ToArray(),
                        ResourceIds = rowIds
                    }
                    );
            }


            _displayScoreProgress = new List<ProgressBar>();
            for (int i = 0; i < ResourceIdentifiers.ScoreProgressBar.Length; i++)
            {
                var pb = FindViewById<ProgressBar>(ResourceIdentifiers.ScoreProgressBar[i]);
                _displayScoreProgress.Add(pb);
                int id;
                switch (i)
                {
                    case 0:
                        id = Resource.Drawable.progress_drawable_red;
                        break;
                    case 1:
                        id = Resource.Drawable.progress_drawable_orange;
                        break;
                    case 2:
                        id = Resource.Drawable.progress_drawable_green;
                        break;
                    case 3:
                        id = Resource.Drawable.progress_drawable_blue;
                        break;
                    default:
                        id = Resource.Drawable.progress_drawable_purple;
                        break;
                }
                pb.ProgressDrawable = Resources.GetDrawable(id);
            }


            _playerNameEditTexts = new List<EditText>();
            ResourceIdentifiers.PlayerNameEditText.ForEach(
                i => _playerNameEditTexts.Add(FindViewById<EditText>(i))
            );

            _displayScoreViews = new List<TextView>();
            foreach (var id in ResourceIdentifiers.ScoreTextView)
            {
                _displayScoreViews.Add(FindViewById<TextView>(id));
            }
        }

        private void AddEventHandler()
        {
            foreach (var row in _scoreViews)
            {
                for (var i = 0; i < row.EditTexts.Length; i++)
                {
                    switch (i)
                    {
                        case 0:
                            {
                                row.EditTexts[i].FocusChange += delegate { UpdateScore(0, row); };
                                break;
                            }
                        case 1:
                            {
                                row.EditTexts[i].FocusChange += delegate { UpdateScore(1, row); };
                                break;
                            }
                        case 2:
                            {
                                row.EditTexts[i].FocusChange += delegate { UpdateScore(2, row); };
                                break;
                            }
                        case 3:
                            {
                                row.EditTexts[i].FocusChange += delegate { UpdateScore(3, row); };
                                break;
                            }
                        case 4:
                            {
                                row.EditTexts[i].FocusChange += delegate { UpdateScore(4, row); };
                                break;
                            }

                    }
                }
            }
        }

        private void UpdateScore(int playerId, RowOfScore currentRow)
        {
            if (playerId > 4)
            {
                return;
            }

            // collect the scores from every row belonging to 'playerId'
            var currentTotalScore = _scoreViews
                .Select(row => ValidateInputFromScoreEditText(row.EditTexts[playerId]))
                .Aggregate((a, b) => a + b);

            // calculate current row total should be 26
            int rowTotal = currentRow.EditTexts
                .Select(ValidateInputFromScoreEditText)
                .Aggregate((a, b) => a + b);

            // check for all rows are totaling correctly
            // "shooting for the moon" counted
            foreach (var et in currentRow.EditTexts)
            {
                var color = _babyBlueColor;
                if (!(rowTotal == 0 || rowTotal == 26 || rowTotal % 26 == 0))
                {
                    color = _invalidScoreColour;
                }
                et.SetBackgroundColor(color);
            }

            // Adding one for the Progress bar to display nicely (where 0 point still shows the bar)
            _displayScoreProgress[playerId].Progress = currentTotalScore + 1;
            _displayScoreViews[playerId].Text = currentTotalScore.ToString();
            _lastValue = currentRow.EditTexts[playerId].Text;
        }

        public override bool OnPrepareOptionsMenu(IMenu menu)
        {
            if (menu.Size() == 0)
            {
                MenuInflater.Inflate(Resource.Menu.menu, menu);
            }
            return base.OnPrepareOptionsMenu(menu);
        }

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            switch (item.ItemId)
            {
                case Resource.Id.ClearAllFields:
                    {
                        var dialog = new AlertDialog.Builder(this);
                        dialog.SetTitle("Clear Scorecard");
                        dialog.SetMessage("Are you sure you want to delete all the scores?");
                        dialog.SetCancelable(true);
                        dialog.SetPositiveButton("Delete", delegate { OnCreate(null); });
                        dialog.SetNegativeButton("Cancel", delegate { });
                        dialog.SetNeutralButton("Keep Names",
                                                delegate
                                                {
                                                    var bundle = new Bundle();
                                                    bundle.PutStringArrayList(
                                                        "PlayerNames",
                                                        _playerNameEditTexts.Select(et => et.Text).ToList()
                                                        );
                                                    OnCreate(bundle);
                                                });
                        dialog.Show();
                        return true;
                    }
                case Resource.Id.ShameBell:
                    {
                        SoundManager.PlayShame(this);
                        return true;
                    }
                case Resource.Id.JawsRingtone:
                    {
                        SoundManager.PlayJaws(this);
                        return true;
                    }
                case Resource.Id.SwitchScoreView:
                    {
                        if (_rowScoreProgressBar.Visibility == ViewStates.Visible)
                        {
                            _rowScoreProgressBar.Visibility = ViewStates.Gone;
                            _rowScoreTextView.Visibility = ViewStates.Visible;
                        }
                        else
                        {
                            _rowScoreProgressBar.Visibility = ViewStates.Visible;
                            _rowScoreTextView.Visibility = ViewStates.Gone;
                        }
                        return true;
                    }
                default:
                    // If we got here, the user's action was not recognized.
                    // Invoke the superclass to handle it.
                    return base.OnOptionsItemSelected(item);
            }
        }

        private int ValidateInputFromScoreEditText(EditText editText)
        {
            var input = int.Parse(string.IsNullOrEmpty(editText.Text)
                    ? "0"
                    : editText.Text);

            if (input < 0 || input > 27)
            {
                editText.SetBackgroundColor(_invalidScoreColour);
            }

            return input;
        }

        private static void GetAllScoreTextFields(int[][] allScoreEditTextId)
        {
            allScoreEditTextId[0] = new[]
                                    {
                                        Resource.Id.scoreA1,
                                        Resource.Id.scoreA2,
                                        Resource.Id.scoreA3,
                                        Resource.Id.scoreA4,
                                        Resource.Id.scoreA5
                                    };
            allScoreEditTextId[1] = new[]
                                    {
                                        Resource.Id.scoreB1,
                                        Resource.Id.scoreB2,
                                        Resource.Id.scoreB3,
                                        Resource.Id.scoreB4,
                                        Resource.Id.scoreB5
                                    };
            allScoreEditTextId[2] = new[]
                                    {
                                        Resource.Id.scoreC1,
                                        Resource.Id.scoreC2,
                                        Resource.Id.scoreC3,
                                        Resource.Id.scoreC4,
                                        Resource.Id.scoreC5
                                    };
            allScoreEditTextId[3] = new[]
                                    {
                                        Resource.Id.scoreD1,
                                        Resource.Id.scoreD2,
                                        Resource.Id.scoreD3,
                                        Resource.Id.scoreD4,
                                        Resource.Id.scoreD5
                                    };

            allScoreEditTextId[4] = new[]
                                    {
                                        Resource.Id.scoreE1,
                                        Resource.Id.scoreE2,
                                        Resource.Id.scoreE3,
                                        Resource.Id.scoreE4,
                                        Resource.Id.scoreE5
                                    };
            allScoreEditTextId[5] = new[]
                                    {
                                        Resource.Id.scoreF1,
                                        Resource.Id.scoreF2,
                                        Resource.Id.scoreF3,
                                        Resource.Id.scoreF4,
                                        Resource.Id.scoreF5
                                    };
            allScoreEditTextId[6] = new[]
                                    {
                                        Resource.Id.scoreG1,
                                        Resource.Id.scoreG2,
                                        Resource.Id.scoreG3,
                                        Resource.Id.scoreG4,
                                        Resource.Id.scoreG5
                                    };
            allScoreEditTextId[7] = new[]
                                    {
                                        Resource.Id.scoreH1,
                                        Resource.Id.scoreH2,
                                        Resource.Id.scoreH3,
                                        Resource.Id.scoreH4,
                                        Resource.Id.scoreH5
                                    };
            allScoreEditTextId[8] = new[]
                                                {
                                        Resource.Id.scoreI1,
                                        Resource.Id.scoreI2,
                                        Resource.Id.scoreI3,
                                        Resource.Id.scoreI4,
                                        Resource.Id.scoreI5
                                    };
            allScoreEditTextId[9] = new[]
                                                {
                                        Resource.Id.scoreJ1,
                                        Resource.Id.scoreJ2,
                                        Resource.Id.scoreJ3,
                                        Resource.Id.scoreJ4,
                                        Resource.Id.scoreJ5
                                    };
            allScoreEditTextId[10] = new[]
                                                {
                                        Resource.Id.scoreK1,
                                        Resource.Id.scoreK2,
                                        Resource.Id.scoreK3,
                                        Resource.Id.scoreK4,
                                        Resource.Id.scoreK5
                                    };
            allScoreEditTextId[11] = new[]
                                                {
                                        Resource.Id.scoreL1,
                                        Resource.Id.scoreL2,
                                        Resource.Id.scoreL3,
                                        Resource.Id.scoreL4,
                                        Resource.Id.scoreL5
                                    };
            allScoreEditTextId[12] = new[]
                                                {
                                        Resource.Id.scoreM1,
                                        Resource.Id.scoreM2,
                                        Resource.Id.scoreM3,
                                        Resource.Id.scoreM4,
                                        Resource.Id.scoreM5
                                    };
            allScoreEditTextId[13] = new[]
                                                {
                                        Resource.Id.scoreN1,
                                        Resource.Id.scoreN2,
                                        Resource.Id.scoreN3,
                                        Resource.Id.scoreN4,
                                        Resource.Id.scoreN5
                                    };
            allScoreEditTextId[14] = new[]
                                                {
                                        Resource.Id.scoreO1,
                                        Resource.Id.scoreO2,
                                        Resource.Id.scoreO3,
                                        Resource.Id.scoreO4,
                                        Resource.Id.scoreO5
                                    };
            allScoreEditTextId[15] = new[]
                                                {
                                        Resource.Id.scoreP1,
                                        Resource.Id.scoreP2,
                                        Resource.Id.scoreP3,
                                        Resource.Id.scoreP4,
                                        Resource.Id.scoreP5
                                    };
        }
    }
}

