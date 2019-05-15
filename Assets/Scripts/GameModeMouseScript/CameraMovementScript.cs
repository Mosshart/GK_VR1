using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening; // zmienic zeby nie trzeba bylo dociagac paczki.... 

public class CameraMovementScript : MonoBehaviour {

    public enum EMouseButtons
    {
        leftBtn = 0,
        rightBtn = 1,
        middleBtn = 2
    }
    public enum EspeedMultipler
    {
        x2 = 2,
        x4 = 4,
        x8 = 8        
    }
    
    #region settings
    [Header("Speed Options")]
    [Range(1, 10)]
    public float speed = 4f;
    [Range(1, 10)]
    public float dragSpeed = 2f;
    [Range(0, 1)]
    public float scrollWheelMultipler = 0.5f;
    public EspeedMultipler speedMultipler = EspeedMultipler.x2;

    [Space(15)]

    [Header("Movement Bindings")]
    public string Up = "E";
    public string Down = "Q";
    public string Left = "A";
    public string Right= "D";
    public string Forward = "W";
    public string Backward = "S";
    public string SpeedUp = "Left Shift";  
    public string[] GoToSelected = new string[] {"Left Shift", "F" };
    public EMouseButtons Rotate = EMouseButtons.rightBtn;
    public EMouseButtons Select = EMouseButtons.leftBtn;
    [Space(15)]

    [Header("Scale Options")]
    public bool scaleOnSelect;
    public bool highlightOnSelect;
    public Color highlightColor;
    [Space(15)]

    [Header("Other")]
    public GameObject[] ignoredOnSelect;


    private float currentSpeed;
    private float currentDragSpeed;
    private float currentlookSpeedH = 2f;
    private float currentlookSpeedV = 2f;
    private float yaw = 0f;
    private float pitch = 0f;
    private float lookSpeedH = 2f;
    private float lookSpeedV = 2f;
    bool isRendererAdded = false;
    Color previousColor;
    Vector3 oldScale;
    private GameObject selectedObject;
    #endregion

    private void Start()
    {
        yaw = transform.localEulerAngles.y;
        pitch = transform.localEulerAngles.x;
        currentSpeed = speed;
        currentDragSpeed = dragSpeed;
        selectedObject = null;
        if (highlightColor == Color.clear)
        {
            highlightColor = new Color(236, 248, 174, 255);
        }
    }

    private void Update()
    {
        inputManager();
    }

    void inputManager()
    {
        string test = Forward.ToLower();
        if (Input.GetKey(Up.ToLower()))
        {
            moveUp();
        }
        if (Input.GetKey(Down.ToLower()))
        {
            moveDown();
        }
        if (Input.GetKey(Left.ToLower()))
        {
            moveLeft();
        }
        if (Input.GetKey(Right.ToLower()))
        {
            moveRight();
        }
        if (Input.GetKey(Forward.ToLower()))
        {
            moveForward();
        }
        if (Input.GetKey(Backward.ToLower()))
        {
            moveBackward();
        }
        if (Input.GetKey(GoToSelected[0].ToLower()))
        {           
            if (GoToSelected[1] != null && GoToSelected[1] != "")
            {
                if (Input.GetKey(GoToSelected[1].ToLower()))
                {
                  
                    goToSelected();
                }
            }
            else
            {
                
                goToSelected();
            }         
        }
        

        if (Input.GetKeyDown(SpeedUp.ToLower()))
        {
            speedUp();
        }
        else if (Input.GetKeyUp(SpeedUp.ToLower()))
        {
            speedDown();
        }

        if (Input.GetMouseButton((int)Select))
        {
            setClickedObject();
            //transformSelectedObject();
        }
        if (Input.GetMouseButton((int)Rotate))
        {
            rotate();
        }

        if (currentSpeed > 1)
        {
            currentSpeed += Input.GetAxis("Mouse ScrollWheel") * 0.5f;
        }
        //Middle mouse button action
        /*if (Input.GetAxis("Mouse ScrollWheel"))
        {
        }*/
    }
    #region movement

    #region keyboard 
    void speedUp()
    {
        int multipler = (int)speedMultipler;
        currentSpeed *= multipler;
        currentDragSpeed *= multipler;
        currentlookSpeedH *= multipler;
        currentlookSpeedV *= multipler;
        Debug.Log("speedUP: " + currentSpeed);
    }

    void speedDown()
    {
        currentSpeed = speed;
        currentDragSpeed = dragSpeed;
        currentlookSpeedH = lookSpeedH;
        currentlookSpeedV = lookSpeedV;
        Debug.Log("speedDown: " + currentSpeed);
    }

    void moveLeft()
    {
        transform.Translate(-currentSpeed / 10, 0, 0, Space.Self);
    }

    void moveRight()
    {
        transform.Translate(currentSpeed / 10, 0, 0, Space.Self);
    }

    void moveUp()
    {
        transform.Translate(0, currentSpeed / 10, 0, Space.Self);
    }

    void moveDown()
    {
        transform.Translate(0, -currentSpeed / 10, 0, Space.Self);
    }

    void moveForward()
    {
        transform.Translate(0, 0, currentSpeed / 10, Space.Self);
    }

    void moveBackward()
    {
        transform.Translate(0, 0, -currentSpeed / 10, Space.Self);
    }
    #endregion

    #region mouse
    void rotate()
    {
        yaw += currentlookSpeedH * Input.GetAxis("Mouse X"); ;
        pitch -= currentlookSpeedV * Input.GetAxis("Mouse Y"); ;
        transform.eulerAngles = new Vector3(pitch, yaw, 0f);
    }
    #region select
    void goToSelected()
    {
        if (selectedObject != null)
        {
            transform.position = new Vector3(selectedObject.transform.position.x - 10, selectedObject.transform.position.y + 10, selectedObject.transform.position.z);
            transform.LookAt(selectedObject.transform);
            yaw = transform.localEulerAngles.y;
            pitch = transform.localEulerAngles.x;
        }
        else
        {
            Debug.Log("There is new object selected!!!");
        }
    }
    
    private void transformSelectedObject()
    {
        if (scaleOnSelect)
        {
            oldScale = selectedObject.transform.localScale;
            Vector3 newScale = selectedObject.transform.localScale * 2.0F; new Vector3(oldScale.x * 2, oldScale.y * 2, oldScale.z * 2);
            selectedObject.transform.localScale = newScale;

        }
        if (highlightOnSelect == true)
        {
            if (selectedObject.GetComponent<Renderer>() == null)
            {
                selectedObject.AddComponent<Renderer>().material.color = highlightColor;               
                isRendererAdded = true;
            }
            previousColor = selectedObject.GetComponent<Renderer>().material.color;
            selectedObject.GetComponent<Renderer>().material.color = highlightColor;
            isRendererAdded = false;
            oldShader = selectedObject.GetComponent<Renderer>().material.shader;
            Shader test = Shader.Find("Unlit/OutlineShader");
            selectedObject.GetComponent<Renderer>().material.shader = test;
        }
    }
    Shader oldShader;
    private void resetTransform()
    {
        if (scaleOnSelect)
        {
            selectedObject.transform.localScale = oldScale;
            oldScale = default(Vector3);
        }
        if (isRendererAdded && highlightOnSelect)
        {
            Destroy(selectedObject.GetComponent<Renderer>());
        }
        else if(highlightOnSelect)
        {
            selectedObject.GetComponent<Renderer>().material.color = previousColor;
            previousColor = default(Color);
            selectedObject.GetComponent<Renderer>().material.shader = oldShader;
        }
    }

    private void setClickedObject()
    {
        if (selectedObject != null)
            resetTransform();
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, 1000))
        {
            selectedObject = hit.transform.gameObject;

            //check if shoud ignore object
            foreach (GameObject hited in ignoredOnSelect)
            {
                if (selectedObject != null && hited == selectedObject)
                {
                    selectedObject = null;
                    return;
                }
            }

            transformSelectedObject();
        }
        else
        {
            selectedObject = null;
        }

    }

    #endregion

    #endregion

    #endregion
    
}
