using UnityEngine;
using System.Collections;

public class MusicManager : MonoBehaviour {
    public AudioSource menuSong;
    public AudioSource gameSong;
    public AudioSource winSong;

    private AudioSource nowPlaying;

    public void Play(AudioSource song) {
        if (nowPlaying) {
            nowPlaying.Stop();
        }

        nowPlaying = song;
        nowPlaying.Play();
    }
}
