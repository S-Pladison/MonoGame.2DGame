using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Pladi.Utilities.Enums;
using System;
using System.Collections.Generic;

namespace Pladi.Core.Input
{
    public class InputComponent : BasicComponent
    {
        private static readonly HashSet<Keys> blackListKeys;

        static InputComponent()
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

        public Vector2 MousePosition
            => currentMouseState.Position.ToVector2();

        // ...

        public override void Initialize()
        {
            currentKeyboardState = Keyboard.GetState();
            previousKeyboardState = currentKeyboardState;
            currentMouseState = Mouse.GetState();
            previousMouseState = currentMouseState;
            textInputChars = new List<char>();

            Game.Window.TextInput += ProcessTextInput;
            Visible = false;
        }

        public override void Update(GameTime gameTime)
        {
            previousKeyboardState = currentKeyboardState;
            currentKeyboardState = Keyboard.GetState();
            previousMouseState = currentMouseState;
            currentMouseState = Mouse.GetState();
            writingText = false;
        }

        public bool IsPressed(Keys key)
            => Main.Instance.IsActive && currentKeyboardState.IsKeyDown(key);

        public bool IsPressed(MouseInputTypes input)
            => IsPressed(currentMouseState, input);

        private bool IsPressed(MouseState state, MouseInputTypes input)
        {
            if (!Main.Instance.IsActive) return false;

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
            => Main.Instance.IsActive && currentKeyboardState.IsKeyDown(key) && previousKeyboardState.IsKeyDown(key);

        public bool IsHeld(MouseInputTypes input)
            => IsPressed(currentMouseState, input) && IsPressed(previousMouseState, input);

        public bool JustPressed(Keys key)
            => Main.Instance.IsActive && currentKeyboardState.IsKeyDown(key) && !previousKeyboardState.IsKeyDown(key);

        public bool JustPressed(MouseInputTypes input)
            => IsPressed(currentMouseState, input) && !IsPressed(previousMouseState, input);

        public bool JustReleased(Keys key)
            => Main.Instance.IsActive && !currentKeyboardState.IsKeyDown(key) && previousKeyboardState.IsKeyDown(key);
        public bool JustReleased(MouseInputTypes input)
            => !IsPressed(currentMouseState, input) && IsPressed(previousMouseState, input);

        public bool IsMouseMoved()
            => currentMouseState.X != previousMouseState.X || currentMouseState.Y != previousMouseState.Y;

        public int GetMouseScroll()
            => currentMouseState.ScrollWheelValue - previousMouseState.ScrollWheelValue;

        public string GetInputText(string oldString)
        {
            writingText = true;

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
            if (!IsHeld(Keys.Back))
            {
                backSpaceTime = backSpaceSpeed = 0;
                return;
            }

            backSpaceSpeed = Math.Min(backSpaceSpeed += 0.005f * Main.DeltaTime, 1.0f);
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