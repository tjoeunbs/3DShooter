using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    float hAxis;
    float vAxis;

    public float speed;

    Vector3 moveVec;
    Vector3 dodgeVec;

    Animator anim;

    bool walkKeyDown;
    bool jumpKeyDown;

    Rigidbody rigid;

    bool isJump;
    bool isDodge;

    GameObject nearObject;

    public GameObject[] weapons;
    public bool[] hasWeapons;

    bool interactionKeyDown;

    bool attackKeyDown;
    float attackDelay;
    bool isAttackReady;

    public Weapon equipWeapon;

    private void Awake()
    {
        anim = GetComponentInChildren<Animator>();
        rigid = GetComponent<Rigidbody>();
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    float forceGravity = 15;
    private void FixedUpdate()
    {
        rigid.AddForce(Vector3.down * forceGravity);
    }

    // Update is called once per frame
    void Update()
    {
        hAxis = Input.GetAxisRaw("Horizontal");
        vAxis = Input.GetAxisRaw("Vertical");
        walkKeyDown = Input.GetButton("Walk");
        jumpKeyDown = Input.GetButtonDown("Jump");
        interactionKeyDown = Input.GetButtonDown("Interaction");
        attackKeyDown = Input.GetButtonDown("Fire1");

        Move();
        Jump();
        Dodge();

        Attack();

        Interaction();
    }

    void Interaction()
    {
        if (interactionKeyDown && nearObject != null && !isJump && !isDodge)
        {
            if (nearObject.tag == "Weapon")
            {
                Item item = nearObject.GetComponent<Item>();
                int weaponIndex = item.value;
                hasWeapons[weaponIndex] = true;

                Destroy(nearObject);
            }
        }
    }

    void Move()
    {
        moveVec = new Vector3(hAxis, 0, vAxis).normalized;

        if (isDodge)
            moveVec = dodgeVec;

        transform.position += moveVec * speed * (walkKeyDown ? 0.3f : 1f) * Time.deltaTime;

        anim.SetBool("isRun", moveVec != Vector3.zero);
        anim.SetBool("isWalk", walkKeyDown);

        transform.LookAt(transform.position + moveVec); // 앞으로 가는 방향으로 로테이션
    }

    void Jump()
    {
        if(jumpKeyDown && moveVec == Vector3.zero && !isJump && !isDodge)
        {
            rigid.AddForce(Vector3.up * 15, ForceMode.Impulse);

            anim.SetBool("isJump", true);
            anim.SetTrigger("doJump");

            isJump = true;
        }
    }

    void Dodge()
    {
        if (jumpKeyDown && moveVec != Vector3.zero && !isJump && !isDodge)
        {
            dodgeVec = moveVec;
            speed *= 2f;
            anim.SetTrigger("doDodge");
            isDodge = true;

            Invoke("DodgeOut", 0.5f);
        }
    }

    void DodgeOut()
    {
        speed *= 0.5f;
        isDodge = false;
    }

    void Attack()
    {
        if (equipWeapon == null)
            return;

        attackDelay += Time.deltaTime;
        isAttackReady = equipWeapon.rate < attackDelay;

        if (attackKeyDown && isAttackReady && !isDodge)
        {
            equipWeapon.Use();
            attackDelay = 0;

            anim.SetTrigger("doAttack");
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Floor")
        {
            anim.SetBool("isJump", false);

            isJump = false;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Item")
        {
            Item item = other.GetComponent<Item>();
            
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.tag == "Weapon")
            nearObject = other.gameObject;
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Weapon")
            nearObject = null;
    }
}
