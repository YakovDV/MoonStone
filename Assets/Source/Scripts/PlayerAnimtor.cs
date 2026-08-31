using UnityEngine;

public class PlayerAnimtor : MonoBehaviour
{
    private const string AnimatorSpeed = "Speed";

    [SerializeField] private Animator _animator;
    [SerializeField] private PlayerMover _playerMover;

    private void Update()
    {
        _animator.SetFloat(AnimatorSpeed, _playerMover.Speed);
    }
}