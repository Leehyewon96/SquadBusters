using UnityEngine;

public class PlayerAttackCircleSpawnPoint : MonoBehaviour
{
    private bool isAssigned = false;

    public bool GetIsAssigned() => isAssigned;

    public void SetIsAssigned(bool assigned)
    {
        isAssigned = assigned;
    }
}
