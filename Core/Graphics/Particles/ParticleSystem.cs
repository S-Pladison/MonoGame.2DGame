using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pladi.Core.Graphics.Particles
{
    public class ParticleSystem
    {
        public Texture2D Texture { get; init; }

        private readonly List<Particle> particles;
        private readonly UpdateDelegate update;

        // ...

        public ParticleSystem(Texture2D texture, UpdateDelegate update)
        {
            if (texture is null)
                throw new ArgumentNullException(nameof(texture));

            if (update is null)
                throw new ArgumentNullException(nameof(update));

            Texture = texture;

            this.particles = new List<Particle>();
            this.update = update;
        }

        // ...

        public void Add(Particle particle)
        {
            particles.Add(particle);
        }

        public void Update()
        {
            for (int i = 0; i < particles.Count; i++)
			{
                var particle = particles[i];
                particle.Timer -= Main.DeltaTime;

                update.Invoke(particle);

                particle.Position += particle.Velocity * Main.DeltaTime;
            }

            particles.RemoveAll(p => p.Timer <= 0);
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            for (int i = 0; i < particles.Count; i++)
            {
                var particle = particles[i];
                var frame = particle.Frame.Equals(default) ? Texture.Bounds : particle.Frame;
                var position = particle.Position - new Vector2(frame.Width, frame.Height) * 0.5f * particle.Scale;
                var color = particle.Color * (particle.Alpha / 255.0f);

                spriteBatch.Draw(Texture, position, frame, color, particle.Rotation, Vector2.Zero, particle.Scale, SpriteEffects.None, 0);
            }
        }

        // ...

        public delegate void UpdateDelegate(Particle particle);
    }
}