using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour
{
    public int maxHealth;
    public int curHealth;

    Rigidbody rigid;
    BoxCollider boxCollier;
    Material mat;

    public Transform target;
    
    NavMeshAgent agent;

    private void Awake()
    {
        rigid = GetComponent<Rigidbody>();
        boxCollier = GetComponent<BoxCollider>();
        mat = GetComponent<MeshRenderer>().material;
        agent = GetComponent<NavMeshAgent>();
    }

    private void Update()
    {
        agent.SetDestination(transform.position + target.position * 10);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Bullet")
        {
            Bullet bullet = other.GetComponent<Bullet>();

            curHealth -= bullet.damage;

            StartCoroutine(OnDamage());

            Destroy(other.gameObject);
        }

        if (other.tag == "OffenseShort")
        {
            Weapon weapon = other.GetComponent<Weapon>();
            curHealth -= weapon.power;

            StartCoroutine(OnDamage());
        }
    }

    IEnumerator OnDamage()
    {
        mat.color = Color.red;

        yield return new WaitForSeconds(0.1f);

        if (curHealth > 0)
        {
            mat.color = Color.white;
        }
        else
        {
            mat.color = Color.grey;

            Destroy(gameObject, 3.0f);
        }
    }
}
