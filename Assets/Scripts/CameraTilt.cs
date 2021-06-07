using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraTilt : MonoBehaviour
{
    public enum CameraTiltMode
    {
        LookAt,
        Follow
    }

    public CameraTiltMode TiltMode = CameraTiltMode.LookAt;
    public float HorizontalMin = 345;
    public float HorizontalMax = 350;
    public float VerticalMin = 20;
    public float VerticalMax = 30;

    private GameObject _player;

	void Start ()
	{
        _player = GameObject.Find("Player");
    }

	void Update ()
	{
	    if (TiltMode == CameraTiltMode.LookAt)
	    {
            Vector3 distPlayer = _player.transform.position - transform.position;
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(distPlayer), Time.deltaTime);
            float angleX = transform.eulerAngles.x;
            float angleY = transform.eulerAngles.y;

            if (angleX > VerticalMax)
            {
                angleX = VerticalMax;
            }
            else if (angleX < VerticalMin)
            {
                angleX = VerticalMin;
            }

            if (angleY > HorizontalMax)
            {
                angleY = HorizontalMax;
            }
            else if (angleY < HorizontalMin)
            {
                angleY = HorizontalMin;
            }

            transform.eulerAngles = new Vector3(angleX, angleY, 0);
        }
        else if (TiltMode == CameraTiltMode.Follow)
	    {
	        transform.position = new Vector3(_player.transform.position.x, transform.position.y, transform.position.z);
	    }
    }
}
