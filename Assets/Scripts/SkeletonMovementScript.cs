using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class SkeletonMovementScript : MonoBehaviour {

    public float travelingSpeed = 4f;
    public float moveLenght = 30.0f;

    // Use this for initialization
    void Start () {

    }
    IEnumerator moveForward()
    {
        float offset = gameObject.GetComponent<SkeletonMovementScript>().moveLenght;//20.0f;
        yield return gameObject.transform.DOMove((transform.position + (-transform.right) * offset), travelingSpeed, false).SetEase(Ease.Linear).WaitForCompletion();
        //Vector3[] newPos = new[] { (transform.position + (-transform.right) * offset) };      
        //yield return gameObject.transform.DOPath(newPos, 10, PathType.CubicBezier).WaitForCompletion();
       
        StartCoroutine("turnLeft");
        

    }
    IEnumerator turnLeft()
    { 
        float lookDir = gameObject.transform.eulerAngles.y;
        yield return gameObject.transform.DORotate(new Vector3(0, lookDir - 90.0f, 0), 1.5f, RotateMode.Fast).SetEase(Ease.Linear).WaitForCompletion();
     
        StartCoroutine("moveForward");
       
    }


    // Update is called once per frame
    void Update ()
    {
        
        if (Input.GetKeyDown("1"))
        {
            gameObject.GetComponent<Animator>().SetBool("isWalking", true);
            StartCoroutine("moveForward");
        }
        else if(Input.GetKeyDown("2"))
        {
            StopAllCoroutines();
            gameObject.GetComponent<Animator>().SetBool("isWalking", false);
        }

    }

   
 
}
