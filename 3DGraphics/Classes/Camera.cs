using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace _3DGraphics.Classes
{
    internal class Camera
    {
        public double angleX = 0; 

        public readonly double angle90 = Math.PI/180*80;

        public BaseGraphisStructs.CoordinateVector Eye = new(0, 0, 1);
        public BaseGraphisStructs.CoordinateVector Up = new(0, 1, 0);
        public BaseGraphisStructs.CoordinateVector Target = new(0, 0, 0);
        public float Aaspect;
        public float Fov = 70.0f * ((float)Math.PI / 180.0f);
        public float ZFar = 10f;
        public float ZNear = 0.1f;

        public Camera(int width, int height)
        {
            Aaspect = width / height;
        }

        public void RotateAroundX(double angle)
        {
            var saveAngleX = angleX;
            angleX += angle;

            if (angleX > angle90 || angleX < -angle90)
            {
                angleX = saveAngleX;
                return;
            }

            var cos = (float)Math.Cos(angle);
            var sin = (float)Math.Sin(angle);

            var matrixVectY = new Vector3(0, cos, sin);
            var matrixVectZ = new Vector3(0, -sin, cos);

            var tmpVectY = Eye.Coordinates * matrixVectY;
            var tmpVectZ = Eye.Coordinates * matrixVectZ;

            Eye.Y = tmpVectY.Y + tmpVectY.Z;
            Eye.Z = tmpVectZ.Y + tmpVectZ.Z;
        }

        public void RotateAroundY(double angle)
        {
            var cos = (float)Math.Cos(angle);
            var sin = (float)Math.Sin(angle);

            var matrixVectX = new Vector3(cos, 0, -sin);
            var matrixVectZ = new Vector3(sin, 0, cos);

            var tmpVectX = Eye.Coordinates * matrixVectX;
            var tmpVectZ = Eye.Coordinates * matrixVectZ;

            Eye.X = tmpVectX.X + tmpVectX.Z;
            Eye.Z = tmpVectZ.X + tmpVectZ.Z;
        }

        public void RotateAroundZ(double angle)
        {
            var cos = (float)Math.Cos(angle);
            var sin = (float)Math.Sin(angle);

            var matrixVectX = new Vector3(cos, sin, 0);
            var matrixVectY = new Vector3(-sin, cos, 0);

            var tmpVectX = Eye.Coordinates * matrixVectX;
            var tmpVectY = Eye.Coordinates * matrixVectY;

            Eye.X = tmpVectX.X + tmpVectX.Y;
            Eye.Y = tmpVectY.X + tmpVectY.Y;
        }
    }
}
