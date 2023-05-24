using System;
using System.Collections;
using System.Collections.Generic;
using System.Timers;
using UnityEngine;

public class Door : MonoBehaviour
{
    private GameObject Tip;
    private bool isOpen;
    private void Awake()
    {
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
            if (Input.GetKeyDown(KeyCode.F))
            {
                isOpen = true;
                Tip.SetActive(false);
            }
        }
        
    }

    private void Update()
    {
        if (isOpen)
        {
            float num = Mathf.Lerp(0, 2.8f, Time.deltaTime);
            transform.position += Vector3.up * num;
            if (transform.position.y >= 6f)
                isOpen = false;
        }
    }
}
