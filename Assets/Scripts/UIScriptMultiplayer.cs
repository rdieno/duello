using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIScriptMultiplayer : MonoBehaviour
{
    private GameObject _scorePanel;
    private GameObject _blurMask;
    private GameObject _btnPlayAgain;
    private GameObject _btnNewScene;
    private GameObject _btnMainMenu;
    private GameObject _selectionIndicator;
    private GameObject _player;
    private GameObject _player2;

    private int _currentSelection = 0;
    private bool _isChangingIndex = false;
    private float _changeStartTime = 0;

    private bool _changingScene = false;
    private bool _newSceneLoading = false;

    private AsyncOperation _sceneOperation;

    void Start ()
	{
        _scorePanel = GameObject.Find("ScorePanel");
        _blurMask = GameObject.Find("BlurMask");
        _btnPlayAgain = GameObject.Find("ButtonPlayAgain");
        _btnNewScene = GameObject.Find("ButtonNewScene");
        _btnMainMenu = GameObject.Find("ButtonMainMenu");
        _selectionIndicator = GameObject.Find("ScoreButtonIndicator");
        _player = GameObject.Find("Player");
        _player2 = GameObject.Find("Player2");


        switch (_currentSelection)
        {
            case 0:
                _selectionIndicator.transform.position = _btnPlayAgain.transform.position + new Vector3(0, 5, 0);
                break;
            case 1:
                _selectionIndicator.transform.position = _btnNewScene.transform.position + new Vector3(0, 5, 0);
                break;
            case 2:
                _selectionIndicator.transform.position = _btnMainMenu.transform.position + new Vector3(0, 5, 0);
                break;
        }

        _scorePanel.SetActive(false);

        Material blurMat = _blurMask.GetComponent<Image>().material;
        blurMat.SetFloat("_Size", 0);
        blurMat.SetColor("_Color", Color.white);
    }

	void Update ()
	{
        if (!_scorePanel.activeSelf)
        {
            return;
        }

        float blurValue = _scorePanel.GetComponent<Image>().material.GetFloat("_Size");
        if (blurValue < 10)
            blurValue += Time.deltaTime * 8;
        _scorePanel.GetComponent<Image>().material.SetFloat("_Size", blurValue);
        if (blurValue < 10)
            return;

        // Disable Player
        FighterScript playerFight = _player.GetComponent<FighterScript>();
        if (playerFight.enabled)
        {
            playerFight.enabled = false;
        }

        PlayerController playerAnimation = _player.GetComponent<PlayerController>();
        if (playerAnimation != null && playerAnimation.enabled)
        {
            playerAnimation.enabled = false;
        }

        PlayerScript playerScript = _player.GetComponent<PlayerScript>();
        if (playerScript != null && playerScript.enabled)
        {
            playerScript.enabled = false;
        }

        if (_player.activeSelf)
        {
            _player.SetActive(false);
        }

        // Disable Player 2
        Fighter2Script player2Fight = _player2.GetComponent<Fighter2Script>();
        if (player2Fight.enabled)
        {
            player2Fight.enabled = false;
        }

        Player2Controller player2Animation = _player2.GetComponent<Player2Controller>();
        if (player2Animation != null && player2Animation.enabled)
        {
            player2Animation.enabled = false;
        }

        PlayerScript player2Script = _player2.GetComponent<PlayerScript>();
        if (player2Script != null && player2Script.enabled)
        {
            player2Script.enabled = false;
        }

        if (_player2.activeSelf)
        {
            _player2.SetActive(false);
        }

        float moveVertical = Input.GetAxis("Vertical");
        if (moveVertical == 0)
        {
            moveVertical = Input.GetAxis("Vertical2");
        }

        int timeAfterSelecting = Mathf.RoundToInt((Time.fixedTime - _changeStartTime) * 1000);

        if (!_isChangingIndex && !_changingScene)
        {
            if (moveVertical > 0)
            {
                _currentSelection = (_currentSelection + 3 - 1) % 3;

                switch (_currentSelection)
                {
                    case 0:
                        _selectionIndicator.transform.position = _btnPlayAgain.transform.position + new Vector3(0, 5, 0);
                        break;
                    case 1:
                        _selectionIndicator.transform.position = _btnNewScene.transform.position + new Vector3(0, 5, 0);
                        break;
                    case 2:
                        _selectionIndicator.transform.position = _btnMainMenu.transform.position + new Vector3(0, 5, 0);
                        break;
                }

                _isChangingIndex = true;
                _changeStartTime = Time.fixedTime;
            }
            else if (moveVertical < 0)
            {
                _currentSelection = (_currentSelection + 3 + 1) % 3;

                switch (_currentSelection)
                {
                    case 0:
                        _selectionIndicator.transform.position = _btnPlayAgain.transform.position + new Vector3(0, 5, 0);
                        break;
                    case 1:
                        _selectionIndicator.transform.position = _btnNewScene.transform.position + new Vector3(0, 5, 0);
                        break;
                    case 2:
                        _selectionIndicator.transform.position = _btnMainMenu.transform.position + new Vector3(0, 5, 0);
                        break;
                }

                _isChangingIndex = true;
                _changeStartTime = Time.fixedTime;
            }
        }
        else if (moveVertical.Equals(0) || (timeAfterSelecting != 0 && timeAfterSelecting % 1000 == 0))
        {
            _isChangingIndex = false;
        }

	    if (!_changingScene && Input.GetButtonDown("Submit"))
	    {
            TimerScriptMultiplayer.timerIsActive = true;
            switch (_currentSelection)
	        {
                case 0:
	                _sceneOperation = SceneManager.LoadSceneAsync(SceneManager.GetActiveScene().buildIndex, LoadSceneMode.Single);
                    break;
                case 1:
                    int randomNumber = Random.Range(9, 13);
                    while(randomNumber == SceneManager.GetActiveScene().buildIndex)
                    {
                        randomNumber = Random.Range(9, 13);
                    }
                    string newSceneName = "";
                    switch (randomNumber)
                    {
                        case 9:
                            newSceneName = "MultiLevelZero";
                            break;
                        case 10:
                            newSceneName = "MultiLevelOne";
                            break;
                        case 11:
                            newSceneName = "MultiLevelTwo";
                            break;
                        case 12:
                            newSceneName = "MultiLevelThree";
                            break;
                    }
                    
                    LoadingParameters.Captions = new[] { "Loading..." };
                    LoadingParameters.Speeches = new string[] { };
                    LoadingParameters.NextSceneName = newSceneName;
                    _sceneOperation = SceneManager.LoadSceneAsync("LoadingScene", LoadSceneMode.Single);
                    break;
                case 2:
                    LoadingParameters.Captions = new[] { "Loading..." };
                    LoadingParameters.Speeches = new string[] { };
                    LoadingParameters.NextSceneName = "MenuScene";
                    _sceneOperation = SceneManager.LoadSceneAsync("LoadingScene", LoadSceneMode.Single);
                    break;
	        }
            _sceneOperation.allowSceneActivation = false;
            _newSceneLoading = true;
            _changingScene = true;
        }
    }

    void FixedUpdate()
    {
        if (_newSceneLoading)
        {
            Material blurMat = _blurMask.GetComponent<Image>().material;
            float blurSize = blurMat.GetFloat("_Size");
            Color blurColor = blurMat.GetColor("_Color");
            if (blurSize < 10) blurSize += 0.1f;
            float colorR = blurColor.r;
            float colorG = blurColor.g;
            float colorB = blurColor.b;
            if (colorR > 0) colorR -= 0.01f;
            if (colorG > 0) colorG -= 0.01f;
            if (colorB > 0) colorB -= 0.01f;
            if (colorR < 0) colorR = 0;
            if (colorG < 0) colorG = 0;
            if (colorB < 0) colorB = 0;
            blurColor = new Color(colorR, colorG, colorB);

            blurMat.SetFloat("_Size", blurSize);
            blurMat.SetColor("_Color", blurColor);

            if (colorR.Equals(colorG) && colorG.Equals(colorB) && colorB.Equals(0) && blurSize >= 10)
            {
                TimerScript.timerIsActive = true;
                _newSceneLoading = false;
                _sceneOperation.allowSceneActivation = true;
            }
        }
    }
}
