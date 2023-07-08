using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StoveBurnBarFlashingUI : MonoBehaviour
{
    private const string IS_FlASHING = "IsFlashing";

    [SerializeField] private StoveCounter stoveCounter;

    private Animator animator;

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    private void Start()
    {
        stoveCounter.OnProgressChanged += StoveCounter_OnProgressChanged;

        animator.SetBool(IS_FlASHING, false);
    }

    private void StoveCounter_OnProgressChanged(object sender, IHasProgress.OnProgressChangedEventArgs e)
    {
        float burnShowProgressAmount = .5f;
        bool show = e.progressNormalized >= burnShowProgressAmount && stoveCounter.IsFried();

        animator.SetBool(IS_FlASHING, show);
    }
    public void Show()
    {
        gameObject.SetActive(true);
    }
    public void Hide()
    {
        gameObject.SetActive(false);
    }
}
