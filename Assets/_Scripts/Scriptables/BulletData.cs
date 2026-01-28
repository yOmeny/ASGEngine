using UnityEngine;

namespace Scriptables
{
    public enum BulletMass
    {
        g25,
        g30,
        g32,
        g35
    }

    [CreateAssetMenu(fileName = "BulletData", menuName = "Scriptable Objects/BulletData")]
    public sealed class BulletData : ScriptableObject
    {
        [SerializeField] public BulletMass Mass;
        public float Drag;
        public Vector2 Dispersion;



        /// <summary>
        /// Value in percentage 0%-10%
        /// </summary>
        [Range(0, 10), Header("Chance that bullet will behave weirdly.")] public float ChanceForDefect; 

        public float GetMass
        {
            get
            {
                switch (Mass)
                {
                    case BulletMass.g25: return 0.00025f;
                    case BulletMass.g30: return 0.00030f;
                    case BulletMass.g32: return 0.00032f;
                    case BulletMass.g35: return 0.00035f;
                    default: return 0;
                }
            }
        }
    }
}
