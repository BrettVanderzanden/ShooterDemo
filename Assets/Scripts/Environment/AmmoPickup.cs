using UnityEngine;

public class AmmoPickup : MonoBehaviour
{
    Animator _pickupAnimator;
    private static readonly int HOVER_HASH = Animator.StringToHash("Hover");

    private void Awake()
    {
        _pickupAnimator = GetComponentInChildren<Animator>();
    }
    
    void Start()
    {
        _pickupAnimator.Play(HOVER_HASH, 0, 0f);
    }
}
