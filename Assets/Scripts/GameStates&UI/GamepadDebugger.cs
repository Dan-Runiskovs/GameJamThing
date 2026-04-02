using UnityEngine;
using UnityEngine.InputSystem;

public class GamepadDebugger : MonoBehaviour
{
    private int lastCount = -1;

    private void Update()
    {
        if (Gamepad.all.Count != lastCount)
        {
            lastCount = Gamepad.all.Count;

            Debug.Log("------ GAMEPADS ------");
            Debug.Log("Gamepad.all.Count = " + Gamepad.all.Count);

            for (int i = 0; i < Gamepad.all.Count; i++)
            {
                Gamepad pad = Gamepad.all[i];

                Debug.Log(
                    $"Index {i} | Name: {pad.displayName} | " +
                    $"Layout: {pad.layout} | " +
                    $"DeviceId: {pad.deviceId} | " +
                    $"Interface: {pad.description.interfaceName} | " +
                    $"Product: {pad.description.product}"
                );
            }
        }

        for (int i = 0; i < Gamepad.all.Count; i++)
        {
            if (Gamepad.all[i].buttonSouth.wasPressedThisFrame)
            {
                Debug.Log($"A/Cross pressed on index {i} | {Gamepad.all[i].displayName}");
            }
        }
    }
}