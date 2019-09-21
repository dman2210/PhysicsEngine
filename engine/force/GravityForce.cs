using NumSharp;

namespace PhysicsEngine.engine.force
{
    public class GravityForce: IForce
    {
        public GravityForce(double g = -9.8)
        {
            G = g;
        }

        private double G { get; }

        public bool IgnoresMass() => true;

        public int NumForceParams() => 0;

        public NDArray CalculateForce(NDArray positions, NDArray forceParams)
        {
            var force = np.full_like(positions, G);
            force[Slice.All, 0] = 0;
            return force;
        }
    }
}