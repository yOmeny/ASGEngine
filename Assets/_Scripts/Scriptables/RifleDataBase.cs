using System;
using System.Collections;
using UnityEngine;
using System.Collections.Generic;
using Rifle_stuff;

[System.Serializable]
public sealed class RifleDataRecord
{
    public RifleType RifleType;
    public RifleData RifleData;
}

[CreateAssetMenu(fileName = "RifleDataBase", menuName = "Scriptable Objects/RifleDataBase")]
public sealed class RifleDataBase : ScriptableObject, IEnumerable<RifleDataRecord>
{
    [SerializeField] private List<RifleDataRecord> _rifles;

    public RifleDataRecord this[int index] => _rifles[index];

    public IEnumerator<RifleDataRecord> GetEnumerator()
    {
        foreach (RifleDataRecord rifleDataRecord in _rifles)
        {
            yield return rifleDataRecord;
        }
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}
