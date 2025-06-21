using System;
using System.Numerics;
using Raylib_cs;

namespace Space
{
    public class Entity
    {
        private const float G = 0.1f;

        public Vector2 Position;
        public Vector2 Velocity;
        public float Radius;
        public Color Color = Color.White;
        public float Mass;
        public float Temp;
        public bool IsDead = false;

        public Entity(Vector2 position, Vector2 velocity, float mass, float temp = 0)
        {
            Position = position;
            Velocity = velocity;
            Mass = mass;
            Temp = temp;

            Radius = MathF.Sqrt(Mass / MathF.PI);
            UpdateColor();
        }

        public void Attract(Entity other, float deltaTime)
        {
            if (IsDead || other.IsDead)
                return;

            Vector2 direction = other.Position - Position;
            float distance = direction.Length();
            if (distance < 1f) return;

            Vector2 forceDir = Vector2.Normalize(direction);
            float minDistance = Radius + other.Radius;

            // === COLLISION & MERGE ===
            if (distance < minDistance)
            {
                float e1 = 0.5f * Mass * Velocity.Length();
                float e2 = 0.5f * other.Mass * other.Velocity.Length();
                float avgTemp = (Temp + other.Temp + e1 + e2) / 2f;


                Vector2 totalImpulse = (Velocity * Mass + other.Velocity * other.Mass) ;
                float totalMass = Mass + other.Mass;
                Vector2 mergedVelocity = totalImpulse / totalMass;

                if (Mass >= other.Mass)
                {
                    Mass = totalMass;
                    Velocity = mergedVelocity;
                    ChangeTemp(avgTemp);
                    other.IsDead = true;
                }
                else
                {
                    other.Mass = totalMass;
                    other.Velocity = mergedVelocity;
                    other.ChangeTemp(avgTemp);
                    IsDead = true;
                }
            }
            else
            {
                // === GRAVITY ===
                float force = G * (Mass * other.Mass) / (distance * distance);
                Vector2 forceVector = forceDir * force;

                Vector2 accelerationThis = forceVector / Mass;
                Vector2 accelerationOther = -forceVector / other.Mass;

                Velocity += accelerationThis * deltaTime;
                other.Velocity += accelerationOther * deltaTime;
            }
        }

        public void Update(float deltaTime)
        {
            float loss = ((Mass * 0.0001f) * (Temp / 255f)) * deltaTime;
            Mass -= loss;
            if (Mass <= 1f) IsDead = true;

            if (Temp >= 1000) {Temp = 1000;  }
            Temp = MathF.Max(0f, Temp - .0001f * deltaTime * Mass);

            

            Position += Velocity * deltaTime;
            Radius = MathF.Sqrt(Mass / MathF.PI);

            UpdateColor();
        }

        public void Draw()
        {
            Raylib.DrawCircleV(Position, Radius, Color);
        }

        public void ChangeTemp(float newTemp)
        {
            Temp = newTemp;
            UpdateColor();
        }

        private void UpdateColor()
        {

            float tNorm = MathF.Min(Temp / 1000f, 1f);

            int r = (int)(tNorm * 255f);
            int g = (int)(MathF.Max(0.5f - MathF.Abs(tNorm - 0.5f), 0f) * 1.5f * 255f); 
            int b = (int)((1 - tNorm) * 255f);


            Color = new Color(r, g, b, 255);
        }
    }
}
