using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;

public class Messages : MonoBehaviour {

    Player _player;
    [SerializeField] Text _text;

    [SerializeField] float _rangeToShowText;
    string _currentQuote;
    public string Quote
    {
        set { _currentQuote = value; }
    }

    [SerializeField] GameObject _interactUI;
    bool _registered;
    bool _showing;

    [SerializeField] Material _exclamationMat;
    [SerializeField] ParticleSystem _exclamationParticles;

    public event Action MessageShown = delegate { };

	// Use this for initialization
	void Start ()
    {
        _player = FindObjectOfType<Player>();

        _currentQuote = "We need that necromancer's book. \n Maybe we should check in the library";

        _exclamationMat.color = Color.red;
	}
	
    public void ShowText()
    {
        _text.text = _currentQuote;
        _exclamationMat.color = Color.gray;
        _exclamationParticles.Stop();
        if (!_showing)
            StartCoroutine(ShowTextStep());
    }

	// Update is called once per frame
	void Update ()
    {
        if (Vector3.Distance(transform.position, _player.transform.position) < _rangeToShowText)
        {
            if (!_registered)
            {
                _registered = true;
                _player.Interact += ShowText;
                _interactUI.SetActive(true);
            }
        }

        else
        {
            if (_registered)
            {
                _registered = false;
                _player.Interact -= ShowText;
                _interactUI.SetActive(false);
            }
        }
	}

    public void UpdateQuote(string quote, Transform newPositionRotation)
    {
        _currentQuote = quote;
        _exclamationMat.color = Color.red;
        _exclamationParticles.Play();
        transform.position = newPositionRotation.position;
        transform.rotation = newPositionRotation.rotation;
    }

    IEnumerator ShowTextStep()
    {
        _showing = true;
        _interactUI.SetActive(false);
        yield return new WaitForSeconds(5);
        if(_registered) _interactUI.SetActive(true);
        _text.text = "";
        _showing = false;
        MessageShown();
    }
}
