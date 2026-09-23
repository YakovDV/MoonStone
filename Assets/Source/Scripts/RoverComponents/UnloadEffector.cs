using UnityEngine;

public class UnloadEffector : MonoBehaviour
{
    [SerializeField] private ParticleSystem _effect;

    public void Play(Vector3 target)
    {
        AimAt(target);
        _effect.Play();
    }

    private void AimAt(Vector3 target)
    {
        Vector3 direction = target - transform.position;

        if (direction.sqrMagnitude < 0.0001f)
            return;

        transform.rotation = Quaternion.LookRotation(direction);
    }
}