using System;
using Cairo;
using Gtk;
using physics.engine;

namespace PhysicsEngine.drawing
{
    public class Drawer2d : Window
    {
        private ISimulation Simulation { get; }
        private const int MsPerTick = 50;
        private const double TimePerTick = MsPerTick / 1000.0;

        public Drawer2d(ISimulation simulation) : base("Physics Engine")
        {
            Simulation = simulation;
            Maximize();
            ShowAll();
            GLib.Timeout.Add(MsPerTick, () =>
            {
                Simulation.Tick(TimePerTick);
                QueueDraw();
                return true;
            });
            Button btnQuit = new Button("Exit");
            btnQuit.Clicked += OnExitClick;
            KeyPressEvent += OnKeyPressed;
            DeleteEvent += delegate { Application.Quit(); };
            btnQuit.Show();
            Add(btnQuit);
            ShowAll();
        }
        
        void OnExitClick(object sender, EventArgs args)
        {
            Application.Quit();
        }

        void OnKeyPressed(object sender, KeyPressEventArgs args)
        {
            if (args.Event.Key == Gdk.Key.Escape)
            {
                Application.Quit();
            } 
        }

        protected override bool OnDrawn(Context cr)
        {
            cr.Save();
            cr.Translate(AllocatedWidth/2.0, AllocatedHeight/2.0);
            cr.Transform(new Matrix(1, 0, 0, -1, 0, 0));
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
//            base.OnDrawn(cr);
            return true;
        }
    }
}