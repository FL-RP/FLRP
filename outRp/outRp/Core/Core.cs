using AltV.Net.Elements.Entities;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Numerics;
using System.Security.Cryptography;
using System.Text;
using CodeKicker.BBCode.Core;
using System.Text.RegularExpressions;
using AltV.Net.Data;

namespace outRp.Core
{
    public static class Core
    {
        public static T lscGetdata<T>(this IBaseObject element, string key)
        {
            try
            {
                if (element.GetData(key, out T value)) { return value; }
                return default;
            }
            catch { return default; }
        }

        public static void lscSetData(this IBaseObject element, string key, object value)
        {
            try { element.SetData(key, value); return; }
            catch (Exception ex) { Debug.CatchExceptions("vnxSetElementData", ex); }
        }

        public static string GetHexColorcode(int r, int g, int b)
        {
            try
            {
                Color myColor = Color.FromArgb(r, g, b);
                return "{" + myColor.R.ToString("X2") + myColor.G.ToString("X2") + myColor.B.ToString("X2") + "}";
            }
            catch { return ""; }
        }

        public static float ToRadians(float val)
        {
            try
            {
                return (float)(System.Math.PI / 180) * val;
            }
            catch { return 0; }
        }
        public static float ToDegrees(float val)
        {
            try
            {
                return (float)(val * (180 / System.Math.PI));
            }
            catch { return 0; }
        }
        
        public static List<Vector3> SetRandomPositionsWithRadius(Vector3 _position, int _numberOfPositions, float _radius, float _distanceBetween)
        {
            List<Vector3> positions = new List<Vector3>();
                        
            for (int i = 0; positions.Count < _numberOfPositions; i++)
            {
                bool isAdded = true;
                while (isAdded)
                {
                    Vector3 pos = new Vector3();
                    Random rnd = new Random();

                    pos.X = rnd.Next(0, 2) == 0 ? DoubleToFloat(rnd.NextDouble()) * _radius : DoubleToFloat(rnd.NextDouble()) * -_radius;
                    pos.Y = rnd.Next(0, 2) == 0 ? DoubleToFloat(rnd.NextDouble()) * _radius : DoubleToFloat(rnd.NextDouble()) * -_radius;

                    float length = MathF.Sqrt(MathF.Pow(pos.X, 2) + MathF.Pow(pos.Y, 2));

                    if (length < _radius)
                    {
                        bool canAdded = true;
                        foreach (Vector3 posItem in positions)
                        {
                            if (Vector3.Distance(posItem, pos) <= _distanceBetween)
                            {
                                canAdded = false;
                            }

                        }
                        if (canAdded)
                        {
                            positions.Add(pos + _position);
                        }
                        isAdded = false;
                    }
                }
                if (i >= _numberOfPositions * 10)
                    return null;
            }
            return positions;
        }

        public static float DoubleToFloat(double _value)
        {
            float _float;
            float.TryParse(_value.ToString(), out _float);
            return _float;
        }

        
        public static string idPassConverter(string id, string pass)  // Şifreleme SMF -> TO SERVER
        {
            return Hash(id.ToLower(new CultureInfo("ZH", true)) + pass); // true kısmı false yapılacak. ! Eğer çalışmazsa.
        }

        public static string Hash(string input)
        {
            using (SHA1Managed sha1 = new SHA1Managed())
            {
                var hash = sha1.ComputeHash(Encoding.UTF8.GetBytes(input));
                var sb = new StringBuilder(hash.Length * 2);

                foreach (byte b in hash)
                {
                    // can be "x2" if you want lowercase
                    sb.Append(b.ToString("X2"));
                }
                return sb.ToString();
            }
        }

        public static string BBcodeToHTML(string str)
        {
            var x = new BBCodeParser(new[]
                {
                    new BBTag("b", "<strong>", "</strong>",true, true, 1),
                    new BBTag("divbox", "<div style='${content}'", "</div>",true, true, 2),
                    new BBTag("br", "<br>", "",true, false, 3),
                    new BBTag("table", "<table>", "</table>",true, true, 4),
                    new BBTag("tr", "<tr>", "</tr>",true, true, 5),
                    new BBTag("td", "<td>", "</td>",true, true, 6),
                    new BBTag("img", "<img src=${content} style='width: 20px' />", "</img>",true, false, 7),
                    new BBTag("font", "", "",true, true, 8),
                    new BBTag("size", "<p style='font-size: ${content}>", "</p>",true, true, 9),
                    new BBTag("color", "<p style='color: ${content}>", "</p>",true, true, 10),

                });
            //return CodeKicker.BBCode.Core.BBCode.ToHtml(str);
            return x.ToHtml(str);
        }

        public static void OutputLog(string message, ConsoleColor color)
        {

            var pieces = Regex.Split(message, @"(\[[^\]]*\])");

            for (int i = 0; i < pieces.Length; i++)
            {
                string piece = pieces[i];

                if (piece.StartsWith("[") && piece.EndsWith("]"))
                {
                    Console.ForegroundColor = color;
                    piece = piece.Substring(1, piece.Length - 2);
                }

                Console.Write(piece);
                Console.ResetColor();

            }
            Console.WriteLine();
        }

        public static Position getBackPos(Position pos, Rotation rot, float Distance)
        {
            return new Position(
                pos.X + rot.Pitch * Distance,
                pos.Y + rot.Roll * Distance,
                pos.Z);
        }
    }
}
