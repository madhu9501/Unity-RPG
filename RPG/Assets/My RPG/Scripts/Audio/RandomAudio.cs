using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MyRPG
{
    public class RandomAudio : MonoBehaviour
    {
        [System.Serializable]
        public class SoundBank{
            public string name;
            public AudioClip[] clips;
        }
        public SoundBank soundBank = new SoundBank();
        private AudioSource audioSource;
        public bool canPlay;
        public bool isPlaying;


        void Awake(){
            audioSource = GetComponent<AudioSource>();
        }

        public void PlayAudioClips(){
            AudioClip clip = soundBank.clips[Random.Range(0, soundBank.clips.Length)];
            if(clip == null){ return;}
            audioSource.clip = clip;
            audioSource.Play();

        }

    }
}

