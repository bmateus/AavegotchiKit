using UnityEngine;

namespace GotchiSDK
{
    public class Aavegotchi_SFX : MonoBehaviour
    {
        [SerializeField] private AudioClip[] AngrySFX;
        [SerializeField] private AudioClip DashLoopSFX;
        [SerializeField] private AudioClip DashEndSFX;
        [SerializeField] private AudioClip[] DeathSFX;
        [SerializeField] private AudioClip[] ShootSFX;
        [SerializeField] private AudioClip[] HappySFX;
        [SerializeField] private AudioClip[] HitSFX;
        [SerializeField] private AudioClip[] MeleeSFX;
        [SerializeField] private AudioClip[] SadSFX;
        [SerializeField] private AudioClip[] SpookySFX;
        [SerializeField] private AudioClip[] ThrowSFX;
        [SerializeField] private AudioClip[] WhirlwindSFX;
        [SerializeField] private AudioClip[] VictoryFinalSFX;

        [SerializeField] private AudioSource gotchiAudioSource;

        //--------------------------------------------------------------------------------------------------
        public void PlayAngrySFX()
        {
            if (enabled)
            {
                int randomIndex = Random.Range(0, AngrySFX.Length);
                gotchiAudioSource.PlayOneShot(AngrySFX[randomIndex]);
            }
        }

        //--------------------------------------------------------------------------------------------------
        public void StartDashLoop()
        {
            if (enabled)
            {
                gotchiAudioSource.clip = DashLoopSFX;
                gotchiAudioSource.loop = true;
                gotchiAudioSource.Play();
            }
        }

        //--------------------------------------------------------------------------------------------------
        public void EndDashLoop()
        {
            if (enabled)
            {
                gotchiAudioSource.PlayOneShot(DashEndSFX);
            }
        }

        //--------------------------------------------------------------------------------------------------
        public void PlayDeathSFX()
        {
            if (enabled)
            {
                int randomIndex = Random.Range(0, DeathSFX.Length);
                gotchiAudioSource.PlayOneShot(DeathSFX[randomIndex]);
            }
        }

        //--------------------------------------------------------------------------------------------------
        public void PlayShootSFX()
        {
            if (enabled)
            {
                int randomIndex = Random.Range(0, ShootSFX.Length);
                gotchiAudioSource.PlayOneShot(ShootSFX[randomIndex]);
            }
        }

        //--------------------------------------------------------------------------------------------------
        public void PlayHappySFX()
        {
            if (enabled)
            {
                int randomIndex = Random.Range(0, HappySFX.Length);
                gotchiAudioSource.PlayOneShot(HappySFX[randomIndex]);
            }
        }

        //--------------------------------------------------------------------------------------------------
        public void PlayHitSFX()
        {
            if (enabled)
            {
                int randomIndex = Random.Range(0, HitSFX.Length);
                gotchiAudioSource.PlayOneShot(HitSFX[randomIndex]);
            }
        }

        //--------------------------------------------------------------------------------------------------
        public void PlayMeleeSFX()
        {
            if (enabled)
            {
                int randomIndex = Random.Range(0, MeleeSFX.Length);
                gotchiAudioSource.PlayOneShot(MeleeSFX[randomIndex]);
            }
        }

        //--------------------------------------------------------------------------------------------------
        public void PlaySadSFX()
        {
            if (enabled)
            {
                int randomIndex = Random.Range(0, SadSFX.Length);
                gotchiAudioSource.PlayOneShot(SadSFX[randomIndex]);
            }
        }

        //--------------------------------------------------------------------------------------------------
        public void PlaySpookySFX()
        {
            if (enabled)
            {
                int randomIndex = Random.Range(0, SpookySFX.Length);
                gotchiAudioSource.PlayOneShot(SpookySFX[randomIndex]);
            }
        }

        //--------------------------------------------------------------------------------------------------
        public void PlayThrowSFX()
        {
            if (enabled)
            {
                int randomIndex = Random.Range(0, ThrowSFX.Length);
                gotchiAudioSource.PlayOneShot(ThrowSFX[randomIndex]);
            }
        }

        //--------------------------------------------------------------------------------------------------
        public void PlayWhirlwindSFX()
        {
            if (enabled)
            {
                int randomIndex = Random.Range(0, WhirlwindSFX.Length);
                gotchiAudioSource.PlayOneShot(WhirlwindSFX[randomIndex]);
            }
        }

        //--------------------------------------------------------------------------------------------------
        public void PlayVictoryFinalSFX()
        {
            if (enabled)
            {
                int randomIndex = Random.Range(0, VictoryFinalSFX.Length);
                gotchiAudioSource.PlayOneShot(VictoryFinalSFX[randomIndex]);
            }
        }

    }
}
