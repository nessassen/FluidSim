using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerControllerFluid : MonoBehaviour
{
    [SerializeField] protected PixelBody pb;
    [SerializeField] protected float moveForce;
    [SerializeField] protected Rigidbody2D rb;
    [SerializeField] protected SpriteRenderer fluidSpr;
    protected float sonarTimer;
    [SerializeField] protected float sonarCD;
    protected float swimTimer;
    [SerializeField] protected float swimCD;
    protected bool spacePressed;
    protected Vector2 inputDir;

    void Start()
    {
        sonarTimer = Time.time - sonarCD;
        swimTimer = Time.time - swimCD;
    }

    void Update()
    {
        spacePressed = spacePressed || Input.GetKeyDown("space");
        inputDir = new Vector2((Input.GetKey(KeyCode.D) ? 1 : 0) - (Input.GetKey(KeyCode.A) ? 1 : 0), (Input.GetKey(KeyCode.W) ? 1 : 0) - (Input.GetKey(KeyCode.S) ? 1 : 0));
        if (inputDir != Vector2.zero) inputDir = inputDir.normalized;
    }

    void FixedUpdate()
    {
        if (sonarTimer + sonarCD < Time.time)
        {
            fluidSpr.enabled = true;
        }
        else
        {
            fluidSpr.enabled = !fluidSpr.enabled;
        }
        if (spacePressed && sonarTimer + sonarCD < Time.time && swimTimer + swimCD < Time.time)
        {
            if (inputDir == Vector2.zero)
            {
                fluidSpr.enabled = false;
                sonarTimer = Time.time;
            }
            else
            {
                if(rb != null && rb.IsAwake()) 
                    rb.AddForce(moveForce * -inputDir);
                if (pb != null && pb.isActiveAndEnabled)
                    pb.AddForce(moveForce * -inputDir);
                swimTimer = Time.time;
            }
        }
        spacePressed = false;
    }
}
