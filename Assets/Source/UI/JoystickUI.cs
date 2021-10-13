using UnityEngine;

public class JoystickUI : MonoBehaviour
{
    public RectTransform innerStick;
    public CanvasGroup canvasGroup;
    public float radiusPX = 100f;

    JoystickInput joystick;

    void Start()
    {
        joystick = Main.Get<JoystickInput>();
        canvasGroup = GetComponent<CanvasGroup>();
        canvasGroup.alpha = 0;
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
            canvasGroup.alpha = Mathf.Lerp(canvasGroup.alpha, 1f, 0.1f);
        }
        else
        {
            innerStick.localPosition = Vector3.Lerp(innerStick.localPosition, Vector3.zero, 0.1f);
            canvasGroup.alpha = Mathf.Lerp(canvasGroup.alpha, 0f, 0.1f);
        }
    }
}