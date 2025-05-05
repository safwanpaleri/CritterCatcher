using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BreakableObjectsWithEffect : BreakableObject
{
    [SerializeField] List<SOStatusEffects> effects = new List<SOStatusEffects>();
    private SOStatusEffects chosenEffect;
    [SerializeField] float effectRange = 5.0f;

    private void Start()
    {
        if (effects.Count > 0)
        {
            chosenEffect = effects[Random.Range(0, effects.Count)];
            ApplyChosenEffect();
        }
    }

    private void ApplyChosenEffect()
    {
        // Change material
        MeshRenderer[] meshR = GetComponentsInChildren<MeshRenderer>(true);
        foreach (var mr in meshR)
        {
            //mr.material.color = chosenEffect.colorChange;
            mr.material.SetColor("_Darkest", chosenEffect.darkestColor);
            mr.material.SetColor("_Brighest", chosenEffect.brightestColor);
        }
    }

    protected override void ExtraBreakEffects()
    {
        base.ExtraBreakEffects();
        if (chosenEffect.visualEffect != null)
        {
            Instantiate(chosenEffect.visualEffect);
        }

        RaycastHit[] hits = Physics.SphereCastAll(transform.position, effectRange, Vector3.forward, 1.0f, chosenEffect.effectable);

        foreach (var hit in hits)
        {
            SOStatusEffects.ApplyEffect(hit.transform.gameObject, chosenEffect.effect);
            return;
        }
    }
}
