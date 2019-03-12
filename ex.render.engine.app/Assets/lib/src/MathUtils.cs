using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;

namespace nexcode.nwcore
{
    public static class Vector2Utils
    {
        static public Vector2 Parse(string value)
        {
            string[] strList = value.Split(';');
            if (strList.Length != 2)
                throw new FormatException("Format must be x ; y in value=" + value);
            float x, y;
            if (float.TryParse(strList[0], NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out x) &&
                float.TryParse(strList[1], NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out y))
                return new Vector2(x, y);
            else
                throw new FormatException("Error parsing double in value=" + value);
        }
    }


    public class MathUtils
    {
        static public Vector2 PerspectiveCorrection(Quad2D quad, Vector2 position)
        {
            Vector2 vA1 = quad[1] - quad[2];
            Vector2 vA2 = quad[3] - quad[2];
            Vector2 vE = quad[0] - quad[1] + quad[2] - quad[3];

            double a, b, c, d, e, f, g, h, den;

            //if ((0.0 == vE.x) && (0.0 == vE.y))
            //{
            //    a = quad[1].x - quad[0].x;
            //    d = quad[1].y - quad[0].y;

            //    b = quad[0].x - quad[1].x;
            //    e = quad[0].y - quad[1].y;

            //    c = quad[0].x;
            //    f = quad[0].y;

            //    g = 0.0;
            //    h = 0.0;
            //}
            //else
            {
                den = ((vA1.x * vA2.y) - (vA2.x * vA1.y));

                if (0.0 == den)
                {
                    den = 0.000001;
                }

                g = ((vE.x * vA2.y) - (vA2.x * vE.y)) / den;
                h = ((vA1.x * vE.y) - (vE.x * vA1.y)) / den;

                a = (quad[1].x - quad[0].x) + (g * quad[1].x);
                d = (quad[1].y - quad[0].y) + (g * quad[1].y);

                b = (quad[3].x - quad[0].x) + (h * quad[3].x);
                e = (quad[3].y - quad[0].y) + (h * quad[3].y);

                c = quad[0].x;
                f = quad[0].y;
            }

            den = ((position.x * g) + (position.y * h) + 1.0);

            if (0.0 == den)
            {
                den = 0.00001;
            }

            double x = ((position.x * a) + (position.y * b) + c) / den;
            double y = ((position.x * d) + (position.y * e) + f) / den;

            return new Vector2((float)x, (float)y);
        }
        static public bool IsMormalized(double value)
        {
            if (0 <= value && value <= 1)
                return true;
            else return false;
        }
        static public bool IsInside(double value, double min, double max)
        {
            if (max < min)
                return false;
            if (min <= value && value <= max)
                return true;
            else
                return false;
        }
        static public bool IsInside(Vector2 value, Rectangle2D rectangle)
        {
            return IsInside(value.x, rectangle.left, rectangle.right) &&
                IsInside(value.y, rectangle.bottom, rectangle.top);
        }
        static public float RescaleValue(float inValue, float inMin, float inMax, float outMin, float outMax)
        {
            float a = (outMax - outMin) / (inMax - inMin);
            float b = outMin - (a * inMin);
            return a * inValue + b;
        }
        static public double RescaleValue(double inValue, double inMin, double inMax, double outMin, double outMax)
        {
            double a = (outMax - outMin) / (inMax - inMin);
            double b = outMin - (a * inMin);
            return a * inValue + b;
        }
        static public float RescaleValueInRange(float inValue, float inMin, float inMax, float outMin, float outMax)
        {
            if (inMax == inMin)
            {
                return outMax;
            }
            // Input check
            if (inValue < inMin)
            {
                inValue = inMin;
            }
            if (inValue > inMax)
            {
                inValue = inMax;
            }
            float result = RescaleValue(inValue, inMin, inMax, outMin, outMax);

            return result;
        }
        static public double RescaleValueInRange(double inValue, double inMin, double inMax, double outMin, double outMax)
        {
            if (inMax == inMin)
            {
                return outMax;
            }
            // Input check
            if (inValue < inMin)
            {
                inValue = inMin;
            }
            if (inValue > inMax)
            {
                inValue = inMax;
            }
            double result = RescaleValue(inValue, inMin, inMax, outMin, outMax);

            return result;
        }
        static public double RadianToDegree(double value)
        {
            return value * 180.0 / System.Math.PI;
        }
        static public double DegreeToRadian(double value)
        {
            return value * System.Math.PI / 180.0;
        }
    }

}