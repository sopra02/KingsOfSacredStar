using KingsOfSacredStar.World;
using Microsoft.Xna.Framework.Input;

namespace KingsOfSacredStar.InputWrapper
{
    internal static class CameraWrapper
    {
        private static Camera sCamera;
        private const float CameraSpeed = 5.0f;
        public static void Init(Camera cam)
        {
            sCamera = cam;
        }

        public static void LoadInputManager(InputManager input)
        {
            input.SetBorderAction(InputManager.Border.Top, MoveCameraUp);
            input.SetBorderAction(InputManager.Border.Bottom, MoveCameraDown);
            input.SetBorderAction(InputManager.Border.Left, MoveCameraLeft);
            input.SetBorderAction(InputManager.Border.Right, MoveCameraRight);
            input.AddHoldKeyboard(Keys.W, MoveCameraUp);
            input.AddHoldKeyboard(Keys.S, MoveCameraDown);
            input.AddHoldKeyboard(Keys.A, MoveCameraLeft);
            input.AddHoldKeyboard(Keys.D, MoveCameraRight);
            input.AddHoldKeyboard(Keys.Space, MoveCameraZoomIn);
            input.AddHoldKeyboard(Keys.LeftControl, MoveCameraZoomOut);

            input.AddOnKeyboard(Keys.PageDown, MoveCameraZoomIn);
            input.AddOnKeyboard(Keys.PageUp, MoveCameraZoomOut);
        }
        // to fix


        private static void MoveCameraUp()
        {
            sCamera.MoveCameraDown(-CameraSpeed);
        }

        private static void MoveCameraDown()
        {
            sCamera.MoveCameraDown(CameraSpeed);
        }

        private static void MoveCameraLeft()
        {
            sCamera.MoveCameraRight(-CameraSpeed);
        }

        private static void MoveCameraRight()
        {
            sCamera.MoveCameraRight(CameraSpeed);
        }

        private static void MoveCameraZoomIn()
        {
            sCamera.MoveCameraZoomIn(CameraSpeed);
        }

        private static void MoveCameraZoomOut()
        {
            sCamera.MoveCameraZoomIn(-CameraSpeed);
        }
    }
}
