using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BeepyPlayTemp : MonoBehaviour
{

    [SerializeField] AudioSource m_AudioSource;
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(PlayBeepSound());
    }

    private IEnumerator PlayBeepSound()
    {
        yield return new WaitForSeconds(Random.Range(2,8));

        Beep();
        StartCoroutine(PlayBeepSound());
    }
    private void Beep()
    {
        m_AudioSource.Play();
    }
}
