using NumSharp;

namespace PhysicsEngine.engine.constraint
{
    public interface IConstraint
    {
        double constraint(NDArray positions);
        NDArray gradient(NDArray positions);
        NDArray hessian(NDArray positions);
    }
}