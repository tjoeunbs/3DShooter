using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public int damage;

    public bool isShort;

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Floor")
        {
            Destroy(gameObject, 3.0f);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!isShort && other.gameObject.tag == "Wall")
        {
            Destroy(gameObject);
        }
    }
}
