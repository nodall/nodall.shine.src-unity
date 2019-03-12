using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Nex.Math
{
    public class MathTools
    {
        static public NVector2D PerspectiveCorrection(NQuad2D quad, NVector2D position)
        {
            NVector2D vA1 = quad[1] - quad[2];
            NVector2D vA2 = quad[3] - quad[2]; 
            NVector2D vE = quad[0] - quad[1] + quad[2] - quad[3];

            double a, b, c, d, e, f, g, h, den;

            //if ((0.0 == vE.X) && (0.0 == vE.Y))
            //{
            //    a = quad[1].X - quad[0].X;
            //    d = quad[1].Y - quad[0].Y;

            //    b = quad[0].X - quad[1].X;
            //    e = quad[0].Y - quad[1].Y;

            //    c = quad[0].X;
            //    f = quad[0].Y;

            //    g = 0.0;
            //    h = 0.0;
            //}
            //else
            {
                den = ((vA1.X * vA2.Y) - (vA2.X * vA1.Y));

                if (0.0 == den)
                {
                    den = 0.000001;
                }

                g = ((vE.X * vA2.Y) - (vA2.X * vE.Y)) / den;
                h = ((vA1.X * vE.Y) - (vE.X * vA1.Y)) / den;

                a = (quad[1].X - quad[0].X) + (g * quad[1].X);
                d = (quad[1].Y - quad[0].Y) + (g * quad[1].Y);

                b = (quad[3].X - quad[0].X) + (h * quad[3].X);
                e = (quad[3].Y - quad[0].Y) + (h * quad[3].Y);

                c = quad[0].X;
                f = quad[0].Y;
            }

            den = ((position.X * g) + (position.Y * h) + 1.0);

            if (0.0 == den)
            {
                den = 0.00001;
            }

            double x = ((position.X * a) + (position.Y * b) + c) / den;
            double y = ((position.X * d) + (position.Y * e) + f) / den;

            return new NVector2D(x, y);
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
        static public bool IsInside(NVector2D value, NRectangle2D rectangle)
        {
            return IsInside(value.X, rectangle.Left, rectangle.Right) &&
                IsInside(value.Y, rectangle.Bottom , rectangle.Top);
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
