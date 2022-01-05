using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class MainMenuController : MonoBehaviour
{
    public List<string> carColor;

    public TMP_Dropdown carColorDropDown;

    private string selectedCarColor;

    private void Start() {
        carColorDropDown.ClearOptions();
        carColorDropDown.AddOptions(carColor);
        selectedCarColor = carColor[0];

    }

    public void SetColorCar(int colorCarIndex)
    {
        selectedCarColor = carColor[colorCarIndex];
    }

    public void StartButtonClicked()
    {
        GameManager.MainManager.LoadLevel("Track01", GameState.Preparing);
    }

    public void QuitButtonClicked()
    {
        Application.Quit();
    }
}
