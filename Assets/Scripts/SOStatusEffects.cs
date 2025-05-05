using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "StatusEffect", menuName = "ScriptableObjects/StatusEffects", order = 1)]
public class SOStatusEffects : ScriptableObject
{
    public enum EEffects
    {
        Slow,
        Fast,
        InvertMovement
    }
    public ParticleSystem visualEffect;
    public Color darkestColor, brightestColor;
    public LayerMask effectable;
    public EEffects effect;
    public static void ApplyEffect(GameObject go, EEffects effect)
    {
        FirstPersonController pc;
        pc = go.GetComponentInParent<FirstPersonController>();
        if (pc == null)
            return;

        switch (effect)
        {
            case EEffects.Slow:
            Effects.Slow(pc);
                break;
            case EEffects.Fast:
                Effects.Fast(pc);
                break;
            case EEffects.InvertMovement:
                Effects.InvertMovement(pc);
                break;
            default:
                break;
        }
    }
}
public enum EEffects
{
    Slow,
    Speed
}
public static class Effects
{
    public static void Slow(FirstPersonController controller)
    {
        controller.GetComponent<MovementController>().AddMovementMultiplier(0.65f, 3f);
    }
    public static void Fast(FirstPersonController controller)
    {
        controller.GetComponent<MovementController>().AddMovementMultiplier(2.0f, 1.25f);
    }
    public static void InvertMovement(FirstPersonController controller)
    {
        controller.InvertMovement(5.0f);
    }
}