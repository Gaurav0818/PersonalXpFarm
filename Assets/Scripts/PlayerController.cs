using Cinemachine;
using UnityEngine;
using UnityEngine.Animations.Rigging;

public class PlayerController : MonoBehaviour
{
    
#region - Struct and class -
    public enum CameraType
    {
        ThirdPerson,
        FirstPerson
    }
        
#endregion
    
    
#region - Private Variables -

    private Animator animator;                  // Reference to Animator

    private float horizontal;                   // Horizontal Axis
    private float vertical;                     // Vertical Axis
    
    private bool stopRotate = true;
    private float stopRotateTimer;

    private bool setTo3PWhenChangingTo1P;
    private Vector3 camera3PPositionBeforeShift;
    private float camera3PTo1PTimer;

    private bool jumpPressed;
    private bool shiftPressed; 
        
    private bool fovChange;             // To Change Fov (3rd Person)
    private float minFov = 60;                  // min Fov (3rd Person)
    private float fov = 60;

    private int dirInt;
    private int runBool;

    private bool modeCameraIsFps;
        
        
        
    public CameraType cameraType;
    
#endregion

    
#region - Public Variables -

    public CharacterController charController;
    public float walkSpeed = 0.15f;             // Default Walk Speed
    public float runSpeed = 1.0f;               // Default Run Speed
    public float jumpInertialForce = 10f;       // Default horizontal inertial force 
    public float turnSpeed = 0.5f;


    [Range(60, 90)] 
    public float maxFov = 70;
    
    [Range(10, 100)] 
    public float runFovChangeSpeed = 20;
    
    public Ik_Set_Parameters headSphere;
    public GameObject headReference;
    public GameObject playerCamera3P;
    public GameObject playerCamera1P;
    public CinemachineFreeLook cmCameraValues;
    public CameraFps playerBodyFpsCameraScript;
    public CameraFps controllerBodyFpsCameraScript;
    public GameObject controller;
    
    public Rig rigRifle;
    
#endregion

    public int count ;
    
#region - Start/Update -

    private void Start()
    {
        animator = GetComponent<Animator>();
        dirInt = Animator.StringToHash("Dir");
        runBool = Animator.StringToHash("Run");
    }
    
    private void Update()
    {
        print(count);
        jumpPressed = false;
        
        horizontal = Input.GetAxis("Horizontal");
        vertical = Input.GetAxis("Vertical");

        animator.SetInteger(dirInt, vertical < 0 ? -1 : (vertical==0 ? 0 : 1));

        jumpPressed = Input.GetKeyDown(KeyCode.Space);

        print(stopRotateTimer);
        
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
        cameraType = Input.GetKey(KeyCode.Mouse1) ? SetCameraTo1P() : SetCameraTo3P();
        
        if (cameraType == CameraType.FirstPerson)
        {
            playerCamera1P.SetActive(true);
            rigRifle.weight = 1;
            UpdateFps();
        }
        else
        {
            UpdateTps();
            playerCamera1P.SetActive(false);
            rigRifle.weight = 0;
            
        }
    }

    private CameraType SetCameraTo1P()
    {
        if (cameraType == CameraType.ThirdPerson)
        {
            camera3PPositionBeforeShift = playerCamera3P.transform.forward;
            setTo3PWhenChangingTo1P = true;
            
        }
        return CameraType.FirstPerson;
    }

    private CameraType SetCameraTo3P()
    {
        return CameraType.ThirdPerson;
    }

#endregion


#region - FPS -

    private void UpdateFps()
    {
        playerBodyFpsCameraScript.enabled = true;
        modeCameraIsFps = true;
        controllerBodyFpsCameraScript.enabled = true;
    }
    

#endregion
    

#region -TPS -

    private void UpdateTps()
    {
        playerBodyFpsCameraScript.enabled = false;
        controllerBodyFpsCameraScript.enabled = false;
        if (modeCameraIsFps)
        {
            transform.localEulerAngles = new Vector3(0,transform.localEulerAngles.y,0);
            
            var localEulerAngles = controller.transform.localEulerAngles;
            
            localEulerAngles = new Vector3(localEulerAngles.x,
                                            localEulerAngles.y,
                                            localEulerAngles.z);
            
            controller.transform.localEulerAngles = localEulerAngles;
            modeCameraIsFps = false;
        }
    }
    
    
    
#endregion

    
#region - Rotation -

    private void Rotation()
    { 
        animator.SetBool(runBool, shiftPressed);
        
        if(vertical !=0 ) 
            if (cameraType == CameraType.ThirdPerson)
            {
                if (stopRotate)
                {
                    stopRotateTimer += Time.deltaTime;
                    if (stopRotateTimer > 0.7 )
                    {

                        stopRotateTimer = 0;
                        stopRotate = false;
                        count += 1;
                    }
                }
                else
                    if(!Input.GetKey(KeyCode.LeftAlt))
                        Rotation3dPerson();
                
                shiftPressed = Input.GetKey(KeyCode.LeftShift);
            }

        if (cameraType == CameraType.FirstPerson)
        {
            shiftPressed = false;
            
            if (setTo3PWhenChangingTo1P)
            {
                if (camera3PTo1PTimer < 1)
                {
                    camera3PTo1PTimer += Time.deltaTime;
                }
                else
                {
                    setTo3PWhenChangingTo1P = false;
                    camera3PTo1PTimer = 0;
                }
                
                Rotation3PTo1P();
            }
            else
            {
                stopRotate = true;
               // stopRotateTimer = 0;

                Rotation1StPerson();
            }
        }
    }

    private void Rotation3PTo1P()
    {
        Vector3 cameraRef = camera3PPositionBeforeShift;
        Vector3 dir = new Vector3( cameraRef.x, 0, cameraRef.z );

        transform.rotation= Quaternion.Slerp(transform.rotation,Quaternion.LookRotation(dir), 0.5f);
    }

    private void Rotation1StPerson()
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
