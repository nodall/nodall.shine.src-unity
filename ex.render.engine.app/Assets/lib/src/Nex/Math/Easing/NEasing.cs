using System;

namespace Nex.Math.Easing
{
    public static class NEasing
    {
        public static float Ease(double linearStep, float acceleration, NEasingType type)
        {
            float easedStep = acceleration > 0 ? EaseIn(linearStep, type) :
                              acceleration < 0 ? EaseOut(linearStep, type) :
                              (float)linearStep;

            return MathHelper.Lerp(linearStep, easedStep, System.Math.Abs(acceleration));
        }
        public static float EaseIn(double linearStep, NEasingType type)
        {
            switch (type)
            {
                case NEasingType.Step: return linearStep < 0.5 ? 0 : 1;
                case NEasingType.Linear: return (float)linearStep;
                case NEasingType.Sine: return Sine.EaseIn(linearStep);
                case NEasingType.Quadratic: return Power.EaseIn(linearStep, 2);
                case NEasingType.Cubic: return Power.EaseIn(linearStep, 3);
                case NEasingType.Quartic: return Power.EaseIn(linearStep, 4);
                case NEasingType.Quintic: return Power.EaseIn(linearStep, 5);
            }
            throw new NotImplementedException();
        }
        public static float EaseOut(double linearStep, NEasingType type)
        {
            switch (type)
            {
                case NEasingType.Step: return linearStep < 0.5 ? 0 : 1;
                case NEasingType.Linear: return (float)linearStep;
                case NEasingType.Sine: return Sine.EaseOut(linearStep);
                case NEasingType.Quadratic: return Power.EaseOut(linearStep, 2);
                case NEasingType.Cubic: return Power.EaseOut(linearStep, 3);
                case NEasingType.Quartic: return Power.EaseOut(linearStep, 4);
                case NEasingType.Quintic: return Power.EaseOut(linearStep, 5);
            }
            throw new NotImplementedException();
        }

        public static float EaseInOut(double linearStep, NEasingType easeInType, NEasingType easeOutType)
        {
            return linearStep < 0.5 ? EaseInOut(linearStep, easeInType) : EaseInOut(linearStep, easeOutType);
        }
        public static float EaseInOut(double linearStep, NEasingType type)
        {
            switch (type)
            {
                case NEasingType.Step: return linearStep < 0.5 ? 0 : 1;
                case NEasingType.Linear: return (float)linearStep;
                case NEasingType.Sine: return Sine.EaseInOut(linearStep);
                case NEasingType.Quadratic: return Power.EaseInOut(linearStep, 2);
                case NEasingType.Cubic: return Power.EaseInOut(linearStep, 3);
                case NEasingType.Quartic: return Power.EaseInOut(linearStep, 4);
                case NEasingType.Quintic: return Power.EaseInOut(linearStep, 5);
            }
            throw new NotImplementedException();
        }

        static class Sine
        {
            public static float EaseIn(double s)
            {
                return (float)System.Math.Sin(s * MathHelper.HalfPi - MathHelper.HalfPi) + 1;
            }
            public static float EaseOut(double s)
            {
                return (float)System.Math.Sin(s * MathHelper.HalfPi);
            }
            public static float EaseInOut(double s)
            {
                return (float)(System.Math.Sin(s * MathHelper.Pi - MathHelper.HalfPi) + 1) / 2;
            }
        }
        static class Power
        {
            public static float EaseIn(double s, int power)
            {
                return (float)System.Math.Pow(s, power);
            }
            public static float EaseOut(double s, int power)
            {
                var sign = power % 2 == 0 ? -1 : 1;
                return (float)(sign * (System.Math.Pow(s - 1, power) + sign));
            }
            public static float EaseInOut(double s, int power)
            {
                s *= 2;
                if (s < 1) return EaseIn(s, power) / 2;
                var sign = power % 2 == 0 ? -1 : 1;
                return (float)(sign / 2.0 * (System.Math.Pow(s - 2, power) + sign * 2));
            }
        }
        static class MathHelper
        {
            public const float Pi = (float)System.Math.PI;
            public const float HalfPi = (float)(System.Math.PI / 2);

            public static float Lerp(double from, double to, double step)
            {
                return (float)((to - from) * step + from);
            }
        }
    }
}