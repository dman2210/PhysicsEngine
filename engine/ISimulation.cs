using System.Collections.Generic;
using NumSharp;

namespace physics.engine
{
    public class Particle
    {
        public NDArray Position { get; set; }
        public NDArray Velocity { get; set; }
    }
    
    public interface ISimulation
    {
        void AddParticle(int id, Particle particleInfo);
        void Tick(double time);
        IList<int> ParticleIds();
        Particle GetParticle(int id);
    }

    public static class Simulation
    {
        public static ISimulation Get()
        {
            return new InertiaSimulation();
        }
    }
}