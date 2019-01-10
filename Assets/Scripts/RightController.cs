/*  ControllerTest.cs
 *  Written by Evan Suma Rosenberg, Ph.D.
 *  University of Minnesota
 *  Email: suma@umn.edu
 */
/* 11-22-2018
* Modified by Danhua Zhang, Master
* University of Minnesota
* Email: zhan5954@umn.edu
*/

/* This script is attached to the clipping plane and
 * used for implementing the function of right controller.
 * The orientation of the controller is used to rotate subcubes.
 */

using UnityEngine;

public class RightController : MonoBehaviour
{
    private SteamVR_TrackedController controller;
    private Plane cplane;
    private Clipping clip;
    private Clipping inertia_clip;

    private bool start_rotate;  // true if the cube is currently rotating and lock the calculation of dimension
    private bool clockwise;     // true if rotating clockwise
    private bool inertia1;      // true if need round in mode1
    private bool inertia2;      // true if need round in mode2
    private bool gripdown;      // true if the grip button is pressed
    private bool right;
    private int mode;           // 1: rotate by joystick
                                // 2: rotate by grip button
    private int dim_No;         // current dimension to be rotated, of value 1, 2, 3
    private int inertia_dim_No; // round rotation needed in mode2

    private float lastangle;    // the rotated angle of last frame
    private float rotate_angle; // the rotate delta between subsequent frames in mode1
    private float accum_angle;  // the total rotate delta in mode1
    private float rotate_angle2;// the rotate delta between subsequent frames in mode2
    private float accum_angle2; // the total rotate delta in mode2

    private float frame;        // number of frames to update each rotation in mode1
    private float speed;        // the speed of rotation in mode1

    private float joystick_offset;  // only beyond this threshold will the cube be rotated in mode2

    public bool debugMessages = false;
    public GameObject cube;

    // Use this for initialization
    private void Start ()
    {
        // The initial normal is pointing up
        Vector3 normal = new Vector3(0, 1, 0);
        cplane = new Plane(normal, transform.position);

        start_rotate = false;
        clockwise = true;
        inertia1 = false;
        inertia2 = false;
        gripdown = false;
        right = true;
        mode = 1;

        dim_No = 0;

        frame = 10;
        speed = 6;

        rotate_angle = 90 / frame;
        accum_angle = 0;
        accum_angle2 = 0;
        
        joystick_offset = 0.9f;

    }

    // Registers the controller event handlers
    private void OnEnable()
    {
        controller = GetComponent<SteamVR_TrackedController>();
        controller.TriggerClicked += OnTriggerClicked;
        controller.TriggerUnclicked += OnTriggerUnclicked;
        controller.MenuButtonClicked += OnMenuButtonClicked;
        controller.MenuButtonUnclicked += OnMenuButtonUnclicked;
        controller.TriggerClicked += OnTriggerClicked;
        controller.PadClicked += OnPadClicked;
        controller.PadUnclicked += OnPadUnclicked;
        controller.PadTouched += OnPadTouched;
        controller.PadUntouched += OnPadUntouched;
        controller.Gripped += OnGripped;
        controller.Ungripped += OnUngripped;
    }

    // Unregisters the controller event handlers
    private void OnDisable()
    {
        controller.TriggerClicked -= OnTriggerClicked;
        controller.TriggerUnclicked -= OnTriggerUnclicked;
        controller.MenuButtonClicked -= OnMenuButtonClicked;
        controller.MenuButtonUnclicked -= OnMenuButtonUnclicked;
        controller.TriggerClicked -= OnTriggerClicked;
        controller.PadClicked -= OnPadClicked;
        controller.PadUnclicked -= OnPadUnclicked;
        controller.PadTouched -= OnPadTouched;
        controller.PadUntouched -= OnPadUntouched;
        controller.Gripped -= OnGripped;
        controller.Ungripped -= OnUngripped;
    }

    // Calculate the dimension with which intersected the cube 
    // and distance from the cube center to the plane.
    // If the plane is intersected with the negative axis of the cube
    // the distance will be the opposite number of its absolute value. 
    // The plane here means cplane, without size restriction.
    // Only called when the GameObject plane collide with cube.
    private void ClipDetect()
    {        
        Vector3 cube_x = (cube.transform.rotation * new Vector3(1, 0, 0)).normalized;
        Vector3 cube_y = (cube.transform.rotation * new Vector3(0, 1, 0)).normalized;
        Vector3 cube_z = (cube.transform.rotation * new Vector3(0, 0, 1)).normalized;
        Vector3 PlaneCenterInCubeLocal = cube.transform.InverseTransformPoint(transform.position);

        clip.distance = Mathf.Abs(cplane.GetDistanceToPoint(cube.transform.position));

        if (Mathf.Abs(Vector3.Dot(cplane.normal, cube_x)) >= 3 / Mathf.Sqrt(10))
        {
            clip.dim = 'x';
            if(clip.distance < 1.5 * cube.GetComponent<RubikCube>().GetSize())
            {
                GetComponent<Renderer>().material.color = Color.red;
                GameObject stick = transform.Find("Cube").gameObject;
                if (stick != null)
                    stick.GetComponent<Renderer>().material.color = Color.red;
            }
            else
            {
                GetComponent<Renderer>().material.color = Color.white;
                GameObject stick = GameObject.Find("Cube");
                if (stick != null)
                    stick.GetComponent<Renderer>().material.color = Color.white;
            }                
            if (PlaneCenterInCubeLocal.x < 0)
                clip.distance = -clip.distance;
        }
        else if (Mathf.Abs(Vector3.Dot(cplane.normal, cube_y)) >= 3 / Mathf.Sqrt(10))
        {
            clip.dim = 'y';
            if (clip.distance < 1.5 * cube.GetComponent<RubikCube>().GetSize())
            {
                GetComponent<Renderer>().material.color = Color.green;
                GameObject stick = GameObject.Find("Cube");
                if (stick != null)
                    stick.GetComponent<Renderer>().material.color = Color.green;
            }
            else
            {
                GetComponent<Renderer>().material.color = Color.white;
                GameObject stick = GameObject.Find("Cube");
                if (stick != null)
                    stick.GetComponent<Renderer>().material.color = Color.white;
            }
            if (PlaneCenterInCubeLocal.y < 0)
                clip.distance = -clip.distance;
        }
        else if (Mathf.Abs(Vector3.Dot(cplane.normal, cube_z)) >= 3 / Mathf.Sqrt(10))
        {
            clip.dim = 'z';
            if (clip.distance < 1.5 * cube.GetComponent<RubikCube>().GetSize())
            {
                GetComponent<Renderer>().material.color = Color.blue;
                GameObject stick = GameObject.Find("Cube");
                if (stick != null)
                    stick.GetComponent<Renderer>().material.color = Color.blue;
            }
            else
            {
                GetComponent<Renderer>().material.color = Color.white;
                if (mode == 2)
                {
                    GameObject stick = GameObject.Find("Cube");
                    if (stick != null)
                        stick.GetComponent<Renderer>().material.color = Color.white;
                }
            }
                
            if (PlaneCenterInCubeLocal.z < 0)
                clip.distance = -clip.distance;
        }
        else
        {
            clip.dim = 'n';
            GetComponent<Renderer>().material.color = Color.white;
            GameObject stick = GameObject.Find("Cube");
            if (!stick)
                stick.GetComponent<Renderer>().material.color = Color.white;
        }
    }

    // Update is called once per frame
    private void LateUpdate ()
    {
        if (!start_rotate)
        {
            Vector3 normal = (transform.rotation * new Vector3(0, 1, 0)).normalized;
            cplane = new Plane(normal, transform.position);
        }
        
        mode = cube.GetComponent<LeftController>().Mode();

        if (mode == 1)
        {
            transform.localScale = new Vector3(0.01f, 1, 0.01f);
            UpdateMode1();
        }
        else if (mode == 2)
        {
            transform.localScale = new Vector3(0.001f, 1, 0.02f);
            UpdateMode2();
        }

        lastangle = transform.localEulerAngles.y;
    }

    private void UpdateMode1()
    {
        if (!start_rotate)
            ClipDetect();

        //the plane is parallel to one dimension
        //float distance_cube_plane = Vector3.Distance(cube.transform.position, transform.position);
        float size = cube.GetComponent<RubikCube>().GetSize();
        //if (clip.dim != 'n' && distance_cube_plane < 1.5*size)
        if (clip.dim != 'n')
        {
            if (0.5 * size < clip.distance && clip.distance < 1.5 * size)
            {
                dim_No = 3;
                if (accum_angle < 90)
                {
                    if (controller.controllerState.rAxis2.x >= joystick_offset)
                    {
                        start_rotate = true;
                        accum_angle += rotate_angle;
                        right = true;
                        print("Rotate " + clip.dim + " " + dim_No + " clockwise " + clockwise);
                        cube.GetComponent<RubikCube>().RotateMode1(clip.dim, dim_No, rotate_angle, accum_angle, cplane.normal, right);
                    }
                    else if (controller.controllerState.rAxis2.x <= -joystick_offset)
                    {
                        start_rotate = true;
                        accum_angle += rotate_angle;
                        right = false;
                        print("Rotate " + clip.dim + " " + dim_No + " clockwise " + clockwise);
                        cube.GetComponent<RubikCube>().RotateMode1(clip.dim, dim_No, rotate_angle, accum_angle, cplane.normal, right);
                    }
                }
                else
                    start_rotate = false;
            }
            else if (-0.5 * size < clip.distance && clip.distance < 0.5 * size)
            {
                dim_No = 2;
                if (accum_angle < 90)
                {
                    if (controller.controllerState.rAxis2.x >= joystick_offset)
                    {
                        start_rotate = true;
                        accum_angle += rotate_angle;
                        right = true;
                        print("Rotate " + clip.dim + " " + dim_No + " clockwise " + clockwise);
                        cube.GetComponent<RubikCube>().RotateMode1(clip.dim, dim_No, rotate_angle, accum_angle, cplane.normal, right);
                    }
                    else if (controller.controllerState.rAxis2.x <= -joystick_offset)
                    {
                        start_rotate = true;
                        accum_angle += rotate_angle;
                        right = false;
                        print("Rotate " + clip.dim + " " + dim_No + " clockwise " + clockwise);
                        cube.GetComponent<RubikCube>().RotateMode1(clip.dim, dim_No, rotate_angle, accum_angle, cplane.normal, right);
                    }
                }
                else
                    start_rotate = false;
            }
            else if (-1.5 * size < clip.distance && clip.distance < -0.5 * size)
            {
                dim_No = 1;
                if (accum_angle < 90)
                {
                    if (controller.controllerState.rAxis2.x >= joystick_offset)
                    {
                        start_rotate = true;
                        accum_angle += rotate_angle;
                        right = true;
                        print("Rotate " + clip.dim + " " + dim_No + " clockwise " + clockwise);
                        cube.GetComponent<RubikCube>().RotateMode1(clip.dim, dim_No, rotate_angle, accum_angle, cplane.normal, right);
                    }
                    else if (controller.controllerState.rAxis2.x <= -joystick_offset)
                    {
                        start_rotate = true;
                        accum_angle += rotate_angle;
                        right = false;
                        print("Rotate " + clip.dim + " " + dim_No + " clockwise " + clockwise);
                        cube.GetComponent<RubikCube>().RotateMode1(clip.dim, dim_No, rotate_angle, accum_angle, cplane.normal, right);
                    }
                }
                else
                    start_rotate = false;
            }
        }

        if (Mathf.Abs(controller.controllerState.rAxis2.x) < joystick_offset && Mathf.Abs(controller.controllerState.rAxis2.y) < joystick_offset)
        {
            if (0 < accum_angle && accum_angle < 90)
            {
                inertia1 = true;
                start_rotate = true;
            }
            else
            {
                inertia1 = false;
                start_rotate = false;
                accum_angle = 0;
            }
        }

        if (inertia1)
        {
            if (accum_angle < 90)
            {
                accum_angle += rotate_angle;
                cube.GetComponent<RubikCube>().RotateMode1(clip.dim, dim_No, rotate_angle, accum_angle, cplane.normal, right);
            }
            else
            {
                inertia1 = false;
                start_rotate = false;
                accum_angle = 0;
            }
        }


    }

    private void UpdateMode2()
    {
        if (!start_rotate) // lock the dimension when the cube is rotating
            ClipDetect();

        if (gripdown)
        {
            //the plane is parallel to one dimension
            float size = cube.GetComponent<RubikCube>().GetSize();

            rotate_angle2 = transform.localEulerAngles.y - lastangle;
            if (rotate_angle2 > 200) // anticlockwise and there is a gap, looking down
            {
                rotate_angle2 = rotate_angle2 - 360;
            }
            else if (rotate_angle2 < -200) // clock wise and there is a gap, looking down
            {
                rotate_angle2 = 360 + rotate_angle2;
            }

            rotate_angle2 = Mathf.Abs(rotate_angle2);

            if (clip.dim != 'n')
            {
                rotate_angle2 *= speed;
                if (0.5 * size < clip.distance && clip.distance < 1.5 * size)
                {
                    start_rotate = true;
                    accum_angle2 += rotate_angle2;
                    dim_No = 3;
                    accum_angle2 = cube.GetComponent<RubikCube>().RotateMode2(clip.dim, 3, rotate_angle2, accum_angle2, cplane.normal);
                }
                else if (-0.5 * size < clip.distance && clip.distance < 0.5 * size)
                {
                    start_rotate = true;
                    accum_angle2 += rotate_angle2;
                    dim_No = 2;
                    accum_angle2 = cube.GetComponent<RubikCube>().RotateMode2(clip.dim, 2, rotate_angle2, accum_angle2, cplane.normal);
                }
                else if (-1.5 * size < clip.distance && clip.distance < -0.5 * size)
                {
                    start_rotate = true;
                    accum_angle2 += rotate_angle2;
                    dim_No = 1;
                    accum_angle2 = cube.GetComponent<RubikCube>().RotateMode2(clip.dim, 1, rotate_angle2, accum_angle2, cplane.normal);
                }
            }
        }
        if (inertia2)
        {
            if (Mathf.Abs(accum_angle2) < 45)
            { // no need to update dimension
                rotate_angle2 = -accum_angle2;
                accum_angle2 = 0;
            }
            else if (Mathf.Abs(accum_angle2) >= 45)
            { // need to update dimension
                if (accum_angle2 > 0)
                {
                    rotate_angle2 = 90 - accum_angle2;
                    accum_angle2 = 90;
                }
                else
                {
                    rotate_angle2 = -90 - accum_angle2;
                    accum_angle2 = -90;
                }
            }

            cube.GetComponent<RubikCube>().RotateMode2(inertia_clip.dim, inertia_dim_No, rotate_angle2, accum_angle2, cplane.normal);
            accum_angle2 = 0;
            inertia2 = false;
        }

        lastangle = transform.localEulerAngles.y;
    }

    private void OnGripped(object sender, ClickedEventArgs e)
    {
        if (mode == 2)
        {
            gripdown = true;
            Debug.Log("Grip button clicked.");
        } 
    }

    private void OnUngripped(object sender, ClickedEventArgs e)
    {
        if (mode == 2)
        {
            gripdown = false;
            if (accum_angle2 != 0)
            {
                inertia2 = true;
                inertia_clip = clip;
                inertia_dim_No = dim_No;
            }
            start_rotate = false;

            Debug.Log("Grip button unclicked.");
        }
    }

    private void OnTriggerClicked(object sender, ClickedEventArgs e)
    {
        if(debugMessages)
        {
            Debug.Log("Trigger clicked.");
        }
    }

    private void OnTriggerUnclicked(object sender, ClickedEventArgs e)
    {
        if (debugMessages)
        {
            Debug.Log("Trigger unclicked.");
        }
    }

    private void OnMenuButtonClicked(object sender, ClickedEventArgs e)
    {
        if (debugMessages)
        {
            Debug.Log("Menu button clicked.");
        }
    }

    private void OnMenuButtonUnclicked(object sender, ClickedEventArgs e)
    {
        if (debugMessages)
        {
            Debug.Log("Menu button unclicked.");
        }
    }

    private void OnPadClicked(object sender, ClickedEventArgs e)
    {
        if (debugMessages)
        {
            Debug.Log("Pad clicked.");
        }
    }

    private void OnPadUnclicked(object sender, ClickedEventArgs e)
    {
        if (debugMessages)
        {
            Debug.Log("Pad unclicked.");
        }
    }

    private void OnPadTouched(object sender, ClickedEventArgs e)
    {
        if (debugMessages)
        {
            Debug.Log("Pad touched.");
        }
    }

    private void OnPadUntouched(object sender, ClickedEventArgs e)
    {
        if (debugMessages)
        {
            Debug.Log("Pad untouched.");
        }
    }
}
