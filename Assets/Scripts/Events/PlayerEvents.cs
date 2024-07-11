using System;
using UnityEngine;

public static class PlayerEvents
{
    public static event EventHandler<PlayerMovementEventArgs> MovementActive;

    public static void OnMovementActive(bool newPlayerMoveState)
    {
        MovementActive?.Invoke(null, new PlayerMovementEventArgs(newPlayerMoveState));
    }

    public static event EventHandler<PlayerDeathEventArgs> PlayerDeath;

    public static void OnPlayerDeath(object sender, PlayerDeathEventArgs args)
    {
        Debug.Log("Player Death Event Triggered");
        PlayerDeath?.Invoke(sender, args);
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

public class PlayerDeathEventArgs
{
    public Checkpoint currentCheckpoint;

    public PlayerDeathEventArgs(Checkpoint currentCheckpoint)
    {
        this.currentCheckpoint = currentCheckpoint;
    }

}


