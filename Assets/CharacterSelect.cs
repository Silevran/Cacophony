using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class CharacterSelect : MonoBehaviour 
{
    [SerializeField]
    private GameObject _textArea = null;
    [SerializeField]
    private GameObject _characterSelect = null;
    [SerializeField]
    private GameObject _readyState = null;

    [SerializeField]
    private AudioSource _player = null;

    [SerializeField]
    private List<AudioClip> _personalSound = new List<AudioClip>();

    [SerializeField]
    private List<string> _names = new List<string>();
    [SerializeField]
    private Text _character = null;

    private int currentSelection = 0;

    public bool isReady = false;

    public int GetSelection()
    {
        return currentSelection;
    }

    public void ChangeSelection(int direction)
    {
        if(isReady)
        {
            return;
        }

        currentSelection += direction;
        if (currentSelection < 0)
        {
            currentSelection = _personalSound.Count - 1;
        }
        else if (currentSelection >= _personalSound.Count)
        {
            currentSelection = 0;
        }

        _player.Stop();
        _player.PlayOneShot(_personalSound[currentSelection]);
        _character.text = _names[currentSelection];
    }

    public void ControllerSelected()
    {
        _textArea.SetActive(false);
        _characterSelect.SetActive(true);
        _player.PlayOneShot(_personalSound[currentSelection]);
    }

    public void CharacterSelected()
    {
        _readyState.SetActive(true);
        isReady = true;
    }

    public void CharacterUnselected()
    {
        _readyState.SetActive(false);
        isReady = false;
    }

    public void ClearSelf()
    {
        gameObject.SetActive(false);
    }

    public void NewGame()
    {
        _textArea.SetActive(true);
        _characterSelect.SetActive(false);
        _readyState.SetActive(false);
        gameObject.SetActive(true);

        isReady = false;

        currentSelection = 0;
    }
}
