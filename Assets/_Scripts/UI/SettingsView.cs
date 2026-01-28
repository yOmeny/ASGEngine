using System;
using System.Collections.Generic;
using System.Linq;
using Rifle_stuff;
using Scriptables;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public sealed class SettingsView : MonoBehaviour
    {
        private const string LAST_RIFLE_SELECTED = "last_rifle_selected";
        private const string LAST_BULLET_SELECTED = "last_bullet_selected";

        private RifleDataRecord _cachedRifle;

        [SerializeField] private RifleDataBase _rifles;
        [SerializeField] private BulletsDatabase _bullets;
        [SerializeField] private TMP_Dropdown _riflesDropdown;
        [SerializeField] private TMP_Dropdown _bulletDropdown;
        [SerializeField] private RifleController _rifleController;
        [SerializeField] private Slider _hopupSlider;
        [SerializeField] private Slider _rifleForceSlider;

        private void Awake()
        {
            _riflesDropdown.ClearOptions();
            _riflesDropdown.AddOptions(_rifles.Select(s => s.RifleType.ToString()).ToList());
           _bulletDropdown.ClearOptions();
           _bulletDropdown.AddOptions(_bullets.Select(s => s.Data.Mass.ToString()).ToList());

           _riflesDropdown.onValueChanged.AddListener(OnRifleChanged);
           _bulletDropdown.onValueChanged.AddListener(OnBulletChanged);

           _rifleForceSlider.onValueChanged.AddListener(OnRifleForceSliderValueChanged);
           _hopupSlider.onValueChanged.AddListener(OnHopupSliderValueChanged);
           Init();
        }

        private void OnRifleForceSliderValueChanged(float value)
        {
            _cachedRifle.RifleData.MuzzleForce = value;
        }

        private void OnHopupSliderValueChanged(float value)
        {
            _cachedRifle.RifleData.HopUp = value;
            _rifleController.SetHopUp(value);// change from chat added
        }

        private void Init()
        {
            _rifleController.InitController(GetRifle(), GetBullet());
        }

        private RifleDataRecord GetRifle()
        {
            var rifleIndex = PlayerPrefs.GetInt(LAST_RIFLE_SELECTED, 0);
            _cachedRifle = _rifles[rifleIndex];
            _rifleForceSlider.SetValueWithoutNotify(_cachedRifle.RifleData.MuzzleForce);
            _hopupSlider.SetValueWithoutNotify(_cachedRifle.RifleData.HopUp);
            return _cachedRifle;
        }

        private BulletProjectile GetBullet()
        {
            var bulletIndex = PlayerPrefs.GetInt(LAST_BULLET_SELECTED, 0);
            var bullet = _bullets[bulletIndex];
            return bullet;
        }

        private void OnRifleChanged(int index)
        {
            PlayerPrefs.SetInt(LAST_RIFLE_SELECTED, index);
            PlayerPrefs.Save();

            _rifleController.ChangeRifle(GetRifle());

        }

        private void OnBulletChanged(int index)
        {
            PlayerPrefs.SetInt(LAST_BULLET_SELECTED, index);
            PlayerPrefs.Save();

            _rifleController.ChangeBullet(GetBullet());
        }
    }
}
