using System;
using UnityEngine;
using Mirror;

public class PlayerController : NetworkBehaviour
{
    public static event Action<Transform> OnPlayerSpawned;

    public override void OnStartAuthority()
    {
        base.OnStartAuthority();

        // Trigger the event when the player spawns
        OnPlayerSpawned?.Invoke(transform);
    }
}
