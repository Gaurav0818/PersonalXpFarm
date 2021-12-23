using UnityEngine;

public class PlayerController : MonoBehaviour
{
    #region - Private Variables -

    private Animator animator;                  // Reference to Animator
    private Rigidbody rb;                       // Reference to Player RigidBody
    private Vector3 colExtents;                 // Collider extends for ground test
        
    private float horizontal;                   // Horizontal Axis
    private float vertical;                     // Vertical Axis
        
    public bool sprint;                         // Boolean to determine whether a player is in sprint mode 
    private bool isJumping;                     // Boolean to determine whether a player is in jump mode 
    public  bool isMoving;                      // Boolean to determine whether a player is moving
    private float speed;                        // Moving Speed
    private int speedDir;                       // For dir of speed 
        
    private int hFloat;                         // h axis Animator Variable
    private int vFloat;                         // v axis Animator Variable
    private int speedFloat;                     // speed Animator Variable
    private int jumpBool;                       // jump Animator Variable
    private int groundBool;                     // ground Animator Variable
    
    #endregion

    #region - Public Variables -

    public float walkSpeed = 0.15f;             // Default Walk Speed
    public float runSpeed = 1.0f;               // Default Run Speed
    public float jumpHeight = 1.5f;             // Default jump Height
    public float jumpInertialForce = 10f;       // Default horizontal inertial force 
    public float turnSpeed = 0.5f;
    public float speedDampTime = 0.1f;          // Default Speed for Turning to match camera
    public float stoppingDampTime = 0.1f; 
    public bool isGrounded;

    public Ik_Set_Parameters headSphere;
    public GameObject playerCamera;
    
    #endregion
    
    private void Start()
    {
        animator = (Animator) GetComponent(typeof(Animator));
       // rb = GetComponent<Rigidbody>();
       // colExtents = GetComponent<Collider>().bounds.extents;
       // 
       // hFloat = Animator.StringToHash("H");
       // vFloat = Animator.StringToHash("V");
       // speedFloat = Animator.StringToHash("Speed");
       // jumpBool = Animator.StringToHash("Jump");
       // groundBool = Animator.StringToHash("Grounded");
    }

    private void Update()
    {
        // Get and Store Input Axis
        horizontal = Input.GetAxis("Horizontal");
        vertical = Input.GetAxis("Vertical");

     //  // Set Animator Values
     //  animator.SetFloat(hFloat,horizontal);
     //  animator.SetFloat(vFloat, vertical);
     //  
     //  //Store Ground Value
     //  isGrounded = CheckGround();
     //  animator.SetBool(groundBool,isGrounded);

     //  // Player movement
     //  sprint = Input.GetKey(KeyCode.LeftShift);

     //  if (!isJumping && Input.GetKeyDown(KeyCode.Space))
     //      isJumping = true;
     //  else
     //      JumpMovement();
     //  
     //  SetSpeed();

     //  isMoving = speed > 0.1f;

     //  if (isMoving)
     //  {
     //      Rotation();
     //  }
     //  
     //  MoveMovement();
     Vector2 pcAngle= getAnglePlayerAndCamera();  
     SetHeadIkAngle(pcAngle);

    }

    private Vector2 getAnglePlayerAndCamera()
    {
        Vector3 cForword = playerCamera.transform.forward;
        Vector3 pForword = transform.forward;

        return new Vector2(cForword.x - pForword.x, cForword.y - pForword.y);

    }

    private void SetHeadIkAngle(Vector3 pcAngle)
    {
        headSphere.SetParameters(pcAngle.x,pcAngle.y);
    }

    private void JumpMovement()
    {
        if (isGrounded)
        {
            animator.SetBool(jumpBool,true);
            
        }
        else
        {
            
        }
    }

    private bool CheckGround()
    {
        Ray ray = new Ray(transform.position + Vector3.up * (2 * colExtents.x), Vector3.down);
        return Physics.SphereCast(ray, colExtents.x, colExtents.x + 0.2f);
    }

    private void MoveMovement()
    {
        
        if (isGrounded)
        {
            rb.useGravity = true;
        }
        else
        {
            Vector3 hVelocity = rb.velocity;
            hVelocity.y = 0;
            rb.velocity = hVelocity;
        }

        Vector2 dir = new Vector2(horizontal, vertical);
        speed = dir.normalized.magnitude;

        //isMoving = speed > 0.1f;
        animator.SetFloat(speedFloat, speed , speedDampTime, Time.deltaTime);
    }

    private void SetSpeed()
    {
        
        if (vertical > 0)
        {
            speedDir = 1;
            
            if (sprint)
            {
                speed = speed * speedDir * runSpeed;
            }
            else
            {
                speed = speed * speedDir * walkSpeed;
            }
        }
        else if (vertical <= 0)
        {
            speed = 0;
        }

    }

    private void Rotation()
    {
        Vector3 forward = playerCamera.transform.TransformDirection(Vector3.forward);
        Vector3 right = playerCamera.transform.TransformDirection(Vector3.right);
        Vector3 left = playerCamera.transform.TransformDirection(Vector3.left);

        forward.y = 0.0f;
        forward = forward.normalized;
        
        Vector3 targetDir;

        if (horizontal < 0 && vertical!=0 )
        {
            targetDir = left + forward;
        }
        else if(horizontal > 0 && vertical!=0)
        {
            targetDir = right + forward;
        }
        else if (horizontal > 0)
        {
            targetDir = right;
        }
        else if (horizontal < 0)
        {
            targetDir = left;
        }
        else if(isMoving)
        {
            targetDir = forward;
        }
        else
        {
            targetDir = Vector3.forward;
        }
        
        if (speed>0.2 && targetDir != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(targetDir);
            transform.rotation = Quaternion.Slerp(transform.rotation,targetRotation, turnSpeed*Time.deltaTime);
        }
    }

}
