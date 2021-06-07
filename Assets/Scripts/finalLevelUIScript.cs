using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
 
public class finalLevelUIScript : MonoBehaviour
{
    private GameObject _scorePanel;
    private GameObject _blurMask;
    private GameObject _btnRestart;
    private GameObject _btnMainMenu;
    private GameObject _selectionIndicator;
    private GameObject _enemy;

    private int _currentSelection = 0;
    private bool _isChangingIndex = false;
    private float _changeStartTime = 0;

    private bool _changingScene = false;
    private bool _newSceneLoading = false;

    private AsyncOperation _sceneOperation;

    void Start()
    {
        _scorePanel = GameObject.Find("ScorePanel");
        _blurMask = GameObject.Find("BlurMask");
        _btnRestart = GameObject.Find("ButtonRestart");
        _btnMainMenu = GameObject.Find("ButtonMainMenu");
        _selectionIndicator = GameObject.Find("ScorePanel/ScoreButtonIndicator");
        _enemy = GameObject.Find("Enemy");

        switch (_currentSelection)
        {
            case 0:
                _selectionIndicator.transform.position = _btnRestart.transform.position + new Vector3(0, 5, 0);
                break;
            case 1:
                _selectionIndicator.transform.position = _btnMainMenu.transform.position + new Vector3(0, 5, 0);
                break;
        }

        _scorePanel.SetActive(false);

        Material blurMat = _blurMask.GetComponent<Image>().material;
        blurMat.SetFloat("_Size", 0);
        blurMat.SetColor("_Color", Color.white);
    }

    void Update()
    {
        if (!_scorePanel.activeSelf)
            return;

        float blurValue = _scorePanel.GetComponent<Image>().material.GetFloat("_Size");
        if (blurValue < 10)
            blurValue += Time.deltaTime * 8;
        _scorePanel.GetComponent<Image>().material.SetFloat("_Size", blurValue);
        if (blurValue < 10)
            return;

        // disable enemy
        // todo: disable player too
        EnemyFighterScript enemyAI = _enemy.GetComponent<EnemyFighterScript>();
        if (enemyAI.enabled)
            enemyAI.enabled = false;
        EnemyController enemyAnimation = _enemy.GetComponent<EnemyController>();
        if (enemyAnimation != null && enemyAnimation.enabled)
            enemyAnimation.enabled = false;
        EnemyScript enemyScript = _enemy.GetComponent<EnemyScript>();
        if (enemyScript != null && enemyScript.enabled)
            enemyScript.enabled = false;
        if (_enemy.activeSelf)
            _enemy.SetActive(false);

        float moveVertical = Input.GetAxis("Vertical");

        int timeAfterSelecting = Mathf.RoundToInt((Time.fixedTime - _changeStartTime) * 1000);

        if (!_isChangingIndex && !_changingScene)
        {
            if (moveVertical > 0)
            {
                _currentSelection = (_currentSelection + 2 - 1) % 2;

                switch (_currentSelection)
                {
                    case 0:
                        _selectionIndicator.transform.position = _btnRestart.transform.position + new Vector3(0, 5, 0);
                        break;
                    case 1:
                        _selectionIndicator.transform.position = _btnMainMenu.transform.position + new Vector3(0, 5, 0);
                        break;
                }

                _isChangingIndex = true;
                _changeStartTime = Time.fixedTime;
            }
            else if (moveVertical < 0)
            {
                _currentSelection = (_currentSelection + 2 + 1) % 2;

                switch (_currentSelection)
                {
                    case 0:
                        _selectionIndicator.transform.position = _btnRestart.transform.position + new Vector3(0, 5, 0);
                        break;
                    case 1:
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
            switch (_currentSelection)
            {
                case 0:
                    LoadingParameters.Captions = new[] { "This young man saved the Earth.", "Then he moved to our planet,", "and became our ancestor." };
                    LoadingParameters.Speeches = new[] { "end_1", "end_2", "end_3" };
                    LoadingParameters.NextSceneName = "MenuScene";
                    _sceneOperation = SceneManager.LoadSceneAsync("LoadingScene", LoadSceneMode.Single);
                    break;
                case 1:
                    _sceneOperation = SceneManager.LoadSceneAsync("MenuScene", LoadSceneMode.Single);
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
