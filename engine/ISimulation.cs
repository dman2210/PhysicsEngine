using System.Collections.Generic;
using System.Collections.Immutable;
using NumSharp;
using PhysicsEngine.engine;
using PhysicsEngine.engine.constraint;
using PhysicsEngine.engine.force;

namespace physics.engine
{
    public class Particle
    {
        public NDArray Position { get; set; }
        public NDArray Velocity { get; set; }
    }
    
    public interface ISimulation
    {
//        void AddParticle(int id, Particle particleInfo);
        void Tick(double time);
        IList<int> ParticleIds();
        Particle GetParticle(int id);
    }

    public static class Simulation
    {
        public static InertiaSimulation Get()
        {
            return new InertiaSimulation();
        }

        public static ForceSimulation GetForceSimulation()
        {
            return new ForceSimulation(new ElectricForce());
        }

        public static ForceSimulation GetConstrainedSimulation()
        {
            return new ForceConstraintsSimulation(new List<IForce>{new ElectricForce()}, ImmutableList<IConstraint>.Empty);
        }
    }
}