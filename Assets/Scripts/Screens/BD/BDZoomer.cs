using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BDZoomer : MonoBehaviour
{

    public float zoom_speed = 0.5f;

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.touchCount == 2)
        {
            Touch first_touch = Input.GetTouch(0);
            Touch second_touch = Input.GetTouch(1);

            Vector2 first_touch_prev_pos = first_touch.position - first_touch.deltaPosition;
            Vector2 second_touch_prev_pos = second_touch.position - second_touch.deltaPosition;


            //@Magnitude
            float prev_touch_delta_mag = (first_touch_prev_pos - second_touch_prev_pos).magnitude;
            
            float touch_delta_mag = (first_touch.position - second_touch.position).magnitude;

            //@Frames diff

            float delta_mag_diff = prev_touch_delta_mag - touch_delta_mag;

            //@Lets zoom the cam

            // Camera.main.orthographicSize += delta_mag_diff * zoom_speed;

            //@Prevent leaving current cam scale
            //float current_cam_scale = FindObjectOfType<BDReaderEngine>().cam_future_size; 

            //Camera.main.orthographicSize = Mathf.Max(Camera.main.orthographicSize , current_cam_scale);


        }
    }
}
