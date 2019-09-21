using System;
using System.Collections.Generic;
using System.Linq;
using NumSharp;

namespace PhysicsEngine.engine.force
{
    public abstract class ParticleForce : IForce
    {
        public virtual bool IgnoresMass()
        {
            return false;
        }

        public abstract int NumForceParams();

        public NDArray CalculateForce(NDArray positions, NDArray forceParams)
        {
            var result = np.zeros_like(positions);
            for (var i = 0; i < positions.shape[0]; i++)
            {
                var x1 = positions[i, 0].GetValue<double>();
                var y1 = positions[i, 1].GetValue<double>();
                for (var j = i + 1; j < positions.shape[0]; j++)
                {
                    var x2 = positions[j, 0].GetValue<double>();
                    var y2 = positions[j, 1].GetValue<double>();
                    var f = CalculateForce(x1, y1, x2, y2, forceParams[i].ToArray<double>().Concat(forceParams[j].ToArray<double>()).ToList());
                    var d = Math.Sqrt((x1 - x2) * (x1 - x2) + (y1 - y2) * (y1 - y2));
                    result[i][0] += f * (x2 - x1) / d;
                    result[i][1] += f * (y2 - y1) / d;
                    result[j][0] += f * (x1 - x2) / d;
                    result[j][1] += f * (y1 - y2) / d;
                }
            }

            return result;
        }

        public abstract double CalculateForce(double x1, double y1, double x2, double y2, IList<double> forceParams);
    }
}