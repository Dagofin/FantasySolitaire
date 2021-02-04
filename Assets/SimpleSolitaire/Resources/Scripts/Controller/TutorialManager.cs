using System.Collections;
using UnityEngine;

public class TutorialManager : MonoBehaviour
{
    public GameObject Tutorial;
    public GameObject CardLayer;

    private const string FIRST_PLAY = "FirstPlay";

    /// <summary>
    /// Activate tutorial if player game first one.
    /// </summary>
    private IEnumerator Start ()
    {
        yield return new WaitForSeconds(0.1f);

        if (!PlayerPrefs.HasKey(FIRST_PLAY))
        {
			CardLayer.SetActive(true);
			Tutorial.SetActive(true);
        }
	}

    /// <summary>
    /// Close game tutorial window <see cref="Tutorial"/>.
    /// </summary>
    public void CloseTutorial()
    {
        PlayerPrefs.SetInt(FIRST_PLAY, 1);
        Tutorial.SetActive(false);
	}

	/// <summary>
	/// Is first play or not.
	/// </summary>
	public bool IsHasKey()
	{
		return PlayerPrefs.HasKey(FIRST_PLAY);
	}
}
