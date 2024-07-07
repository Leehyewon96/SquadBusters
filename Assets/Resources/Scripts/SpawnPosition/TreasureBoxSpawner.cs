using UnityEngine;

public class TreasureBoxSpawner : Spawner
{
    [SerializeField] protected ItemType itemType = ItemType.TreasureBox;

    protected override void Awake()
    {
        SetPath($"Prefabs/Item/TreasureBox");
    }
}
