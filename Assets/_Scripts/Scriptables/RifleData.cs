using UnityEngine;

public enum FireMode
{
    None,
    Semi,
    FullAuto
}

[CreateAssetMenu(fileName = "RifleData", menuName = "Scriptable Objects/RifleData")]
public class RifleData : ScriptableObject
{
    public string WeaponName;

    public float MuzzleForce;
    public float Recoil;
    public float FireRate; 

    public float HopUp;
    public float MaxHopUp=100f;
    public float MagnusMultiplier = 0.25f;

    [Range(0, 10), Header("Chance that rifle won't suck bullet and throw 2 bullets in next shoot.")]
    public float RifleDoubleFeedChance;



    public FireMode DefaultFireMode = FireMode.Semi;
    public bool existedFireMode = false;
}
