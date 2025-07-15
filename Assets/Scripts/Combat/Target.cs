using System;
using UnityEngine;

public class Target : MonoBehaviour, IHittable
{
    public static Action<Target> OnTargetBreak;

    public void TakeHit()
    {
        OnTargetBreak?.Invoke(this);
        Destroy(this.gameObject);
    }
}
