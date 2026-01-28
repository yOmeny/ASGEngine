using System.Collections;
using System.Collections.Generic;
using UnityEngine;
// ReSharper disable CollectionNeverUpdated.Local
#pragma warning disable CS0649 // Field is never assigned to, and will always have its default value

namespace Scriptables
{
    [CreateAssetMenu(fileName = "BulletsDatabase", menuName = "Scriptable Objects/BulletsDatabase")]
    public sealed class BulletsDatabase : ScriptableObject, IEnumerable<BulletProjectile>
    {
        [SerializeField] private List<BulletProjectile> _bullets;
        public BulletProjectile this[int index] => _bullets[index];
        public IEnumerator<BulletProjectile> GetEnumerator()
        {
            foreach (BulletProjectile bulletData in _bullets)
            {
                yield return bulletData;
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
