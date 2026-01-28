using System.Diagnostics;
using UnityEngine;

namespace Scriptables
{
   

    public enum BulletBrand
    {
        Cheap,
        Standard,
        Premium
    }

    

    [CreateAssetMenu(fileName = "BulletData", menuName = "Scriptable Objects/BulletData")]
    public sealed class BulletData : ScriptableObject
    {
        [SerializeField] public float Mass = 0.00025f;
        public float Drag;
        public Vector2 Dispersion;

        [Header("Quality")]
        public BulletBrand Brand;

       
        /// <summary>
        /// Value in percentage 0%-10%
        /// </summary>
        [Range(0, 10), Header("Chance that bullet will behave weirdly.")] public float ChanceForDefect; 

        
    }
}
