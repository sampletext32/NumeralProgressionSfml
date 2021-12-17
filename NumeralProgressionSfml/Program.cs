using System;
using System.Collections.Generic;
using SFML.Audio;
using SFML.Graphics;
using SFML.System;
using SFML.Window;

namespace NumeralProgressionSfml
{
    class Program
    {
        static void Main(string[] args)
        {
            float leftTimeSeconds = 30;

            float shapeSize   = 50;
            int   shapesCount = 10;

            bool win  = false;
            bool lose = false;

            uint windowWidth  = 800;
            uint windowHeight = 600;
            var videoMode = new VideoMode(
                width: windowWidth,
                height: windowHeight,
                bpp: 24
            );

            var window = new RenderWindow(videoMode, "Kragmorta Game");

            Random      random = new(DateTime.Now.Millisecond);
            List<Shape> shapes = new(shapesCount);

            Font       font  = new Font("arial.ttf");
            List<Text> texts = new(shapesCount);

            window.Resized += (sender, eventArgs) => { window.SetView(new View(new FloatRect(0, 0, window.Size.X, window.Size.Y))); };

            window.Closed += (sender, eventArgs) => { window.Close(); };


            SoundBuffer soundBuffer = new("terraria-soundtrack.ogg");
            Sound sound = new(soundBuffer)
            {
                Loop = true
            };

            // Generate shapes
            for (int i = 0; i < shapesCount; i++)
            {
                switch (random.Next(0, 3))
                {
                    case 0:
                        shapes.Add(new CircleShape(shapeSize / 2) { FillColor = Color.White });
                        break;
                    case 1:
                        shapes.Add(new RectangleShape(new Vector2f(shapeSize, shapeSize)) { FillColor = Color.White });
                        break;
                    case 2:
                        var convexShape = new ConvexShape(3) { FillColor = Color.White };
                        convexShape.SetPoint(0, new Vector2f(0, 0));
                        convexShape.SetPoint(1, new Vector2f(shapeSize / 2, shapeSize));
                        convexShape.SetPoint(2, new Vector2f(shapeSize, 0));
                        shapes.Add(convexShape);
                        break;
                }

                texts.Add(new Text((shapesCount - i).ToString(), font, 16) { FillColor = Color.Black });
            }

            ApplyColorAlpha(shapes);

            for (int i = 0; i < shapesCount; i++)
            {
                // All shapes are fully inside screen
                shapes[i].Position = new Vector2f(
                    (float)(random.NextDouble() * (windowWidth - shapeSize * 2) + shapeSize),
                    (float)(random.NextDouble() * (windowHeight - shapeSize * 2) + shapeSize)
                );

                texts[i].Position = shapes[i].Position +
                                    new Vector2f(
                                        shapeSize / 2 - texts[i].GetGlobalBounds().Width / 2,
                                        shapeSize / 2 - texts[i].GetGlobalBounds().Height / 2
                                    );
            }

            window.MouseButtonPressed += (sender, eventArgs) =>
            {
                int i = shapes.Count - 1;
                if (shapes[i].Position.X <= eventArgs.X && shapes[i].Position.X + shapeSize >= eventArgs.X &&
                    shapes[i].Position.Y <= eventArgs.Y && shapes[i].Position.Y + shapeSize >= eventArgs.Y)
                {
                    shapes.RemoveAt(i);
                    texts.RemoveAt(i);
                    ApplyColorAlpha(shapes);

                    if (i == 0)
                    {
                        win = true;
                    }
                    else
                    {
                        Console.WriteLine($"Left {shapes.Count}");
                    }
                }
            };
            window.MouseButtonReleased += (sender, eventArgs) =>
            {
                // engine.OnMouseButtonReleased(eventArgs.X, eventArgs.Y, eventArgs.Button.ToKragMouseButton());
            };

            Clock clock = new Clock();
            window.SetFramerateLimit(60);
            sound.Play();

            RectangleShape timeBackground = new RectangleShape(new Vector2f(200, 50)) { FillColor = new Color(255, 255, 255, 50) };

            Text timeText = new Text() { Font = font, CharacterSize = 16 };
            Text winText  = new Text() { Font = font, CharacterSize = 48, FillColor = Color.Red, DisplayedString = "WIN" };
            Text loseText = new Text() { Font = font, CharacterSize = 48, FillColor = Color.Red, DisplayedString = "GAME OVER" };
            loseText.Position = new Vector2f(
                windowWidth / 2 - loseText.GetGlobalBounds().Width / 2,
                windowHeight / 2 - loseText.GetGlobalBounds().Height / 2
            );

            while (window.IsOpen)
            {
                window.DispatchEvents();

                float deltaTime = clock.Restart().AsSeconds();

                // keep track of time
                leftTimeSeconds -= deltaTime;

                if (leftTimeSeconds <= 0)
                {
                    lose = true;
                }

                timeText.DisplayedString = leftTimeSeconds.ToString("F2") + " secs";

                window.Clear();

                if (win)
                {
                    window.Draw(winText);
                }
                else if (lose)
                {
                    window.Draw(loseText);
                }
                else
                {
                    window.Draw(timeBackground);
                    window.Draw(timeText);
                    for (var i = 0; i < shapes.Count; i++)
                    {
                        window.Draw(shapes[i]);
                        window.Draw(texts[i]);
                    }
                }

                window.Display();
            }

            if (sound.Status == SoundStatus.Playing)
            {
                sound.Stop();
            }

            Console.WriteLine("Exiting");
        }

        private static void ApplyColorAlpha(List<Shape> shapes)
        {
            if (shapes.Count == 1)
            {
                shapes[0].FillColor = new Color(255, 255, 255, 255);
            }
            else
            {
                for (int i = 0; i < shapes.Count; i++)
                {
                    shapes[i].FillColor = new Color(255, 255, 255, (byte)(((float)i / shapes.Count) * 255));
                }
            }
        }
    }
}