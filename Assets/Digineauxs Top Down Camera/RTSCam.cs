using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Just drag the prefab into the scene heirachy and you have a working camera you can customize in the inspector. 
//If you insist on just using this script, assembly instructions are below.

//you need to create any empty object to use as a cameraRig
//then create a an empty object for rotRig as a child within the cameraRig
// then add a camera as a child to the rotRig.

//the object names don't actually matter, just makes it easier if you need to look something up or change soemthing about the camera setup.
//should look like this:
//RTSCamRig         - empty object. Attach script to this one.
//  RotRig          - empty object
//      RTSCamera   - Camera
// RTSCamera needs rotation X set to 90 and position y set to 10.

//attach this script to the rts camrig
//Drag rotRig into "Rot Rig" on the inspector
//drag RTSCamera into "Camera Transform" on the inspector


public class RTSCam : MonoBehaviour
{
    #region input variables
    [Header("The default values can be found at the top of the code where it says 'public float speed = 20' or similar. look for the =")]

    [Space]
    [Space]

    [Header("Input Settings")]
    [Tooltip("Input Manager buttons need to be created in the input manager and enabled by uncommenting them in the script")]
    public bool enableInputManagerButtons = false;
    [Tooltip("this toggles whether a button must be held to rotate mouse or whether that button toggles the mouse rotation. Mouse rotation requires input manager whether this is ticked or not")]
    public bool toggleRotateWithMouse = false;
    [Tooltip("If neither input modes are ticked, this one will be active. Requires no setup")]
    public bool enableHardCodedButtons = true;
    [Tooltip("If you want to quickly disable all camera inputs for testing or to implement your own")]
    public bool disableBothInputs = false;
    [Tooltip("Mouse will be locked to viewable area. may not work in the editor game window. may not work in editor")]
    public bool lockMouse = true;
    [Tooltip(" mouse is locked to centre of screen and made invisble. overides 'lock mouse' when on. May not work in editor")]
    public bool hideMouse = false;
    private bool RotatingWithMouse;
    private float leftRightAxis;
    private float forwardBackAxis;
    private float zoomAxis;
    private float rotateVerticalAxis;
    private float rotateHorizontalAxis;
    private float boostAxis;
    private float tempMouseRotX;
    private float tempMouseY;
    private float tempMouseRotButtonHeld;
    #endregion
    #region panning variables
    private Vector3 vec3Move; // temporary vector storage to be applied to our actual position at the end

    [Space]
    [Space]
    [Header("Panning/ horizontal movement settings")]
    [Tooltip("pans camera when your mouse is near the edge of the screen.")]
    public bool enableEdgePanning = true;
    [Tooltip("Disables horizontal movement")]
    public bool EnablePanning = true;
    [Tooltip("horizontal movement speed")]
    public float normalSpeed = 15f;
    [Tooltip("The smoothness of stopping when you let go of any panning controls. Bigger numbers = more sudden stop. set it to one to see how it works")]
    public float moveTime = 8f; //how quickly the cam stops after button release when lerping. Higher numbers stop faster.
    #endregion
    #region rotation Variables
    [Space]
    [Space]
    [Header("Rotation Settings")]
    [Tooltip("Don't forget to have numblock on for they numberpad rotation keys")]
    public bool EnableHorizontalRotation = true;
    [Tooltip("How quickly the camera spins sideways ")]
    public float rotateSpeed = 40.0f;
    public bool EnableVerticalRotation = true;
    [Tooltip("How quickly the camera spins up and down")]
    public float rotateSpeedVert = 20.0f;
    [Tooltip("Limit the down rotation. 270 is level with RTSCameraRig. These numbers don't correspond to the inspectors rotation because quaternions are dumb ")]
    public float minXRot = 270; //0 Unitiy handle rotation in a dumb way. The rotation in the inspector uses negative values if the engine uses quaternions and euler angles to actually rotate in 360 degrees. So you have to convert it.
    [Tooltip("Limit the up rotation. They are harcoded not to exceed 90 degree angles (looking straight down or up), relative to the camerig and camera. due to the way rotation is handled. otherwise alot fo stuff breaks")]
    public float maXrot = 359; //-89
    #endregion
    #region zoom variables
    [Space]
    [Space]
    [Header("Zoom Settings")]
    public bool EnableZoom = true;
    public bool EnableMousewheelZoom = true;
    [Tooltip("Zoom Speed")]
    public float zoomSpeed = 14f;
    [Tooltip("Toggle wheether zoom limits are active")]
    public bool limitZoom = true;
    private float zoomDistance;
    [Tooltip("How close you can zoom. This will automatically be set to 1 if lower than 1")]
    public float zoomMin = 1f;
    [Tooltip("How far away you can zoom")]
    public float zoomMax = 40f;
    #endregion
    #region boost variables
    [Space]
    [Space]
    [Header("Extra and slower speed button modifiers. eg. hold shift to pan faster")]
    public bool panSpeedBoost = true;
    private float moveSpeed;
    public float slowSpeed = 2.5f;
    public float fastSpeed = 30;

    public bool zoomSpeedBoost = true;
    private float zoomMoveSpeed;
    public float slowZoomSpeed = 1f;
    public float fastZoomSpeed = 20;


    public bool rotateVerticalSpeedBoost = true;
    private float rotateVerticalMoveSpeed;
    public float slowVerticalRotateSpeed = 5f;
    public float fastVerticalRotateSpeed = 100;

    public bool RotateHorizontalSpeedBoost = true;
    private float RotateHorizontalMoveSpeed;
    public float slowRotateSpeed = 5f;
    public float fastRotateSpeed = 100;
    #endregion
    #region starting location
    [Space]
    [Space]
    [Header("camera Starting positions. Disable these if you want to move it in the inspector and know what your doing. CamRig = horizontal position and Y rotation, rotRig = x Rotation and Camera = zoom")]
    [Tooltip("This moves the RTS camrig, which acts as the centre of rotation and zoom")]
    public bool enableStartPosition = true;
    public float startX = 0f;
    [Tooltip("This also acts as the lowest point you can zoom too")]
    public float startY = 5f;
    public float startZ = 0f;
    private Vector3 startVector3;

    public bool enableStartXrotation = true;
    [Tooltip("try to ensure this is between the vertical rotation limits. if it's too far outside of these the rotational calculations bug out and you will probably end up upside down with half your controls inverted.")]
    public float startRotX = 300f;

    public bool enableStartYrotation = true;
    public float startRotY = 0f;

    [Tooltip("How far from the camera rig, should the camera start.")]
    public bool enableStartZoom = true;
    public float startZoom = 20f;
    #endregion
    #region object variables
    [Space]
    [Header("Don't change these if you don't know what your doing")]
    [Tooltip("This should be the object that is attached to the RTSCamRig and should have a camera attached to it as a child")]
    public GameObject rotRig;
    [Tooltip("the cameras object, attached to the RotRig as a child")]
    public GameObject cameraTransform;//set this to cam attached to rig
    #endregion
    /// ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////// end of variables
    void InputHandler()
    {
        //input manager inputs. 
        //Create Axis entries in the input manager so unity quits bugging you with errors.
        // the bit between quotes has to match Axes name, in project Settings/Input manager.
        //Make sure to set a button in both the negative and positive section of an axes.
        //You can have multiple axes of the same name to add many keybinds to single input here. 
        //If you know how, you could also replace these with hardcoded values, the new input system or a system from the asset store. just make or convert their outputs to a float between -1 and 1, with it sitting at 0 when nothing is pressed.
        // more info here: https://docs.unity3d.com/Manual/class-InputManager.html
        // for mapping controller keys: https://answers.unity.com/questions/1350081/xbox-one-controller-mapping-solved.html


        /////////////////////////////////////////////////////////////////////////////////////////////////////////////////// remove the */ and /* to enable these
  /*      if (enableInputManagerButtons == true)
        {
            boostAxis = Input.GetAxisRaw("Camera Boost"); //speed up or slow actions. i use shift and ctrl
            forwardBackAxis = Input.GetAxisRaw("Camera Forward And Back"); //w and s , arrow keys   
            leftRightAxis = Input.GetAxisRaw("Camera Left And Right");  //a and d , arrow keys               
            zoomAxis = Input.GetAxisRaw("Camera Zoom");   // + and -. for numbpad keys enclose them in brackets in the input manager
            tempMouseRotButtonHeld = Input.GetAxisRaw("Camera Rotate With Mouse");// the button for toggling mouse rotation. i use middle mouse.
            rotateVerticalAxis = -Input.GetAxisRaw("Camera Rotate Vertical"); // keyboard keys for rotating up and down. i use arrow keys
            rotateHorizontalAxis = -Input.GetAxisRaw("Camera Rotate Horizontal");// keyboard keys for rotating left and right. i use arrow keys
            tempMouseY = -Input.GetAxisRaw("Camera Rotate With Mouse X") * 10; //the mouse up and down, you can use joystics and such here too
            tempMouseRotX = -Input.GetAxisRaw("Camera Rotate With Mouse Y") * 10; //the mouse left and right, you can use joystics and such here too
            tempMouseRotButtonHeld = Input.GetAxisRaw("Camera Rotate With Mouse Toggle");
        }
  */
        /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        //end input manager inputs

        //Hardcoded inputs
        // Keynames can be found here: https://docs.unity3d.com/ScriptReference/KeyCode.html
        //put the button you want between the "", making sure its spelt correctly including capitals.
        //to add more keys to a single action write :
        //Input.GetKey(Keycode.KeyName) ||
        //after the first ( of a line.
        // || means "or", as in "if this key or this key is pressed"
        //use && for "if this key and this key is pressed"

        if (enableHardCodedButtons == true)
        {
            if (Input.GetKey(KeyCode.LeftShift)) { boostAxis = 1; }//speed up
            if (Input.GetKey(KeyCode.LeftControl)) { boostAxis = -1; }//slow down
            if (Input.GetKey(KeyCode.W)) { forwardBackAxis = 1f; } //move forward
            if (Input.GetKey(KeyCode.S)) { forwardBackAxis = -1; } //move Backward
            if (Input.GetKey(KeyCode.A)) { leftRightAxis = -1; } //move left
            if (Input.GetKey(KeyCode.D)) { leftRightAxis = 1; } //move right
            if (Input.GetKey(KeyCode.Equals) || Input.GetKey(KeyCode.KeypadPlus)) { zoomAxis = 1; } //zoom in
            if (Input.GetKey(KeyCode.Minus) || Input.GetKey(KeyCode.KeypadMinus)) { zoomAxis = -1; }//zoom out
            if (Input.GetKey(KeyCode.R) || Input.GetKey(KeyCode.UpArrow)) { rotateVerticalAxis = -1; } //rotate up
            if (Input.GetKey(KeyCode.F) || Input.GetKey(KeyCode.DownArrow)) { rotateVerticalAxis = 1; } //rotate Down
            if (Input.GetKey(KeyCode.Q) || Input.GetKey(KeyCode.LeftArrow)) { rotateHorizontalAxis = 1; } //rotate Left
            if (Input.GetKey(KeyCode.E) || Input.GetKey(KeyCode.RightArrow)) { rotateHorizontalAxis = -1; } //rotate Right

        }
        //End hardcoded inputs

        //Default controller support, may be added in the future if theres interest.
    }

    void MousRotation()
    {
        //mouse rotation


        if (toggleRotateWithMouse == true)
        {
            if (tempMouseRotButtonHeld != 0)
            {
                if (RotatingWithMouse == true) { RotatingWithMouse = false; } //toggle mouse rotation
                else { RotatingWithMouse = true; }
            }

            if (RotatingWithMouse == true)// apply rotation when toggled on
            {
                rotateVerticalAxis = rotateVerticalAxis + tempMouseRotX;
                rotateHorizontalAxis = rotateHorizontalAxis + tempMouseY;
            }
        }
        else if (toggleRotateWithMouse != true && tempMouseRotButtonHeld != 0)// if this button is held the mouse will rotate camera
        {
            rotateVerticalAxis = rotateVerticalAxis + tempMouseRotX;
            rotateHorizontalAxis = rotateHorizontalAxis + tempMouseY;
        }

        Debug.Log(rotateHorizontalAxis);
    }
    void Start()
    {
        if(disableBothInputs == true) //disable both inputs
        {
            enableHardCodedButtons = false;
            enableInputManagerButtons = false;
        }
        if( enableHardCodedButtons == false && enableInputManagerButtons == false && disableBothInputs == false) //Enable hardcoded inputs if no inputs are enabled and diasble both inputs isn't ticked.
        {
            enableHardCodedButtons = true;
        }

        MouseState();
        vec3Move = transform.position;
        StartingPosition();
    }

    void LateUpdate() //when our methods actually do their thing, order matters.
    {
        InputHandler();
        if (enableEdgePanning == true) { ScreenEdgePan(); }
        SetMoveSpeeds();
        if (EnablePanning == true) { HorizontalMovement(); }

        if (EnableZoom == true) { Zoom(); }
        MousRotation();
        if (EnableVerticalRotation == true) { VerticalRotation(); }
        if (EnableHorizontalRotation == true) { Rotation(); }


        if (enableHardCodedButtons == true) //set axis back to 0 at the end of the loop, since hardcoded buttons don't return 0 by default, like "GetAxis" does
        {
            boostAxis = 0;
            forwardBackAxis = 0;
            leftRightAxis = 0;
            zoomAxis = 0;
            rotateVerticalAxis = 0;
            rotateHorizontalAxis = 0;
            tempMouseRotButtonHeld = 0;

        }
    }

    void MouseState() //locking and hiding mouse
    {
        if(lockMouse == true)
        {
            Cursor.lockState = CursorLockMode.Confined;
        }

        if(hideMouse == true)
        {
            Cursor.lockState = CursorLockMode.Locked;
        }
    }
    void SetMoveSpeeds() 
    {
        moveSpeed = normalSpeed;
        zoomMoveSpeed = zoomSpeed;
        rotateVerticalMoveSpeed = rotateSpeedVert;
        RotateHorizontalMoveSpeed = rotateSpeed;
 

       if (panSpeedBoost == true)
        {
            if (boostAxis > 0)//positive button/axis
            {
                moveSpeed = fastSpeed;
            }

            else if (boostAxis < 0) //negative button/axis
            {
                moveSpeed = slowSpeed;
            }
        }         
        
       if (zoomSpeedBoost == true)
       {
            if (boostAxis > 0)//positive button/axis
            {
                zoomMoveSpeed = fastZoomSpeed;
            }

            else if (boostAxis < 0) //negative button/axis
            {
                zoomMoveSpeed = slowZoomSpeed;
            }
       }         
        
       if (rotateVerticalSpeedBoost == true)
       {
           if (boostAxis < 0)//positive button/axis
           {
                rotateVerticalMoveSpeed = slowVerticalRotateSpeed;
           }

           else if (boostAxis > 0) //negative button/axis
           {
               rotateVerticalMoveSpeed = fastVerticalRotateSpeed;          
           }
        }         
        
        if (RotateHorizontalSpeedBoost == true)
        {
            if (boostAxis < 0)//positive button/axis
            {
                RotateHorizontalMoveSpeed = slowRotateSpeed;
            }

            else if (boostAxis > 0) //negative button/axis
            {
                RotateHorizontalMoveSpeed = fastRotateSpeed;
            }
        }         
    }

    void ScreenEdgePan() 
    {
        if(Input.mousePosition.y >= Screen.height * 0.99) //forward
        {
            forwardBackAxis = 1;
        }     
        
        if(Input.mousePosition.y <= Screen.height * 0.01) //back
        {
            forwardBackAxis = -1;
        }       
        
        if(Input.mousePosition.x >= Screen.width * 0.99) //left
        {
            leftRightAxis = 1;
        }        
        
        if(Input.mousePosition.x <= Screen.width * 0.01) //right
        {
            leftRightAxis = -1;
        }       
    }

    void HorizontalMovement() //x and z movement handler, using old/current input manager
    {

        if (leftRightAxis != 0)
        {
            vec3Move += transform.right * (moveSpeed / 150) * leftRightAxis;
        }

        if (forwardBackAxis != 0)
        {
            vec3Move += transform.forward * (moveSpeed / 150)  * forwardBackAxis;
        }

        transform.position = Vector3.Lerp(transform.position, vec3Move, Time.deltaTime * moveTime); //lerp makes it smoothly slide. Add a toggle for this in settings later.
    }

    void Rotation()
    {
        transform.Rotate(0,(RotateHorizontalMoveSpeed * Time.deltaTime) * rotateHorizontalAxis, 0); //left and right Y. doesnt need any fancy clamping or localizing, because its a different object.
    }

    void VerticalRotation()
    {

        Vector3 xClampVect = rotRig.transform.eulerAngles; //get current 360 rotation and convert it to inspector roation

        xClampVect.x += (rotateVerticalMoveSpeed * Time.deltaTime) * -rotateVerticalAxis; //increase our variables rotation if button axis is recieved. the - just inverts the input, so positive is up and negative is down.

        if(minXRot < 90) { minXRot = 90; } //ensures limits cant exceed breaking points
        if(maXrot > 359) { maXrot = 359; }
        xClampVect.x = Mathf.Clamp(xClampVect.x, minXRot, maXrot); //if the value is outside fo the min and max, it will clamp the axis to the min or max
        xClampVect.y = Mathf.Clamp(xClampVect.y, 0, 0); // the Y axis spazzed out in one of my tests, not sure why, but now it can't.
        rotRig.transform.localRotation = Quaternion.Euler(xClampVect); //Set rotRigs quaternion to xClampVect. Quaternion.euler converts it back from inspector values to 360 degrees.
    }

    void Zoom()
    {
        zoomDistance = Vector3.Distance(cameraTransform.transform.position, transform.position);

        if (zoomMin < 1) { zoomMin = 1; } // maintains a minimum zoom of atleast 1, so that it won't zoom through the camera Rig.


        if (EnableMousewheelZoom == true)
        {
            zoomAxis = zoomAxis + Input.mouseScrollDelta.y * 42; //scrollwheel
        }

        if (limitZoom == true) //zoom limits
        {
            if (zoomAxis != 0 && zoomDistance < zoomMin + 1.00001 * (zoomAxis* moveSpeed *Time.deltaTime))//if cam is whithin 1.00001 x zoomMin. To prevent a single move calculation from exceeding the limit,
            {
                zoomAxis = -Mathf.Clamp(zoomAxis, 0, 99999); //then prevent zooming in further
            }

            else if (zoomAxis != 0 && zoomDistance > zoomMax + 1.00001 * (zoomAxis* moveSpeed *Time.deltaTime))//if cam is within 1.00001 x zoomMax.
            {
               zoomAxis = Mathf.Clamp(zoomAxis, 0, 99999);//then prevent zooming out further
            }
        }

        if (limitZoom == false) // prevent zooming past the camera rig, when limits are off
        {
            if (zoomAxis != 0 && zoomDistance < zoomMin + 1.00001 * (zoomAxis * moveSpeed * Time.deltaTime))//if cam is whithin 1.00001 x zoomMin. To prevent a single move calculation from exceeding the limit,
            {
                zoomAxis = -Mathf.Clamp(zoomAxis, 0, 99999); //then prevent zooming in further
            }
        }

        if (zoomAxis != 0)// Moves our camera toward and away from the cameraRig.
        {
            cameraTransform.transform.LookAt(gameObject.transform); //ensures our camera is always looking at our rig.
            cameraTransform.transform.Translate(Vector3.forward * zoomAxis * zoomMoveSpeed * Time.deltaTime); //moves the camera back or forth whereever it is looking, which of course is the cameraRig.
        }

    }

    void StartingPosition()
    {
        if(enableStartPosition == true)
        {
            startVector3.x = startX;
            startVector3.y = startY;
            startVector3.z = startZ;
            transform.position = startVector3;
        }

        if(enableStartXrotation == true)
        {
            rotRig.transform.localRotation = Quaternion.Euler(startRotX, 0, 0);
        }    
        
        if(enableStartYrotation == true)
        {
            transform.Rotate(0, startRotY, 0);
        }

        if(enableStartZoom == true)
        {
                        cameraTransform.transform.Translate(Vector3.forward * -startZoom);
        }
        cameraTransform.transform.LookAt(gameObject.transform);
    }
}
