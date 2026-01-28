using System;
using System.Linq;
using Scriptables;
using UnityEngine;
using UnityEngine.InputSystem;
using Random = UnityEngine.Random;

namespace Rifle_stuff
{
    public enum RifleType
    {
        None,
        M4
    }

    public class RifleController : MonoBehaviour
    {
        [SerializeField] private BulletProjectile _bulletPrefab;
        [SerializeField] private Transform riflePosition;


        private float _lastShotTime;
        private float recoilResetDelay = 0.08f; // ile po strzale zaczyna wracać
        private float recoilReturnSpeed = 6f;

        private const float recoilAngleMultiplier = 40f;

        private float _nextFireTime;
        //private float _currentRecoil;
        private FireMode _currentFireMode;


        private RifleData currentData;
        private float currentHopUp;


        private Vector2 _currentRecoil;
        private Vector3 _currentShotDirection;
        private bool _pendingDoubleFeed = false;
        public void InitController(RifleDataRecord rifle, BulletProjectile bullet)
        {
            ChangeRifle(rifle);
            ChangeBullet(bullet);
        }

        public void ChangeRifle(RifleDataRecord rifle)
        {
            currentData = rifle.RifleData;
            _currentFireMode = currentData.DefaultFireMode;
            currentHopUp = currentData.HopUp;
            _currentShotDirection = riflePosition.forward;
        }

        public void ChangeBullet(BulletProjectile bullet)
        {
            _bulletPrefab = bullet;
        }

        void Update()
        {

            if (GamePause.IsPaused || currentData == null)
            {
                return;
            }


            // SEMI
            if (_currentFireMode == FireMode.Semi && Mouse.current.leftButton.wasPressedThisFrame)
            { 
                Fire();
            }

            // FULL AUTO
            if (_currentFireMode == FireMode.FullAuto && Mouse.current.leftButton.isPressed && Time.time >= _nextFireTime)
            {
                Fire();
                _nextFireTime = Time.time + currentData.FireRate;
            }

            if (Keyboard.current.fKey.wasPressedThisFrame)
            {
                if (TryToggleFireMode())
                {
                    if (_currentFireMode == FireMode.Semi)
                    {
                        Debug.Log("FireMode: " + _currentFireMode);
                    }
                    else
                    {
                        Debug.Log("FireMode: " + _currentFireMode);
                    }

                }
                else
                {
                    Debug.LogWarning("You cannot change fire mode in this weapon type: ");
                }
            }


            //gradual reduction of recoil
            // = Mathf.Lerp(_currentRecoil, 0f, Time.deltaTime * 6f);
            //_currentRecoil = Vector2.Lerp(_currentRecoil, Vector2.zero, Time.deltaTime * 6f);

            // =============================
            // RECOIL RESET PO CZASIE
            // =============================
            if (Time.time - _lastShotTime > recoilResetDelay)
            {
                _currentRecoil = Vector2.Lerp(_currentRecoil, Vector2.zero, Time.deltaTime * recoilReturnSpeed);
            }

            _currentShotDirection = Vector3.Slerp(_currentShotDirection, riflePosition.forward, Time.deltaTime * 4f);

        }

        void Fire()
        {

            if (_bulletPrefab == null || riflePosition == null)
                return;

            
           

            // =============================
            // 1. FIZYKA KULKI (pęd)
            // =============================
            float bulletMass = _bulletPrefab.Data.Mass;
            float muzzleVelocity = Mathf.Sqrt(2f * currentData.MuzzleForce / bulletMass);
            float bulletMomentum = bulletMass * muzzleVelocity;

            // =============================
            // 2. RECOIL BRONI (NIE LOSOWY)
            // =============================
            float recoilStrength = bulletMomentum * currentData.Recoil;

            //_currentRecoil.y += recoilStrength* recoilAngleMultiplier;              // pitch (zawsze w górę)
            //_currentRecoil.x += recoilStrength* recoilAngleMultiplier * 0.25f;      // delikatny yaw (deterministyczny)

            float yawRandomFactor = Random.Range(-currentData.Recoil, currentData.Recoil);
            float pitchRandomFactor = Random.Range(currentData.Recoil * 0.8f, currentData.Recoil * 1.2f);


            _currentRecoil.y += recoilStrength * recoilAngleMultiplier * pitchRandomFactor;
            _currentRecoil.x += recoilStrength * recoilAngleMultiplier * yawRandomFactor;

            float maxYaw = currentData.Recoil * recoilAngleMultiplier * 0.25f * 3f;
            float maxPitch = currentData.Recoil * recoilAngleMultiplier * 6f;

            _currentRecoil.x = Mathf.Clamp(_currentRecoil.x, -maxYaw, maxYaw);
            _currentRecoil.y = Mathf.Clamp(_currentRecoil.y, 0f, maxPitch);

            //_currentRecoil.y = Mathf.Clamp(_currentRecoil.y, 0f, currentData.Recoil * 6f);
            //_currentRecoil.x = Mathf.Clamp(_currentRecoil.x, -currentData.Recoil * 3f, currentData.Recoil * 3f);

            //Quaternion recoilRotation =
            //    Quaternion.AngleAxis(_currentRecoil.x, Vector3.up) *
            //    Quaternion.AngleAxis(-_currentRecoil.y, Vector3.right);

            ////Vector3 shotDirection = recoilRotation * riflePosition.forward;

            //_currentShotDirection = recoilRotation * _currentShotDirection;
            //_currentShotDirection.Normalize();

            // lokalna oś "prawo" aktualnego kierunku strzału
            Vector3 localRight = Vector3.Cross(Vector3.up, _currentShotDirection).normalized;


            // pitch (góra/dół) – wokół PRAWEJ osi lufy
            Quaternion pitchRotation =
                Quaternion.AngleAxis(-_currentRecoil.y, localRight);


            // yaw (lewo/prawo) – wokół osi świata
            Quaternion yawRotation =
                Quaternion.AngleAxis(_currentRecoil.x, Vector3.up);


            // najpierw pitch, potem yaw
            Quaternion recoilRotation = yawRotation * pitchRotation;


            // aplikacja
            _currentShotDirection = recoilRotation * _currentShotDirection;
            _currentShotDirection.Normalize();


            Vector3 shotDirection = _currentShotDirection;

            // =============================
            // 3. DISPERSION KULKI (LOSOWA)
            // =============================
            Vector2 dispersion = _bulletPrefab.Data.Dispersion;

            float dispersionYaw = Random.Range(-dispersion.x, dispersion.x);
            float dispersionPitch = Random.Range(-dispersion.y, dispersion.y);

            Quaternion dispersionRotation =
                Quaternion.AngleAxis(dispersionYaw, Vector3.up) *
                Quaternion.AngleAxis(dispersionPitch, Vector3.right);

            shotDirection = dispersionRotation * shotDirection;

           

            // =============================
            // 5. SPAWN KULKI
            // =============================
            //BulletProjectile bullet = Instantiate(_bulletPrefab, riflePosition.position, Quaternion.identity);

            // =============================
            // 6. HOP-UP (BEZ ZMIAN)
            // =============================
            Vector3 barrelForward = riflePosition.forward;
            Vector3 barrelRight = Vector3.Cross(Vector3.up, barrelForward).normalized;
            Vector3 angularForce = barrelRight * currentHopUp;

            //
            // Defects
            //
            float finalVelocity = muzzleVelocity;
            int bulletCount = 1;

            //
            // Bullet DEfect
            //

            float maxBulletDefect;

            switch (_bulletPrefab.Data.Brand)
            {
                case BulletBrand.Cheap:
                    maxBulletDefect = 1.0f;   
                    break;
                case BulletBrand.Standard:
                    maxBulletDefect = 0.5f;  
                    break;
                case BulletBrand.Premium:
                    maxBulletDefect = 0.2f;  
                    break;
                default:
                    maxBulletDefect = 0f;
                    break;
            }
            float bulletDefectSliderNormalized = _bulletPrefab.Data.ChanceForDefect / 10f;
            float finalBulletDefectChance = bulletDefectSliderNormalized * maxBulletDefect;
            bool isDefect = Random.value < finalBulletDefectChance;

            if (isDefect)
            {
                //muzzleVelocity *= Random.Range(0.3f, 0.7f);
                finalVelocity *= 0.3f;
                //worse solution
                //Vector3 defectDrop = Vector3.down * Random.Range(0.2f, 0.6f);
                //shotDirection = (shotDirection + defectDrop).normalized;
            }

            //
            // Rifle Defect
            //

            float rifleDoubleFeedChance = currentData.RifleDoubleFeedChance/10f;
            if (_pendingDoubleFeed)
            {
                bulletCount = 2;
                finalVelocity *= Random.Range(0.45f, 0.6f);
                _pendingDoubleFeed = false;
            }
            else
            {
                // losujemy czy BROŃ NIE ZASSIE kulki
                if (Random.value < rifleDoubleFeedChance)
                {
                    _pendingDoubleFeed = true;
                    return; // brak strzału – klik, pustka, dramat
                }
            }


            if (bulletCount > 1)
            {
                for (int i = 0; i < bulletCount; i++)
                {
                    Vector3 dir = shotDirection;
                    float yaw = Random.Range(-2f, 2f);
                    float pitch = Random.Range(-2f, 2f); // ledwo ledwo


                    Quaternion q = Quaternion.AngleAxis(yaw, Vector3.up) * Quaternion.AngleAxis(pitch, localRight);


                    dir = q * shotDirection;

                    float energySplit = Random.Range(0.35f, 0.65f);
                    float bulletVelocity = (i == 0) ? finalVelocity * energySplit : finalVelocity * (1f - energySplit);


                    BulletProjectile bullet = Instantiate(_bulletPrefab, riflePosition.position, Quaternion.identity);
                    bullet.Initialize(dir * bulletVelocity, angularForce, currentData.MagnusMultiplier);
                }
            }
            else
            {
                BulletProjectile bullet = Instantiate(_bulletPrefab, riflePosition.position, Quaternion.identity);
                bullet.Initialize(shotDirection * finalVelocity, angularForce, currentData.MagnusMultiplier);
            }
            _lastShotTime = Time.time;


            //bullet.Initialize(shotDirection * muzzleVelocity, angularForce, currentData.MagnusMultiplier);
        }


        public bool TryToggleFireMode()
        {
            if (!currentData.existedFireMode)
                return false;

            if (_currentFireMode == FireMode.Semi)
            {
                _currentFireMode = FireMode.FullAuto;
            }
            else
            {
                _currentFireMode = FireMode.Semi;
            }

            return true;
        }

        public void SetHopUp(float value)
        {
            //currentHopUp = Mathf.Clamp(value, -currentData.MaxHopUp, currentData.MaxHopUp);
            //currentHopUp = Mathf.Lerp(0f, 8000f, value); // 300 rad/s = ~30 obrotów/s
            currentHopUp = value * currentData.MaxHopUp; ;
            Debug.Log($"[Rifle] HopUp applied: {currentHopUp}");
        }

    }
}