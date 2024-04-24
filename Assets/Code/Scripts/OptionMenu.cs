using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class OptionMenu : MonoBehaviour
{
    [SerializeField] private AudioMixer audioMixer;

    public void PantallaCompleta(bool pantallaCompleta)
    {
        Screen.fullScreen = pantallaCompleta; // Establece el modo de pantalla completa según el valor booleano proporcionado
    }

    public void CambiarVolumen(float volumen)
    {
        audioMixer.SetFloat("Volumen", volumen); // Cambia el volumen del mezclador de audio utilizando el parámetro "Volumen" y el valor proporcionado
    }

    public void CambiarCalidad(int index)
    {
        QualitySettings.SetQualityLevel(index); // Cambia el nivel de calidad gráfica de la aplicación o el juego según el índice proporcionado
    }
}
