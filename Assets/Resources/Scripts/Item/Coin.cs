using UnityEngine;

public class Coin : MonoBehaviour
{
    private void Awake()
    {
        
    }

    public void SetActive(bool isActive)
    {
        gameObject.SetActive(isActive);
    }
}
