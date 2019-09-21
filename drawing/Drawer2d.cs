using System;
using Cairo;
using Gtk;
using physics.engine;

namespace PhysicsEngine.drawing
{
    public class Drawer2d : Window
    {
        private ISimulation Simulation { get; }
        private const int MsPerTick = 100;
        private const double TimePerTick = MsPerTick / 1000.0;

        public Drawer2d(ISimulation simulation) : base("Physics Engine")
        {
            Simulation = simulation;
            Maximize();
            ShowAll();
            GLib.Timeout.Add(100, () =>
            {
                Simulation.Tick(TimePerTick);
                QueueDraw();
                return true;
            });
            DeleteEvent += delegate { Application.Quit(); };
        }

        protected override bool OnDrawn(Context cr)
        {
            cr.Save();
            cr.Translate(AllocatedWidth/2.0, AllocatedHeight/2.0);
            cr.LineWidth = 2;
            cr.SetSourceColor(new Color(1, 0, 0));

            var ids = Simulation.ParticleIds();
            foreach (var id in ids)
            {
                var particle = Simulation.GetParticle(id);

                cr.Arc(particle.Position[0], particle.Position[1], 2, 0, 2 * Math.PI);
                cr.Fill();
            }
            
            cr.Restore();
            return true;
        }
    }
}