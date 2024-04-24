using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Plot : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private SpriteRenderer sr;
    [SerializeField] private Color hoverColor;
    [SerializeField] private BuildManager buildManager; // Referencia al BuildManager

    public GameObject towerObj;
    public Turret turret;
    private Color startColor;

    private void Start()
    {
        startColor = sr.color;
    }

    private void OnMouseEnter()
    {
        sr.color = hoverColor;
    }

    private void OnMouseExit()
    {
        sr.color = startColor;
    }

    public void OnMouseDown()
    {
        // Verifica si el juego está pausado antes de continuar
        if (PauseMenu.instance != null && PauseMenu.instance.isPaused)
        {
            return;
        }

        if (UIManager.main.IsHoveringUI()) return;

        if (towerObj != null)
        {
            Turret turretComponent = towerObj.GetComponent<Turret>();
            TurretSlomo turretSlomoComponent = towerObj.GetComponent<TurretSlomo>();

            if (turretComponent != null)
            {
                turretComponent.OpenUpgradeUI();
            }
            else if (turretSlomoComponent != null)
            {
                turretSlomoComponent.OpenUpgradeUI();
            }

            return;
        }

        Tower towerToBuild = BuildManager.main.GetSelectedTower();

        if (towerToBuild.cost > LevelManager.main.currency)
        {
            Debug.Log("You can't afford this tower");
            return;
        }

        LevelManager.main.SpendCurrency(towerToBuild.cost);

        // Incrementa el costo de la torreta en la clase Tower
        towerToBuild.cost += 50; // Aumenta el costo en 50 unidades

        towerObj = Instantiate(towerToBuild.prefab, transform.position, Quaternion.identity);
        turret = towerObj.GetComponent<Turret>();
    }

}
