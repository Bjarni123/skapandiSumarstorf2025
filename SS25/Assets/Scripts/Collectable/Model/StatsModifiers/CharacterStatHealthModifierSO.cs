using UnityEngine;

[CreateAssetMenu]
public class CharacterStatHealthModifierSO : CharacterStatModifierSO
{
    public override void AffectCharacter(GameObject character, float val)
    {
        PlayerHealth health = character.GetComponent<PlayerHealth>();
        if (health != null)
        {
            int intValue = Mathf.RoundToInt(val);
            if (val >= 0)
                health.AddHealth(intValue);
            else
                health.TakeDamage(-intValue);
        }
    }
}