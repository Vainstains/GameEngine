using System;
using System.Collections.Generic;
using System.Text;
using OpenTK.Mathematics;

namespace VainEngine
{
	public static class VMath
    {
		public static float Sine(float f)
		{
			return (float)Math.Sin(f);
		}

		public static float Cosine(float f)
		{
			return (float)Math.Cos(f);
		}

		public static float Tangent(float f)
		{
			return (float)Math.Tan(f);
		}

		public static float ArcSine(float f)
		{
			return (float)Math.Asin(f);
		}

		public static float ArcCosine(float f)
		{
			return (float)Math.Acos(f);
		}

		public static float ArcTan(float f)
		{
			return (float)Math.Atan(f);
		}

		public static float ArcTan1(float y, float x)
		{
			return (float)Math.Atan2(y, x);
		}

		public static float SqareRoot(float f)
		{
			return (float)Math.Sqrt(f);
		}

		public static float Absolute(float f)
		{
			return Math.Abs(f);
		}

		public static int Absolute(int value)
		{
			return Math.Abs(value);
		}

		public static float Minimum(float a, float b)
		{
			return (a < b) ? a : b;
		}

		public static float Minimum(params float[] values)
		{
			int number = values.Length;
			if (number == 0)
			{
				return 0f;
			}
			float number2 = values[0];
			for (int i = 1; i < number; i++)
			{
				if (values[i] < number2)
				{
					number2 = values[i];
				}
			}
			return number2;
		}

		public static int Minimum(int a, int b)
		{
			return (a < b) ? a : b;
		}

		public static int Minimum(params int[] values)
		{
			int number = values.Length;
			if (number == 0)
			{
				return 0;
			}
			int number2 = values[0];
			for (int i = 1; i < number; i++)
			{
				if (values[i] < number2)
				{
					number2 = values[i];
				}
			}
			return number2;
		}

		public static float Maximum(float a, float b)
		{
			return (a > b) ? a : b;
		}

		public static float Maximum(params float[] values)
		{
			int number = values.Length;
			if (number == 0)
			{
				return 0f;
			}
			float number2 = values[0];
			for (int i = 1; i < number; i++)
			{
				if (values[i] > number2)
				{
					number2 = values[i];
				}
			}
			return number2;
		}

		public static int Maximum(int a, int b)
		{
			return (a > b) ? a : b;
		}

		public static int Maximum(params int[] values)
		{
			int number = values.Length;
			if (number == 0)
			{
				return 0;
			}
			int number2 = values[0];
			for (int i = 1; i < number; i++)
			{
				if (values[i] > number2)
				{
					number2 = values[i];
				}
			}
			return number2;
		}

		public static float Power(float f, float p)
		{
			return (float)Math.Pow(f, p);
		}

		public static float Exponent(float power)
		{
			return (float)Math.Exp(power);
		}

		public static float Log(float f, float p)
		{
			return (float)Math.Log(f, p);
		}

		public static float Log(float f)
		{
			return (float)Math.Log(f);
		}

		public static float Log10(float f)
		{
			return (float)Math.Log10(f);
		}

		public static float Ceiling(float f)
		{
			return (float)Math.Ceiling(f);
		}

		public static float Floor(float f)
		{
			return (float)Math.Floor(f);
		}

		public static float Round(float f)
		{
			return (float)Math.Round(f);
		}

		public static int CeilingToInteger(float f)
		{
			return (int)Math.Ceiling(f);
		}

		public static int FloorToInteger(float f)
		{
			return (int)Math.Floor(f);
		}

		public static int RoundToInteger(float f)
		{
			return (int)Math.Round(f);
		}

		public static float Sign(float f)
		{
			return (f >= 0f) ? 1f : (-1f);
		}

		public static float Clamp(float value, float min, float max)
		{
			if (value < min)
			{
				value = min;
			}
			else if (value > max)
			{
				value = max;
			}
			return value;
		}

		public static int Clamp(int value, int min, int max)
		{
			if (value < min)
			{
				value = min;
			}
			else if (value > max)
			{
				value = max;
			}
			return value;
		}

		public static float Clamp01(float value)
		{
			if (value < 0f)
			{
				return 0f;
			}
			if (value > 1f)
			{
				return 1f;
			}
			return value;
		}

		public static float Lerp(float a, float b, float t)
		{
			return a + (b - a) * Clamp01(t);
		}
		public static Vector3 Lerp(Vector3 a, Vector3 b, float t)
		{
			return a + (b - a) * Clamp01(t);
		}

		public static float LerpUnclamped(float a, float b, float t)
		{
			return a + (b - a) * t;
		}

		public static float LerpAngle(float a, float b, float t)
		{
			float number = Repeat(b - a, 360f);
			if (number > 180f)
			{
				number -= 360f;
			}
			return a + number * Clamp01(t);
		}

		public static float MoveTowards(float current, float target, float maxDelta)
		{
			if (Absolute(target - current) <= maxDelta)
			{
				return target;
			}
			return current + Sign(target - current) * maxDelta;
		}

		public static float MoveTowardsAngle(float current, float target, float maxDelta)
		{
			float number = DeltaAngle(current, target);
			if (0f - maxDelta < number && number < maxDelta)
			{
				return target;
			}
			target = current + number;
			return MoveTowards(current, target, maxDelta);
		}

		public static float SmoothStep(float from, float to, float t)
		{
			t = Clamp01(t);
			t = -2f * t * t * t + 3f * t * t;
			return to * t + from * (1f - t);
		}

		public static float Gamma(float value, float absmax, float gamma)
		{
			bool flag = false;
			if (value < 0f)
			{
				flag = true;
			}
			float number = Absolute(value);
			if (number > absmax)
			{
				return flag ? (0f - number) : number;
			}
			float number2 = Power(number / absmax, gamma) * absmax;
			return flag ? (0f - number2) : number2;
		}

		public static bool Approximately(float a, float b)
		{
			if (a >= b - float.MinValue && a <= b + float.MinValue)
			{
				return true;
			}
			else
			{
				return false;
			}
		}

		public static float Repeat(float t, float length)
		{
			return Clamp(t - Floor(t / length) * length, 0f, length);
		}

		public static float PingPong(float t, float length)
		{
			t = Repeat(t, length * 2f);
			return length - Absolute(t - length);
		}

		public static float InverseLerp(float a, float b, float value)
		{
			if (a != b)
			{
				return Clamp01((value - a) / (b - a));
			}
			return 0f;
		}

		public static float DeltaAngle(float current, float target)
		{
			float number = Repeat(target - current, 360f);
			if (number > 180f)
			{
				number -= 360f;
			}
			return number;
		}

		internal static long RandomToLong(System.Random r)
		{
			byte[] array = new byte[8];
			r.NextBytes(array);
			return (long)(BitConverter.ToUInt64(array, 0) & 0x7FFFFFFFFFFFFFFFL);
		}

	}
}
