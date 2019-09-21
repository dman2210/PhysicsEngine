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
        private IList<IForce> Forces { get; }
        private Dictionary<int, int> ParticleIdToDataIndex { get; } = new Dictionary<int, int>();

        private int TotalForceParams { get; }

        /// <summary>
        /// An n*m matrix of data for our points.
        /// n is the number of points.
        /// </summary>
        private NDArray Data { get; set; }

        public ForceSimulation(params IForce[] forces)
        {
            Forces = forces;
            var numPositionParams = 2;
            var numVelocityParams = 2;
            var numMassParams = 1;
            TotalForceParams = forces.Sum(f => f.NumForceParams());
            var totalParams = numPositionParams + numVelocityParams + numMassParams + TotalForceParams;

            var i = 0;
            PositionSlice = new Slice(i, i + numPositionParams);
            i += numPositionParams;
            VelocitySlice = new Slice(i, i + numVelocityParams);
            i += numVelocityParams;
            MassSlice = new Slice(i, i + numMassParams);
            i += numMassParams;
            ForceSlices = new List<Slice>();
            foreach (var force in forces)
            {
                ForceSlices.Add(new Slice(i, i + force.NumForceParams()));
                i += force.NumForceParams();
            }

            Data = np.empty(new Shape(0, totalParams));
        }

        private NDArray CalculateForce(NDArray data)
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

        private Slice PositionSlice { get; }

        private Slice VelocitySlice { get; set; }

        private Slice MassSlice { get; set; }

        private List<Slice> ForceSlices { get; set; }
        public void Tick(double time)
        {
            // Use runge kutta later
            if (Data.shape[0] <= 0) return;
            var force = CalculateForce(Data);
            Data[Slice.All, PositionSlice] += Data[Slice.All, VelocitySlice] * time + .5 * force * time * time;
        }

        public IList<int> ParticleIds()
        {
            return ParticleIdToDataIndex.Keys.ToList();
        }

        public void AddParticle(int id, double x, double y, double vx, double vy, double mass,
            params double[] forceParams)
        {
            if (forceParams.Length != TotalForceParams)
            {
                throw new ArgumentException("Wrong number of parameters for forces");
            }
            var row = new List<double>
            {
                x, y, vx, vy, mass
            };
            row.AddRange(forceParams);
            Data = Data.vstack(np.array(new[] {row.ToArray()}));
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