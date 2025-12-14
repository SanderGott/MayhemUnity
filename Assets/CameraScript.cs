using UnityEngine;
using UnityEngine.InputSystem;

public class CameraScript : MonoBehaviour
{
    public Transform target;
    public Vector3 offset;



    void LateUpdate()
    {
        Vector3 pos = transform.position;
        pos.x = target.position.x;
        pos.y = target.position.y;
        transform.position = pos;


        if (Keyboard.current == null) return;
        Camera cam = Camera.main;



        if (Keyboard.current.qKey.isPressed) cam.orthographicSize -= 0.1f;
        if (Keyboard.current.eKey.isPressed) cam.orthographicSize += 0.1f;


    }
}
