using rogalik.Common;

namespace rogalik.Components;

public class Legs : Component
{
    public int movePoints;
    public Legs(int movePoints)
    {
        this.movePoints = movePoints;
    }
    
    public void Move(Point point)
    {
        parent.location.TryMoving(parent, point);
    }
}