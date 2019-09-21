using NumSharp;

namespace PhysicsEngine.engine.force
{
    public class GravityForce: IForce
    {
        public GravityForce(double g = -100)
        {
            G = g;
        }

        private double G { get; }

        public bool IgnoresMass() => true;

        public int NumForceParams() => 0;

        public NDArray CalculateForce(NDArray positions, NDArray forceParams)
        {
            var squared = positions * positions;
            var ds = np.sqrt(squared[Slice.All, 0] + squared[Slice.All, 1]);
            var normed = positions / ds.reshape(-1, 1);
            var force = G * normed;
            return force;
        }
    }
}