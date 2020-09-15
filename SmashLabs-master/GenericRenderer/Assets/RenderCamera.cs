using GenericRenderer.Game;
using OpenTK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GenericRenderer.Assets
{
    public enum ProjectionMode
    {
        Orthographic,
        Perspective
    }
    public class RenderCamera
    {
        public Vector3 CameraPosition { get; set; } = new Vector3(0,0,20);
        public Quaternion CameraRotation { get; set; } = Quaternion.Identity;
        public float FOV { get; set; } = 40;
        public ProjectionMode Mode { get; private set; } = ProjectionMode.Orthographic;
        public Matrix4 CameraTranslationMatrix => Matrix4.CreateTranslation(-CameraPosition);
        public Matrix4 CameraRotationMatrix => Matrix4.CreateFromQuaternion(CameraRotation);
        public Matrix4 CameraViewMatrix => CameraRotationMatrix * CameraTranslationMatrix;
        public Matrix4 CameraProjectionMatrix
        {
            get
            {
                switch (Mode)
                {
                    case ProjectionMode.Perspective: return Matrix4.CreatePerspectiveFieldOfView(MathHelper.DegreesToRadians(FOV),Window.MainWindow.ScreenAspect,1,1000);
                    case ProjectionMode.Orthographic: return Matrix4.CreateOrthographic(FOV * Window.MainWindow.ScreenAspect,FOV,1,1000);
                }

                return Matrix4.Identity;
            }
        }
        public Matrix4 CameraViewThrusum => CameraViewMatrix * CameraProjectionMatrix; //This goes into the shader
    }
}
