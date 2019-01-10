using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class UIControllerBehaviour : MonoBehaviour {

    [SerializeField] private Button _restartButton;

    private bool _AButton;

	// Use this for initialization
	void Start ()
    {
		
	}
	
	// Update is called once per frame
	void Update ()
    {
        _AButton = Input.GetKeyUp(KeyCode.Joystick1Button0);

        _restartButton.onClick.AddListener(RestartGame);
	}

    public void RestartGame()
    {
        if(_AButton)
        {
            _restartButton.enabled = true;
        }
    }
}
