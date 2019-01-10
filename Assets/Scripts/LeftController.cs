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

using UnityEngine;

public class LeftController : MonoBehaviour
{    
    private SteamVR_TrackedController controller;

    private int mode; // 2 modes in total
    private bool switchmode;

    public bool debugMessages = false;
    public GameObject cube;

    // Use this for initialization
    private void Start ()
    {
        mode = 1;
    }

    public int Mode()
    {
        return mode;
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

    // Update is called once per frame
    private void Update()
    {
        if (debugMessages)
        {
            // if pad is touched, prints the current touchpad coordinates
            if (controller.padTouched)
            {
                Debug.Log("Pad coordinates: " + controller.controllerState.rAxis0.x + " " + controller.controllerState.rAxis0.y);
            }

            // if joystick is moved more than .15, print the joystick coordinates
            if(Mathf.Abs(controller.controllerState.rAxis2.x) > 0.15 || Mathf.Abs(controller.controllerState.rAxis2.y) > 0.15)
            {
                Debug.Log("Joystick coordinates: " + controller.controllerState.rAxis2.x + " " + controller.controllerState.rAxis2.y);
            }
        }
	}

    private void OnGripped(object sender, ClickedEventArgs e)
    {
        mode = 3 - mode;
        Debug.Log("Grip button clicked.");
    }

    private void OnUngripped(object sender, ClickedEventArgs e)
    {
        if (debugMessages)
        {
            Debug.Log("Grip button unclicked.");
        }
    }

    private void OnTriggerClicked(object sender, ClickedEventArgs e)
    {
        if (debugMessages)
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
        Debug.Log("Left controller Pad clicked.");
    }

    private void OnPadUnclicked(object sender, ClickedEventArgs e)
    {
        Debug.Log("Left controller Pad unclicked.");
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
