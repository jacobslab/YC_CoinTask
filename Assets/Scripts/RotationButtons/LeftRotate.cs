using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
public class LeftRotate : MonoBehaviour
{
    public GameObject obj;
    public float speed = 3000f;
    /// <summary>
    /// The position (X and Y distance) finger moved in previous frame
    /// </summary>
    public Vector2 fingerDeltaPosition;

    public Image JumpButton;
    public int JumpButtonFingerID = -1;
    private bool IsInRect(RectTransform rect, Vector2 screenPoint)
    {
        return RectTransformUtility.RectangleContainsScreenPoint(rect, screenPoint);
    }

    void Start()
    {
        obj = GameObject.FindGameObjectWithTag("Player");
    }

    // Update is called once per frame
    void Update()
    {
        foreach (Touch _touch in TouchScreenInputWrapper.touches)
        {
            if (_touch.phase == TouchPhase.Began)
            {
                if (IsInRect(JumpButton.rectTransform, _touch.position))
                {
                    //Jump button pressed
                    Debug.Log("Jump button pressed");
                    obj.transform.Rotate(0, -10*Time.deltaTime, 0);
                    
                    
                }
            }
            else if (_touch.phase == TouchPhase.Stationary)
            {
                if (IsInRect(JumpButton.rectTransform, _touch.position))
                {
                    //Jump button pressed
                    Debug.Log("Jump button touched continuously");
                    obj.transform.Rotate(0, -10*Time.deltaTime, 0);
                    
                }
            }
            
            else if (_touch.phase == TouchPhase.Ended || _touch.phase == TouchPhase.Canceled)
            {
                if (_touch.fingerId == JumpButtonFingerID)
                {
                    //Jump button released
                    Debug.Log("Jump button released");
                }
            }

            fingerDeltaPosition = _touch.deltaPosition;
        }

    }
}