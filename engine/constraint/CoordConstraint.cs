using NumSharp;

namespace PhysicsEngine.engine.constraint
{
    public class CoordConstraint: IConstraint
    {
        public CoordConstraint(int i)
        {
            I = i;
        }

        private int I { get; }

        public double constraint(NDArray positions)
        {
            return 0;
        }

        public NDArray gradient(NDArray data)
        {
            var ret = np.zeros(data.shape[0] * 2);
            ret[I] = 1;
//            ret[2*Id1] = 2*(data[Id1][0].GetValue<double>()-data[Id2][0].GetValue<double>());
//            ret[2*Id1 + 1] = 2*(data[Id1][1].GetValue<double>()-data[Id2][1].GetValue<double>());
//            ret[2*Id2] = -2*(data[Id1][0].GetValue<double>()-data[Id2][0].GetValue<double>());
//            ret[2*Id2 + 1] = -2*(data[Id1][1].GetValue<double>()-data[Id2][1].GetValue<double>());
            return ret;
        }

        public NDArray hessian(NDArray data)
        {
            var ret = np.zeros(data.shape[0] * 2, data.shape[0]*2);
//            ret[2*Id1, 2*Id1] = 2;
//            ret[2*Id1, 2*Id2] = -2;
//            ret[2*Id1 + 1, 2*Id1 + 1] = 2;
//            ret[2*Id1 + 1, 2*Id2 + 1] = -2;
//            ret[2*Id2, 2*Id1] = -2;
//            ret[2*Id2, 2*Id2] = 2;
//            ret[2*Id2 + 1, 2*Id1 + 1] = -2;
//            ret[2*Id2 + 1, 2*Id2 + 1] = 2;
            return ret;
        }
    }
}