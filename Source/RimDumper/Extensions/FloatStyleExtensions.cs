using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using Verse;

namespace RimDumper.Extensions
{
    // TODO: refactor
    public static class FloatStyleExtensions
    {
        private static float ToDecimalIfSmall(this float f)
        {
            if (Mathf.Abs(f) < 1f)
            {
                return (float)Math.Round(f, 2);
            }
            if (Mathf.Abs(f) < 10f)
            {
                return (float)Math.Round(f, 1);
            }
            return (float)Mathf.RoundToInt(f);
        }

        private static float ToPercentNew(this float f)
        {
            return (f * 100f).ToDecimalIfSmall();
        }

        private static float ToPercentNew(this float f, int digits)
        {
            return (float)Math.Round((f + 1E-05f) * 100f, digits);
        }

        private static float ToTemperature(this float celsiusTemp, int digits = 1)
        {
            celsiusTemp = GenTemperature.CelsiusTo(celsiusTemp, Prefs.TemperatureMode);
            return (float)Math.Round(celsiusTemp, digits);
        }

        private static float ToTemperatureOffset(this float celsiusTemp, int digits = 1)
        {
            celsiusTemp = GenTemperature.CelsiusToOffset(celsiusTemp, Prefs.TemperatureMode);
            return (float)Math.Round(celsiusTemp, digits);
        }

        private static float ToWorkAmount(this float workAmount)
        {
            return Mathf.CeilToInt(workAmount / 60f);
        }

        private static float ToMoney(this float f, int? digits = null)
        {
            if (digits == null)
            {
                if (f >= 10f || f == 0f)
                {
                    digits = 0;
                }
                else
                {
                    digits = 2;
                }
            }
            return (float)Math.Round(f, (int)digits);
        }

        public static float? ByStyle(this float? f, ToStringStyle style)
        {
            return f?.ByStyle(style);
        }

        public static float ByStyle(this float value, ToStringStyle style, ToStringNumberSense numberSense = ToStringNumberSense.Absolute)
        {
            if (style == ToStringStyle.Temperature && numberSense == ToStringNumberSense.Offset)
            {
                style = ToStringStyle.TemperatureOffset;
            }
            if (numberSense == ToStringNumberSense.Factor)
            {
                if (value >= 10f)
                {
                    style = ToStringStyle.FloatMaxTwo;
                }
                else
                {
                    style = ToStringStyle.PercentZero;
                }
            }


            float result;
            switch (style)
            {
                case ToStringStyle.Integer:
                    result = Mathf.RoundToInt(value);
                    break;
                case ToStringStyle.FloatOne:
                    result = (float)Math.Round(value, 1);
                    break;
                case ToStringStyle.FloatTwo:
                    result = (float)Math.Round(value, 2);
                    break;
                case ToStringStyle.FloatThree:
                    result = (float)Math.Round(value, 3);
                    break;
                case ToStringStyle.FloatMaxOne:
                    result = (float)Math.Round(value, 1); //f.ToString("0.#");
                    break;
                case ToStringStyle.FloatMaxTwo:
                    result = (float)Math.Round(value, 2); //f.ToString("0.##");
                    break;
                case ToStringStyle.FloatMaxThree:
                    result = (float)Math.Round(value, 3); //f.ToString("0.###");
                    break;
                case ToStringStyle.FloatTwoOrThree:
                    result = (float)Math.Round(value, (value == 0f || Mathf.Abs(value) >= 0.01f) ? 2 : 3);
                    break;
                case ToStringStyle.PercentZero:
                    result = value.ToPercentNew();
                    break;
                case ToStringStyle.PercentOne:
                    result = value.ToPercentNew(1);
                    break;
                case ToStringStyle.PercentTwo:
                    result = value.ToPercentNew(2);
                    break;
                case ToStringStyle.Temperature:
                    result = value.ToTemperature(1);
                    break;
                case ToStringStyle.TemperatureOffset:
                    result = value.ToTemperatureOffset(1);
                    break;
                case ToStringStyle.WorkAmount:
                    result = value.ToWorkAmount();
                    break;
                case ToStringStyle.Money:
                    result = value.ToMoney(null);
                    break;
                default:
                    Log.Error("Unknown ToStringStyle " + style);
                    result = value;
                    break;
            }
            // if (numberSense == ToStringNumberSense.Offset)
            // {
            // 	if (f >= 0f)
            // 	{
            // 		value = "+" + value;
            // 	}
            // }
            // else if (numberSense == ToStringNumberSense.Factor)
            // {
            // 	value = "x" + value;
            // }
            return result;
        }
    }
}