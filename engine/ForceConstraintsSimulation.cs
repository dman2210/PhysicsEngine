using System.Collections.Generic;
using System.Linq;
using NumSharp;
using physics.engine;
using PhysicsEngine.engine.constraint;

namespace PhysicsEngine.engine
{
    public class ForceConstraintsSimulation: ForceSimulation
    {
        
        public ForceConstraintsSimulation(IList<IForce> forces, IList<IConstraint> constraints): base(forces.ToArray())
        {
            Constraints = new ConstraintsDecorator(new ForceCalculator(forces), constraints);
        }

        private ConstraintsDecorator Constraints { get; }

        public override void Tick(double time)
        {
            if (Data.shape[0] <= 0) return;

            var fParamZeros = np.zeros_like(Data[Slice.All, new Slice(4)]);
            
            var k1 = time * Data[Slice.All, VelocitySlice].hstack(Constraints.CalculateAcceleration(Data));
            var k1d2 = Data + (k1 / 2).hstack(fParamZeros);
            var k2 = time * k1d2[Slice.All, VelocitySlice].hstack(Constraints.CalculateAcceleration(k1d2));
            var k2d2 = Data + (k2 / 2).hstack(fParamZeros);
            var k3 = time * k2d2[Slice.All, VelocitySlice].hstack(Constraints.CalculateAcceleration(k2d2));
            var k3d4 = Data + k3.hstack(fParamZeros);
            var k4 = time * k3d4[Slice.All, VelocitySlice].hstack(Constraints.CalculateAcceleration(k3d4));
            Data += 1.0 / 6.0 * (k1 + 2 * k2 + 2 * k3 + k4).hstack(fParamZeros);
        }
    }
}