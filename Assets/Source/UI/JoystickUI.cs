using UnityEngine;

public class JoystickUI : MonoBehaviour
{
    public RectTransform innerStick;
    public CanvasGroup canvasGroup;
    public float radiusPX = 100f;

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
        if (joystick.IsDown())
        {
            var rectTrans = GetComponent<RectTransform>();
            var parent = (RectTransform)rectTrans.parent;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                parent, joystick.GetOriginScreenPos(), null, out var position
            );
            rectTrans.anchoredPosition = position;

            innerStick.localPosition = Vector3.Lerp(innerStick.localPosition, -joystick.GetRawVector() * radiusPX, 0.9f);
        }
        else
        {
            transform.position = originPoint;
            innerStick.localPosition = Vector3.zero;
        }
    }
}