using UnityEngine;

public class FollowSphere : MonoBehaviour
{
    private float startPosY;
    private float startPosZ;
    public GameObject sphere;

    // Start is called before the first frame update
    void Start()
    {
        startPosY = transform.position.y;
        startPosZ = transform.position.z;
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 spherePos = sphere.transform.position;
        spherePos.y = startPosY;
        spherePos.z = startPosZ;
        transform.position = spherePos;
    }
}
