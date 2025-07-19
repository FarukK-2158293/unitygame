using UnityEngine;

[CreateAssetMenu(menuName = "Powerups/SpeedBuff")]
public class SpeedBuff : PowerItem
{
    private float defaultrunSpeed = 40f;
    public int speedMultiplier = 2;

    public override void Apply(GameObject target)
    {
        PlayerMovement movement = target.GetComponent<PlayerMovement>();
        if (movement)
            movement.runSpeed = movement.runSpeed * speedMultiplier;  
    }

    public void removeSpeedBuff(GameObject target)
    {
        PlayerMovement movement = target.GetComponent<PlayerMovement>();
        if (movement)
            movement.runSpeed = defaultrunSpeed;
    }
}
