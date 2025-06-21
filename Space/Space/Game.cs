using System.Collections.Generic;
using System.Numerics;
using Raylib_cs;

namespace Space
{
    public class Game
    {
        public List<Entity> entities;

        public Game()
        {
            entities = new List<Entity>
            {
            };
        }

        public void Update(float deltaTime)
        {
            foreach (var entity in entities)
            {
                
                foreach (var otherEntity in entities)
                {
                    if (otherEntity != entity)
                    {
                        entity.Attract(otherEntity, deltaTime);
                    }
                }
            }

            foreach (var entity in entities)
            {
                entity.Update(deltaTime);
            }
            
                
            entities.RemoveAll(e => e.IsDead);
        }

        public void Draw()
        {
            foreach (var entity in entities)
                entity.Draw();
        }
    }
}