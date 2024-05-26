using System.Collections;

public interface IPowerUp
{
    float Duration { get; }

    bool IsActive { get; }


    void Activate();

    IEnumerator Deactivate();
}

public enum PowerUpTypes
{ 
    Jump,
    Gravity,
}

