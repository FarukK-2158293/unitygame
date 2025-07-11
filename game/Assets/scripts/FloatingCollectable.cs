using UnityEngine;

public class FloatingCollectable : MonoBehaviour
{
    public float floatAmplitude = 0.5f;
    public float floatFrequency = 1.0f;
    private Vector3 startPos;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        startPos = transform.position;    
    }

    // Update is called once per frame
    void Update()
    {
        float newY = startPos.y + Mathf.Sin( Time.time * floatFrequency) * floatAmplitude;
        transform.position = new Vector3(startPos.x, newY, startPos.z);
    }
}
