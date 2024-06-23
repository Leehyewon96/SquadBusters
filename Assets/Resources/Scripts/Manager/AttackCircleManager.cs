using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class AttackCircleManager : MonoBehaviour
{
    private List<AttackCircle> attackCircleList = new List<AttackCircle>();

    private void Awake()
    {
        attackCircleList = GetComponentsInChildren<AttackCircle>(true).ToList();
        InitAttackCircles();
    }

    public void InitAttackCircles()
    {
        foreach (AttackCircle circle in attackCircleList)
        {
            circle.SetActive(false);
            circle.UpdateIsUsed(false);
        }
    }

    public AttackCircle GetAttackCircle()
    {
        AttackCircle circle = attackCircleList.Find(c => !c.isUsed);
        if(circle == null)
        {
            circle = Instantiate(attackCircleList.FirstOrDefault(), transform);
            attackCircleList.Add(circle);
        }

        circle.SetActive(true);
        circle.UpdateIsUsed(true);

        return circle;
    }
}
