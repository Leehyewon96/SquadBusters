using System.Collections.Generic;
using UnityEngine;

public class AttackCircleStat : MonoBehaviour
{
    private List<GameObject> units = new List<GameObject>();
    [HideInInspector] public int coin = 0;
    [HideInInspector] public int gem = 0;

    public float attackRadius = 2f;


    public void AddUnits(GameObject newUnit)
    {
        if(!units.Contains(newUnit))
        {
            units.Add(newUnit);
        }
    }

    public void RemoveUnits(GameObject unit)
    {
        if(units.Contains(unit))
        {
            units.Remove(unit);
        }
    }

    
}
