using System;
using System.Collections.Generic;

namespace rogalik.Framework;

public abstract class GameSystem
{
    protected World world;

    protected virtual List<Type> LocalDependencies() => new List<Type>();
    
    private static void CreateSystems(List<GameSystem> result, List<Type> systemTypes, World world, 
        List<Type> existedTypes)
    {
        foreach (var T in systemTypes)
        {
            if (!existedTypes.Contains(T))
            {
                var t = (GameSystem)Activator.CreateInstance(T, world);
                existedTypes.Add(T);
                var tDependencies = t.LocalDependencies();
                CreateSystems(result, tDependencies, world, existedTypes);
                result.Add(t);
            }
        }
    }
    
    public static List<GameSystem> CreateSystems(List<Type> systemTypes, World world)
    {
        var result = new List<GameSystem>();
        var existedTypes = new List<Type>();
        CreateSystems(result, systemTypes, world, existedTypes);
        return result;
    }
    
    
    public GameSystem(World world)
    {
        this.world = world;
    }

    public abstract void Update(uint ticks);
}
