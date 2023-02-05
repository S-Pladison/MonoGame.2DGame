using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Pladi.Utilities.Enums;
using SharpDX.MediaFoundation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using static System.Net.Mime.MediaTypeNames;

namespace Pladi.Core.Input
{
    public class InputManager
    {
        private static readonly HashSet<Keys> blackListKeys;

        static InputManager()
        {
            blackListKeys = new() { Keys.Back };
        }

        // ...

        private KeyboardState currentKeyboardState;
        private KeyboardState previousKeyboardState;
        private MouseState currentMouseState;
        private MouseState previousMouseState;
        private List<char> textInputChars;
        private bool writingText;
        private float backSpaceTime;
        private float backSpaceSpeed;

        // ...

        public InputManager(GameWindow window)
        {
            currentKeyboardState = Keyboard.GetState();
            previousKeyboardState = currentKeyboardState;
            currentMouseState = Mouse.GetState();
            previousMouseState = currentMouseState;
            textInputChars = new List<char>();

            window.TextInput += ProcessTextInput;
        }

        // ...

        public void Update(GameTime gameTime)
        {
            previousKeyboardState = currentKeyboardState;
            currentKeyboardState = Keyboard.GetState();
            previousMouseState = currentMouseState;
            currentMouseState = Mouse.GetState();
            writingText = false;
        }

        public bool IsPressed(Keys key)
            => Main.IsGameActive && currentKeyboardState.IsKeyDown(key);

        public bool IsPressed(MouseInputTypes input)
            => IsPressed(currentMouseState, input);

        private bool IsPressed(MouseState state, MouseInputTypes input)
        {
            if (!Main.IsGameActive) return false;

            return input switch
            {
                MouseInputTypes.LeftButton => state.LeftButton == ButtonState.Pressed,
                MouseInputTypes.MiddleButton => state.MiddleButton == ButtonState.Pressed,
                MouseInputTypes.RightButton => state.RightButton == ButtonState.Pressed,
                MouseInputTypes.Button1 => state.XButton1 == ButtonState.Pressed,
                MouseInputTypes.Button2 => state.XButton2 == ButtonState.Pressed,
                _ => false,
            };
        }

        public bool IsHeld(Keys key)
            => Main.IsGameActive && currentKeyboardState.IsKeyDown(key) && previousKeyboardState.IsKeyDown(key);

        public bool IsHeld(MouseInputTypes input)
            => IsPressed(currentMouseState, input) && IsPressed(previousMouseState, input);

        public bool JustPressed(Keys key)
            => Main.IsGameActive && currentKeyboardState.IsKeyDown(key) && !previousKeyboardState.IsKeyDown(key);

        public bool JustPressed(MouseInputTypes input)
            => IsPressed(currentMouseState, input) && !IsPressed(previousMouseState, input);

        public bool JustReleased(Keys key)
            => Main.IsGameActive && !currentKeyboardState.IsKeyDown(key) && previousKeyboardState.IsKeyDown(key);
        public bool JustReleased(MouseInputTypes input)
            => !IsPressed(currentMouseState, input) && IsPressed(previousMouseState, input);

        public Vector2 GetMousePosition()
            => currentMouseState.Position.ToVector2();

        public bool IsMouseMoved()
            => currentMouseState.X != previousMouseState.X || currentMouseState.Y != previousMouseState.Y;

        public int GetMouseScroll()
            => currentMouseState.ScrollWheelValue - previousMouseState.ScrollWheelValue;

        public string GetInputText(string oldString)
        {
            foreach (var ch in textInputChars)
            {
                oldString += ch;
            }

            textInputChars.Clear();

            GetInputText_BackSpace(ref oldString);

            return oldString;
        }

        private void GetInputText_BackSpace(ref string oldText)
        {
            writingText = true;

            if (!IsHeld(Keys.Back))
            {
                backSpaceTime = backSpaceSpeed = 0;
                return;
            }

            backSpaceSpeed = Math.Min(backSpaceSpeed += 0.005f, 1.0f);
            backSpaceTime = Math.Max(backSpaceTime -= backSpaceSpeed, 0);

            if (backSpaceTime is not 0) return;
            
            oldText = oldText[..Math.Max(oldText.Length - 1, 0)];
            backSpaceTime = 1f;
        }

        private void ProcessTextInput(object sender, TextInputEventArgs e)
        {
            if (!writingText || blackListKeys.Contains(e.Key)) return;

            textInputChars.Add(e.Character);
        }
    }
}