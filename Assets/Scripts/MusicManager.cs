using UnityEngine;
using System.Collections;

public class MusicManager : MonoBehaviour {
    public AudioSource menuSong;
    public AudioSource desertSong;
    public AudioSource beachSong;
    public AudioSource safariSong;
    public AudioSource tundraSong;
    public AudioSource forestSong;
    public AudioSource gameSong;
    public AudioSource winSong;

    private AudioSource nowPlaying;

    public void Play(AudioSource song) {
        if (nowPlaying && nowPlaying.isPlaying) {
            nowPlaying.Stop();
        }

        nowPlaying = song;
        nowPlaying.Play();
    }

    public void Stop() {
        nowPlaying.Stop();
    }
}
