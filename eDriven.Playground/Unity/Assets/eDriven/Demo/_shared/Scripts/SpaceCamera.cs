using UnityEngine;

public class SpaceCamera : MonoBehaviour
{
    public float Intensity = 15f;
    public float Speed = 20f;

    private Quaternion _startRotation;
    private Quaternion _endRotation;
    private float _lerp;

    private float _actualSpeed;

// ReSharper disable UnusedMember.Local
    void Start()
// ReSharper restore UnusedMember.Local
    {
        _startRotation = transform.rotation;
        GenerateEndPoint();
    }

// ReSharper disable UnusedMember.Local
    void Update()
// ReSharper restore UnusedMember.Local
    {
        _lerp += _actualSpeed * Time.deltaTime;
        var lerp = (Mathf.Cos(_lerp * Mathf.Deg2Rad) + 1f) / 2f;
        transform.rotation = Quaternion.Lerp(_endRotation, _startRotation, lerp);

        if (_lerp >= 360f)
        {
            _lerp -= 360f;
            GenerateEndPoint();
        }
    }

    void GenerateEndPoint()
    {
        var x = Random.Range(-Intensity, Intensity);
        var y = Random.Range(-Intensity, Intensity);
        var z = Random.Range(-Intensity, Intensity);

        _endRotation = _startRotation * Quaternion.Euler(new Vector3(x, y , z));
        _actualSpeed = Speed * Mathf.Max(x, y, z) / Intensity;
    }
}