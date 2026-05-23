using AlgernonCommons.Keybinding;
using ColossalFramework.UI;
using ICities;
using TrainDisplay.Settings;
using UnityEngine;

namespace TrainDisplay;

public class UIThreading : ThreadingExtensionBase
{
    public AudioClip ToggleSound
    {
        get
        {
            field ??= UIView.GetAView().defaultClickSound;
            return field;
        }
    }
    public AudioClip DisabledToggleSound
    {
        get
        {
            field ??= UIView.GetAView().defaultDisabledClickSound;
            return field;
        }
    }

    public override void OnUpdate(float realTimeDelta, float simulationTimeDelta)
    {
        if (KeyTriggered(TrainDisplaySettings.ToggleKey))
        {
            if (DisplayUIManager.Instance != null)
            {
                if (!DisplayUIManager.Instance.OverrideStatus.HasValue) DisplayUIManager.Instance.OverrideStatus = DisplayUIManager.Instance.enabled;

                DisplayUIManager.Instance.OverrideStatus = !DisplayUIManager.Instance.OverrideStatus;
                AudioManager.instance.PlaySound(DisplayUIManager.Instance.enabled ? ToggleSound : DisabledToggleSound, 1f);
            }
            else
                AudioManager.instance.PlaySound(DisabledToggleSound, 1f);
        }
    }

    private static bool KeyTriggered(Keybinding key)
    {
        // Check primary key.
        if (!Input.GetKeyDown((KeyCode)key.Key))
        {
            return false;
        }

        // Check modifier keys.
        if ((Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl)) != key.Control)
        {
            return false;
        }

        if ((Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift)) != key.Shift)
        {
            return false;
        }

        if ((Input.GetKey(KeyCode.LeftAlt) || Input.GetKey(KeyCode.RightAlt) || Input.GetKey(KeyCode.AltGr)) != key.Alt)
        {
            return false;
        }

        // If we got here, all checks passed.
        return true;
    }
}