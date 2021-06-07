using UnityEngine;
using UnityEngine.UI;

public class HealthPointDisplayMultiplayer : MonoBehaviour
{
    private const float MAX_HEALTH = 100;

    private DamageHandler _player;
    private Damage2Handler _player2;

    private GameObject _playerHealthValue;
    private Text _playerHealthNumber;
    private Text _playerHealthMax;

    private GameObject _player2HealthValue;
    private Text _player2HealthNumber;
    private Text _player2HealthMax;

    private int _currentPlayerHealth = (int)MAX_HEALTH;
    private int _currentPlayer2Health = (int)MAX_HEALTH;
    private int _targetPlayerHealth = (int)MAX_HEALTH;
    private int _targetPlayer2Health = (int)MAX_HEALTH;

    void Start ()
	{
        _player = GameObject.Find("Player").GetComponent<DamageHandler>();
        _player2 = GameObject.Find("Player2").GetComponent<Damage2Handler>();

        _playerHealthValue = GameObject.Find("PlayerHealthValue");
        _playerHealthNumber = GameObject.Find("PlayerHealthNumber").GetComponent<Text>();
        _playerHealthMax = GameObject.Find("PlayerHealthMax").GetComponent<Text>();

        _player2HealthValue = GameObject.Find("Player2HealthValue");
        _player2HealthNumber = GameObject.Find("Player2HealthNumber").GetComponent<Text>();
        _player2HealthMax = GameObject.Find("Player2HealthMax").GetComponent<Text>();

        _playerHealthMax.text = "/" + MAX_HEALTH;
        _player2HealthMax.text = "/" + MAX_HEALTH;
    }

    void Update ()
	{
        _targetPlayerHealth = _player.getHealth();
        _targetPlayer2Health = _player2.getHealth();

        if (_currentPlayerHealth < _targetPlayerHealth) _currentPlayerHealth++;
        if (_currentPlayerHealth > _targetPlayerHealth) _currentPlayerHealth--;
        if (_currentPlayer2Health < _targetPlayer2Health) _currentPlayer2Health++;
        if (_currentPlayer2Health > _targetPlayer2Health) _currentPlayer2Health--;

        float playerHealthFloat = _currentPlayerHealth / MAX_HEALTH;
	    playerHealthFloat = playerHealthFloat + (1 - playerHealthFloat) * 0.05f; // magic number!

        float player2HealthFloat = _currentPlayer2Health / MAX_HEALTH;
        player2HealthFloat = player2HealthFloat + (1 - player2HealthFloat) * 0.05f;

        _playerHealthNumber.text = _targetPlayerHealth.ToString("N0");
        _player2HealthNumber.text = _targetPlayer2Health.ToString("N0");

        _playerHealthValue.GetComponent<Image>().material.SetFloat("_Value", playerHealthFloat);
        _player2HealthValue.GetComponent<Image>().material.SetFloat("_Value", player2HealthFloat);
    }
}
