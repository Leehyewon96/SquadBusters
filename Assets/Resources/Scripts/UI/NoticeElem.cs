using System.Collections;
using UnityEngine;

public class NoticeElem : UIBase
{
    [SerializeField] private NoticeType type;

    private GameObject target = null;
    private RectTransform rectTransform;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
    }

    public void Update()
    {
        if (target == null) return;
        SetPos();
    }

    public NoticeType GetNoticeType() => type;

    public void SetTarget(GameObject newTarget) => target = newTarget;

    public void SetPos()
    {
        Vector3 pos = Camera.main.WorldToScreenPoint(target.transform.position + Vector3.up * 3f);
        rectTransform.position = pos;
    }

    public void Disable(float delay)
    {
        StartCoroutine(CoDisable(delay));
    }

    private IEnumerator CoDisable(float delay)
    {
        yield return new WaitForSeconds(delay);
        gameObject.SetActive(false);
    }
}
