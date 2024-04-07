using rogalik.Common;
using rogalik.Framework;

namespace rogalik.Time;

// public class ActionSystem : GameSystem
// {
//     public ActionSystem(World world) : base(world)
//     {
//     }
//
//     public override void Update(uint ticks)
//     {
//         var filter = new Filter().With<ActionQueue>().Apply(world.objects);
//
//         foreach (var obj in filter)
//         {
//             var action = obj.GetComponent<ActionQueue>().queue.Peek().action;
//             var time = obj.GetComponent<ActionQueue>().queue.Peek().Item2;
//             
//         }
//     }
// }