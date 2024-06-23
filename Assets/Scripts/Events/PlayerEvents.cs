using System;
using UnityEngine;

public static class PlayerEvents
{
    public static event EventHandler<PlayerMovementEventArgs> MovementActive;

    public static void OnMovementActive(bool newPlayerMoveState)
    {
        MovementActive?.Invoke(null, new PlayerMovementEventArgs(newPlayerMoveState));
    }
}

public class PlayerMovementEventArgs
{
    public bool newPlayerMoveState;

    public PlayerMovementEventArgs(bool newPlayerMoveState)
    {
        this.newPlayerMoveState = newPlayerMoveState;
    }
    
}

