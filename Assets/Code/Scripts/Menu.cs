using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Menu : MonoBehaviour
{
    [Header("References")]
    [SerializeField] TextMeshProUGUI currencyUI;
    [SerializeField] TextMeshProUGUI towerCostUI;
    [SerializeField] Animator anim;

    private bool isMenuOpen = true;

    public void ToggleMenu()
    {
        isMenuOpen = !isMenuOpen;
        anim.SetBool("MenuOpen", isMenuOpen);
    }

    private void Update()
    {
        // Actualiza la moneda en la interfaz de usuario
        currencyUI.text = LevelManager.main.currency.ToString();

        // Obtiene la torreta seleccionada
        Tower selectedTower = BuildManager.main.GetSelectedTower();

        // Actualiza el costo de la torreta seleccionada en la interfaz de usuario
        if (selectedTower != null)
        {
            towerCostUI.text = selectedTower.cost.ToString();
        }
    }
}
