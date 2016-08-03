using UnityEngine;

namespace Zumo {
    public class InterestingAudioSource {
        AudioSource audio;

        public InterestingAudioSource (GameObject gameObject) {
            audio = gameObject.AddComponent<AudioSource>();
        }

        public void PlayOnce (AudioClip clip) {
            messWithOptions();
            audio.PlayOneShot(clip);
        }

        void messWithOptions () {
            audio.pitch = Random.Range(0.9f, 1.1f);
            audio.volume = Random.Range(0.9f, 1f);
            audio.panStereo = Random.Range(-0.1f, 0.1f);
        }
    }
}
