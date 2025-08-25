using UnityEngine;

public interface ITriggerCheckable
{
    bool IsAggroed { get; set; }
    bool IsWithinAttackingRange { get; set; }
    void SetAggroStatus(bool isAggroed);
    void SetAttackingRangeBool(bool isWithinAttackingRange);
}
