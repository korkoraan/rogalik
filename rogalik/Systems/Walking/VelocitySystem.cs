using rogalik.Framework;

namespace rogalik.Systems.Walking;

public class Position : IComponent
{
    public Point point;

    public Position(Point point)
    {
        this.point = point;
    }
    
    public Position(int x, int y, int z)
    {
        point = new Point(x, y, z);
    }

    public static Position operator +(Position pos, Velocity velocity)
    {
        pos.point += velocity.point;
        return pos;
    }
}

public class Velocity : IComponent
{
    public Point point;

    public Velocity(int x, int y, int z)
    {
        point = new Point(x, y, z);
    }
}

public class VelocitySystem : GameSystem, IUpdateSystem
{
    public void Update(uint ticks)
    {
        var filter = new Filter().With<Position>().With<Velocity>().Apply(world.objects);

        foreach (var obj in filter)
        {
            var position = obj.GetComponent<Position>();
            var velocity = obj.GetComponent<Velocity>();
            position += velocity;
            obj.RemoveComponent(velocity);
        }
    }

    public VelocitySystem(World world) : base(world)
    {
    }
}