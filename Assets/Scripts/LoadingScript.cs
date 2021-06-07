using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LoadingScript : MonoBehaviour
{
    private readonly List<AudioClip> _audioClips = new List<AudioClip>();

    private Image _cover;
    private float _progressBarWidth;
    private RectTransform _progressBar;
    private Text _captionText;
    private AudioSource _audioSource;

    private State _currentState = State.FadeIn;
    private State _currentSubstate = State.FadeIn;
    private int _currentCaptionIndex = 0;

    private AsyncOperation _sceneLoading = null;

    void Start ()
    {
        _cover = GameObject.Find("Cover").GetComponent<Image>();
        _progressBarWidth = ((RectTransform)GameObject.Find("ProgressBg").transform).rect.width;
        _progressBar = (RectTransform)GameObject.Find("ProgressBar").transform;
        _captionText = GameObject.Find("CaptionText").GetComponent<Text>();
        _audioSource = GetComponent<AudioSource>();

        _cover.color = new Color(0, 0, 0, 1);
        _captionText.color = new Color(1, 1, 1, 0);

        foreach (string audioFile in LoadingParameters.Speeches)
        {
            _audioClips.Add(Resources.Load<AudioClip>("Audios/Scenes/" + audioFile));
        }
    }

    void Update()
    {
        if (_currentState == State.FadeIn)
            return;

        if (_sceneLoading == null)
        {
            _sceneLoading = SceneManager.LoadSceneAsync(LoadingParameters.NextSceneName);
            _sceneLoading.allowSceneActivation = false;
        }

        // update progress bar
        float progressValue = _progressBarWidth * _sceneLoading.progress / 0.9f;
        _progressBar.sizeDelta = new Vector2(progressValue, _progressBar.sizeDelta.y);
    }

	void FixedUpdate ()
	{
	    if (_currentState == State.FadeIn)
	    {
	        float coverAlpha = _cover.color.a;
	        if (coverAlpha > 0)
	            coverAlpha -= 0.01f;
	        if (coverAlpha <= 0)
	            coverAlpha = 0;
	        _cover.color = new Color(0, 0, 0, coverAlpha);

            if (coverAlpha <= 0)
                _currentState = State.Captions;
	    }
        else if (_currentState == State.Captions)
	    {
            float captionAlpha = _captionText.color.a;

	        if (_currentSubstate == State.FadeIn && captionAlpha.Equals(0f))
	        {
	            _captionText.text = LoadingParameters.Captions[_currentCaptionIndex];
                if (_audioClips.Count > _currentCaptionIndex)
                    _audioSource.PlayOneShot(_audioClips[_currentCaptionIndex]);
            }

            if (_currentSubstate == State.FadeIn && captionAlpha < 1)
	        {
	            captionAlpha += 0.01f;
	            if (captionAlpha > 1)
	                captionAlpha = 1;
                _captionText.color = new Color(1, 1, 1, captionAlpha);
	        }
	        else if (_currentSubstate == State.FadeIn && captionAlpha >= 1)
	        {
                if (!_audioSource.isPlaying)
                    _currentSubstate = State.FadeOut;
	        }
            else if (_currentSubstate == State.FadeOut && captionAlpha > 0)
	        {
                captionAlpha -= 0.015f;
                if (captionAlpha < 0)
                    captionAlpha = 0;
                _captionText.color = new Color(1, 1, 1, captionAlpha);
            }
            else if (_currentSubstate == State.FadeOut && captionAlpha <= 0)
            {
                _currentSubstate = State.FadeIn;
                _currentCaptionIndex++;
                if (_currentCaptionIndex >= LoadingParameters.Captions.Length)
                    _currentState = State.FadeOut;
            }
        }
        else if (_currentState == State.FadeOut && _sceneLoading.progress >= 0.9f)
        {
            float coverAlpha = _cover.color.a;
            if (coverAlpha < 1)
                coverAlpha += 0.01f;
            if (coverAlpha >= 1)
                coverAlpha = 1;
            _cover.color = new Color(0, 0, 0, coverAlpha);

            if (coverAlpha >= 1)
                _sceneLoading.allowSceneActivation = true;
        }
    }

    private enum State
    {
        FadeIn,
        Captions,
        FadeOut,
    }
}
