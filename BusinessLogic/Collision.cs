using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TP.ConcurrentProgramming.Data;

namespace TP.ConcurrentProgramming.BusinessLogic
{
    internal record Collision : Data.ICollision
    {
        public required DateTime TimeStamp { get; init; }
        public required int BallId1 { get; init; }
        public required int? BallId2 { get; init; }
        public required double Radius1 { get; init; }
        public required double? Radius2 { get; init; }
        public required IVector Position1 { get; init; }
        public required IVector? Position2 { get; init; }
        public required IVector Velocity1Before { get; init; }
        public required IVector? Velocity2Before { get; init; }
        public required IVector Velocity1After { get; init; }
        public required IVector? Velocity2After { get; init; }

    }
}
