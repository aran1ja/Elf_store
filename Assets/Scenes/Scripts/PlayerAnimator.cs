using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimator : MonoBehaviour
{
    private const string IS_WALKING = "IsWalking";
    private Animator animator;
    private Player player;

    private void Awake() {
        player = GetComponent<Player>();
        animator = GetComponentInChildren<Animator>();
    }

    private void Update() {
        if (animator != null && player != null) {
            animator.SetBool(IS_WALKING, player.IsWalking());
        }
    }
}
