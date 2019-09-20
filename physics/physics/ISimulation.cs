using System;
using System.Collections.Generic;
using NumSharp;

namespace physics
{
    public class Particle
    {
        NDArray Position { get; set; }
        NDArray Velocity { get; set; }
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
        static ISimulation Get()
        {
            return new NothingSimulation();
        }
    }
}