using System.Collections;
using UnityEngine;

public class Rotater : MonoBehaviour
{

    void Update()
    {

        transform.Rotate(new Vector3(15, 30, 45) * Time.deltaTime);

    }
}
