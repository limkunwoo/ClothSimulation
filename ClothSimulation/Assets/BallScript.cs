using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallScript : MonoBehaviour
{
    float ball_time = 0;
    
    static Vector3 Velocity;
    Vector3 prevPosition;
    // Start is called before the first frame update
    void Start()
    {
        transform.position = new Vector3(0, 2, 0);
        prevPosition = new Vector3(0, 2, 0);
    }

    // Update is called once per frame
    void Update()
    {
        ball_time++;
        Vector3 newPosition = new Vector3(0, 2, Mathf.Cos(Time.time) * 7);
        this.transform.position = newPosition;

        Velocity = newPosition - prevPosition;
        prevPosition = newPosition;
    }

    static public Vector3 GetVelocity()
    {
        return Velocity;
    }
}
