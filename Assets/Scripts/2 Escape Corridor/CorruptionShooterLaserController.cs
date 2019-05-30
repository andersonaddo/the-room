using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CorruptionShooterLaserController : GenericV3DLaserController
{

    //For affecting the color of the drone shooting the lazer
    [SerializeField] Renderer droneRenderer;
    [SerializeField] float droneRendererColorMultiplier = 3f;

    public CorruptionShooterSFX sfxcontroller;

    [SerializeField] List<Renderer> destructionAndDisappearRenderers;

    override public void startShooting()
    {
        base.startShooting();
        sfxcontroller.playShootingSFX();
    }


    override protected void Update()
    {
        base.Update();

        foreach (Renderer rend in destructionAndDisappearRenderers)
        {
            rend.material.SetFloat("_GammaLinear", gammaLinear);
        }

        //Updating Drone Material
        if (droneRenderer != null)
        {
            droneRenderer.material.SetColor("_EmissionColor", color * droneRendererColorMultiplier);
        }
    }

    override protected void UpdateColorOfChildren()
    {
        if (colorizeAll)
        {
            foreach (Renderer rend in destructionAndDisappearRenderers)
            {
                rend.material.SetColor("_FinalColor", color);
            }
        }
    }
}