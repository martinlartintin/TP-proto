using UnityEngine;

[System.Serializable]
public class Attack
{
    public string attackName;

    public int damage;

    [Header("Cooldown")]
    public int cooldownTurns = 0;
    [HideInInspector] public int currentCooldown = 0;
}
