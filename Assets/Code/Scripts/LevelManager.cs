// LevelManager.cs
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LevelManager : MonoBehaviour
{
    public static LevelManager main;

    public Transform startPoint;
    public Transform[] path;

    public int currency;
    public Slider healthSlider; // Referencia al Slider de la barra de vida del jugador

    private int maxHealth = 100;
    private int currentHealth;

    private int currentWave; // Agregar la propiedad para almacenar el valor de la oleada actual

    private void Awake()
    {
        main = this;
    }

    private void Start()
    {
        currency = 200;
        currentHealth = maxHealth;
        UpdateHealthDisplay();
    }

    public int CurrentWave // Propiedad para obtener el valor de la oleada actual
    {
        get { return currentWave; }
    }

    public void IncreaseCurrency(int amount)
    {
        currency += amount;
    }

    public bool SpendCurrency(int amount)
    {
        if (amount <= currency)
        {
            currency -= amount;
            return true;
        }
        else
        {
            Debug.Log("You don't have enough to purchase this item");
            return false;
        }
    }

    public void ReducePlayerHealth(int damage)
    {
        currentHealth -= damage;
        UpdateHealthDisplay();

        if (currentHealth <= 0)
        {
            // Reiniciar el nivel o recargar la escena
            RestartLevel();
        }
    }

    private void UpdateHealthDisplay()
    {
        healthSlider.value = currentHealth;
    }

    private void RestartLevel()
    {
        // Reiniciar el nivel recargando la escena actual
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
