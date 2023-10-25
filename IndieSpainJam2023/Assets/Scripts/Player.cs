using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Player : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] bool canMove;
    [SerializeField] bool canUseTelescope;
    [SerializeField] float speedWalk;
    [SerializeField] float speedRun;
    float speedCur;

    [Header("Damage")]
    [SerializeField] bool isInvincible;
    [SerializeField] float invinsibleTime;

    //Compenents
    Rigidbody2D rb;
    Animator anim;
    SpriteRenderer renderer;

    //Private
    Vector2 dPad;
    bool isRunning;

    //Encapsuling
    public bool IsRunning { get => isRunning; set => isRunning = value; }
    public bool CanMove { get => canMove; set => canMove = value; }
    public bool IsInvincible { get => isInvincible; set => isInvincible = value; }
    public bool CanUseTelescope { get => canUseTelescope; set => canUseTelescope = value; }

    [SerializeField] GameObject bonfire;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        renderer = GetComponent<SpriteRenderer>();
    }

    // Start is called before the first frame update
    void Start()
    {
        canMove = true;
        canUseTelescope = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (canUseTelescope)
            UseTelescope();

        if (!canMove)
        {
            dPad = Vector2.zero;
            return;
        }

        TopDownMovement();
        //TankMovement();
        Animation();
        MakeBonfire();
    }
    void Animation()
    {
        anim.SetBool("isWalking", dPad.sqrMagnitude > 0 && isRunning == false);
        anim.SetBool("IsRunning", dPad.sqrMagnitude > 0 && isRunning == true);
    }

    void TopDownMovement()
    {
        dPad.x = Input.GetAxisRaw("Horizontal");
        dPad.y = Input.GetAxisRaw("Vertical");

        if (Input.GetKey(KeyCode.Z))
        {
            if (dPad.sqrMagnitude > 0)
            {
                isRunning = true;
                speedCur = speedRun;
            }
            else
                isRunning = false;
        }
        else
        {
            isRunning = false;
            speedCur = speedWalk;
        }

        if (dPad.sqrMagnitude >= 0.1f) transform.rotation = Quaternion.LookRotation(Vector3.forward, dPad);
    }


    void FixedUpdate()
    {
        if (canMove)
            rb.MovePosition(rb.position + dPad * speedCur * Time.fixedDeltaTime);
    }

    void MakeBonfire()
    {
        if (GameManager.instance.CanMakeABonfire() && DayNightSystem2D.instance.IsNight())
        {

            if (Input.GetKeyDown(KeyCode.C))
            {
                Debug.Log("Hacer hoguera");
                AudioManager.instance.Play("wood");
                GameManager.instance.ResetWood();
                Instantiate(bonfire, transform.position, Quaternion.identity);
            }
        }
    }

    void UseTelescope()
    {
        if (Input.GetKeyDown(KeyCode.X))
        {
            GameManager.instance.UseTelescope();
        }
    }

    void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag("Bonfire"))
        {
            if (DayNightSystem2D.instance.IsNight())
            {
                if (canMove && Input.GetKeyDown(KeyCode.C))
                {
                    collision.GetComponent<Collider2D>().enabled = false;
                    GameManager.instance.PassToNextMorning();
                }
            }
        }

        if (collision.CompareTag("Hitbox"))
        {
            if (isInvincible || GameManager.instance.IsTelecopeOpen)
                return;

            isInvincible = true;

            AudioManager.instance.Play("wolfBite");
            GameManager.instance.DamageWolf();

            StartCoroutine(Invincible());
            StartCoroutine(Flickering());
        }
    }

    IEnumerator Invincible()
    {
        yield return new WaitForSeconds(invinsibleTime);
        isInvincible = false;
    }

    IEnumerator Flickering()
    {
        Color c = renderer.color;

        while (isInvincible)
        {
            c.a = 0;
            renderer.color = c;
            yield return null;

            c.a = 100;
            renderer.color = c;
            yield return null;
        }
    }

    public void PlaySandSound()
    {
        if (dPad.sqrMagnitude >= 0.1f)
            AudioManager.instance.Play("sandStep");
    }
}