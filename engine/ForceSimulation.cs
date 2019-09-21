using System;
using System.Collections.Generic;
using System.Linq;
using NumSharp;
using physics.engine;

namespace PhysicsEngine.engine
{
    public interface IForce
    {
        /// <summary>
        /// Determines if the force ignores mass (e.g. gravity)
        /// </summary>
        /// <returns></returns>
        bool IgnoresMass();
        int NumForceParams();
        NDArray CalculateForce(NDArray positions, NDArray forceParams);
    }

    public class ForceSimulation: ISimulation
    {
        private Dictionary<int, int> ParticleIdToDataIndex { get; } = new Dictionary<int, int>();
        protected ForceCalculator ForceCalculator { get; }

        /// <summary>
        /// An n*m matrix of data for our points.
        /// n is the number of points.
        /// </summary>
        protected NDArray Data { get; set; }
        protected Slice PositionSlice { get; }

        protected Slice VelocitySlice { get; }


        public ForceSimulation(params IForce[] forces)
        {
            ForceCalculator = new ForceCalculator(forces);

            Data = np.empty(new Shape(0, ForceCalculator.TotalParams));
            VelocitySlice = ForceCalculator.VelocitySlice;
            PositionSlice = ForceCalculator.PositionSlice;
        }

        public virtual void Tick(double time)
        {
            // Use runge kutta later
            if (Data.shape[0] <= 0) return;

            var fParamZeros = np.zeros_like(Data[Slice.All, new Slice(4)]);
            
            var k1 = time * Data[Slice.All, VelocitySlice].hstack(ForceCalculator.CalculateAcceleration(Data));
            var k1d2 = Data + (k1 / 2).hstack(fParamZeros);
            var k2 = time * k1d2[Slice.All, VelocitySlice].hstack(ForceCalculator.CalculateAcceleration(k1d2));
            var k2d2 = Data + (k2 / 2).hstack(fParamZeros);
            var k3 = time * k2d2[Slice.All, VelocitySlice].hstack(ForceCalculator.CalculateAcceleration(k2d2));
            var k3d4 = Data + k3.hstack(fParamZeros);
            var k4 = time * k3d4[Slice.All, VelocitySlice].hstack(ForceCalculator.CalculateAcceleration(k3d4));
            Data += 1.0 / 6.0 * (k1 + 2 * k2 + 2 * k3 + k4).hstack(fParamZeros);
        }
 
        public IList<int> ParticleIds()
        {
            return ParticleIdToDataIndex.Keys.ToList();
        }

        public void AddParticle(int id, double x, double y, double vx, double vy, double mass,
            params double[] forceParams)
        {
            if (forceParams.Length < ForceCalculator.TotalForceParams)
            {
                throw new ArgumentException("Too few parameters for forces");
            }
            var row = new List<double>
            {
                x, y, vx, vy, mass
            };
            row.AddRange(forceParams);
            if (Data.shape[0] == 0)
            {
                Data = np.array(new[] {row.ToArray()});
            }
            else
            {
                Data = Data.vstack(np.array(new[] {row.ToArray()}));
            }

            ParticleIdToDataIndex[id] = Data.shape[0] - 1;
        }

        public Particle GetParticle(int id)
        {
            var row = Data[ParticleIdToDataIndex[id]];
            return new Particle
            {
                Position = row[PositionSlice],
                Velocity = row[VelocitySlice]
            };
        }
    }
}