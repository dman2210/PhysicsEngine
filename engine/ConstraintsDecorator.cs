using System.Collections.Generic;
using System.Linq;
using NumSharp;
using PhysicsEngine.engine.constraint;

namespace PhysicsEngine.engine
{
    public class ConstraintsDecorator
    {
        private ForceCalculator ForceCalculator { get; }
        private IList<IConstraint> Constraints { get; }

        public ConstraintsDecorator(ForceCalculator forceCalculator, IList<IConstraint> constraints)
        {
            ForceCalculator = forceCalculator;
            Constraints = constraints;
        }

        public NDArray CalculateAcceleration(NDArray data)
        {
            if (Constraints.Count == 0)
            {
                return ForceCalculator.CalculateAcceleration(data);
            }
            var flattenedVelocities = data[Slice.All, ForceCalculator.VelocitySlice].flatten().reshape(-1, 1);
            var masses = data[Slice.All, ForceCalculator.MassSlice];
            var repeated = np.repeat(masses, 2);
            var massDiag = diag(repeated);
            var constraintGradient = np.empty(Constraints.Count, 2 * data.shape[0]);
            var h = np.empty(Constraints.Count, 1);
            for (int i = 0; i < Constraints.Count; i++)
            {
                constraintGradient[i] = Constraints[i].gradient(data[i, ForceCalculator.PositionSlice]);
                h[i] = np.dot(
                    np.dot(flattenedVelocities.transpose(),
                        Constraints[i].hessian(data[i, ForceCalculator.PositionSlice])), flattenedVelocities);
            }

            var forces = masses * ForceCalculator.CalculateAcceleration(data);

            var a = massDiag.hstack(-constraintGradient.transpose())
                .vstack(constraintGradient.hstack(np.zeros(Constraints.Count, Constraints.Count)));
            var b = forces.vstack(-h);

            var solved = np.dot(a.inv(), b);

            return solved[new Slice(0, data.shape[0] * 2)].reshape(-1, 2);
        }

        private NDArray diag(NDArray arr)
        {
            var ret = np.zeros(arr.size, arr.size);
            for (var i = 0; i < arr.size; i++)
            {
                ret[i, i] = arr[i];
            }

            return ret;
        }
    }
}