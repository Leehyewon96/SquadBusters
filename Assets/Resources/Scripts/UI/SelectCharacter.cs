using UnityEngine;
using UnityEngine.UI;

public class SelectCharacter : MonoBehaviour
{
    public CharacterButton char1 = null;
    public CharacterButton char2 = null;
    public CharacterButton char3 = null;

    private void Awake()
    {
        char1.SetCharacterType((CharacterType)Random.Range(0, (int)CharacterType.Player - 1));
        char2.SetCharacterType((CharacterType)Random.Range(0, (int)CharacterType.Player - 1));
        char3.SetCharacterType((CharacterType)Random.Range(0, (int)CharacterType.Player - 1));

        char1.GetComponent<Button>().onClick.AddListener(delegate { SelectChar(char1.characterType); });
        char2.GetComponent<Button>().onClick.AddListener(delegate { SelectChar(char2.characterType); });
        char3.GetComponent<Button>().onClick.AddListener(delegate { SelectChar(char3.characterType); });
    }

    public void SetActive(bool isActive)
    {
        gameObject.SetActive(isActive);
    }

    public void SelectChar(CharacterType newType)
    {
        GameManager.Instance.player.GetComponent<ICharacterUIInterface>().SelectCharacter(newType);
        SetActive(false);
    }
}
