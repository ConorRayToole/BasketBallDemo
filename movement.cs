//created by Conor Toole

using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class movement : MonoBehaviour
{
    public float moveSpeed = 10;
    public Transform Ball;
    public Transform shooting;
    public Transform dribble;
    public Transform Target;
    public Transform Start;

    public Vector3 v;

    private bool BallInHands = true;
    private bool BallInAir = false;
    private float distX = 0; //distance between player and hoop in x
    private float distZ = 0;
    private bool D = false; //false for x true for z
    private float T = 0;
    private float hold = -1f;
    private float holdL = 1f;

    void Update()
    {
        distX = Mathf.Abs(transform.position.x - Start.position.x);
        distZ = Mathf.Abs(transform.position.z - Start.position.z);
        if (distX > distZ){
            D = false;
        }
        else {
            D = true;
        }

        v.y = Start.position.y;
        //  makes sure that ball only misses in the direction of the shooter
        if (D == false){
            v.z = Start.position.z;
        }
        else if (D == true){
            v.x = Start.position.x;
        }
        
        Target.position = Start.position;

        Vector3 direction = new Vector3(Input.GetAxisRaw("Vertical"), 0, ((Input.GetAxisRaw("Horizontal"))*-1));
        transform.position += direction * moveSpeed * Time.deltaTime;

        if (BallInHands) {
            if (Input.GetKey(KeyCode.Space)) { //hold to shoot
                if (transform.position.z > 0 && D == true){ //left side of hoop
                    if (holdL > - 1f){
                        holdL = holdL - 0.01f;
                    } 
                }
                else{ //right side of hoop and front side
                    if (hold < 1f){
                        hold += 0.01f;
                    } 
                }
                
                
                Ball.position = shooting.position;
                transform.LookAt(Target.parent.position);
            }
            else{ //dribbling
                Ball.position = dribble.position + Vector3.up * Mathf.Abs(Mathf.Sin(Time.time*5));
                transform.LookAt(transform.position + direction);   
            }

            if (Input.GetKeyUp(KeyCode.Space)) {  
                BallInHands = false;
                BallInAir = true;
                T = 0;
            }
        }

        if (BallInAir){
            //shooting mechanic
            if (D == false){ // shooting from front
                v.x = hold + 5;
            }
            else if (transform.position.z > 0){ //shooting from left
                v.z = holdL;
            }
            else { //shooting from right
                v.z = hold;
            }
            Target.position = v;
            ////////////////

            T += Time.deltaTime;
            float duration = 0.5f;
            float t01 = T / duration;

            Vector3 A = shooting.position;
            Vector3 B = Target.position;
            Vector3 pos = Vector3.Lerp(A,B, t01);

            //Arc
            Vector3 arc = Vector3.up * 2 *  Mathf.Sin(t01 * 3.14f);
            Ball.position = pos + arc;

            if (t01 >= 1) { //once ball reaches target it turns into rigid body 
                BallInAir = false;
                hold = -1f; 
                holdL = 1f;
                Ball.GetComponent<Rigidbody>().isKinematic = false;
            }
        }
    }
    
    //pick up ball
    private void OnTriggerEnter(Collider other) { //triggers when two object collide aka player and ball
        if (!BallInHands && !BallInAir) {
            BallInHands = true;
            Ball.GetComponent<Rigidbody>().isKinematic = true;
        }
    }
}
