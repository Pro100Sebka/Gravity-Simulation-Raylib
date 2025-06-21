using System;
using System.Numerics;
using System.Collections.Generic;
using Raylib_cs;

namespace Space
{
    public class Program
    {
        public static float TimeScale = 100f;
        
        private const int WindowWidth = 800;
        private const int WindowHeight = 600;
        
        static Vector2? dragStart = null;

        public static void Main()
        {
            Raylib.InitWindow(WindowWidth, WindowHeight, "Space Gravity");
            Raylib.SetTargetFPS(144);

            Game game = new Game();

            Camera2D camera = new Camera2D();
            camera.Zoom = 1f;
            camera.Offset = new Vector2(WindowWidth / 2f, WindowHeight / 2f);
            camera.Target = Vector2.Zero;

            while (!Raylib.WindowShouldClose())
            {
                float deltaTime = Raylib.GetFrameTime() * TimeScale;
                
                // ==== CAMERA CONTROL ====
                float cameraSpeed = 10f / camera.Zoom;
                if (Raylib.IsKeyDown(KeyboardKey.D)) camera.Target.X += cameraSpeed;
                if (Raylib.IsKeyDown(KeyboardKey.A)) camera.Target.X -= cameraSpeed;
                if (Raylib.IsKeyDown(KeyboardKey.S)) camera.Target.Y += cameraSpeed;
                if (Raylib.IsKeyDown(KeyboardKey.W)) camera.Target.Y -= cameraSpeed;
                
                float wheel = Raylib.GetMouseWheelMove();
                if (wheel != 0)
                {
                    camera.Zoom += wheel * 0.1f;
                    if (camera.Zoom < 0.1f) camera.Zoom = 0.1f;
                }
                
                // ==== TIME CONTROL ====
                if (Raylib.IsKeyPressed(KeyboardKey.Space))
                {
                    if (TimeScale == 100) { TimeScale = 0; }
                    else TimeScale = 100;
                }

                // ==== GAME UPDATE ====
                game.Update(deltaTime);

                // ==== ENTITY CREATION ====
                if (Raylib.IsMouseButtonPressed(MouseButton.Left))
                {
                    Vector2 startMousePos = Raylib.GetMousePosition();
                    dragStart = Raylib.GetScreenToWorld2D(startMousePos, camera);
                }
                
                if (Raylib.IsMouseButtonReleased(MouseButton.Left))
                {
                    Vector2 mousePos = Raylib.GetMousePosition();
                    Vector2 dragEnd = Raylib.GetScreenToWorld2D(mousePos, camera);
                    Entity entity = new Entity(dragStart.Value, (dragStart.Value - dragEnd) * -.025f , 1000, 100);
                    game.entities.Add(entity);
                    dragStart = null;
                }
                
                //====OTHER INPUT=====
                if (Raylib.IsKeyPressed(KeyboardKey.R))
                {
                    for (int i = 0; i < game.entities.Count; i++)
                    {
                        game.entities[i].IsDead = true;
                    }
                }
                
                // ==== DRAWING ====
                Raylib.BeginDrawing();
                Raylib.ClearBackground(Color.Black);

                Raylib.BeginMode2D(camera);
                game.Draw();
                Raylib.EndMode2D();

                // UI
                Raylib.DrawText("Space Simulation Game", 10, 10, 20, Color.White);
                Raylib.DrawText($"Zoom: {camera.Zoom:F2}", 10, 35, 18, Color.Green);
                Raylib.DrawText($"Controls:", 10, 75, 20, Color.LightGray);
                Raylib.DrawText($"Space = Pause\nWASD = Move\nMouseWheel = Zoom\nLeftClick = Spawn\nDragAndDrop = Spawn", 10, 100, 15, Color.Gray);

                Raylib.EndDrawing();
            }

            Raylib.CloseWindow();
        }
    }
}
