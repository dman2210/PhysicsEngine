using System;
using Cairo;
using Gdk;
using Gtk;
using NumSharp;
using physics.engine;
using PhysicsEngine.drawing;
using Window = Gtk.Window;

namespace PhysicsEngine
{
    class Program
    {
        private static Random r = new Random();

        static void Main(string[] args)
        {
            Application.Init();
            var sim = Simulation.GetForceSimulation();
            var c = new Drawer2d(sim);

            var width = c.Screen.Width;
            var height = c.Screen.Height;

            for (var i = 0; i < 100; i++)
            {
                sim.AddParticle(i,
                    r.NextDouble() * width - (width / 2.0),
                    r.NextDouble() * height - height / 2.0,
                    r.NextDouble() * 20 - 10,
                    r.NextDouble() * 20 - 10,
                    r.NextDouble() * 9 + 1
                );
            }

            Application.Run();
        }
    }
}