using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class TowerData
{
    public Tower towerPrefab;
    public int cost;
}

public class BuildManager : MonoBehaviour
{
    public static BuildManager main; // Referencia estática a sí mismo

    [Header("References")]
    [SerializeField] private List<TowerData> towers;

    private int selectedTowerIndex = 0;

    private void Awake()
    {
        if (main == null)
        {
            main = this; // Asignar esta instancia como la instancia principal si no hay ninguna asignada
        }
        else
        {
            Destroy(gameObject); // Destruir este objeto si ya hay una instancia principal
        }
    }

    public Tower GetSelectedTower()
    {
        if (selectedTowerIndex >= 0 && selectedTowerIndex < towers.Count)
        {
            return towers[selectedTowerIndex].towerPrefab;
        }
        else
        {
            return null;
        }
    }

    public int GetSelectedTowerCost()
    {
        if (selectedTowerIndex >= 0 && selectedTowerIndex < towers.Count)
        {
            return towers[selectedTowerIndex].cost;
        }
        else
        {
            return 0;
        }
    }

    public void SetSelectedTower(int index)
    {
        if (index >= 0 && index < towers.Count)
        {
            selectedTowerIndex = index;
        }
    }
}
