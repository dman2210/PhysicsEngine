using System.Collections.Generic;
using System.Linq;
using NumSharp;

namespace PhysicsEngine.engine
{
    public class ForceCalculator
    {
        private IList<IForce> Forces { get; }
        public int TotalForceParams { get; }
        
        public Slice PositionSlice { get; }

        public Slice VelocitySlice { get; }

        public Slice MassSlice { get; }

        public List<Slice> ForceSlices { get; }
        
        public Slice ForceParamsSlice { get; }
        
        public int TotalParams { get; }

        public ForceCalculator(IList<IForce> forces)
        {
            Forces = forces;
            var numPositionParams = 2;
            var numVelocityParams = 2;
            var numMassParams = 1;
            TotalForceParams = forces.Sum(f => f.NumForceParams());
            TotalParams = numPositionParams + numVelocityParams + numMassParams + TotalForceParams;

            var i = 0;
            PositionSlice = new Slice(i, i + numPositionParams);
            i += numPositionParams;
            VelocitySlice = new Slice(i, i + numVelocityParams);
            i += numVelocityParams;
            MassSlice = new Slice(i, i + numMassParams);
            i += numMassParams;
            ForceParamsSlice = new Slice(i, i + TotalForceParams);
            // No increment i intentional
            ForceSlices = new List<Slice>();
            foreach (var force in forces)
            {
                ForceSlices.Add(new Slice(i, i + force.NumForceParams()));
                i += force.NumForceParams();
            }
        }

        public NDArray CalculateAcceleration(NDArray data)
        {
            var positions = data[Slice.All, PositionSlice];
            var mass = data[Slice.All, MassSlice];

            var currentForce = np.zeros_like(positions);
            for (var i = 0; i < Forces.Count; i++)
            {
                var f = Forces[i].CalculateForce(positions, data[Slice.All, ForceSlices[i]]);
                if (!Forces[i].IgnoresMass())
                {
                    f /= mass;
                }
                currentForce += f;
            }

            return currentForce;
        }
    }
}