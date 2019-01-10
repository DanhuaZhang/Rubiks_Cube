using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RubikCube : MonoBehaviour {
    private float size;
    private int dim;

    private Transform center_left;
    private Transform center_right;
    private Transform center_up;
    private Transform center_down;
    private Transform center_forward;
    private Transform center_backward;
    private Transform center;

    // Use this for initialization
    void Start () {
        size = 0.024f;
        dim = 3;

        for (int k = 0; k < dim; k++)
        {// z
            for (int j = 0; j < dim; j++)
            {// y
                for (int i = 0; i < dim; i++)
                {// x
                    int index = i + j * dim + k * dim * dim + 1;
                    // transform other cubes according to the center's position and size
                    Transform temp = transform.Find(index.ToString());
                    if (temp != null)
                    {
                        temp.transform.localScale = new Vector3(size, size, size);
                        float x = (i - 1) * size;
                        float y = -(j - 1) * size;
                        float z = (k - 1) * size;
                        temp.transform.localPosition = new Vector3(x, y, z);
                        temp.GetComponent<LocalIndex>().dimx = i + 1;
                        temp.GetComponent<LocalIndex>().dimy = j + 1;
                        temp.GetComponent<LocalIndex>().dimz = k + 1;
                    }
                    else Debug.Log("No child with the name " + index.ToString() + "attached to the cube");
                }
            }          
        }

        UpdateCenters();
    }

    // Calculate the center of the cubes to be rotated 
    // based on the intersectied dimension with number ID dim_No
    public Transform GetRotateCenter(char dimension, int dim_No)
    {
        Transform rotate_center = transform;

        if (dim_No == 2)
        {
            rotate_center = center;
        }
        else if (dimension == 'x')
        {
            if (dim_No == 1)
            {
                rotate_center = center_left;
            }
            else if (dim_No == 3)
            {
                rotate_center = center_right;
            }
        }
        else if (dimension == 'y')
        {
            if (dim_No == 1)
            {
                rotate_center = center_up;
            }
            else if (dim_No == 3)
            {
                rotate_center = center_down;
            }
        }
        else if (dimension == 'z')
        {
            if (dim_No == 1)
            {
                rotate_center = center_forward;
            }
            else if (dim_No == 3)
            {
                rotate_center = center_backward;
            }
        }

        return rotate_center;
    }

    // Rotate the intersected plane
    // "dimension" is the intersectied dimension
    // the clipping plane is intersected with the "dim_No"th dimension
    // clockwise determines the rotate orientation
    public void RotateMode1(char dimension, int dim_No, float rotate, float accum_rotate, Vector3 normal, bool right)
    {
        bool clockwise = true;
        

        if (dimension == 'y')
            dim_No = 4 - dim_No;

        // axis around which the subcubes to be rotated in local space
        Vector3 x = (transform.rotation * new Vector3(1, 0, 0)).normalized;
        Vector3 y = (transform.rotation * new Vector3(0, 1, 0)).normalized;
        Vector3 z = (transform.rotation * new Vector3(0, 0, 1)).normalized;
        Transform rotate_center = transform;

        // modify the parent of each subcube
        for (int k = 0; k < dim; k++)
        {// z
            for (int j = 0; j < dim; j++)
            {// y
                for (int i = 0; i < dim; i++)
                {// x
                    int index = i + j * dim + k * dim * dim + 1;
                    Transform temp = transform.Find(index.ToString());
                    rotate_center = GetRotateCenter(dimension, dim_No);

                    if ((dimension == 'x' && temp.GetComponent<LocalIndex>().dimx == dim_No) ||
                        (dimension == 'y' && temp.GetComponent<LocalIndex>().dimy == dim_No) ||
                        (dimension == 'z' && temp.GetComponent<LocalIndex>().dimz == dim_No))
                    {
                        temp.parent = rotate_center;
                        temp.tag = "modified";
                    }
                }
            }
        }

        if (dimension == 'x')
        {
            if (Vector3.Dot(normal, x) > 0)
            {
                if (right)
                    clockwise = true;
                else
                {
                    clockwise = false;
                    rotate = -rotate;
                }
            }
            else
            {
                if (right)
                {
                    clockwise = false;
                    rotate = -rotate;
                }
                else
                    clockwise = true;
            }
                
            print("rotate x");
            rotate_center.Rotate(x, rotate, Space.World);
        }
        else if (dimension == 'y')
        {
            if (Vector3.Dot(normal, y) > 0)
            {
                if (right)
                    clockwise = true;
                else
                {
                    clockwise = false;
                    rotate = -rotate;
                }
            }
            else
            {
                if (right)
                {
                    clockwise = false;
                    rotate = -rotate;
                }
                else
                    clockwise = true;
            }
            print("rotate y");
            rotate_center.Rotate(y, rotate, Space.World);
        }
        else if (dimension == 'z')
        {
            if (Vector3.Dot(normal, z) > 0)
            {
                if (right)
                    clockwise = true;
                else
                {
                    clockwise = false;
                    rotate = -rotate;
                }
            }
            else
            {
                if (right)
                {
                    clockwise = false;
                    rotate = -rotate;
                }
                else
                    clockwise = true;
            }
            print("rotate z");
            rotate_center.Rotate(z, rotate, Space.World);
        }

        Transform[] children = GetComponentsInChildren<Transform>();
        
        // only update dimension when the angle is multiple of 90
        if (Mathf.Abs(accum_rotate - 90) < 0.0001f)
        {
            for (int i = 0; i < children.Length; i++)
            {
                if (children[i].tag == "modified")
                { 
                    if (dimension == 'x')
                    {
                        if (clockwise)
                        {
                            int temp = children[i].GetComponent<LocalIndex>().dimy;
                            children[i].GetComponent<LocalIndex>().dimy = children[i].GetComponent<LocalIndex>().dimz;
                            children[i].GetComponent<LocalIndex>().dimz = 4 - temp;
                        }
                        else
                        {
                            int temp = children[i].GetComponent<LocalIndex>().dimy;
                            children[i].GetComponent<LocalIndex>().dimy = 4 - children[i].GetComponent<LocalIndex>().dimz;
                            children[i].GetComponent<LocalIndex>().dimz = temp;
                        }
                    }
                    else if (dimension == 'y')
                    {
                        if (clockwise)
                        {
                            int temp = children[i].GetComponent<LocalIndex>().dimx;
                            children[i].GetComponent<LocalIndex>().dimx = children[i].GetComponent<LocalIndex>().dimz;
                            children[i].GetComponent<LocalIndex>().dimz = 4 - temp;
                        }
                        else
                        {
                            int temp = children[i].GetComponent<LocalIndex>().dimx;
                            children[i].GetComponent<LocalIndex>().dimx = 4 - children[i].GetComponent<LocalIndex>().dimz;
                            children[i].GetComponent<LocalIndex>().dimz = temp;
                        }
                    }
                    else if (dimension == 'z')
                    {
                        if (clockwise)
                        {
                            int temp = children[i].GetComponent<LocalIndex>().dimy;
                            children[i].GetComponent<LocalIndex>().dimy = 4 - children[i].GetComponent<LocalIndex>().dimx;
                            children[i].GetComponent<LocalIndex>().dimx = temp;
                            
                        }
                        else
                        {
                            int temp = children[i].GetComponent<LocalIndex>().dimy;
                            children[i].GetComponent<LocalIndex>().dimy = children[i].GetComponent<LocalIndex>().dimx;
                            children[i].GetComponent<LocalIndex>().dimx = 4 - temp;
                        }
                    }
                }
            }
        }

        // reset the parent
        for (int i = 0; i < children.Length; i++)
        {            
            if (children[i].tag == "modified")
                children[i].tag = "subcube";
            
            if (children[i].tag == "subcube")
                children[i].parent = transform;
        }

        UpdateCenters();
    }

    public float RotateMode2(char dimension, int dim_No, float rotate, float accum_rotate, Vector3 normal)
    {
        bool clockwise = true;
        //if (accum_rotate < 0)
        //{
        //    clockwise = false;
        //}

        if (dimension == 'y')
            dim_No = 4 - dim_No;

        // axis around which the subcubes to be rotated in local space
        Vector3 x = (transform.rotation * new Vector3(1, 0, 0)).normalized;
        Vector3 y = (transform.rotation * new Vector3(0, 1, 0)).normalized;
        Vector3 z = (transform.rotation * new Vector3(0, 0, 1)).normalized;
        Transform rotate_center = transform;

        // modify the parent of each subcube
        for (int k = 0; k < dim; k++)
        {// z
            for (int j = 0; j < dim; j++)
            {// y
                for (int i = 0; i < dim; i++)
                {// x
                    int index = i + j * dim + k * dim * dim + 1;
                    Transform temp = transform.Find(index.ToString());
                    rotate_center = GetRotateCenter(dimension, dim_No);

                    if ((dimension == 'x' && temp.GetComponent<LocalIndex>().dimx == dim_No) ||
                        (dimension == 'y' && temp.GetComponent<LocalIndex>().dimy == dim_No) ||
                        (dimension == 'z' && temp.GetComponent<LocalIndex>().dimz == dim_No))
                    {
                        temp.parent = rotate_center;
                        temp.tag = "modified";
                    }
                }
            }
        }

        if (dimension == 'x')
        {
            if (Vector3.Dot(normal, x) > 0)
                clockwise = true;
            else
            {
                clockwise = false;
                rotate = -rotate;
            }
            rotate_center.Rotate(x, rotate, Space.World);
        }
        else if (dimension == 'y')
        {
            if (Vector3.Dot(normal, y) > 0)
                clockwise = true;
            else
            {
                clockwise = false;
                rotate = -rotate;
            }
            rotate_center.Rotate(y, rotate, Space.World);
        }
        else if (dimension == 'z')
        {
            if (Vector3.Dot(normal, z) > 0)
                clockwise = true;
            else
            {
                clockwise = false;
                rotate = -rotate;
            }
            rotate_center.Rotate(z, rotate, Space.World);
        }

        Transform[] children = GetComponentsInChildren<Transform>();

        // update dimension
        if (Mathf.Abs(Mathf.Abs(accum_rotate) - 90) < 0.0001f)
        {
            if (accum_rotate > 0)
                accum_rotate -= 90;
            else
                accum_rotate += 90;
            for (int i = 0; i < children.Length; i++)
            {
                if (children[i].tag == "modified")
                {
                    if (dimension == 'x')
                    {
                        if (clockwise)
                        {
                            int temp = children[i].GetComponent<LocalIndex>().dimy;
                            children[i].GetComponent<LocalIndex>().dimy = children[i].GetComponent<LocalIndex>().dimz;
                            children[i].GetComponent<LocalIndex>().dimz = 4 - temp;
                        }
                        else
                        {
                            int temp = children[i].GetComponent<LocalIndex>().dimy;
                            children[i].GetComponent<LocalIndex>().dimy = 4 - children[i].GetComponent<LocalIndex>().dimz;
                            children[i].GetComponent<LocalIndex>().dimz = temp;
                        }
                    }
                    else if (dimension == 'y')
                    {
                        if (clockwise)
                        {
                            int temp = children[i].GetComponent<LocalIndex>().dimx;
                            children[i].GetComponent<LocalIndex>().dimx = children[i].GetComponent<LocalIndex>().dimz;
                            children[i].GetComponent<LocalIndex>().dimz = 4 - temp;
                        }
                        else
                        {
                            int temp = children[i].GetComponent<LocalIndex>().dimx;
                            children[i].GetComponent<LocalIndex>().dimx = 4 - children[i].GetComponent<LocalIndex>().dimz;
                            children[i].GetComponent<LocalIndex>().dimz = temp;
                        }
                    }
                    else if (dimension == 'z')
                    {
                        if (clockwise)
                        {
                            int temp = children[i].GetComponent<LocalIndex>().dimy;
                            children[i].GetComponent<LocalIndex>().dimy = 4 - children[i].GetComponent<LocalIndex>().dimx;
                            children[i].GetComponent<LocalIndex>().dimx = temp;
                        }
                        else
                        {
                            int temp = children[i].GetComponent<LocalIndex>().dimy;
                            children[i].GetComponent<LocalIndex>().dimy = children[i].GetComponent<LocalIndex>().dimx;
                            children[i].GetComponent<LocalIndex>().dimx = 4 - temp;
                        }
                    }
                }
            }
        }

        // reset the parent
        for (int i = 0; i < children.Length; i++)
        {
            if (children[i].tag == "modified")
                children[i].tag = "subcube";

            if (children[i].tag == "subcube")
                children[i].parent = transform;
        }

        UpdateCenters();

        return accum_rotate;
    }

    public void UpdateCenters()
    {
        for (int k = 0; k < dim; k++)
        {// z
            for (int j = 0; j < dim; j++)
            {// y
                for (int i = 0; i < dim; i++)
                {// x
                    int index = i + j * dim + k * dim * dim + 1;
                    Transform temp = transform.Find(index.ToString());
                    if (temp.transform.GetComponent<LocalIndex>().dimx == 2 &&
                        temp.transform.GetComponent<LocalIndex>().dimy == 2 &&
                        temp.transform.GetComponent<LocalIndex>().dimz == 2)
                    {
                        center = temp;
                    }
                    if (temp.transform.GetComponent<LocalIndex>().dimx == 1 &&
                        temp.transform.GetComponent<LocalIndex>().dimy == 2 &&
                        temp.transform.GetComponent<LocalIndex>().dimz == 2)
                    {
                        center_left = temp;
                        //center_left.transform.localPosition += new Vector3(-0.005f, 0, 0);
                    }
                    if (temp.transform.GetComponent<LocalIndex>().dimx == 3 &&
                        temp.transform.GetComponent<LocalIndex>().dimy == 2 &&
                        temp.transform.GetComponent<LocalIndex>().dimz == 2)
                    {
                        center_right = temp;
                        //center_right.transform.localPosition += new Vector3(0.005f, 0, 0);
                    }
                    if (temp.transform.GetComponent<LocalIndex>().dimx == 2 &&
                        temp.transform.GetComponent<LocalIndex>().dimy == 1 &&
                        temp.transform.GetComponent<LocalIndex>().dimz == 2)
                    {
                        center_up = temp;
                        //center_up.transform.localPosition += new Vector3(0, 0.005f, 0);
                    }
                    if (temp.transform.GetComponent<LocalIndex>().dimx == 2 &&
                        temp.transform.GetComponent<LocalIndex>().dimy == 3 &&
                        temp.transform.GetComponent<LocalIndex>().dimz == 2)
                    {
                        center_down = temp;
                        //center_down.transform.localPosition += new Vector3(0, -0.005f, 0);
                    }
                    if (temp.transform.GetComponent<LocalIndex>().dimx == 2 &&
                        temp.transform.GetComponent<LocalIndex>().dimy == 2 &&
                        temp.transform.GetComponent<LocalIndex>().dimz == 1)
                    {
                        center_forward = temp;
                        //center_forward.transform.localPosition += new Vector3(0, 0, -0.005f);
                    }
                    if (temp.transform.GetComponent<LocalIndex>().dimx == 2 &&
                        temp.transform.GetComponent<LocalIndex>().dimy == 2 &&
                        temp.transform.GetComponent<LocalIndex>().dimz == 3)
                    {
                        center_backward = temp;
                        //center_backward.transform.localPosition += new Vector3(0, 0, 0.005f);
                    }
                }
            }
        }
    }

    public float GetSize()
    {
        return size;
    }

    // Update is called once per frame
    void Update () { 
        
    }
}
