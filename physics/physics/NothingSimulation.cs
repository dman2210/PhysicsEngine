using System.Collections.Generic;
using System.Linq;

namespace physics
{
    public class NothingSimulation : ISimulation
    {
        private IDictionary<int, Particle> Particles { get; } = new Dictionary<int, Particle>();
        
        public void AddParticle(int id, Particle particleInfo)
        {
            Particles[id] = particleInfo;
        }

        public void Tick(double time)
        {
            // Nothing
        }

        public IList<int> ParticleIds()
        {
            return Particles.Keys.ToList();
        }

        public Particle GetParticle(int id)
        {
            return Particles[id];
        }
    }
}