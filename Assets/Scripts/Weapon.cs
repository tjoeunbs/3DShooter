using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    public enum OffenseDistance { Short, Long };
    public OffenseDistance type;
    public int power;
    public float rate;
    public BoxCollider area;
    public TrailRenderer effect;

    public void Use()
    {
        if (type == OffenseDistance.Short)
        {
            StopCoroutine("Swing");
            StartCoroutine("Swing");
        }
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
