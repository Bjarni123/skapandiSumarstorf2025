using UnityEngine;

[CreateAssetMenu(fileName = "AttackSO", menuName = "Attacks/ComboAutoAttacks")]


public class AttackSO : ScriptableObject
{
    public AnimatorOverrideController animatorOV; // animator overrider
    public float damage;
    public GameObject swishPrefab;
}
