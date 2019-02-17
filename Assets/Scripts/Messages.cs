using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Messages : MonoBehaviour {

    Player _player;
    TextMesh _text;

    [SerializeField] float _rangeToShowText;
    string _currentQuote;
    public string Quote
    {
        set { _currentQuote = value; }
    }

    [SerializeField] GameObject _interactUI;
    bool _registered;
    bool _showing;

	// Use this for initialization
	void Start ()
    {
        _player = FindObjectOfType<Player>();
        _text = GetComponent<TextMesh>();

        _currentQuote = "We need that necromancer's book. \n Maybe we should check in the library";

        FindObjectOfType<Necromancer>().OnBossDeath += () =>
        {
            _currentQuote = "We got the book. Now let's get out of here, quickly.";
        };
	}
	
    public void ShowText()
    {
        _text.text = _currentQuote;
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

        transform.forward = (transform.position - Camera.main.transform.position).normalized;
	}

    public void UpdateQuote(string quote)
    {
        _currentQuote = quote;
    }

    IEnumerator ShowTextStep()
    {
        _showing = true;
        yield return new WaitForSeconds(5);
        _text.text = "";
        _showing = false;
    }
}
