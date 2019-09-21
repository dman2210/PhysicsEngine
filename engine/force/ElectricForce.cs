using System.Collections.Generic;

namespace PhysicsEngine.engine.force
{
    public class ElectricForce: ParticleForce
    {
        public ElectricForce(double electricParam = 50000)
        {
            ElectricParam = electricParam;
        }

        private double ElectricParam { get; }
        
        public override int NumForceParams() => 1;

        public override double CalculateForce(double x1, double y1, double x2, double y2, IList<double> forceParams)
        {
            var distance2 = (x1 - x2) * (x1 - x2) + (y1 - y2) * (y1 - y2);
            var totalCharge = forceParams[0] * forceParams[1];
            return ElectricParam * totalCharge / distance2;
        }
    }
}