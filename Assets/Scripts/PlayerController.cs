using Cinemachine;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Animations.Rigging;

public class PlayerController : MonoBehaviour
{
    
    #region - Struct and class -
        private enum CameraType
        {
            ThirdPerson,
            FirstPerson,
        }
        
    #endregion
    
    
    #region - Private Variables -

        private Animator animator;                  // Reference to Animator
        // private Rigidbody rb;                       // Reference to Player RigidBody
        // private Vector3 colExtents;                 // Collider extends for ground test
        private CharacterController charController;
            
        private float horizontal;                   // Horizontal Axis
        private float vertical;                     // Vertical Axis
    
        private bool stopRotate = true;
        private float stopRotateTimer;

        private bool setTo3PWhenChangingTo1P;
        private Vector3 Camera3PPositionBeforeShift;
        private float Camera3PTo1Ptimer = 0;
        private float Camera3PTo1PTime = 0.5f;
        
       // private bool sprint;                         // Boolean to determine whether a player is in sprint mode 
       // private bool isJumping;                     // Boolean to determine whether a player is in jump mode 
       // private bool isMoving;                      // Boolean to determine whether a player is moving
        // private float speed;                        // Moving Speed
        // private int speedDir;                       // For dir of speed 

        private bool jumpPressed;
        private bool shiftPressed; 
        
        private bool fovChange;             // To Change Fov (3rd Person)
        private float minFov = 60;                  // min Fov (3rd Person)
        private float fov = 60;
        
        // private int hFloat;                         // h axis Animator Variable
        // private int vFloat;                         // v axis Animator Variable
        // private int speedFloat;                     // speed Animator Variable
        // private int jumpBool;                       // jump Animator Variable
        // private int groundBool;                     // ground Animator Variable
    
        private int dirInt;
        private int runBool;
        
        
        
        private CameraType cameraType;
    
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
    
    
        [Range(60, 90)] 
        public float maxFov = 70;
    
        [Range(10, 100)] 
        public float runFovChangeSpeed = 20;
    
        public Ik_Set_Parameters headSphere;
        public GameObject headReference;
        public GameObject playerCamera3P;
        public GameObject playerCamera1P;
        public CinemachineFreeLook cmCameraValues;
    
        public Rig rigRifle;
    
    #endregion


    #region - Start/Update -

        private void Start()
        {
            animator = GetComponent<Animator>();
            charController = GetComponent<CharacterController>();
            dirInt = Animator.StringToHash("Dir");
            runBool = Animator.StringToHash("Run");
        }
    
        private void Update()
        {
            jumpPressed = false;
            
            horizontal = Input.GetAxis("Horizontal");
            vertical = Input.GetAxis("Vertical");

            animator.SetInteger(dirInt, vertical < 0 ? -1 : 1);

            jumpPressed = Input.GetKeyDown(KeyCode.Space);

            
            Rotation();
    
            SwitchCamera();

            Head();

            SetFov();
            Move();
        }
    

    #endregion
    
    
    #region - Switch FPS/TPS -
    

        private void SwitchCamera()
        {
            cameraType = Input.GetKey(KeyCode.Mouse1) ? SetCameraTo1P() 
                                                      : SetCameraTo3P();
            
            if (cameraType == CameraType.FirstPerson)
            {
                playerCamera1P.SetActive(true);
                rigRifle.weight = 1;
    
            }
            else
            {
                playerCamera1P.SetActive(false);
                rigRifle.weight = 0;
            }
        }

        private CameraType SetCameraTo1P()
        {
            if (cameraType == CameraType.ThirdPerson)
            {
                Camera3PPositionBeforeShift = playerCamera3P.transform.forward;
                setTo3PWhenChangingTo1P = true;
                Debug.Log(cameraType);
            }
            return CameraType.FirstPerson;
        }

        private CameraType SetCameraTo3P()
        {
            return CameraType.ThirdPerson;
        }

    #endregion

    
    #region - Rotation -

        private void Rotation()
        { 
            animator.SetBool(runBool, shiftPressed);
            
            if(vertical !=0 ) 
                if (cameraType == CameraType.ThirdPerson)
                {
                    shiftPressed = Input.GetKey(KeyCode.LeftShift);

                    if (stopRotate)
                    {
                        stopRotateTimer += Time.deltaTime;
                        if (stopRotateTimer > 1 + 0.01)
                        {
                            stopRotate = false;
                            stopRotateTimer = 0;
                        }
                    }
                    else
                    if(!Input.GetKey(KeyCode.LeftAlt))
                        Rotation3dPerson();
                }

            if (cameraType == CameraType.FirstPerson)
            {
                shiftPressed = false;
                
                if (setTo3PWhenChangingTo1P)
                {
                    if (Camera3PTo1Ptimer < 1)
                    {
                        Camera3PTo1Ptimer += Time.deltaTime;
                    }
                    else
                    {
                        setTo3PWhenChangingTo1P = false;
                        Camera3PTo1Ptimer = 0;
                    }
                    
                    Rotation3PTo1P();
                }
                else
                {
                    stopRotate = true;
                    stopRotateTimer = 0;
                
                    
                    Rotation1stPerson();
                }
            }
        }

        private void Rotation3PTo1P()
        {
            Vector3 cameraRef = Camera3PPositionBeforeShift;
            Vector3 dir = new Vector3( cameraRef.x, 0, cameraRef.z );

            transform.rotation= Quaternion.Slerp(transform.rotation,Quaternion.LookRotation(dir), 0.5f);
        }

        private void Rotation1stPerson()
        {
            
        }
    
        private void Rotation3dPerson()
        {
            Vector3 headRef = headReference.transform.position;
            Vector3 cameraRef = playerCamera3P.transform.position;
            Vector3 dir = new Vector3(headRef.x - cameraRef.x, 0, headRef.z - cameraRef.z);
    
            //float angle = Mathf.Abs(Vector3.Angle(playerCamera3P.transform.forward, transform.forward));
            if (shiftPressed)
                turnSpeed = turnSpeed * 3; 
            
            transform.rotation= Quaternion.Slerp(transform.rotation,Quaternion.LookRotation(dir),turnSpeed * Time.deltaTime);
        }
    
    #endregion

    
    #region - Movement -

        private void Move()
        {
            Vector3 moveDir;

            if (cameraType == CameraType.ThirdPerson)
            {
                moveDir = playerCamera3P.transform.forward * vertical;
                moveDir = moveDir + playerCamera3P.transform.right * horizontal;
                moveDir.Normalize();
            }
            else
            {
                moveDir = playerCamera1P.transform.forward * vertical;
                moveDir = moveDir + playerCamera1P.transform.right * horizontal;
                moveDir.Normalize();
            }
            
            if(shiftPressed)
                moveDir *= runSpeed;
            else
                moveDir *= walkSpeed;
            
            moveDir.y = Jump();
            moveDir *= Time.deltaTime;
            charController.Move(moveDir);
        }

        private float Jump()
        {
            if (!Grounded())
                return CalculateVerticalVelocity();
            else if (jumpPressed)
                return ApplyJump();
            else
                return 0;
        }

        private float ApplyJump()
        {
            return 0;
        }

        private float CalculateVerticalVelocity()
        {
            return -jumpInertialForce;
        }

        private bool Grounded()
        {
            return charController.isGrounded;
        }

    #endregion
    

    /*
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
    */
    
    
    #region - Run FOV -
    
        void SetFov()
        {
    
            cmCameraValues.m_Lens.FieldOfView = fov;
            
            if (fovChange)
                if (fov <= maxFov)
                    fov += Time.deltaTime * runFovChangeSpeed ;
                else
                    fov = maxFov;
            else 
                if (fov <= minFov)
                    fov = minFov;
                else
                    fov -= Time.deltaTime * runFovChangeSpeed ;
        }
    
        public void SetFovChangeTrue()
        {
            fovChange = true;
        }
    
        public void SetFovChangeFalse()
        {
            fovChange = false;
        }
        
    
    #endregion
    

    #region - Head Mov -

        private void Head()
        {
            Vector2 pcAngle= GetAnglePlayerAndCamera();  
            SetHeadIkAngle(pcAngle);
        }
        
        private Vector2 GetAnglePlayerAndCamera()
        {
            Vector3 pForward = transform.forward;
    
            Vector3 cForward = cameraType == CameraType.ThirdPerson ? playerCamera3P.transform.forward 
                                                                    : playerCamera1P.transform.forward;
    
            Vector2 angle = new Vector2(cForward.x - pForward.x, cForward.y - pForward.y);
            
            return angle;
        }
    
        private void SetHeadIkAngle(Vector3 pcAngle)
        {
            headSphere.SetParameters(pcAngle.x,pcAngle.y);
        }
    
    #endregion
    
}
