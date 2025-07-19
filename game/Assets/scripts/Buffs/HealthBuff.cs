using UnityEngine;

[CreateAssetMenu(menuName = "Powerups/HealthBuff")]
public class HealthBuff : PowerItem
{
    public int amount = 1;
    public float interval = 2f;

    public override void Apply(GameObject target)
    {
        PlayerStats stats = target.GetComponent<PlayerStats>();
        if (stats)
        {
            stats.StartRegen(amount, interval);
        }
    }
}
