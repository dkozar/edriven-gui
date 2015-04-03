using UnityEngine;
using System.Collections;

public class Planet : MonoBehaviour 
{
    private float SpeedScalar = 48;
	private float CloudsSpeed = 0.002f;
    //private float _rotation;
    private float _rotationSpeed;
	private float _cloudsOffset;

    void Start()
    {
        _rotationSpeed = 360.0f / 24.0f / 60.0f / 60.0f;//360 deg / 24h / 60 min / 60 sec
    }

	void Update () 
    {
        transform.rotation = Quaternion.AngleAxis(-_rotationSpeed * SpeedScalar * Time.deltaTime, transform.up) * transform.rotation;
		renderer.material.SetTextureOffset ("_CloudsTex", new Vector2(_cloudsOffset,0));
		_cloudsOffset += CloudsSpeed * Time.deltaTime;
		if(_cloudsOffset >= 1f)
			_cloudsOffset -= 1f;
	}
}
