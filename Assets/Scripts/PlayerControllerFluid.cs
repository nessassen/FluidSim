using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerControllerFluid : MonoBehaviour
{
    [SerializeField] protected float moveForce;
    [SerializeField] protected float defaultDrag;
    [SerializeField] protected float brakeDrag;
    [SerializeField] protected PixelBody pb;
    [SerializeField] protected List<SpriteRenderer> fluidSprends;
    protected float sonarTimer;
    [SerializeField] protected float sonarCD;
    protected float swimTimer;
    [SerializeField] protected float swimCD;
    protected bool spacePressed;
    protected bool spaceHeld;
    protected Vector2 inputDir;

    void Start()
    {
        sonarTimer = Time.time - sonarCD;
        swimTimer = Time.time - swimCD;
    }

    void Update()
    {
        spacePressed = spacePressed || Input.GetKeyDown("space");
        spaceHeld = Input.GetKey("space");
        inputDir = new Vector2((Input.GetKey(KeyCode.D) ? 1 : 0) - (Input.GetKey(KeyCode.A) ? 1 : 0), (Input.GetKey(KeyCode.W) ? 1 : 0) - (Input.GetKey(KeyCode.S) ? 1 : 0));
        if (inputDir != Vector2.zero) inputDir = inputDir.normalized;
    }

    void FixedUpdate()
    {
        if (spaceHeld)
        {
            if (inputDir == Vector2.zero)
            {
                foreach(SpriteRenderer sprend in fluidSprends)
                {

                    sprend.enabled = !sprend.enabled;
                }
                if (pb != null)
                    pb.drag = brakeDrag;
            }
            else
            {
                foreach (SpriteRenderer sprend in fluidSprends)
                {

                    sprend.enabled = true;
                }
                if (pb != null)
                    pb.drag = defaultDrag;
                if (pb != null && pb.isActiveAndEnabled)
                    pb.AddForce(moveForce * -inputDir);
            }
        }
        else
        {
            foreach (SpriteRenderer sprend in fluidSprends)
            {
                sprend.enabled = true;
            }
            if (pb != null)
                pb.drag = brakeDrag;
        }
        spacePressed = false;
    }
}
