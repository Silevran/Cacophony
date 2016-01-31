using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SelectPlayers : MonoBehaviour 
{
    [SerializeField]
    private List<KeyboardControl> _playerControls = new List<KeyboardControl>();

    [SerializeField]
    private HPBar _hpBar = null;

    private bool _paused = false;

	// Update is called once per frame
	void Update () 
    {
        //Start Button
        if (Input.GetButtonDown("Player1Start"))
        {
            if(_playerControls[0].SetupPlayerControls("Player1"))
            {
                
            }
        }
        else if (Input.GetButtonDown("Player2Start"))
        {
            if (_playerControls[1].SetupPlayerControls("Player2"))
            {
            }
        }
        else if (Input.GetButtonDown("Escape"))
        {
            Application.Quit();
        }
	}

    public void SelectPressed()
    {
        _paused = !_paused;
        for (int i = 0; i < _playerControls.Count; i++)
        {
            _playerControls[i].GameState(_paused);
        }
    }

    public void CharacterSelected()
    {
        for (int i = 0; i < _playerControls.Count; i++)
        {
            if(!_playerControls[i].selectedCharacter)
            {
                return;
            }
        }

        for (int i = 0; i < _playerControls.Count; i++)
        {
            _playerControls[i].BeginCountdown();
        }
    }

    public void ApplyDamage(int player, char type)
    {
        _hpBar.Attack(player, type);
    }

    public void UpdateDamage(int player, int damage, char type)
    {
        _hpBar.PrepareDamage(player, damage, type);
    }

    public void RemoveDamage(int player, char type)
    {
        _hpBar.RemoveDamage(player, type);
    }

    public void NewGame()
    {
        _paused = false;

        for(int i = 0; i < _playerControls.Count; i++)
        {
            _playerControls[i].NewGame();
        }
    }
}
