using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class OptionMenu : MonoBehaviour
{
    [SerializeField] private AudioMixer audioMixer;

    public void PantallaCompleta(bool pantallaCompleta)
    {
        Screen.fullScreen = pantallaCompleta; // Establece el modo de pantalla completa seg�n el valor booleano proporcionado
    }

    public void CambiarVolumen(float volumen)
    {
        audioMixer.SetFloat("Volumen", volumen); // Cambia el volumen del mezclador de audio utilizando el par�metro "Volumen" y el valor proporcionado
    }

    public void CambiarCalidad(int index)
    {
        QualitySettings.SetQualityLevel(index); // Cambia el nivel de calidad gr�fica de la aplicaci�n o el juego seg�n el �ndice proporcionado
    }
}
