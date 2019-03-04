using UnityEngine;
using UnityEngine.EventSystems;

public class FloatingJoystick : Joystick
{
    Vector2 joystickCenter = Vector2.zero;
    Player playerScript;
    public Vector2 fixedScreenPosition;

    void Start()
    {
        //Uncomment if dont want to be visible alltime
        //background.gameObject.SetActive(false);

        //playerScript = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
        playerScript = Player.player.gameObject.GetComponent<Player>();
    }

    public override void OnDrag(PointerEventData eventData)
    {
        Vector2 direction = eventData.position - joystickCenter;
        inputVector = (direction.magnitude > background.sizeDelta.x / 2f) ? direction.normalized : direction / (background.sizeDelta.x / 2f);
        ClampJoystick();
        handle.anchoredPosition = (inputVector * background.sizeDelta.x / 2f) * handleLimit;
    }

    public override void OnPointerDown(PointerEventData eventData)
    {
        background.gameObject.SetActive(true);
        background.position = eventData.position;
        handle.anchoredPosition = Vector2.zero;
        joystickCenter = eventData.position;
        playerScript.arming = true;
    }

    public override void OnPointerUp(PointerEventData eventData)
    {
        playerScript.Attack();
        playerScript.arming = false;

        //Uncomment if dont want to be visible alltime
        //background.gameObject.SetActive(false);

        inputVector = Vector2.zero;
        //Comment if dont want to be visible alltime
        OnFixed();
        
    }

    void OnFixed()
    {
        joystickCenter = fixedScreenPosition;
        background.gameObject.SetActive(true);
        handle.anchoredPosition = Vector2.zero;
        background.anchoredPosition = fixedScreenPosition;
    }
}