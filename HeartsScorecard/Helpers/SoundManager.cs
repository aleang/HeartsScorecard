using Android.App;
using Android.Media;

namespace HeartsScorecard.Helpers
{
    public static class SoundManager
    {
        private static MediaPlayer _shamePlayer;
        private static MediaPlayer _jawsPlayer;
        private static MediaPlayer _wololoPlayer;

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

        public static void PlayWololo(Activity activity)
        {
            if (_wololoPlayer == null)
            {
                _wololoPlayer = MediaPlayer.Create(activity, Resource.Raw.Wololo);
                _wololoPlayer.Looping = true;
            }

            if (_wololoPlayer.IsPlaying)
            {
                _wololoPlayer.Stop();
            }
            else
            {
                _wololoPlayer.Reset();
                _wololoPlayer = MediaPlayer.Create(activity, Resource.Raw.Wololo);
                _wololoPlayer.Looping = true;
                _wololoPlayer.Start();

            }
        }
    }
}