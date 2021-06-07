using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameLogic : MonoBehaviour
{
    public List<string> MenuItemScenes = new List<string> { "Prologue", "MultiLevelZero", "HighScoreScene", "Credits" };
    public bool HighScoreScene = false;
    private AsyncOperation _sceneOperation;
    private bool _isNewSceneLoading = false;

    public Texture2D CursorTexture;

    public AudioClip MenuSelection;
    public AudioClip SceneChanging;
    private AudioSource _audioSource;

    private GameObject _menu;
    private GameObject _menuSelector;
    private GameObject _menuItemHolder;
    private GameObject _logo;
    private GameObject _blur;

    private float _cameraAngleZ = 0;
    private float _cameraAngleZGrow = 0.01f;
    private float _logoAngleY = 0;
    private float _logoAngleYGrow = 0.2f;

    private bool _menuLoaded = false;

    private int _selectedMenuItemIndex = 0;
    private bool _isChangingIndex = false;
    private float _changeStartTime = 0;

    private const int MENU_SELECTOR_OFFSET_X = 25;
    private const int MENU_SELECTOR_OFFSET_Y = 5;

    private const int CAMERA_ANGLE_Z_MAX = 1;
    private const int LOGO_ANGLE_Y_MAX = 10;

    void Start ()
	{
		_menu = GameObject.Find("MenuCanvas");
        _menuItemHolder = GameObject.Find("MenuSelection");
        _menuSelector = GameObject.Find("MenuSelector");
        _logo = GameObject.Find("Logo");
        _blur = GameObject.Find("Blur");
        _audioSource = GetComponent<AudioSource>();

        Material blurMat = _blur.GetComponent<Image>().material;
        blurMat.SetFloat("_Size", 10);
        blurMat.SetColor("_Color", new Color(128, 0, 0));

        Cursor.SetCursor(CursorTexture, Vector2.zero, CursorMode.Auto);
        PlayerPrefs.SetInt("currentScoreDuello", 0);
        PlayerPrefs.SetInt("p1WinsDuello", 0);
        PlayerPrefs.SetInt("p2WinsDuello", 0);
    }

    void Update ()
	{
	    float moveHorizontal = Input.GetAxis("Horizontal");
        if (moveHorizontal == 0) {
            moveHorizontal = Input.GetAxis("Horizontal2");
        }
	    float moveVertical = Input.GetAxis("Vertical");
        if (moveVertical == 0) {
            moveVertical = Input.GetAxis("Vertical2");
        }
        int menuItemCount = _menuItemHolder.transform.childCount;

        Camera.main.transform.eulerAngles = new Vector3(moveVertical * 5, moveHorizontal * -5, _cameraAngleZ);
        _logo.transform.eulerAngles = new Vector3(0, _logoAngleY, 0);
        _menu.transform.eulerAngles = new Vector3(moveVertical * 7, moveHorizontal * -7, 0);

        // disable menu item selection during scene loadings
        if (_isNewSceneLoading || !_menuLoaded)
            return;

        Vector3 mousePositionOrigin = Input.mousePosition;
        mousePositionOrigin.z = 210;
        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(mousePositionOrigin);
        float menuSelectorWidth = _menuSelector.GetComponent<RectTransform>().sizeDelta.x;

        // mouse navigation
        for (int i = 0; i < _menuItemHolder.transform.childCount; i++)
        {
            RectTransform item = (RectTransform)_menuItemHolder.transform.GetChild(i);
            float halfWidth = menuSelectorWidth / 2;
            float halfHeight = item.sizeDelta.y / 2;
            if (mousePosition.x > item.position.x - halfWidth && mousePosition.x < item.position.x + halfWidth &&
                mousePosition.y > item.position.y - halfHeight + MENU_SELECTOR_OFFSET_Y && mousePosition.y < item.position.y + halfHeight + MENU_SELECTOR_OFFSET_Y)
            {
                if (!HighScoreScene && i != _selectedMenuItemIndex)
                {
                    _audioSource.PlayOneShot(MenuSelection);
                    _selectedMenuItemIndex = i;
                }
                else if (Input.GetMouseButtonDown(0))
                {
                    _audioSource.PlayOneShot(SceneChanging);
                    if (MenuItemScenes[_selectedMenuItemIndex].Equals("Prologue") && Application.platform == RuntimePlatform.PS4)
                        LoadingParameters.Captions = new[] { "Loading...", "Still loading...",  "...","We are trying very hard to load it.",
                            "But it takes time...", "Time is money,", "So it takes money to load.", "We can see it takes long...", "But you know...", "It's PS4.", "Should be up soon...", "Enjoy." };
                    else
                        LoadingParameters.Captions = new[] { "Loading..." };
                    if (_selectedMenuItemIndex == 1)
                        TimerScriptMultiplayer.timerIsActive = true;
                    LoadingParameters.Speeches = new string[] {};
                    LoadingParameters.NextSceneName = MenuItemScenes[_selectedMenuItemIndex];
                    _sceneOperation = SceneManager.LoadSceneAsync("LoadingScene");
                    _sceneOperation.allowSceneActivation = false;
                    _isNewSceneLoading = true;
                }
            }
        }

        int timeAfterSelecting = Mathf.RoundToInt((Time.fixedTime - _changeStartTime) * 1000);
        
        if (!HighScoreScene && !_isChangingIndex)
	    {
	        if (moveVertical > 0)
            {
                _audioSource.PlayOneShot(MenuSelection);
                _selectedMenuItemIndex = (_selectedMenuItemIndex + menuItemCount - 1) % menuItemCount;
	            _isChangingIndex = true;
	            _changeStartTime = Time.fixedTime;
	        }
	        else if (moveVertical < 0)
            {
                _audioSource.PlayOneShot(MenuSelection);
                _selectedMenuItemIndex = (_selectedMenuItemIndex + menuItemCount + 1) % menuItemCount;
	            _isChangingIndex = true;
                _changeStartTime = Time.fixedTime;
            }
	    }
	    else if (moveVertical.Equals(0) || (timeAfterSelecting != 0 && timeAfterSelecting % 1000 == 0))
	    {
	        _isChangingIndex = false;
	    }

        if (!HighScoreScene && _selectedMenuItemIndex >= 0 && _selectedMenuItemIndex < menuItemCount)
	    {
	        Transform menuItemTransform = _menuItemHolder.transform.GetChild(_selectedMenuItemIndex);
	        Vector3 menuItemPosition = menuItemTransform.localPosition + menuItemTransform.parent.localPosition;
            _menuSelector.transform.localPosition = menuItemPosition + new Vector3(MENU_SELECTOR_OFFSET_X, MENU_SELECTOR_OFFSET_Y, 0);
	    }

	    if (Input.GetButtonDown("Submit") && (_selectedMenuItemIndex >= 0 && _selectedMenuItemIndex < MenuItemScenes.Count))
	    {
            _audioSource.PlayOneShot(SceneChanging);
            if (MenuItemScenes[_selectedMenuItemIndex].Equals("Prologue") && Application.platform == RuntimePlatform.PS4)
                LoadingParameters.Captions = new[] { "Loading...", "Still loading...",  "...","We are trying very hard to load it.",
                        "But it takes time...", "Time is money,", "So it takes money to load.", "We can see it takes long...", "But you know...", "It's PS4.", "Should be up soon...", "Enjoy." };
            else
                LoadingParameters.Captions = new[] { "Loading..." };
            if (_selectedMenuItemIndex == 1)
                TimerScriptMultiplayer.timerIsActive = true;
            LoadingParameters.Speeches = new string[] { };
            LoadingParameters.NextSceneName = MenuItemScenes[_selectedMenuItemIndex];
            _sceneOperation = SceneManager.LoadSceneAsync("LoadingScene");
            _sceneOperation.allowSceneActivation = false;
	        _isNewSceneLoading = true;
	    }
	}

    void FixedUpdate()
    {
        _cameraAngleZ += _cameraAngleZGrow;
        if (_cameraAngleZ >= CAMERA_ANGLE_Z_MAX || _cameraAngleZ <= -CAMERA_ANGLE_Z_MAX)
            _cameraAngleZGrow *= -1;

        _logoAngleY += _logoAngleYGrow;
        if (_logoAngleY >= LOGO_ANGLE_Y_MAX || _logoAngleY <= -LOGO_ANGLE_Y_MAX)
            _logoAngleYGrow *= -1;

        if (_isNewSceneLoading)
        {
            Material blurMat = _blur.GetComponent<Image>().material;
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
                _sceneOperation.allowSceneActivation = true;
        }

        if (!_menuLoaded)
        {
            Material blurMat = _blur.GetComponent<Image>().material;
            float blurSize = blurMat.GetFloat("_Size");
            Color blurColor = blurMat.GetColor("_Color");
            if (blurSize > 0) blurSize -= 8 * Time.deltaTime;
            if (blurSize < 0) blurSize = 0;
            float colorR = blurColor.r;
            float colorG = blurColor.g;
            float colorB = blurColor.b;
            if (colorR < 1) colorR += 0.8f * Time.deltaTime;
            if (colorG < 1) colorG += 0.8f * Time.deltaTime;
            if (colorB < 1) colorB += 0.8f * Time.deltaTime;
            if (colorR > 1) colorR = 1;
            if (colorG > 1) colorG = 1;
            if (colorB > 1) colorB = 1;
            blurColor = new Color(colorR, colorG, colorB);


            if (!SplashScreen.isFinished)
            {
                blurMat.SetFloat("_Size", 15);
                blurMat.SetColor("_Color", Color.red);
                return;
            }
            
            blurMat.SetFloat("_Size", blurSize);
            blurMat.SetColor("_Color", blurColor);

            if (colorR.Equals(colorG) && colorG.Equals(colorB) && colorB.Equals(1) && blurSize.Equals(0))
                _menuLoaded = true;
        }
    }
}
