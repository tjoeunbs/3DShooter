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
    bool isAttackReady = true;

    public Weapon equipWeapon;

    bool swapKeyButton1;
    bool swapKeyButton2;
    bool swapKeyButton3;

    bool isSwapWeapon = false;

    int equipWeaponIndex = -1;

    public GameObject[] grenades;

    public int hasGrenade;
    public int maxHasGrenade;

    bool reloadKeyDown;

    public int ammo;
    public int maxAmmo;

    bool isReload;

    public GameObject grenadeObj;

    bool grenadeKeyDown;

    public Camera followCamera;

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

        StopToWall();
    }

    bool isBorder = false;
    void StopToWall()
    {
        Debug.DrawRay(transform.position, transform.forward * 5, Color.green);
        isBorder = Physics.Raycast(transform.position, transform.forward, 5, LayerMask.GetMask("Wall"));
    }

    // Update is called once per frame
    void Update()
    {
        hAxis = Input.GetAxisRaw("Horizontal");
        vAxis = Input.GetAxisRaw("Vertical");
        walkKeyDown = Input.GetButton("Walk");
        jumpKeyDown = Input.GetButtonDown("Jump");
        interactionKeyDown = Input.GetButtonDown("Interaction");

        swapKeyButton1 = Input.GetButtonDown("Swap1");
        if (swapKeyButton1)
            Debug.Log("입력 값 : " + swapKeyButton1);
        swapKeyButton2 = Input.GetButtonDown("Swap2");
        swapKeyButton3 = Input.GetButtonDown("Swap3");

        attackKeyDown = Input.GetButton("Fire1");

        reloadKeyDown = Input.GetButtonDown("Reload");

        grenadeKeyDown = Input.GetButtonDown("Fire2");

        Move();
        Jump();
        Dodge();

        Turn();

        Attack();

        Reload();

        Swap();

        ThrowGrenade();

        Interaction();
    }

    void ThrowGrenade()
    {
        if (hasGrenade == 0)
            return;

        if (grenadeKeyDown && !isReload && !isSwapWeapon && !isDodge)
        {
            Ray ray = followCamera.ScreenPointToRay(Input.mousePosition);
            RaycastHit rayHit;
            if (Physics.Raycast(ray, out rayHit, 100))
            {
                Vector3 nextVec = rayHit.point - transform.position;
                nextVec.y = 10;

                GameObject instantGrenade = Instantiate(grenadeObj, transform.position, transform.rotation);
                Rigidbody rigidGrenade = instantGrenade.GetComponent<Rigidbody>();
                rigidGrenade.AddForce(nextVec, ForceMode.Impulse);
                rigidGrenade.AddTorque(Vector3.back * 10, ForceMode.Impulse);

                hasGrenade--;
                grenades[hasGrenade].SetActive(false);
            }
        }
    }

    void Reload()
    {
        if (equipWeapon == null)
            return;
        if (equipWeapon.type == Weapon.OffenseDistance.Short)
            return;

        if (ammo == 0)
            return;

        if (reloadKeyDown && !isJump && !isDodge && !isSwapWeapon && isAttackReady)
        {
            anim.SetTrigger("doReload");
            isReload = true;

            Invoke("ReloadOut", 3f);
        }
    }

    void ReloadOut()
    {
        int reAmmo = ammo < equipWeapon.maxAmmo ? ammo : equipWeapon.maxAmmo;
        equipWeapon.curAmmo = reAmmo;

        ammo -= reAmmo;
        isReload = false;
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

        if (isSwapWeapon || !isAttackReady)
            moveVec = Vector3.zero;

        if (!isBorder)
        {
            transform.position += moveVec * speed * (walkKeyDown ? 0.3f : 1f) * Time.deltaTime;
        }

        anim.SetBool("isRun", moveVec != Vector3.zero);
        anim.SetBool("isWalk", walkKeyDown);
    }

    void Turn()
    {
        transform.LookAt(transform.position + moveVec); // 앞으로 가는 방향으로 로테이션

        if (attackKeyDown)
        {
            Ray ray = followCamera.ScreenPointToRay(Input.mousePosition);
            RaycastHit rayHit;
            if (Physics.Raycast(ray, out rayHit, 100))
            {
                Vector3 nextVec = rayHit.point - transform.position;
                nextVec.y = 0;
                transform.LookAt(transform.position + nextVec);
            }
        }
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

    void Swap()
    {
        if (swapKeyButton1 && (!hasWeapons[0] || equipWeaponIndex == 0))
            return;
        if (swapKeyButton2 && (!hasWeapons[1] || equipWeaponIndex == 1))
            return;
        if (swapKeyButton3 && (!hasWeapons[2] || equipWeaponIndex == 2))
            return;

        int weaponIndex = -1;
        if (swapKeyButton1) weaponIndex = 0;
        if (swapKeyButton2) weaponIndex = 1;
        if (swapKeyButton3) weaponIndex = 2;

        if ((swapKeyButton1 || swapKeyButton2 || swapKeyButton3) && !isJump && !isDodge)
        {
            if(equipWeapon != null)
            {
                equipWeapon.gameObject.SetActive(false);
            }

            equipWeaponIndex = weaponIndex;
            equipWeapon = weapons[weaponIndex].GetComponent<Weapon>();
            equipWeapon.gameObject.SetActive(true);

            anim.SetTrigger("doSwap");
            isSwapWeapon = true;

            Invoke("SwapOut", 0.4f);
        }
    }

    void SwapOut()
    {
        isSwapWeapon = false;
    }

    void Attack()
    {
        if (equipWeapon == null)
            return;

        attackDelay += Time.deltaTime;
        isAttackReady = equipWeapon.rate < attackDelay;

        if (attackKeyDown && isAttackReady && !isDodge && !isSwapWeapon)
        {
            equipWeapon.Use();
            attackDelay = 0;

            anim.SetTrigger(equipWeapon.type == Weapon.OffenseDistance.Short 
                ? "doAttack" : "doShot");
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
            if (item.type == Item.Type.Grenade)
            {
                if (hasGrenade == maxHasGrenade)
                    return;

                grenades[hasGrenade].SetActive(true);
                hasGrenade += item.value;
            }
            if (item.type == Item.Type.Ammo)
            {
                if (ammo >= maxAmmo)
                    return;

                ammo += item.value;
            }
            Destroy(other.gameObject);
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
