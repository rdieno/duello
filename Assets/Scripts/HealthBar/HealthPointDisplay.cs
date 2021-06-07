using UnityEngine;
using UnityEngine.UI;

public class HealthPointDisplay : MonoBehaviour
{
    private const float MAX_HEALTH = 100;

    private DamageHandler _player;
    private EnemyDamageHandler _enemy;

    private GameObject _playerHealthValue;
    private Text _playerHealthNumber;
    private Text _playerHealthMax;

    private GameObject _enemyHealthValue;
    private Text _enemyHealthNumber;
    private Text _enemyHealthMax;

    private int _currentPlayerHealth = (int)MAX_HEALTH;
    private int _currentEnemyHealth = (int)MAX_HEALTH;
    private int _targetPlayerHealth = (int)MAX_HEALTH;
    private int _targetEnemyHealth = (int)MAX_HEALTH;

    void Start ()
	{
        _player = GameObject.Find("Player").GetComponent<DamageHandler>();
        _enemy = GameObject.Find("Enemy").GetComponent<EnemyDamageHandler>();

        _playerHealthValue = GameObject.Find("PlayerHealthValue");
        _playerHealthNumber = GameObject.Find("PlayerHealthNumber").GetComponent<Text>();
        _playerHealthMax = GameObject.Find("PlayerHealthMax").GetComponent<Text>();

        _enemyHealthValue = GameObject.Find("EnemyHealthValue");
        _enemyHealthNumber = GameObject.Find("EnemyHealthNumber").GetComponent<Text>();
        _enemyHealthMax = GameObject.Find("EnemyHealthMax").GetComponent<Text>();

        _playerHealthMax.text = "/" + MAX_HEALTH;
        _enemyHealthMax.text = "/" + MAX_HEALTH;
    }

    void Update ()
	{
        _targetPlayerHealth = _player.getHealth();
        _targetEnemyHealth = _enemy.getHealth();

        if (_currentPlayerHealth < _targetPlayerHealth) _currentPlayerHealth++;
        if (_currentPlayerHealth > _targetPlayerHealth) _currentPlayerHealth--;
        if (_currentEnemyHealth < _targetEnemyHealth) _currentEnemyHealth++;
        if (_currentEnemyHealth > _targetEnemyHealth) _currentEnemyHealth--;

        float playerHealthFloat = _currentPlayerHealth / MAX_HEALTH;
	    playerHealthFloat = playerHealthFloat + (1 - playerHealthFloat) * 0.05f; // magic number!

        float enemyHealthFloat = _currentEnemyHealth / MAX_HEALTH;
        enemyHealthFloat = enemyHealthFloat + (1 - enemyHealthFloat) * 0.05f;

        _playerHealthNumber.text = _targetPlayerHealth.ToString("N0");
        _enemyHealthNumber.text = _targetEnemyHealth.ToString("N0");

        _playerHealthValue.GetComponent<Image>().material.SetFloat("_Value", playerHealthFloat);
        _enemyHealthValue.GetComponent<Image>().material.SetFloat("_Value", enemyHealthFloat);
    }
}
