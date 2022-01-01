using Cinemachine;
using System;
using UnityEngine;
using UnityEditor;
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


#region - Public Variables -

    [Header("-------  Weapon Related  -------")]
    public GameObject meleeAxe;
    public GameObject meleeAxeComponent;
    public GameObject gunAssult;
    public bool holdMeleeWeapon = false;

    [Header("-------  Camera Related  -------")]
    public CameraType cameraType;
    public CinemachineFreeLook cmCameraValues;
    public GameObject playerCamera3P;
    public GameObject playerCamera1P;
    public CameraFps playerBodyFpsCameraScript;
    public CameraFps controllerBodyFpsCameraScript;


    [Header("-------  Rig/IK related  -------")]
    public Ik_Set_Parameters headSphere;
    public GameObject headReference;
    public Rig rigRifle;

    
    [Header("-------  Values  -------")]
    public bool isDashing;
    public bool canShoot;
    public float speed = 0;
    [Range(0, 30)]
    public float dashSpeed = 20;
    [Range(0, 20)]
    public float gravity = 9.8f;       
    [Range(0, 2)]
    public float turnSpeed = 0.5f;
    [Range(60, 80)]
    public float maxFov = 70;
    [Range(10, 100)]
    public float runFovChangeSpeed = 20;

    [Header("-------  Extra  -------")]
    public GameObject capsule;
    public GameObject playerBody;
    public Animator animator;
    public CharacterController charController;
    public Vector3 moveDir;
    public bool attack;
    public bool moveHead;

#endregion


#region - Private Variables -


    private float horizontal;                   // Horizontal Axis
    private float vertical;                     // Vertical Axis
    
    private bool stopRotate = true;
    private float stopRotateTimer;

    private bool setTo3PWhenChangingTo1P;
    private Vector3 camera3PPositionBeforeShift;
    private float camera3PTo1PTimer;

    private bool shiftPressed;
        
    private float minFov = 50;                  // min Fov (3rd Person)
    private float fov = 50;

    private int dirInt;
    private int hDirInt;
    private int runBool;
    private int BoostRunBool;
    private int AttackBool;

    private bool modeCameraIsFps;
    private bool runBoostMode;

    private int dashAttempts;
    private float dashStartTime; 

#endregion


#region - Start/Update -

    private void Start()
    {
        dirInt = Animator.StringToHash("Dir");
        hDirInt = Animator.StringToHash("H_Dir");
        runBool = Animator.StringToHash("Run");
        BoostRunBool = Animator.StringToHash("BoostRun");
        AttackBool = Animator.StringToHash("Attack");

        moveHead = true;
    }
    
    private void Update()
    {

        horizontal = Input.GetAxis("Horizontal");
        vertical = Input.GetAxis("Vertical");

        animator.SetInteger( hDirInt, horizontal < 0 ? -1 : ( horizontal ==0 ? 0 : 1 ));
        animator.SetInteger( dirInt, vertical < 0 ? -1 : ( vertical == 0 ? 0 : 1 ));

        animator.SetBool(BoostRunBool, runBoostMode ? true : false);
        
        

        if (cameraType == CameraType.ThirdPerson)
            Tps();
        else
            Fps();

        Head();

        SetFov();

        animator.SetBool(AttackBool, attack);

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

    private void Fps()
    {
        DisableWeapon(meleeAxe);
        EnableWeapon(gunAssult);
        canShoot = true;
        SwitchCamera();
        HandleDash();
        Move();
        Rotation();
    }

    private void UpdateFps()
    {
        playerBodyFpsCameraScript.enabled = true;
        modeCameraIsFps = true;
        controllerBodyFpsCameraScript.enabled = true;
    }
    

#endregion
    

#region - TPS -

    private void Tps()
    {
        canShoot = false;
        shiftPressed = Input.GetKey(KeyCode.LeftShift);
        HandleRunBoost();

        if (runBoostMode)
        {
            RunBoostMove();
            attack = false;
        }
        else
        {
            SwitchCamera();
            Move();
            PlayerBodyRotation();
        }

        if (Input.GetKeyDown(KeyCode.Mouse0))
            attack = true;

        if (!holdMeleeWeapon)
        {
            DisableWeapon(gunAssult);
            DisableWeapon(meleeAxe);
            Rotation();
        }
        else
        {
            EnableWeapon(meleeAxe);
            DisableWeapon(gunAssult);
            MeleeSound();
        }
    }

    private void UpdateTps()
    {
        playerBodyFpsCameraScript.enabled = false;
        controllerBodyFpsCameraScript.enabled = false;
        if (modeCameraIsFps)
        {
            transform.localEulerAngles = new Vector3(0,transform.localEulerAngles.y,0);
            
            var localEulerAngles = charController.transform.localEulerAngles;
            
            localEulerAngles = new Vector3(localEulerAngles.x,
                                            localEulerAngles.y,
                                            localEulerAngles.z);
            
            charController.transform.localEulerAngles = localEulerAngles;
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


#region - Body Rotation -

    private void PlayerBodyRotation()
    {
        Vector3 dir;

        if (vertical > 0)
        {
            dir = capsule.transform.forward;

            dir += ForHorizontalBodyRotation();

        }
        else if (vertical < 0)
            dir = capsule.transform.forward;
        else
        {
            if (horizontal != 0)
                dir = ForHorizontalBodyRotation();
            else
                dir = capsule.transform.forward;
        }

        playerBody.transform.rotation = Quaternion.Slerp(playerBody.transform.rotation, Quaternion.LookRotation(dir), 1000000);
    }

    private Vector3 ForHorizontalBodyRotation()
    {
        if (horizontal > 0)
        {
            return capsule.transform.right;
        }
        else if (horizontal < 0)
        {
            return -capsule.transform.right;
        }
        else
        {
            return Vector3.zero;
        }
    }


    #endregion


#region - Movement -

    private void Move()
    {
        moveDir = Vector3.zero;

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
        
        moveDir *= speed;

        moveDir.y = CalculateVerticalVelocity();
        moveDir *= Time.deltaTime;
        charController.Move(moveDir);
    }

    private float CalculateVerticalVelocity()
    {
        return -gravity;
    }

    private bool Grounded()
    {
        return charController.isGrounded;
    }


    #endregion


#region - RunBoostMode -

    void HandleRunBoost()
    {
        bool isTryingToRunBoost = Input.GetKeyDown(KeyCode.Space);
        if (isTryingToRunBoost)
        {
            runBoostMode = true;
        }

        if (Input.GetKeyDown(KeyCode.S))
        {
            runBoostMode = false;
        }

    }

    void RunBoostMove()
    {
        moveDir = Vector3.zero;

        moveDir = playerCamera3P.transform.forward ;
        moveDir.Normalize();

        moveDir *= speed;

        moveDir.y = CalculateVerticalVelocity();
        moveDir *= Time.deltaTime;
        charController.Move(moveDir);
        moveDir.y = 0;
        playerBody.transform.rotation = Quaternion.Slerp(playerBody.transform.rotation, Quaternion.LookRotation(moveDir), 1000000);
    }

#endregion


#region - Dash -

    void HandleDash()
    {
        bool isTryingToDash = Input.GetKeyDown(KeyCode.Space);

        if (isTryingToDash && !isDashing)
        {
            if (dashAttempts < 50)
            {
                OnStartDash();
            }
        }

        if (isDashing)
        {
            if (Time.time - dashStartTime <= 0.4f)
            {
                if (speed.Equals(0))
                {
                    // Player is not giving any input 
                    charController.Move(transform.forward * 30f * Time.deltaTime);
                }
                else
                {
                    charController.Move(moveDir.normalized * 100f * Time.deltaTime);
                }
            }
            else
            {
                OnEndDash();
            }
        }

    }

    void OnStartDash()
    {
        isDashing = true;
        dashStartTime = Time.time;
        dashAttempts += 1;

    }
    void OnEndDash()
    {
        isDashing = false;
        dashAttempts = 0;
    }

#endregion


#region - Run FOV -

    void SetFov()
    {
    
        cmCameraValues.m_Lens.FieldOfView = fov;
        
        if (speed > 5)
            if (fov <= maxFov)
                fov += Time.deltaTime * runFovChangeSpeed / 2 ;
            else
                fov = maxFov;
        else 
            if (fov <= minFov)
                fov = minFov;
            else
                fov -= Time.deltaTime * runFovChangeSpeed / 2;
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
        Vector3 pForward = playerBody.transform.forward;
    
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


#region - Weapon -

    private void DisableWeapon(GameObject weapon)
    {
        weapon.SetActive(false);
    }
    private void EnableWeapon(GameObject weapon)
    {
        weapon.SetActive(true);
    }

    private void MeleeSound()
    {
        Vector3 wForword = meleeAxeComponent.transform.forward;
        Vector3 cForword = charController.transform.forward;
        wForword.Normalize();
        cForword.Normalize();

        if ( (Mathf.Round(wForword.x * 100f) / 100f) == -0.3 )
        {

        }
           
    }


#endregion

}
