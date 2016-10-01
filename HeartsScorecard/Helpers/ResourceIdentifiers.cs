using System.Collections.Generic;

namespace HeartsScorecard.Helpers
{
    public class ResourceIdentifiers
    {
        public static int[] ScoreProgressBar =
        {
            Resource.Id.scoreProgress1,
            Resource.Id.scoreProgress2,
            Resource.Id.scoreProgress3,
            Resource.Id.scoreProgress4,
            Resource.Id.scoreProgress5
        };
        public static List<int> PlayerNameEditText = new List<int>
                                                     {
                                                         Resource.Id.player1,
                                                         Resource.Id.player2,
                                                         Resource.Id.player3,
                                                         Resource.Id.player4,
                                                         Resource.Id.player5
                                                     };

        public static int[] ScoreTextView =
        {
            Resource.Id.displayScore1,
            Resource.Id.displayScore2,
            Resource.Id.displayScore3,
            Resource.Id.displayScore4,
            Resource.Id.displayScore5
        };
        public static int[] PlayerNameAutoComplete =
        {
            Resource.Id.player1,
            Resource.Id.player2,
            Resource.Id.player3,
            Resource.Id.player4,
            Resource.Id.player5
        };
    }
}