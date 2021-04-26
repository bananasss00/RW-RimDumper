//#define DEBUG

using System;

using UnityEngine;
using Verse;
//using HarmonyLib;

namespace RimDumper.Extensions
{
    public static class FloatExtensions
    {
        public static bool IsNull(this float floatValue)
        {
            return floatValue is > (-0.001f) and < 0.001f;
        }

        public static float? Nullify(this float floatValue)
        {
            return floatValue is > (-0.001f) and < 0.001f ? null : floatValue;
        }

        public static float? Nullify(this float? floatValue)
        {
            return floatValue is null or (> (-0.001f) and < 0.001f) ? null : floatValue;
        }

        public static float? ToPercent(this float? floatValue)
        {
            return floatValue == null ? null : (float?)Math.Round((float)floatValue * 100f, 2);
        }

        public static float ToPercent(this float floatValue)
        {
            return (float)Math.Round(floatValue * 100f, 2);
        }

        public static float RoundTo2(this float floatValue)
        {
            return (float)Math.Round(floatValue, 2);
        }

        public static float? RoundTo2(this float? floatValue)
        {
            return floatValue == null ? null : (float?)Math.Round((float)floatValue, 2);
        }

        public static float? ByStyle(this float? f, ToStringStyle style)
        {
            return f?.ByStyle(style);
        }

        public static float ByStyle(this float f, ToStringStyle style)
        {
            var value = style switch
            {
                ToStringStyle.Integer => Mathf.RoundToInt(f),
                ToStringStyle.PercentZero or ToStringStyle.PercentOne or ToStringStyle.PercentTwo => f.ToPercent(),
                ToStringStyle.WorkAmount => Mathf.CeilToInt(f / 60f),
                //case ToStringStyle.Temperature:
                //case ToStringStyle.TemperatureOffset:
                _ => f,
            };
            return value;
        }
    }
}