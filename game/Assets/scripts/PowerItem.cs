using UnityEngine;

[CreateAssetMenu(fileName = "PowerItem", menuName = "Scriptable Objects/PowerItem")]
public abstract class PowerItem : ScriptableObject
{
    public abstract void Apply(GameObject target);
}
