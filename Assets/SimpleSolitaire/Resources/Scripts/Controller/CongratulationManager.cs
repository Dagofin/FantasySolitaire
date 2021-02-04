using UnityEngine;
using UnityEngine.UI;

public class CongratulationManager : MonoBehaviour
{
    public Text CongratulationText;

    public string[] CongratsArray;

	/// <summary>
	/// Get congratulation text from array and setup.
	/// </summary>
    public void CongratulationTextFill()
    {
        CongratulationText.text = CongratsArray[UnityEngine.Random.Range(0, CongratsArray.Length - 1)];
    }
}
