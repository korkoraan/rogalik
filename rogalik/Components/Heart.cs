using System.Linq;
using rogalik.Framework;
using rogalik.Rendering;

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
        var name = owner.GetType().ToString().Split('.').Last();
        UIData.messages.Add($"{name} receives {damage} damage");
        if (_healthPoints < 0)
        {
            UIData.messages.Add($"{name} heart stops beating, and he dies");
            _ownerMind?.Die();
            owner.RemoveComponent(this);
        }
    }
}