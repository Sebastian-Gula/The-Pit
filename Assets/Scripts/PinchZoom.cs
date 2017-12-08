using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class PinchZoom : MonoBehaviour
{
    private Camera thisCamera;

    public float ZoomSpeed;


    void Awake()
    {
        thisCamera = GetComponent<Camera>();
    }


    void Update()
    {
        if (Input.touchCount == 2)
        {
            Touch touchZero = Input.GetTouch(0);
            Touch touchOne = Input.GetTouch(1);

            Vector2 touchZeroPrevPos = touchZero.position - touchZero.deltaPosition;
            Vector2 touchOnePrevPos = touchOne.position - touchOne.deltaPosition;

            float prevTouchDeltaMag = (touchZeroPrevPos - touchOnePrevPos).magnitude;
            float touchDeltaMag = (touchZero.position - touchOne.position).magnitude;

            float deltaMagnitudeDiff = prevTouchDeltaMag - touchDeltaMag;

            thisCamera.orthographicSize += deltaMagnitudeDiff * ZoomSpeed;
            if (thisCamera.orthographicSize > 3.9) thisCamera.orthographicSize = 3.9f;
            if (thisCamera.orthographicSize < 2) thisCamera.orthographicSize = 2;

            GameManager.Instance.zoom.value = thisCamera.orthographicSize;
        }
    }
}
