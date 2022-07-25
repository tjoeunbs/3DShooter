using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grenades : MonoBehaviour
{
    public Transform player;

    public float orbitSpeed;
    Vector3 offset;

    // Start is called before the first frame update
    void Start()
    {
        offset = transform.position - player.position;        
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = player.position + offset;
        transform.RotateAround(player.position, Vector3.up, orbitSpeed * Time.deltaTime);
        offset = transform.position - player.position;
    }
}
