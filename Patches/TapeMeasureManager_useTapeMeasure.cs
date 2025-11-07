using HarmonyLib;
using MelonLoader;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using UnityEngine;

namespace WorldEditMod.Patches
{
    [HarmonyPatch(typeof(TapeMeasureManager), nameof(TapeMeasureManager.useTapeMeasure))]
    internal class TapeMeasureManager_useTapeMeasure
    {
        // LMAO the amount of spelling errors in the source code.
        // Messuring, Messurable, Mesure...
        static readonly FieldInfo CurrentlyMeasuring = AccessTools.Field(typeof(TapeMeasureManager), "currentlyMessuring");
        static readonly FieldInfo MeasurementSaved = AccessTools.Field(typeof(TapeMeasureManager), "measurementSaved");
        static bool Prefix(ref TapeMeasureManager __instance)
        {
            try
            {
                bool currentlyMeasuring = (bool)CurrentlyMeasuring.GetValue(__instance);
                bool measurementSaved = (bool)MeasurementSaved.GetValue(__instance);
                bool isShifting = Input.GetKey(KeyCode.LeftShift);
                bool shouldBypass = (!currentlyMeasuring && !measurementSaved) && (Core.Instance.IsMeasuring || isShifting);
                if (!shouldBypass)
                {
                    return true;
                }
            }
            catch(Exception e)
            {
                Melon<Core>.Logger.Error(e);
                return true;
            }

            Core.Instance.UseCustomTapeMeasure();

            return false;
        }
    }
}
