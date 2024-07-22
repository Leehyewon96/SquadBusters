using UnityEngine;
using UnityEngine.UI;

public class SkillUI : UIBase
{
    private Button button = null;
    private ItemType skillType;

    public delegate void DoSkill(ItemType type);
    public DoSkill doSkill = null;

    private void Awake()
    {
        button = GetComponentInChildren<Button>();
        SetInteractable(false);
        button.onClick.AddListener(OnButtonClick);
    }

    public void SetInteractable(bool isEnable)
    {
        button.interactable = isEnable;
    }

    public void UpdateSkillType(ItemType type)
    {
        skillType = type;

        //��ų ������ ������Ʈ �ڵ� �߰� 
    }

    public void OnButtonClick()
    {
        if (doSkill != null)
        {
            doSkill.Invoke(skillType);
        }

        SetInteractable(false);
    }
}
