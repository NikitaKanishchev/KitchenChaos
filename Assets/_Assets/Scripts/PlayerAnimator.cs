using System;
using UnityEngine;

public class PlayerAnimator : MonoBehaviour
{
    private const string IS_Walking = "IsWalking";

    private Animator _animator;

    [SerializeField] private Player _player;

    private void Awake() => 
        _animator = GetComponent<Animator>();

    private void Update() => 
        _animator.SetBool(IS_Walking, _player.IsWalking());
}