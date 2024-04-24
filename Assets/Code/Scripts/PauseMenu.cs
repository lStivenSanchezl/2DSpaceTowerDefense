using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    public static PauseMenu instance;

    public string mainMenu;

    public GameObject pauseScreen;
    public GameObject pauseOverlay; // El panel que cubre la pantalla cuando el menú de pausa está activo
    public bool isPaused;

    private string currentScene;

    // Start is called before the first frame update
    void Awake()
    {
        instance = this;
    }

    void Start()
    {
        // Obtener el nombre de la escena actual
        currentScene = SceneManager.GetActiveScene().name;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetButtonDown("Menu"))
        {
            PauseUnpause();
        }
    }

    public void PauseUnpause()
    {
        isPaused = !isPaused;
        pauseScreen.SetActive(isPaused);
        pauseOverlay.SetActive(isPaused); // Activa/desactiva el panel de superposición cuando se activa/desactiva el menú de pausa
        Time.timeScale = isPaused ? 0f : 1f; // Pausar/despausar el tiempo

        // Desactivar o activar los componentes de movimiento y colisiones
        TogglePlayerComponents(!isPaused);
    }

    private void TogglePlayerComponents(bool enable)
    {
        // Buscar el jugador por etiqueta o nombre
        GameObject player = GameObject.FindGameObjectWithTag("Player");

        if (player != null)
        {
            // Obtener los componentes que deben ser desactivados/activados
            MonoBehaviour[] scripts = player.GetComponents<MonoBehaviour>();
            Collider2D[] colliders = player.GetComponentsInChildren<Collider2D>();

            // Activar/desactivar los componentes de los scripts
            foreach (MonoBehaviour script in scripts)
            {
                if (script != this) // No desactivar este script
                {
                    script.enabled = enable;
                }
            }

            // Activar/desactivar los colliders
            foreach (Collider2D collider in colliders)
            {
                collider.enabled = enable;
            }
        }
    }

    public void RestartLevel()
    {
        SceneManager.LoadScene(currentScene);
        Time.timeScale = 1f;
    }

    public void MainMenu()
    {
        SceneManager.LoadScene(mainMenu);
        Time.timeScale = 1f;
    }
}
