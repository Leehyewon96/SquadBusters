using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class FastMoveUI : UIBase
{
    [SerializeField] private Button button = null;
    private EventTrigger eventTrigger = null;

    private float coolTime = 10f;
    private float curFilledTime = 0f;
    private float state = 1f;

    public Action onMoveFast;
    public Action onMoveCommon;

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

        button.image.fillAmount = 0f;
        InvokeRepeating("UpdateCoolTime", 0f, 1f);
    }

    private void Update()
    {
        if (curFilledTime <= 0)
            onMoveCommon?.Invoke();
    }

    public void MoveFast(BaseEventData e)
    {
        state = -1f;
        onMoveFast?.Invoke();
    }

    public void MoveCommon(BaseEventData e)
    {
        state = 1f;
        onMoveCommon?.Invoke();
    }

    public void SetInteractable(bool isInteractable)
    {
        button.interactable = isInteractable;
        eventTrigger.enabled = isInteractable;
    }


    public void UpdateCoolTime()
    {
        curFilledTime += 1f * state;
        curFilledTime = Mathf.Clamp(curFilledTime, 0f, coolTime);
        button.image.fillAmount = Mathf.Clamp01(curFilledTime / coolTime);
    }
}
