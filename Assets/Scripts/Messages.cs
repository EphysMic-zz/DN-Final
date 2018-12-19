using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Messages : MonoBehaviour {

    Player _player;
    TextMesh _text;

    [SerializeField] float _rangeToShowText;
    string _currentQuote;

    public string Quote
    {
        set { _currentQuote = value; }
    }

	// Use this for initialization
	void Start ()
    {
        _player = FindObjectOfType<Player>();
        _text = GetComponent<TextMesh>();

        _currentQuote = "We need that necromancer's book. Maybe we should check in the library";

        FindObjectOfType<Necromancer>().OnBossDeath += () =>
        {
            _currentQuote = "We got the book. Now let's get out of here, quickly.";
        };
	}
	
	// Update is called once per frame
	void Update ()
    {
		if(Vector3.Distance(transform.position, _player.transform.position) <= _rangeToShowText)
            _text.text = _currentQuote;

        else
            _text.text = "";

        transform.forward = (transform.position - Camera.main.transform.position).normalized;
	}

    public void UpdateQuote(string quote)
    {
        _currentQuote = quote;
    }
}
