using UnityEngine;

namespace Zumo {
    public class MusicManager : MonoBehaviour {
        [Header("Songs")]
        public AudioSource menuSong;
        public AudioSource gameSong;
        public AudioSource winSong;

        private AudioSource nowPlaying;

        public void Play (AudioSource song) {
            Stop();

            nowPlaying = song;
            nowPlaying.Play();
        }

        public void Stop () {
            if (nowPlaying && nowPlaying.isPlaying) {
                nowPlaying.Stop();
            }
        }
    }
}
