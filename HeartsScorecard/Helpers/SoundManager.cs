using Android.App;
using Android.Media;

namespace HeartsScorecard.Helpers
{
    public static class SoundManager
    {
        private static MediaPlayer _shamePlayer;
        private static MediaPlayer _jawsPlayer;

        public static void PlayShame(Activity activity)
        {
            if (_shamePlayer == null)
            {
                _shamePlayer = MediaPlayer.Create(
                    activity,
                    Resource.Raw.Shame);
            }

            if (_shamePlayer.IsPlaying)
            {
                _shamePlayer.Stop();
            }
            else
            {
                _shamePlayer.Reset();
                _shamePlayer = MediaPlayer.Create(activity, Resource.Raw.Shame);
                _shamePlayer.Start();
            }
        }

        public static void PlayJaws(Activity activity)
        {
            if (_jawsPlayer == null)
            {
                _jawsPlayer = MediaPlayer.Create(activity, Resource.Raw.Jaws);
                _jawsPlayer.Looping = true;
            }

            if (_jawsPlayer.IsPlaying)
            {
                _jawsPlayer.Stop();
            }
            else
            {
                _jawsPlayer.Reset();
                _jawsPlayer = MediaPlayer.Create(activity, Resource.Raw.Jaws);
                _jawsPlayer.Looping = true;
                _jawsPlayer.Start();

            }
        }
    }
}