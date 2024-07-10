using UnityEngine;

public class AttackCircleStat : MonoBehaviour
{
    public int coin = 0;
    public int gem = 0;

    public float attackRadius = 2f;

    public void AddCoin(int cnt)
    {
        coin += cnt;
    }

    public void AddGem(int cnt)
    {
        gem += cnt;
    }
}
