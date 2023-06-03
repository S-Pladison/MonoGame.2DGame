using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

namespace Pladi.Core.Graphics.Particles
{
    public class ParticleSystem
    {
        // [public properties and fields]

        public Texture2D Texture { get; init; }

        // [private properties and fields]

        private readonly List<Particle> particles;
        private readonly UpdateDelegate update;

        // [constructors]

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

        // [public methods]

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

        // [delegates]

        public delegate void UpdateDelegate(Particle particle);
    }
}