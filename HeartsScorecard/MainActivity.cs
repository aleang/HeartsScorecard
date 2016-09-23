using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Android.App;
using Android.Content;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;

namespace HeartsScorecard
{
    [Activity(Label = "Hearts Scorecard", 
        MainLauncher = true, 
        Icon = "@drawable/icon")]
    public class MainActivity : Activity
    {
        List<RowOfScore> scoreViews;
        List<TextView> displayScoreViews;
        private int[] currentScore;

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.Main);
            FindViews();
            currentScore = new int[5];
            AddEventHandler();
            //// Get our button from the layout resource,
            //// and attach an event to it
            //Button button = FindViewById<Button>(Resource.Id.MyButton);

            //button.Click += delegate { button.Text = string.Format("{0} clicks!", count++); };
        }

        private void AddEventHandler()
        {
            foreach (var rowOfScore in scoreViews)
            {
                for (var i = 0; i < rowOfScore.EditTexts.Length; i++)
                {
                    switch (i)
                    {
                        case 0:
                        {
                            rowOfScore.EditTexts[i].FocusChange += delegate { UpdateScore(0); };
                            break;
                        }
                        case 1:
                        {
                            rowOfScore.EditTexts[i].FocusChange += delegate { UpdateScore(1); };
                            break;
                        }
                        case 2:
                        {
                            rowOfScore.EditTexts[i].FocusChange += delegate { UpdateScore(2); };
                            break;
                        }
                        case 3:
                        {
                            rowOfScore.EditTexts[i].FocusChange += delegate { UpdateScore(3); };
                            break;
                        }
                        case 4:
                        {
                            rowOfScore.EditTexts[i].FocusChange += delegate { UpdateScore(4); };
                            break;
                        }

                    }
                }
            }
        }

        private void UpdateScore(int playerId)
        {
            if (playerId > 4) return;
            var score = scoreViews
                .Select(row => ValidateInputFromScoreEditText(row.EditTexts[playerId]))
                .Aggregate((a, b) => a + b);
            displayScoreViews[playerId].Text = score.ToString();
        }

        private int ValidateInputFromScoreEditText(EditText editText)
        {
            var input = int.Parse(
                string.IsNullOrEmpty(editText.Text)
                    ? "0"
                    : editText.Text);
            if (input < 0 || input > 26)
            {
                editText.Text = string.Empty;
                return 0;
            }

            return input;
        }

        private void FindViews()
        {
            scoreViews = new List<RowOfScore>();
            var allScoreEditTextId = new int[Resources.GetInteger(Resource.Integer.rowCount)][];

            GetResourceIds(allScoreEditTextId);

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

            var displayScoreIds = new[]
                                  {
                                      Resource.Id.displayScore1,
                                      Resource.Id.displayScore2,
                                      Resource.Id.displayScore3,
                                      Resource.Id.displayScore4,
                                      Resource.Id.displayScore5
                                  };
            displayScoreViews = new List<TextView>();
            foreach (var id in displayScoreIds)
            {
                displayScoreViews.Add(FindViewById<TextView>(id));
            }
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

