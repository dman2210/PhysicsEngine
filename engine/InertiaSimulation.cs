using System.Collections.Generic;

namespace physics.engine
{
    public class InertiaSimulation: NothingSimulation
    {
        public override void Tick(double time)
        {
            foreach (var p in Particles.Values)
            {
                p.Position += p.Velocity * time;
            }
        }
    }
}