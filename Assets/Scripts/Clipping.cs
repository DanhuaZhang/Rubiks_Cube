using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct Clipping {
    // dim records the dimension with which the clipping plane is intersecting with
    // distance is the distance from the center of the rubik's cube to the clipping plane
    public char dim;
    public float distance;
}
