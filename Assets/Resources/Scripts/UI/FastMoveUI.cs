using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class FastMoveUI : MonoBehaviour
{
    [SerializeField] private Button button = null;
    private EventTrigger eventTrigger = null;

    public delegate void OnMoveFast();
    public OnMoveFast onMoveFast;

    public delegate void OnMoveCommon();
    public OnMoveCommon onMoveCommon;

    private void Awake()
    {
        eventTrigger = button.gameObject.GetComponent<EventTrigger>();

        EventTrigger.Entry pointerDownEntry = new EventTrigger.Entry();
        pointerDownEntry.eventID = EventTriggerType.PointerDown;
        pointerDownEntry.callback.AddListener(MoveFast);
        eventTrigger.triggers.Add(pointerDownEntry);

        EventTrigger.Entry pointerUpEntry = new EventTrigger.Entry();
        pointerUpEntry.eventID = EventTriggerType.PointerUp;
        pointerUpEntry.callback.AddListener(MoveCommon);
        eventTrigger.triggers.Add(pointerUpEntry);
    }

    public void MoveFast(BaseEventData e)
    {
        Debug.Log($"MoveFast");
        if(onMoveFast != null)
        {
            Debug.Log($"MoveFast Invoke");
            onMoveFast.Invoke();
        }
    }

    public void MoveCommon(BaseEventData e)
    {
        Debug.Log($"MoveCommon");
        if (onMoveCommon != null)
        {
            Debug.Log($"onMoveCommon Invoke");
            onMoveCommon.Invoke();
        }
    }
}
