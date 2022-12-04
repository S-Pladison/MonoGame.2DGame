using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace Pladi.Input
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
            => currentKeyboardState.IsKeyDown(key);

        public bool IsPressed(MouseInput input)
            => IsPressed(currentMouseState, input);

        private bool IsPressed(MouseState state, MouseInput input)
        {
            return input switch
            {
                MouseInput.LeftButton => state.LeftButton == ButtonState.Pressed,
                MouseInput.MiddleButton => state.MiddleButton == ButtonState.Pressed,
                MouseInput.RightButton => state.RightButton == ButtonState.Pressed,
                MouseInput.Button1 => state.XButton1 == ButtonState.Pressed,
                MouseInput.Button2 => state.XButton2 == ButtonState.Pressed,
                _ => false,
            };
        }

        public bool IsHeld(Keys key)
            => currentKeyboardState.IsKeyDown(key) && previousKeyboardState.IsKeyDown(key);

        public bool IsHeld(MouseInput input)
            => IsPressed(currentMouseState, input) && IsPressed(previousMouseState, input);

        public bool JustPressed(Keys key)
            => currentKeyboardState.IsKeyDown(key) && !previousKeyboardState.IsKeyDown(key);

        public bool JustPressed(MouseInput input)
            => IsPressed(currentMouseState, input) && !IsPressed(previousMouseState, input);

        public bool JustReleased(Keys key)
            => !currentKeyboardState.IsKeyDown(key) && previousKeyboardState.IsKeyDown(key);
        public bool JustReleased(MouseInput input)
            => !IsPressed(currentMouseState, input) && IsPressed(previousMouseState, input);

        public Vector2 GetMousePosition()
            => currentMouseState.Position.ToVector2();

        public bool IsMouseMoved()
            => currentMouseState.X != previousMouseState.X || currentMouseState.Y != previousMouseState.Y;

        public int GetMouseScroll()
            => currentMouseState.ScrollWheelValue - previousMouseState.ScrollWheelValue;
    }
}