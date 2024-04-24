using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthEnemy : MonoBehaviour
{
    [Header("Attributes")]
    [SerializeField] private int hitPoint = 2;
    [SerializeField] private int baseCurrencyWorth = 50; // Valor base de la moneda
    private int currencyWorth; // Valor actual de la moneda

    private bool isDestroyed = false;

    private SpriteRenderer spriteRenderer; // Referencia al SpriteRenderer

    private void Start()
    {
        UpdateCurrencyWorth(); // Actualizar el valor de la moneda al iniciar

        // Obtener referencia al componente SpriteRenderer
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public void TakeDamage(int dmg)
    {
        hitPoint -= dmg;

        if (hitPoint <= 0 && !isDestroyed)
        {
            EnemySpawner.onEnemyDestroy.Invoke();
            LevelManager.main.IncreaseCurrency(currencyWorth);
            isDestroyed = true;
            Destroy(gameObject);
        }
        else
        {
            // Ajustar la opacidad del sprite según la cantidad de vida restante
            float opacity = Mathf.Clamp01((float)hitPoint / 10f); // Suponiendo que la vida máxima es 10 (ajusta según tu necesidad)
            opacity = Mathf.Max(opacity, 0.39f); // Limitar la opacidad mínima a 0.39 (aproximadamente 100 en escala de 0 a 1)
            spriteRenderer.color = new Color(1f, 1f, 1f, opacity); // Asignar la nueva opacidad al color del sprite

            // Si la vida llega a 0, destruir el GameObject
            if (hitPoint <= 0 && !isDestroyed)
            {
                isDestroyed = true;
                Destroy(gameObject);
            }
        }
    }

    public void SetHealth(int newHealth)
    {
        hitPoint = newHealth;
    }

    private void UpdateCurrencyWorth()
    {
        currencyWorth = baseCurrencyWorth + (LevelManager.main.CurrentWave * 5); // Aumentar el valor de la moneda con cada oleada
    }
}
