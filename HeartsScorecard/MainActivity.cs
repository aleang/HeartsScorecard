using Android.App;
using Android.Content.Res;
using Android.Graphics;
using Android.Graphics.Drawables;
using Android.Media;
using Android.OS;
using Android.Views;
using Android.Widget;
using System.Collections.Generic;
using System.Linq;

namespace HeartsScorecard
{
    [Activity(Label = "Hearts Scorecard",
        MainLauncher = true,
        Icon = "@drawable/icon")]
    public class MainActivity : Activity
    {
        List<RowOfScore> scoreViews;
        List<ProgressBar> displayScoreProgress;
        private MediaPlayer _shamePlayer;
        private string[] _playerNames;
        private ArrayAdapter _autoCompleteAdapter;
        private Color _babyBlueColor;

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.Main);
            InitialiseStuff();
            FindViews();
            AddEventHandler();
        }

        private void InitialiseStuff()
        {
            _shamePlayer = MediaPlayer.Create(
                            this,
                            Resource.Raw.Shame);
            _playerNames = new[]
                           {
                               "Pheng", "Matty", "Paul", "Anna", "Diego", "Irene", "Wiremu", "Marcos", "Clarita", "Michael", "Justin"
                           };
            _autoCompleteAdapter = new ArrayAdapter(this, Android.Resource.Layout.SimpleDropDownItem1Line, _playerNames);
            _babyBlueColor = Resources.GetColor(Resource.Color.babyblue);
        }

        private void FindViews()
        {
            scoreViews = new List<RowOfScore>();
            var allScoreEditTextId = new int[Resources.GetInteger(Resource.Integer.rowCount)][];

            GetResourceIds(allScoreEditTextId);

            var autoCompletePlayerIds = new[]
                                  {
                                      Resource.Id.player1,
                                      Resource.Id.player2,
                                      Resource.Id.player3,
                                      Resource.Id.player4,
                                      Resource.Id.player5
                                  };
            foreach (var view in autoCompletePlayerIds
                .Select(id => FindViewById<AutoCompleteTextView>(id)))
            {
                view.Adapter = _autoCompleteAdapter;
                view.SetDropDownBackgroundDrawable(new ColorDrawable(Color.DeepSkyBlue));
            }

            foreach (var rowIds in allScoreEditTextId)
            {
                scoreViews.Add(
                    new RowOfScore
                    {
                        EditTexts = rowIds
                            .Select(id => FindViewById<EditText>(id))
                            .ToArray(),
                        ResourceIds = rowIds
                    }
                );
            }

            var displayProgressIds = new[]
                                     {
                                         Resource.Id.scoreProgress1,
                                         Resource.Id.scoreProgress2,
                                         Resource.Id.scoreProgress3,
                                         Resource.Id.scoreProgress4,
                                         Resource.Id.scoreProgress5
                                     };
            displayScoreProgress = new List<ProgressBar>();
            for (int i = 0; i < displayProgressIds.Length; i++)
            {
                var pb = FindViewById<ProgressBar>(displayProgressIds[i]);
                displayScoreProgress.Add(pb);
                var drawableId =
                    i == 0 ? Resource.Drawable.progress_drawable_red :
                    i == 1 ? Resource.Drawable.progress_drawable_orange :
                    i == 2 ? Resource.Drawable.progress_drawable_green :
                    i == 3 ? Resource.Drawable.progress_drawable_blue :
                            Resource.Drawable.progress_drawable_purple;
                pb.ProgressDrawable = Resources.GetDrawable(drawableId);
            }
        }

        private void AddEventHandler()
        {
            foreach (var row in scoreViews)
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
            if (playerId > 4) return;
            var score = scoreViews
                .Select(row => ValidateInputFromScoreEditText(row.EditTexts[playerId]))
                .Aggregate((a, b) => a + b);

            // calculate current row total should be 26
            int rowTotal = currentRow.EditTexts
                .Select(et => ValidateInputFromScoreEditText(et))
                .Aggregate((a, b) => a + b);

            foreach (var et in currentRow.EditTexts)
            {
                var color = _babyBlueColor;
                if (!(rowTotal == 0 || rowTotal == 26))
                {
                    color = Color.LightPink;
                }
                et.SetBackgroundColor(color);

            }

            displayScoreProgress[playerId].Progress = score;
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
                        dialog.Show();
                        return true;
                    }
                case Resource.Id.ShameBell:
                    {
                        if (_shamePlayer.IsPlaying)
                        {
                            _shamePlayer.Stop();
                        }
                        else
                        {
                            _shamePlayer.Start();
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
            var input = int.Parse(
                string.IsNullOrEmpty(editText.Text)
                    ? "0"
                    : editText.Text);
            if (input < 0 || input > 26)
            {
                editText.SetBackgroundColor(Color.DarkRed);
            }
            else if (input >= 13 && !_shamePlayer.IsPlaying)
            {
                _shamePlayer.Start();
            }

            return input;
        }

        private static void GetResourceIds(int[][] allScoreEditTextId)
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
            //allScoreEditTextId[0] = new[]
            //                                    {
            //                            Resource.Id.scoreA1,
            //                            Resource.Id.scoreA2,
            //                            Resource.Id.scoreA3,
            //                            Resource.Id.scoreA4,
            //                            Resource.Id.scoreA5
            //                        };
        }
    }

    public class RowOfScore
    {
        public EditText[] EditTexts { get; set; }

        public int[] ResourceIds { get; set; }
    }
}

