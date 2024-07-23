using UnityEngine;

public class NoticeElem : UIBase
{
    [SerializeField] private NoticeType type;

    private Vector3 pos = Vector3.zero;
    private GameObject target = null;

    private void Awake()
    {
        type = NoticeType.MoneyTree;
    }

    public void Update()
    {
        if(target == null)
        {
            return;
        }
        SetPos();
    }

    public NoticeType GetNoticeType()
    {
        return type;
    }

    public void SetTarget(GameObject newTarget)
    {
        target = newTarget;
    }

    public void SetPos()
    {
        pos = Camera.main.WorldToScreenPoint(target.transform.position + Vector3.up * 2.5f);
        GetComponent<RectTransform>().position = pos;
    }
}
