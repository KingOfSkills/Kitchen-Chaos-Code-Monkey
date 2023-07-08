using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ProgressBarUI : MonoBehaviour
{
    [SerializeField] private GameObject hasProgressGameObject;
    [SerializeField] private Image progressBar;

    private IHasProgress hasProgress;

    private void Start()
    {
        hasProgress = hasProgressGameObject.GetComponent<IHasProgress>();
        if (hasProgress == null)
        {
            Debug.LogError("Game Object " + hasProgressGameObject + " does not have a component that implement IHasProgress!");
        }

        hasProgress.OnProgressChanged += CuttingCounter_OnProgressChanged;

        progressBar.fillAmount = 0f;

        Hide();
    }
    private void CuttingCounter_OnProgressChanged(object sender, IHasProgress.OnProgressChangedEventArgs e)
    {
        progressBar.fillAmount = e.progressNormalized;

        if (progressBar.fillAmount == 0f || progressBar.fillAmount == 1f)
        {
            Hide();
        }
        else
        {
            Show();
        }
    }
    private void Show()
    {
        gameObject.SetActive(true);
    }
    private void Hide()
    {
        gameObject.SetActive(false);
    }
}
