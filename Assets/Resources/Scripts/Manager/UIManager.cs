using UnityEngine;

public enum UIType
{
    SelectCharacter,

}

public class UIManager : MonoBehaviour
{
    public SelectCharacter selectCharacter = null;

    public void ShowUI(UIType uiType)
    {
        switch (uiType)
        {
            case UIType.SelectCharacter:
                selectCharacter.SetActive(true);
                break;
            default:
                break;
        }

    }
}
