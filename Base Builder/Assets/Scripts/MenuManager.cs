using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
public class MenuManager : MonoBehaviour
{
    //questo quando la scena da caricare è pesante per avere il meno tempo di attesa
    //fatta per il main
    public GameObject pannelloImpo;
    private IEnumerator apri(string nome)
    {
        AsyncOperation ope = SceneManager.LoadSceneAsync(nome);
        while(!ope.isDone)
        {
            yield return null;
        }
    }
    public void apriUI()
    {
        StartCoroutine(apri("Main"));
    }
    public void chiudi()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
    Application.Quit();
#endif
    }

    public void apriImpo()
    {
        if (pannelloImpo.activeInHierarchy) pannelloImpo.SetActive(false);
        else pannelloImpo.SetActive(true);
    }
}

