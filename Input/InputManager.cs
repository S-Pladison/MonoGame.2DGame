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
            switch (input)
            {
                case MouseInput.LeftButton:
                    return state.LeftButton == ButtonState.Pressed;
                case MouseInput.MiddleButton:
                    return state.MiddleButton == ButtonState.Pressed;
                case MouseInput.RightButton:
                    return state.RightButton == ButtonState.Pressed;
                case MouseInput.Button1:
                    return state.XButton1 == ButtonState.Pressed;
                case MouseInput.Button2:
                    return state.XButton2 == ButtonState.Pressed;
                case MouseInput.None:
                    break;
            }
            return false;
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