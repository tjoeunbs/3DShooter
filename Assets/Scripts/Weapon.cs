using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    public enum OffenseDistance { Short, Long };
    public OffenseDistance type;
    public int power;
    public float rate;

    public int maxAmmo;
    public int curAmmo;

    public BoxCollider area;
    public TrailRenderer effect;

    public Transform bulletPos;
    public GameObject bullet;
    public Transform bulletCasePos;
    public GameObject bulletCase;

    public void Use()
    {
        if (type == OffenseDistance.Short)
        {
            StopCoroutine("Swing");
            StartCoroutine("Swing");
        }
        else if (type == OffenseDistance.Long && curAmmo > 0)
        {
            curAmmo--;
            StartCoroutine("Shot");
        }
    }

    IEnumerator Shot()
    {
        GameObject instantBullet = Instantiate(bullet, bulletPos.position, bulletPos.rotation);
        Rigidbody bulletRigid = instantBullet.GetComponent<Rigidbody>();
        bulletRigid.velocity = bulletPos.forward * 50; 

        yield return null;

        GameObject instantBulletCase = Instantiate(bulletCase, bulletCasePos.position, bulletCasePos.rotation);
        Rigidbody bulletCaseRigid = instantBulletCase.GetComponent<Rigidbody>();
        Vector3 caseVec = bulletCasePos.forward * Random.Range(-3, -2) + Vector3.up * Random.Range(2, 3);
        bulletCaseRigid.AddForce(caseVec, ForceMode.Impulse);
        bulletCaseRigid.AddTorque(Vector3.up * 10, ForceMode.Impulse);
    }

    IEnumerator Swing()
    {
        yield return new WaitForSeconds(0.1f);
        area.enabled = true;
        effect.enabled = true;

        yield return new WaitForSeconds(0.3f);
        area.enabled = false;
        
        yield return new WaitForSeconds(0.3f);
        effect.enabled = false;
    }
}
