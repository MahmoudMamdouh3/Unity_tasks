using UnityEngine;

public class RiderTest : MonoBehaviour
{
    [SerializeField] private float rotationSpeed = 50f;

    void Start()
    {
        // If this appears in the Unity Console, the connection is working!
        Debug.Log("<color=green>Rider is successfully connected to Unity!</color>");
    }

    void Update()
    {
        // This will rotate the object so you can see the script is active
        transform.Rotate(Vector3.up * (rotationSpeed * Time.deltaTime));
    }
}