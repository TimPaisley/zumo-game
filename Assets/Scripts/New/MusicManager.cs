using UnityEngine;

namespace Zumo {
    [RequireComponent(typeof(AudioSource))]
    public class MusicManager : MonoBehaviour {
        [Header("Songs")]
        public AudioClip menuSong;
        public AudioClip winSong;

        AudioSource nowPlaying;

        void Awake () {
            nowPlaying = GetComponent<AudioSource>();
        }

        public void Play (AudioClip song) {
            if (nowPlaying.clip == song) {
                return;
            }

            Stop();

            nowPlaying.clip = song;
            nowPlaying.Play();
        }

        public void Stop () {
            if (nowPlaying && nowPlaying.isPlaying) {
                nowPlaying.Stop();
            }
        }
    }
}
