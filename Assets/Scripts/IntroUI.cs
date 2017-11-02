using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class IntroUI : MonoBehaviour {

	public Text introText;
	public GameObject characterPanel;
	public Image introImage;

	// Use this for initialization
	void Start () {
		characterPanel.SetActive (false);
		introText.color = Color.clear;
		if (!PlayerPrefs.HasKey ("First_Intro")) {
			StartCoroutine (AnimateText ());

		} else {
			StartCoroutine (IntroScene ());
		}
	}

	IEnumerator IntroScene(){
		introImage.gameObject.SetActive (true);
		yield return new WaitForSeconds (3f);
		SceneManager.LoadScene ("Home");
	}


	IEnumerator AnimateText(){
		introImage.gameObject.SetActive (true);
		yield return new WaitForSeconds (2f);
		StartCoroutine (FadeImage(Color.white, Color.clear, 2f));
		yield return new WaitForSeconds (2f);
		introImage.gameObject.SetActive (false);
		introText.text = "THE SANCTUARY used to be one of the most ancient and peaceful kingdoms in the world.";
		StartCoroutine (Fade(Color.clear, Color.white, 1f));
		yield return new WaitForSeconds (4f);
		StartCoroutine (Fade(Color.white, Color.clear, 1f));
		yield return new WaitForSeconds (1f);
		introText.text = "It was just simply a paradise, till one day HYDRA army came.";
		StartCoroutine (Fade(Color.clear, Color.white, 1f));
		yield return new WaitForSeconds (4f);
		StartCoroutine (Fade(Color.white, Color.clear, 1f));
		yield return new WaitForSeconds (1f);
		introText.text = "That was the day that permanently changed the destiny of the whole kingdom";
		StartCoroutine (Fade(Color.clear, Color.white, 1f));
		yield return new WaitForSeconds (4f);
		StartCoroutine (Fade(Color.white, Color.clear, 1f));
		yield return new WaitForSeconds (1f);
		introText.text = "as HYDRA drowned THE SANCTUARY to the deepest hell of destruction";
		StartCoroutine (Fade(Color.clear, Color.white, 1f));
		yield return new WaitForSeconds (4f);
		StartCoroutine (Fade(Color.white, Color.clear, 1f));
		yield return new WaitForSeconds (1f);
		introText.text = "HYDRA burned down all the harvests, villages and houses";
		StartCoroutine (Fade(Color.clear, Color.white, 1f));
		yield return new WaitForSeconds (4f);
		StartCoroutine (Fade(Color.white, Color.clear, 1f));
		yield return new WaitForSeconds (1f);
		introText.text = "They also killed every soldiers and even civilians on their way to the castle.";
		StartCoroutine (Fade(Color.clear, Color.white, 1f));
		yield return new WaitForSeconds (4f);
		StartCoroutine (Fade(Color.white, Color.clear, 1f));
		yield return new WaitForSeconds (1f);
		introText.text = "But 3 sucessors of king and queen were protected by the old wizard of the kingdom during HYDRA's invasions.";
		StartCoroutine (Fade(Color.clear, Color.white, 1f));
		yield return new WaitForSeconds (4f);
		StartCoroutine (Fade(Color.white, Color.clear, 1f));
		yield return new WaitForSeconds (1f);
		introText.text = " With their special abilities, they are the lights at the end of the tunnel for THE SANCTUARY";
		StartCoroutine (Fade(Color.clear, Color.white, 1f));
		yield return new WaitForSeconds (4f);
		StartCoroutine (Fade (Color.white, Color.clear, 1f));
		yield return new WaitForSeconds (1f);
		introImage.color = Color.white;
		characterPanel.SetActive (true);
		yield return null;
	}

	IEnumerator Fade(Color from, Color to, float time) {
		float speed = 1 / time;
		float percent = 0;

		while (percent < 1) {
			percent += Time.deltaTime * speed;
			introText.color = Color.Lerp(from,to,percent);
			yield return null;
		}
	}

	IEnumerator FadeImage(Color from, Color to, float time) {
		float speed = 1 / time;
		float percent = 0;

		while (percent < 1) {
			percent += Time.deltaTime * speed;
			introImage.color = Color.Lerp(from,to,percent);
			yield return null;
		}
	}

	public void ChooseElecGirl(){
		PlayerPrefsController.instance.SetPlayer (0);
		PlayerPrefs.SetInt ("First_Intro", 1);
		StartCoroutine (IntroScene());
	}
	public void ChooseFireWizard(){
		PlayerPrefsController.instance.SetPlayer (1);
		PlayerPrefs.SetInt ("First_Intro", 1);
		StartCoroutine (IntroScene());
	}
	public void ChooseGreenArrow(){
		PlayerPrefsController.instance.SetPlayer (2);
		PlayerPrefs.SetInt ("First_Intro", 1);
		StartCoroutine (IntroScene());
	}
}

