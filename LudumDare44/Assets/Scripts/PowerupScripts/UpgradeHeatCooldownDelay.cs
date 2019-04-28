﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpgradeHeatCooldownDelay : PowerUp
{

    private CraigController cc;
    private float healthCost = 0.4f;

    public override float GetHealthLossAmount()
    {
        return healthCost;
    }

    public override void PowerUpEffect()
    {
        cc.upgradeHeatCooldownDelay();
            //play some unique sound effect?
    }

    public override void SetHealthCostFree()
    {
        healthCost = 0;
    }

    // Start is called before the first frame update
    void Start()
    {
        base.Start();
        cc = (GameObject.FindGameObjectWithTag("Player")).GetComponent<CraigController>();
    }
}
