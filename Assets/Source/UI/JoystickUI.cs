using UnityEngine;

public class JoystickUI : MonoBehaviour
{
    public RectTransform innerStick;
    public CanvasGroup canvasGroup;

    Vector3 originPoint;
    JoystickInput joystick;

    void Start()
    {
        originPoint = transform.position;
        joystick = Main.Get<JoystickInput>();
        canvasGroup = GetComponent<CanvasGroup>();
        canvasGroup.alpha = 0.5f;
    }

    void FixedUpdate()
    {
        if (Main.Get<Player>().unit.IsDead())
            Destroy(gameObject);

        if (joystick.IsDown())
        {
            SnapToTouchPoint();

            var scale_factor = transform.parent.GetComponent<Canvas>().scaleFactor;
            var pixelPoint = joystick.GetOriginScreenPos() - joystick.GetRawVector() * scale_factor;
            var rectTrans = innerStick.GetComponent<RectTransform>();
            var position = ScreenToCanvasPoint(rectTrans, pixelPoint);
            rectTrans.anchoredPosition = Vector3.Lerp(rectTrans.anchoredPosition, position, 0.9f);
        }
        else
        {
            transform.position = originPoint;
            innerStick.localPosition = Vector3.zero;
        }
    }

    void SnapToTouchPoint()
    {
        var rectTrans = GetComponent<RectTransform>();
        var centerOffset = new Vector3(Screen.width / 2f, Screen.height / 2f);
        var position = ScreenToCanvasPoint(rectTrans, joystick.GetOriginScreenPos() + centerOffset);
        rectTrans.anchoredPosition = position;
    }

    Vector2 ScreenToCanvasPoint(RectTransform rectTrans, Vector3 point)
    {
        var parent = (RectTransform)rectTrans.parent;
        var screenPoint = point;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            parent, screenPoint, null, out var position
        );
        return position;
    }
}