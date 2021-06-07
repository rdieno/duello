using UnityEngine;

public class ItemDropTicker : MonoBehaviour
{
    public float RangeXMin = -8;
    public float RangeXMax = 8;
    public int MaxAllowedItems = 3;
    public int CheckIntervalSeconds = 3;
    public float Possibility = 0.3f;

    private int _currentItemCount = 0;
    private int _lastGeneratedTime = -1;
    private AudioSource _player;
    private AudioClip _itemCollected;
    private AudioClip _itemLanded;

    private readonly System.Random _rand = new System.Random();

    private static readonly string[] ITEM_TYPES = { "Jump", "Regeneration", "Resistance", "Speed", "Damage" };

    void Start ()
    {
        _player = gameObject.AddComponent<AudioSource>();
        _itemCollected = Resources.Load<AudioClip>("Audios/item_collected");
        _itemLanded = Resources.Load<AudioClip>("Audios/item_landed");
    }

	void FixedUpdate ()
	{
	    int timeInSeconds = (int)Time.realtimeSinceStartup;

        if (_currentItemCount < MaxAllowedItems && timeInSeconds > _lastGeneratedTime && timeInSeconds % CheckIntervalSeconds == 0 && Random.value < Possibility)
	    {
	        _currentItemCount++;
	        _lastGeneratedTime = timeInSeconds;
            float xPos = Random.Range(RangeXMin, RangeXMax);
            GameObject itemDrop = (GameObject)Instantiate(Resources.Load("Prefabs/ItemDrops/ItemDrop"));
            itemDrop.transform.position = new Vector3(xPos, 10, 0);
	        ItemDropHandler handler = itemDrop.GetComponent<ItemDropHandler>();
            handler.SetOnLandedCallback(OnItemLanded);
            handler.SetType(ITEM_TYPES[_rand.Next(ITEM_TYPES.Length)]);
            handler.SetOnDestroyCallback(OnItemDropDestroyCallback);
            // Debug.Log("dropping item");
	    }
	}

    private void OnItemDropDestroyCallback(bool inBlastZone)
    {
        if (!inBlastZone)
            _player.PlayOneShot(_itemCollected);
        _currentItemCount--;
    }

    private void OnItemLanded()
    {
        _player.PlayOneShot(_itemLanded);
    }
}
