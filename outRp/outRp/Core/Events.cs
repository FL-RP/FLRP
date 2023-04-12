using System;
using AltV.Net.Data;

namespace outRp.Core
{
    public class Events
    {
        public static Position GetTruePosition(Position pos, Rotation rot, float ownX, float ownY)
        {
            //it will be exported
            Position positions;

            //taking radian info
            // 180 Degre COD
            float turninDegreeOfVehicle = rot.Yaw;
            float turningRadianOfVehicle = DegreeToRadian(turninDegreeOfVehicle);
            //float turningRadianOfVehicle = rot.Roll;

            //float turningRadianOfVehicle = rot.Yaw;

            //current 
            float xCurrentPos = pos.X;
            float yCurrentPos = pos.Y;

            //conversion process
            float xPos = ownX * MathF.Cos(turningRadianOfVehicle) - ownY * MathF.Sin(turningRadianOfVehicle);
            float yPos = ownX * MathF.Sin(turningRadianOfVehicle) + ownY * MathF.Cos(turningRadianOfVehicle);

            //applying in game positions
            xPos += xCurrentPos;
            yPos += yCurrentPos;

            positions = new Position(xPos, yPos, pos.Z);

            return positions;
        }

        public static float DegreeToRadian(float degree)
        {
            return (MathF.PI / 360) * degree;
        }


    }
}
