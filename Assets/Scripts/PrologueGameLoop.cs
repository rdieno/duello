using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PrologueGameLoop : MonoBehaviour
{
    private readonly string[] _captions = { "Long, long ago, when we humans still lived on the Earth.", "One day, we were attacked by some aliens.", "They were so powerful, and can even throw asteroids at us.", "A hero, then, rose up." };
    private readonly string[] _speeches = { "pre_1", "pre_2", "pre_3", "pre_4" };

    private Text _caption;
    private readonly List<AudioClip> _clips = new List<AudioClip>();
    private AudioClip _spaceshipAudio;
    private AudioSource _audioSource;
    private Image _cover;

    private int _currentIndex = -1;
    private int _countDown = 100;

    private readonly List<Camera> _cameras = new List<Camera>();

    private GameObject _earth;
    private GameObject _earthCloud;
    private GameObject _ships;
    private GameObject _asteroidPos;

    private List<GameObject> _asteroidPrefabs = new List<GameObject>();

    private readonly System.Random _rand = new System.Random();

    private float _lastRecordedTime = 0;
    private int _asteroidsCount = 0;

    private bool _loadingGame = false;

    void Start ()
	{
	    _caption = GameObject.Find("Caption").GetComponent<Text>();
	    _caption.text = "";
	    _audioSource = GetComponent<AudioSource>();
	    _cover = GameObject.Find("Cover").GetComponent<Image>();

        foreach (string speech in _speeches)
	    {
	        _clips.Add(Resources.Load<AudioClip>("Audios/Scenes/" + speech));
	    }
	    _spaceshipAudio = Resources.Load<AudioClip>("Audios/Scenes/spaceship");

        _cameras.Add(GameObject.Find("Cam1").GetComponent<Camera>());
        _cameras.Add(GameObject.Find("Cam2").GetComponent<Camera>());
        _cameras.Add(GameObject.Find("Cam3").GetComponent<Camera>());

        _earth = GameObject.Find("Earth");
        _earthCloud = GameObject.Find("EarthCloud");
        _ships = GameObject.Find("Ships");
        _asteroidPos = GameObject.Find("AsteroidPos");

        _asteroidPrefabs.Add(Resources.Load<GameObject>("Prefabs/Prologue/Asteroid1"));
        _asteroidPrefabs.Add(Resources.Load<GameObject>("Prefabs/Prologue/Asteroid2"));
        _asteroidPrefabs.Add(Resources.Load<GameObject>("Prefabs/Prologue/Asteroid3"));

        _cameras[1].gameObject.SetActive(false);
        _cameras[2].gameObject.SetActive(false);
    }

    void Update()
    {
        if (Input.GetButton("Fire2") && Input.GetButton("Jump") && !_loadingGame)
        {
            _loadingGame = true;
            LoadingParameters.Captions = new[] { "In Canada, there was a school called BCIT.", "Our hero started his journey here." };
            LoadingParameters.Speeches = new[] { "zero_1", "zero_2" };
            LoadingParameters.NextSceneName = "LevelZero";
            SceneManager.LoadScene("LoadingScene");
        }

        switch (_currentIndex)
        {
            case -1:
            case 0:
                _earth.transform.Translate(new Vector3(0, 0, -0.8f) * Time.deltaTime, Space.World);
                _earthCloud.transform.Rotate(Random.value * 10f * Time.deltaTime, Random.value * 10f * Time.deltaTime, Random.value * 10f * Time.deltaTime);
                _earth.transform.Rotate(new Vector3(0, 2, 0), Random.value * -30f * Time.deltaTime, Space.World);
                break;
            case 1:
                _ships.transform.Translate(new Vector3(0, 0, -0.8f) * Time.deltaTime, Space.World);
                break;
            case 2:
                if (Time.fixedTime - _lastRecordedTime < 0.2f || _asteroidsCount > 15)
                    return;

                _lastRecordedTime = Time.fixedTime;
                _asteroidsCount++;

                Object chosenPrefab = _asteroidPrefabs[_rand.Next(3)];
                GameObject asteroid = (GameObject)Instantiate(chosenPrefab);
                asteroid.transform.localScale = new Vector3(Random.value * 3 + 0.5f, Random.value * 3 + 0.5f, Random.value * 3 + 0.5f);
                asteroid.transform.position = _asteroidPos.transform.position +
                                              new Vector3(Random.value * 50 - 25, Random.value * 50 - 25,
                                                  Random.value * 50 - 25);
                asteroid.GetComponent<Rigidbody>().velocity = new Vector3(0, -50, 50);
                asteroid.AddComponent(typeof(Asteroid));
                break;
            default:
                break;
        }
    }

    void FixedUpdate ()
	{
	    if (_countDown > 1)
	    {
            _caption.text = "";
            _countDown--;
	    }
	    else if (_countDown != -1)
	    {
	        if (_countDown == 1)
	        {
                _currentIndex++;
                if (_currentIndex >= _captions.Length)
                {
                    _caption.text = "";
                    _countDown = -1;
                    return;
                }

                if (_currentIndex == 1)
                {
                    _audioSource.PlayOneShot(_spaceshipAudio, 0.3f);
                    _cameras[0].gameObject.SetActive(false);
                    _cameras[1].gameObject.SetActive(true);
                }
                else if (_currentIndex == 2)
	            {
                    _cameras[1].gameObject.SetActive(false);
                    _cameras[2].gameObject.SetActive(true);
                }

	            _countDown = 0;
	            _caption.text = _captions[_currentIndex];
	            _audioSource.PlayOneShot(_clips[_currentIndex]);
	        }
	        else if (_countDown == 0 && !_audioSource.isPlaying)
	        {
	            _countDown = 70;
	        }
	    }
	    else
	    {
	        if (_countDown > 0)
	            _countDown--;
	        else
	        {
	            float alpha = _cover.color.a;
	            alpha += 0.1f;
	            if (alpha > 1)
	                alpha = 1;
                _cover.color = new Color(_cover.color.r, _cover.color.g, _cover.color.b, alpha);
	            if (alpha >= 1)
	            {
                    _loadingGame = true;
                    LoadingParameters.Captions = new[] { "In Canada, there was a school called BCIT.", "Our hero started his journey here." };
	                LoadingParameters.Speeches = new[] { "zero_1", "zero_2" };
	                LoadingParameters.NextSceneName = "LevelZero";
                    SceneManager.LoadScene("LoadingScene");
	            }
	        }
	    }
	}

    private class Asteroid : MonoBehaviour
    {
        void OnCollisionEnter(Collision collision)
        {
            GameObject.Find("GameLoop").GetComponent<AudioSource>().PlayOneShot(Resources.Load<AudioClip>("Audios/Scenes/asteroid_explosion"), 0.3f);
            GameObject dust = Instantiate(Resources.Load<GameObject>("Prefabs/Dust"));
            dust.transform.position = transform.position;
            Destroy(gameObject);
        }
    }
}
