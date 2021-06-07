using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CreditsScript : MonoBehaviour {

    private AsyncOperation _sceneOperation;

    public AudioClip SceneChanging;
    private AudioSource _audioSource;

    // Use this for initialization
    void Start () {
        _audioSource = GetComponent<AudioSource>();
    }
	
	// Update is called once per frame
	void Update () {
		if (Input.anyKeyDown)
        {
            _audioSource.PlayOneShot(SceneChanging);
            SceneManager.LoadSceneAsync("MenuScene", LoadSceneMode.Single);
        }
	}
}
