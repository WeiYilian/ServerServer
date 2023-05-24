using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Box : MonoBehaviour
{
    private Animator animator;

    private GameObject Tip;

    private bool isOpen;
    
    private void Awake()
    {
        animator = GetComponent<Animator>();
        Tip =GameObject.Find("Canvas").transform.Find("Tip").gameObject;
    }


    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Tip.SetActive(true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Tip.SetActive(false);
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (Input.GetKeyDown(KeyCode.F) && !isOpen)
            {
                isOpen = true;
                animator.SetTrigger("Open");
                transform.GetChild(3).localRotation = Quaternion.Euler(-240,0,0);
                Tip.SetActive(false);
                GameFacade.Instance.GameControler.Box++;
            }
        }
    }
}
