using System;
using System.Collections.Generic;
using KingsOfSacredStar.Sound;
using KingsOfSacredStar.World;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Input;

namespace KingsOfSacredStar.InputWrapper
{
    internal sealed class InputManager
    {
        internal enum Border
        {
            Top,
            Right,
            Bottom,
            Left
        }


        private bool mEnableClipping;
        private const float MinRtSize = 10.0f;

        private readonly Action[] mNearScreenBorder = new Action[Enum.GetValues(typeof(Border)).Length];
        private readonly Dictionary<Keys, Action> mOnKeyboardInputFunctions = new Dictionary<Keys, Action>();
        private readonly Dictionary<Keys, Action> mHoldKeyboardInputFunctions = new Dictionary<Keys, Action>();
        private readonly SoundEffectManager mSoundEffectManager;
        private Action<Rectangle> mOnRectangle;
        private Action<Point> mOnLeftClick;
        private Action<Point> mOnRightClick;
        private readonly Action mMWheelPlus;
        private readonly Action mMWheelMinus;
        private MouseState mLastMouseState;
        private KeyboardState mLastKeyboardState;
        private Point mRtStartPoint;
        private Point mRtEndPoint;
        private Camera mCamera;
        private const float WheelSpeed = 0.3f;

        private readonly int mMouseScreenBorderDz;

        public InputManager(ContentManager content)
        {
            Clear();
            mMouseScreenBorderDz = 10;
            mEnableClipping = false;
            mSoundEffectManager = new SoundEffectManager(content, "sounds/click");
            mMWheelPlus = MoveCameraZoomIn;
            mMWheelMinus = MoveCameraZoomOut;

        }

        private void ClearMouse()
        {
            mOnRectangle = _ => {};
            mOnLeftClick = mOnRightClick = _ => {};
            mLastMouseState = Mouse.GetState();
        }

        private void ClearBorder()
        {
            for (var i = 0; i < mNearScreenBorder.Length; i++)
            {
                mNearScreenBorder[i] = () => {};
            }
        }

        public void ClipMouse(bool setVal)
        {
            mEnableClipping = setVal;
        }


        private void Clear()
        {
            ClearMouse();
            ClearBorder();
            ClearKeyboard();
        }

        private void ClearKeyboard()
        {
            mHoldKeyboardInputFunctions.Clear();
            mOnKeyboardInputFunctions.Clear();
        }

        private void CheckMWheel(MouseState mouseState) {
            var currentMWheel = mouseState.ScrollWheelValue;
            var oldMWheel = mLastMouseState.ScrollWheelValue;
            for (var x = 0; x < Math.Abs(currentMWheel - oldMWheel); x++) {
                if (currentMWheel - oldMWheel > 0) {
                    mMWheelPlus();
                }
                else {
                    mMWheelMinus();
                }
            }
        }

        public void GetCam(Camera cam)
        {
            mCamera = cam;
        }

        private void MoveCameraZoomIn()
        {
            mCamera.MoveCameraZoomIn(WheelSpeed);
        }

        private void MoveCameraZoomOut()
        {
            mCamera.MoveCameraZoomIn(-WheelSpeed);
        }

        public void AddOnKeyboard(Keys key, Action action)
        {
            mOnKeyboardInputFunctions[key] = action;
        }

        public void AddHoldKeyboard(Keys key, Action action)
        {
            mHoldKeyboardInputFunctions[key] = action;
        }


        public void SetMouseEvents(Action<Point> leftClick, Action<Point> rightClick)  // for setting events
        {
            mOnLeftClick = leftClick;
            mOnRightClick = rightClick;
        }


        public void SetSelectAction(Action<Rectangle> select)
        {
            mOnRectangle = select;
        }

        private static int Normalize(int a, int b)
        {
            return b < 0 ? a + b : a;
        }
        private static Rectangle NormalizeRectangle(Rectangle source)
        {
            return new Rectangle(
                Normalize(source.X, source.Width),
                Normalize(source.Y, source.Height),
                Math.Abs(source.Width),
                Math.Abs(source.Height));
        }

        private static int Clamp(int x, int min, int max)
        {
            return Math.Max(Math.Min(x, max), min);
        }

        private static void ClipMouse(MouseState mouseState)
        {
            var x = mouseState.X;
            var y = mouseState.Y;
            var clampedX = Clamp(x, 0, Globals.Resolution.X);
            var clampedY = Clamp(y, 0, Globals.Resolution.Y);
            if (x != clampedX || y != clampedY)
            {
                Mouse.SetPosition(clampedX, clampedY);
            }
        }


        private void CheckMouse()
        {
            var currentMouseState = Mouse.GetState();
            if (mEnableClipping)
                ClipMouse(currentMouseState);
            if (mLastMouseState.LeftButton == ButtonState.Pressed)
            {
                if (currentMouseState.LeftButton == ButtonState.Pressed)
                {
                    mRtEndPoint = currentMouseState.Position;
                }
                else
                {
                    mSoundEffectManager.Play(0);
                    if ((mRtStartPoint - mRtEndPoint).ToVector2().Length() > MinRtSize)
                        mOnRectangle(NormalizeRectangle(new Rectangle(mRtStartPoint, mRtEndPoint-mRtStartPoint)));
                    else
                        mOnLeftClick(currentMouseState.Position);
                    mRtStartPoint = mRtEndPoint = Point.Zero;
                }
            }
            else
            {
                if (currentMouseState.LeftButton == ButtonState.Pressed)
                {
                    mRtEndPoint = currentMouseState.Position;
                    mRtStartPoint = currentMouseState.Position;
                }
            }

            if (mLastMouseState.RightButton == ButtonState.Released &&
                currentMouseState.RightButton == ButtonState.Pressed)
            {
                mOnRightClick(currentMouseState.Position);
            }
            CheckMWheel(currentMouseState);
            mLastMouseState = currentMouseState;
        }

        private void CheckKeyboard()
        {
            var currentKeyboardState = Keyboard.GetState();
            foreach (var kif in mOnKeyboardInputFunctions)
            {
                if (currentKeyboardState.IsKeyDown(kif.Key) && mLastKeyboardState.IsKeyUp(kif.Key))
                {
                    kif.Value();
                }
            }
            foreach (var kif in mHoldKeyboardInputFunctions)
            {
                if (currentKeyboardState.IsKeyDown(kif.Key))
                {
                    kif.Value();

                }
            }

            mLastKeyboardState = currentKeyboardState;
        }


        public Rectangle GetMouseRectangle()
        {
            var size = mRtEndPoint - mRtStartPoint;
            return size.ToVector2().Length() > MinRtSize
                ? NormalizeRectangle(new Rectangle(mRtStartPoint, size))
                : Rectangle.Empty;
        }

        public Point GetMousePosition()
        {
            return mLastMouseState.Position;
        }

        // to fix
        public void Update()
        {
            CheckMouse();
            CheckMouseBorder();
            CheckKeyboard();
        }

        public void SetBorderAction(Border border, Action Event)
        {
            mNearScreenBorder[(int) border] = Event;
        }
        private bool MouseNearScreenBorderLeft()
        {
            return MouseInRectangle(new Rectangle(-mMouseScreenBorderDz, -mMouseScreenBorderDz, 2 * mMouseScreenBorderDz, Globals.Resolution.Y + 2 * mMouseScreenBorderDz));
        }

        private bool MouseNearScreenBorderRight()
        {
            return MouseInRectangle(new Rectangle(Globals.Resolution.X - mMouseScreenBorderDz, -mMouseScreenBorderDz, 2 * mMouseScreenBorderDz, Globals.Resolution.Y + 2 * mMouseScreenBorderDz));
        }

        private bool MouseNearScreenBorderTop()
        {
            return MouseInRectangle(new Rectangle(-mMouseScreenBorderDz, -mMouseScreenBorderDz, Globals.Resolution.X + 2 * mMouseScreenBorderDz, 2 * mMouseScreenBorderDz));
        }

        private bool MouseNearScreenBorderBottom()
        {
            return MouseInRectangle(new Rectangle(-mMouseScreenBorderDz, Globals.Resolution.Y - mMouseScreenBorderDz, Globals.Resolution.X + 2 * mMouseScreenBorderDz, 2 * mMouseScreenBorderDz));
        }

        private void CheckMouseBorder()
        {
            if (!GetMouseRectangle().Equals(Rectangle.Empty))
                return;
            if (MouseNearScreenBorderTop())
            {
                mNearScreenBorder[(int) Border.Top]();
            }
            else if (MouseNearScreenBorderBottom())
            {
                mNearScreenBorder[(int) Border.Bottom]();
            }
            if (MouseNearScreenBorderLeft())
            {
                mNearScreenBorder[(int) Border.Left]();
            }
            else if (MouseNearScreenBorderRight())
            {
                mNearScreenBorder[(int) Border.Right]();
            }
        }

        private static bool MouseInRectangle(Rectangle rectangle)
        {
            return rectangle.Contains(Mouse.GetState().Position);
        }
    }
}
