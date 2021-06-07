using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemDropHandler : MonoBehaviour
{
    private GameObject _player;
    private GameObject _player2;
    private GameObject _enemy;
    private GameObject _blastZone;
    private int _angle = 0;
    private int _rotateDirection = 1;
    private string _type;

    private Action<bool> _onDestroyCallback;
    private Action _onLanded;
    private Action<GameObject> _onCollisionCallback;

    public const int REGENERATION_AMOUNT = 20;
    public const float JUMP_BONUS_FACTOR = 0.3f;
    public const int JUMP_BONUS_LAST_SECONDS = 5;
    public const float RESISTANCE_FACTOR = 0.5f;
    public const int RESISTANCE_LAST_SECONDS = 5;
    public const float SPEED_BUFF_FACTOR = 0.5f;
    public const int SPEED_BUFF_LAST_SECONDS = 10;
    public const float ATTACK_BUFF_FACTOR = 0.3f;
    public const int ATTACK_BUFF_LAST_SECONDS = 5;

    void Start ()
    {
        _player = GameObject.Find("Player");
        _player2 = GameObject.Find("Player2");
        _enemy = GameObject.Find("Enemy");
        _blastZone = GameObject.Find("BlastZone");
        
        gameObject.layer = 31;
        Physics2D.IgnoreLayerCollision(gameObject.layer, gameObject.layer);

        Invoke("DestroyNoCollision", 5f);
    }

    void Update ()
    {
		transform.Rotate(new Vector3(0, 1, 0), _rotateDirection);
        _angle += _rotateDirection;
        if (_angle == 20 || _angle == -20)
            _rotateDirection *= -1;
    }

    public void SetType(string materialName)
    {
        _type = materialName;
        Material newMat = Resources.Load<Material>("Materials/ItemDrops/" + materialName);
        GetComponent<Renderer>().material = newMat;
        
        switch (materialName)
        {
            case "Jump":
                _onCollisionCallback = o =>
                {
                    // Debug.Log(o.name + " gets the " + _type + " buff!");
                    o.SendMessage("SetJumpBuff", new object[] { JUMP_BONUS_FACTOR, Time.realtimeSinceStartup + JUMP_BONUS_LAST_SECONDS });
                    AddEffect(o, JUMP_BONUS_LAST_SECONDS, 0, new Color(0, 0, 255));
                };
                break;
            case "Regeneration":
                _onCollisionCallback = o =>
                {
                    // Debug.Log(o.name + " gets the " + _type + " buff!");
                    o.SendMessage("RestoreHealth", REGENERATION_AMOUNT);
                };
                break;
            case "Resistance":
                _onCollisionCallback = o =>
                {
                    // Debug.Log(o.name + " gets the " + _type + " buff!");
                    o.SendMessage("SetResistanceBuff", new object[] { RESISTANCE_FACTOR, Time.realtimeSinceStartup + RESISTANCE_LAST_SECONDS });
                    AddEffect(o, RESISTANCE_LAST_SECONDS, 1, new Color(255, 255, 0));
                };
                break;
            case "Speed":
                _onCollisionCallback = o =>
                {
                    // Debug.Log(o.name + " gets the " + _type + " buff!");
                    o.SendMessage("SetSpeedBuff", new object[] { SPEED_BUFF_FACTOR, Time.realtimeSinceStartup + SPEED_BUFF_LAST_SECONDS });
                    AddEffect(o, SPEED_BUFF_LAST_SECONDS, 2, new Color(0, 255, 0));
                };
                break;
            case "Damage":
                _onCollisionCallback = o =>
                {
                    // Debug.Log(o.name + " gets the " + _type + " buff!");
                    o.SendMessage("SetAttackBuff", new object[] { ATTACK_BUFF_FACTOR, Time.realtimeSinceStartup + ATTACK_BUFF_LAST_SECONDS });
                    AddEffect(o, ATTACK_BUFF_LAST_SECONDS, 3, new Color(255, 0, 0));
                };
                break;
            default:
                break;
        }
    }

    private void AddEffect(GameObject owner, float duration, int position, Color color)
    {
        GameObject effectParticle = (GameObject)Instantiate(Resources.Load("Prefabs/ItemDrops/EffectParticle"));
        ParticleSystem ps = effectParticle.GetComponent<ParticleSystem>();
        ps.Stop();
        effectParticle.AddComponent<EffectTicker>();
        effectParticle.SendMessage("SetOwner", owner);
        effectParticle.SendMessage("SetPosition", position);
        ParticleSystem.ColorOverLifetimeModule col = ps.colorOverLifetime;
        Gradient gradientColor = new Gradient();
        gradientColor.SetKeys(new []{ new GradientColorKey(color, 0), new GradientColorKey(new Color(color.r, color.g, color.b, 0), 1) }, new []{ new GradientAlphaKey(1, 0), new GradientAlphaKey(0, 1) });
        col.color = gradientColor;
        ParticleSystem.MainModule main = ps.main;
        main.duration = duration;
        ps.Play();
    }

    public void SetOnDestroyCallback(Action<bool> action)
    {
        _onDestroyCallback = action;
    }

    public void SetOnLandedCallback(Action action)
    {
        _onLanded = action;
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        GameObject targetObject = collision.gameObject;

        if (targetObject != _player && targetObject != _player2 && targetObject != _enemy && targetObject != _blastZone)
        {
            // should be landed on ground
            CancelInvoke("DestroyNoCollision");
            _onLanded();
            return;
        }

        Destroy(gameObject);
        _onDestroyCallback(targetObject == _blastZone);

        if (targetObject == _player || targetObject == _player2 || targetObject == _enemy)
        {
            CancelInvoke("DestroyNoCollision");
            _onCollisionCallback(collision.gameObject);
        }
    }

    private void DestroyNoCollision()
    {
        Destroy(gameObject);
        _onDestroyCallback(true);
    }
}
