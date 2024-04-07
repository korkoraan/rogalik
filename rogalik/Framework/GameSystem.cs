namespace rogalik.Framework;

public abstract class GameSystem
{
    protected World world;

    public GameSystem(World world)
    {
        this.world = world;
    }

    public abstract void Update(uint ticks);
}