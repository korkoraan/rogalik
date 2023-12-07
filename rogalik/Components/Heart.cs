using rogalik.Common;
using rogalik.Framework;

namespace rogalik.Components;

public class Heart : Component, IDestructible
{
    private int _healthPoints = 10;
    private Mind _ownerMind;
    
    public override void Init()
    {
        _ownerMind = owner.GetComponent<Mind>();
    }
    
    public void ReceiveDamage(int damage)
    {
        _healthPoints -= damage;
        C.Print($"{owner} receives {damage} damage");
        if (_healthPoints < 0)
        { 
            C.Print($"{owner} heart stops beating, and he dies");
            _ownerMind?.Die();
        }
    }
}