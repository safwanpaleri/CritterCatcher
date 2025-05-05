using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.Video;

public class CutscenePlayer : MonoBehaviour
{

    [Serializable]
    struct Cutscene
    {
        public float time;
        public Sprite sprite;
    }

    [SerializeField] Image image;
    [SerializeField] List<Cutscene> cutscenes = new List<Cutscene>();
    [SerializeField] int cutsceneIndex = 0;
    [SerializeField] int sceneIndexToLoad = 1;
    [SerializeField] private VideoPlayer videoPlayer;
    [SerializeField] private AudioSource musicPlayer;


    void Start()
    {
        
    }

    private void Update()
    {
        if(Input.GetButtonDown("Jump"))
        {
            SkipCutScene();
        }
    }

    public void SkipCutScene()
    {
        SceneManager.LoadScene(sceneIndexToLoad);
    }

    public void PlayCutSceneImages()
    {
        StartCoroutine(playCutScene());
        
    }

    private IEnumerator playCutScene()
    {
        musicPlayer.Play();
        videoPlayer.Play();
        yield return new WaitForSeconds(6f);
        videoPlayer.Stop();
        Camera.main.targetTexture = null;
        videoPlayer.targetCameraAlpha = 0.0f;
        image.gameObject.SetActive(true);
        StartCoroutine(PlayCutscene(cutscenes[cutsceneIndex]));
    }

    private IEnumerator PlayCutscene(Cutscene cutscene)
    {
        image.sprite = cutscene.sprite;
        yield return new WaitForSeconds(cutscene.time);
        cutsceneIndex++;

        if (cutsceneIndex < cutscenes.Count)
        {
            StartCoroutine(PlayCutscene(cutscenes[cutsceneIndex]));
        }
        else
        {
            SceneManager.LoadScene(sceneIndexToLoad);
        }
    }
}
