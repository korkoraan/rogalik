using rogalik.Framework;

namespace rogalik.Components;

public abstract class Mind : Component
{
    public bool dead { get; private set; }
    public delegate void DiesHandler();
    public event DiesHandler Dies;
    
    public uint energyCurrent = 1000;

    public void Die()
    {
        dead = true;
        Dies?.Invoke();
    }

    /// <summary>
    /// returns the next action mind wants to perform 
    /// </summary>
    public abstract Action ChooseNextAction();
}