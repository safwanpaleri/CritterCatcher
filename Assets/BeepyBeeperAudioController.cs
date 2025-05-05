using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BeepyBeeperAudioController : MonoBehaviour
{

    void Start()
    {
        StartCoroutine(PlayBeepSound());
    }

    private IEnumerator PlayBeepSound()
    {
        yield return new WaitForSeconds(Random.Range(2, 8));

        StartCoroutine(PlayBeepSound());
        GetComponent<AudioPlayer>().PlayAudio("Beep");
    }
}
