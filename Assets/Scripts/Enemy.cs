using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour
{
    public int maxHealth;
    public int curHealth;

    Rigidbody rigid;
    public BoxCollider boxCollier;
    Material mat;

    public Transform target;
    
    NavMeshAgent agent;

    Animator anim;

    public bool isChase;

    public bool isAttack;

    public enum Type { A, B, C };

    public Type enemyType;

    public GameObject missile;

    private void Awake()
    {
        rigid = GetComponent<Rigidbody>();
        mat = GetComponentInChildren<MeshRenderer>().material;
        agent = GetComponent<NavMeshAgent>();

        anim = GetComponentInChildren<Animator>();

        Invoke("ChaseStart", 2.0f);
    }

    void ChaseStart()
    {
        isChase = true;
        anim.SetBool("isWalk", true);
    }

    private void FixedUpdate()
    {
        if (isChase)
        {
            rigid.velocity = Vector3.zero;
            rigid.angularVelocity = Vector3.zero;
        }
        Targeting();
    }

    private void Update()
    {
        if (agent.enabled)
        {
            agent.SetDestination(target.position);
            agent.isStopped = !isChase;
        }            
    }

    void Targeting()
    {
        float targetRadius = 0;
        float targetRange = 0;

        switch(enemyType)
        {
            case Type.A:
                targetRadius = 1.5f;
                targetRange = 3.0f;
                break;
            case Type.B:
                targetRadius = 1f;
                targetRange = 12f;
                break;
            case Type.C:
                targetRadius = 0.5f;
                targetRange = 25f;
                break;
        }

        RaycastHit[] rayHits =
                Physics.SphereCastAll(transform.position,
                                        targetRadius,
                                        transform.forward,
                                        targetRange,
                                        LayerMask.GetMask("Player"));
        if (rayHits.Length > 0 && !isAttack)
        {
            StartCoroutine(Attack());
        }
    }

    IEnumerator Attack()
    {
        isChase = false;
        isAttack = true;
        anim.SetBool("isAttack", true);

        switch(enemyType)
        {
            case Type.A:
                yield return new WaitForSeconds(0.2f);
                boxCollier.enabled = true;
                yield return new WaitForSeconds(1f);
                boxCollier.enabled = false;
                yield return new WaitForSeconds(1f);
                break;
            case Type.B:
                yield return new WaitForSeconds(0.1f);
                boxCollier.enabled = true;
                rigid.AddForce(transform.forward * 20, ForceMode.Impulse);
                yield return new WaitForSeconds(0.5f);
                boxCollier.enabled = false;
                rigid.velocity = Vector3.zero;
                yield return new WaitForSeconds(2f);
                break;
            case Type.C:
                yield return new WaitForSeconds(0.5f);
                GameObject instantMissile = Instantiate(missile, transform.position, transform.rotation);
                Rigidbody rigidMissile = instantMissile.GetComponent<Rigidbody>();
                rigidMissile.velocity = transform.forward * 20;
                yield return new WaitForSeconds(2f);
                break;
        }

        isChase = true;
        isAttack = false;
        anim.SetBool("isAttack", false);
    }

    public void HitByGrenade(Vector3 explosionPos, int damage)
    {
        curHealth -= damage;
        Vector3 reactVec = transform.position - explosionPos;
        StartCoroutine(OnDamage(reactVec, true));
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Bullet")
        {
            Bullet bullet = other.GetComponent<Bullet>();

            curHealth -= bullet.damage;

            Vector3 reactVec = transform.position - other.transform.position;

            StartCoroutine(OnDamage(reactVec, false));

            Destroy(other.gameObject);
        }

        if (other.tag == "OffenseShort")
        {
            Weapon weapon = other.GetComponent<Weapon>();
            curHealth -= weapon.power;

            Vector3 reactVec = transform.position - other.transform.position;

            StartCoroutine(OnDamage(reactVec, false));
        }
    }

    IEnumerator OnDamage(Vector3 reactVec, bool isGrenade)
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

            anim.SetTrigger("doDie");
            isChase = false;
            agent.enabled = false;

            if (isGrenade)
            {
                reactVec = reactVec.normalized;
                reactVec += Vector3.up * 3;
                rigid.freezeRotation = false;

                rigid.AddForce(reactVec * 5, ForceMode.Impulse);
                rigid.AddTorque(reactVec * 15, ForceMode.Impulse);
            }
            else
            {
                reactVec = reactVec.normalized;
                reactVec += Vector3.up;
                rigid.AddForce(reactVec * 5, ForceMode.Impulse);
            }

            Destroy(gameObject, 4.0f);
        }
    }
}
