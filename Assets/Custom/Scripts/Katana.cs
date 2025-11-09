using System;
using System.Threading.Tasks;
using UnityEngine;

public class Katana : TargetFinder
{
    [SerializeField] GameObject attackEffect;
    [SerializeField] Animator attackAnim;

    private bool isAttacking;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        OnTargetFound += Attack;
    }

    private void Update()
    {
        FindTarget();
    }

    private async void Attack()
    {
        if (!isAttacking)
        {
            attackAnim.enabled = this;
            isAttacking = true;
            attackEffect.SetActive(true);
            await Task.Delay(2000);
            attackEffect.SetActive(false);
            isAttacking = false;
            attackAnim.enabled = false;
            attackAnim.gameObject.transform.rotation = Quaternion.identity;
        }
    }
}
