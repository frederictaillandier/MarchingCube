using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMove : MonoBehaviour
{
    public float cameraSpeed = 2;
    Vector3 _input;

    void OnGUI()
    {
        if (Event.current.type == EventType.KeyDown)
        {
            switch (Event.current.keyCode)
            {
                case KeyCode.W:
                    _input.z = 1;
                    break;
                case KeyCode.S:
                    _input.z = -1;
                    break;
                case KeyCode.A:
                    _input.x = -1;
                    break;
                case KeyCode.D:
                    _input.x = 1;
                    break;
                default:
                    break;
            }
        }
        if (Event.current.type == EventType.KeyUp)
        {
            switch (Event.current.keyCode)
            {
                case KeyCode.W:
                case KeyCode.S:
                    _input.z = 0;
                    break;
                case KeyCode.A:
                case KeyCode.D:
                    _input.x = 0;
                    break;
                default:
                    break;
            }
        }
        if (Event.current.type == EventType.ScrollWheel)
            _input.y = Event.current.delta.y;
        else
            _input.y = 0;
    }

    void Update()
    {
        this.transform.Translate(_input * Time.deltaTime * cameraSpeed, Space.World);
    }

}
