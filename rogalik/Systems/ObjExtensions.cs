using rogalik.Framework;
using rogalik.Systems.Common;

namespace rogalik.Systems;

public static class ObjCommonExtensions
{
    public static string Description(this Obj obj)
    {
        return obj.GetComponent<Appearance>()?.description ?? "something";
    }
}