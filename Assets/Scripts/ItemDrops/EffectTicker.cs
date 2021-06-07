using UnityEngine;

public class EffectTicker : MonoBehaviour
{
    private GameObject _owner;
    private BoxCollider2D _box;
    private float _tenPercentHeight;
    private int _position;

    void Start()
    {
        Destroy(gameObject, GetComponent<ParticleSystem>().main.duration);
    }

    void SetOwner(GameObject go)
    {
        _owner = go;
        _box = _owner.GetComponent<BoxCollider2D>();
        _tenPercentHeight = _box.size.y * 0.1f;
    }

    void SetPosition(int position)
    {
        _position = position;
    }

    void Update()
    {
        if (_owner != null)
        {
            transform.position = _box.transform.position + new Vector3(0, _tenPercentHeight + _tenPercentHeight * _position, 0);
        }
    }
}
