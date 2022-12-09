using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Pladi.Utilities.Enums;

namespace Pladi.Core.Input
{
    public class InputManager
    {
        private KeyboardState currentKeyboardState;
        private KeyboardState previousKeyboardState;
        private MouseState currentMouseState;
        private MouseState previousMouseState;

        // ...

        public InputManager()
        {
            currentKeyboardState = Keyboard.GetState();
            previousKeyboardState = currentKeyboardState;
            currentMouseState = Mouse.GetState();
            previousMouseState = currentMouseState;
        }

        // ...

        public void Update(GameTime gameTime)
        {
            previousKeyboardState = currentKeyboardState;
            currentKeyboardState = Keyboard.GetState();
            previousMouseState = currentMouseState;
            currentMouseState = Mouse.GetState();
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
    }
}