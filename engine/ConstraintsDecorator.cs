using System.Collections.Generic;
using System.Linq;
using MathNet.Numerics.LinearAlgebra;
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
            var constraintGradient = np.zeros(Constraints.Count, 2 * data.shape[0]);
            var h = np.zeros(Constraints.Count, 1);
            for (var i = 0; i < Constraints.Count; i++)
            {
                constraintGradient[i] = Constraints[i].gradient(data[Slice.All, ForceCalculator.PositionSlice]);
                h[i] = np.dot(
                    np.dot(transpose(flattenedVelocities),
                        Constraints[i].hessian(data[Slice.All, ForceCalculator.PositionSlice])), flattenedVelocities);
            }

            var forces = masses * ForceCalculator.CalculateAcceleration(data);

            var a = massDiag.hstack(-transpose(constraintGradient))
                .vstack(constraintGradient.hstack(np.zeros(Constraints.Count, Constraints.Count)));
            var b = forces.flatten().reshape(-1, 1).vstack(-h);

            var solved = np.dot(inv(a), b);

            return solved[new Slice(0, data.shape[0] * 2)].reshape(-1, 2);
        }

        private NDArray transpose(NDArray arr)
        {
            NDArray ret = np.empty(arr.shape[1], arr.shape[0]);

            for (int i = 0; i < arr.shape[0]; i++)
            {
                for (int j = 0; j < arr.shape[1]; j++)
                {
                    ret[j, i] = arr[i, j];
                }
            }

            return ret;
        }

        private NDArray inv(NDArray arr)
        {
//            var a = Iterate(arr).ToList();
            var mat = Matrix<double>.Build.DenseOfColumns(Iterate(arr).Select(a => ((IEnumerable<object>)a).Cast<double>()));
            var b = mat.Inverse();
            var c = b.ToArray();
            return np.array(c);
        }

        private Matrix<double> ToMatrix(NDArray arr)
        {
            return  Matrix<double>.Build.DenseOfColumns(Iterate(arr).Select(a => ((IEnumerable<object>)a).Cast<double>()));
        }

        private IEnumerable<object> Iterate(NDArray arr)
        {
            if (arr.shape.Length > 1)
            {
                for (var i = 0; i < arr.shape[0]; i++)
                {
                    yield return Iterate(arr[i]);
                }
            }
            else
            {
                for (var i = 0; i < arr.shape[0]; i++)
                {
                    yield return arr[i].GetValue<double>();
                }
            }
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