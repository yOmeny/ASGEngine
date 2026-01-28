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

    public FireMode DefaultFireMode = FireMode.Semi;
    public bool existedFireMode = false;
}
