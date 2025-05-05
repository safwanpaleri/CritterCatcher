using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class AudioPlayer : MonoBehaviour
{
    [Serializable]
    struct AudioClips
    {
        public AudioClip clip;
        public string name;
    }
    // Start is called before the first frame update
    [SerializeField] List<AudioClips> clipList = new List<AudioClips>();
    [SerializeField] AudioSource audioSource;
    Dictionary<string, AudioClip> audioClips = new Dictionary<string, AudioClip>();

    private void Awake()
    {
        foreach (var clip in clipList)
        {
            audioClips[clip.name] = clip.clip;
        }

        audioSource = GetComponent<AudioSource>();
    }

    public void PlayAudio(string name)
    {
        audioSource.loop = false;
        audioSource.resource = audioClips[name];
        audioSource.Play();
    }

    public void PlayAudioLoop(string name)
    {
        audioSource.loop = true;
        audioSource.resource = audioClips[name];
        audioSource.Play();
    }
}
