using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class KeyboardControl : MonoBehaviour
{
    #region defines
    private Color aColor = new Color(211f / 255f, 96f / 255f, 96f / 255f, 99f / 255f); //D3606063
    private Color bColor = new Color(193f / 255f, 211f / 255f, 96f / 255f, 99f / 255f); //C3D36063
    private Color xColor = new Color(105f / 255f, 96f / 255f, 211f / 255f, 99f / 255f); //6960D363
    private Color yColor = new Color(100f / 255f, 211f / 255f, 96f / 255f, 99f / 255f); //64D36063
    #endregion

    #region SerializeFields
    private Dictionary<char, SpawnedObject> _droppingKeys = new Dictionary<char, SpawnedObject>();
    [SerializeField] private Dictionary<char, SpawnKey> _key = new Dictionary<char, SpawnKey>();

    [SerializeField]
    private GameObject _actionBar = null;
    [SerializeField]
    private GameObject _musicPanel = null;
    [SerializeField]
    private GameObject _bottomPanel = null;
    [SerializeField]
    private CharacterSelect _startScreen = null;
    [SerializeField]
    private SelectPlayers _selectPlayers = null;

    [SerializeField]
    private Text _countdownText = null;
    
    [SerializeField]
    private List<SelectArea> _actionAreas = new List<SelectArea>();
    [SerializeField]
    private GameObject _actionZone = null;
    [SerializeField]
    private RectTransform _actionParent = null;
    #endregion

    [SerializeField]
    private List<char> _timeIndex = new List<char>();
    [SerializeField]
    public List<int> _timeTillActivation = new List<int>();

    private int _activations = 1;
    private int _cycledToActivations = 5;
    private bool _started = false;
    private string _playerIdentifier = "";

    private bool _yAxisPressed = false;

    private float countdownTimer = 3f;

    public bool selectedCharacter = false;
	
    public void RegisterObject(SpawnedObject so, char index)
    {
        _droppingKeys.Add(index, so);
        so.SetActivationTime(_timeTillActivation[_timeIndex.IndexOf(index)]);
    }

    public void RemoveObject(char index)
    {
        _droppingKeys.Remove(index);
    }

    public void RegisterKey(SpawnKey sk, char index)
    {
        _key.Add(index, sk);
        sk.soundIndex = _startScreen.GetSelection();
    }
	
	// Update is called once per frame
	void Update () 
    {
        if (_started)
        {
            if(countdownTimer > 0)
            {
                countdownTimer -= Time.deltaTime;
                int culled = (int)countdownTimer;
                _countdownText.text = culled + "s";
                //return;
            }
            if (_countdownText.gameObject.activeSelf)
            {
                _countdownText.gameObject.SetActive(false);
            }

            char index = '0';
            //A Button
            if (Input.GetButtonDown(_playerIdentifier + "A"))
            {
                index = 'A';
            }
            //B Button
            if (Input.GetButtonDown(_playerIdentifier + "B"))
            {
                index = 'B';
            }

            //X Key
            if (Input.GetButtonDown(_playerIdentifier + "X"))
            {
                index = 'X';
            }
            //Y Button
            if (Input.GetButtonDown(_playerIdentifier + "Y"))
            {
                index = 'Y';
            }

            if (index != '0')
            {
                //Spawn a dropping button.
                if (!_key[index].HasSpawned() && _activations > 0)
                {
                    _key[index].SpawnKeyObject();
                    _activations--;

                    if (_activations == 0)
                    {
                        foreach (char c in _key.Keys)
                        {
                            _key[c].SetEnabled(false);
                        }
                    }
                }
                else if (_droppingKeys.ContainsKey(index))
                {
                    if (!_droppingKeys[index].OnButtonPressed())
                    {
                        GameObject obj = GameObject.Instantiate(_actionZone);
                        obj.transform.SetParent(_actionParent);
                        obj.transform.localPosition = _droppingKeys[index].transform.localPosition;
                        obj.transform.localScale = Vector3.one;
                        _actionAreas.Add(obj.GetComponent<SelectArea>());
                        _actionAreas[_actionAreas.Count - 1].buttonType = index;
                        _actionAreas[_actionAreas.Count - 1].isActive = true;
                        _actionAreas[_actionAreas.Count - 1].wasHit = true;

                        _droppingKeys[index].PlaySound();
                    }

                }
            }

            //Pause Button
            if (Input.GetButtonDown(_playerIdentifier + "Select"))
            {
                _selectPlayers.SelectPressed();
            }
        }
        else if(_playerIdentifier.Length > 0)
        {
            //Pause Button
            if (Input.GetButtonDown(_playerIdentifier + "A"))
            {
                if (_startScreen.isReady)
                {
                    selectedCharacter = false;
                    _startScreen.CharacterUnselected();
                }
                else
                {
                    selectedCharacter = true;
                    _startScreen.CharacterSelected();
                    _selectPlayers.CharacterSelected();
                }
            }

            if (Input.GetAxisRaw(_playerIdentifier + "YAxis") > 0.5)
            {
                if (!_yAxisPressed)
                {
                    _yAxisPressed = true;
                    _startScreen.ChangeSelection(1);
                }
            }
            else if(Input.GetAxisRaw(_playerIdentifier + "YAxis") < -0.5)
            {
                if (!_yAxisPressed)
                {
                    _yAxisPressed = true;
                    _startScreen.ChangeSelection(-1);
                }
            }
            else
            {
                _yAxisPressed = false;
            }
        }
	}

    public bool SetupPlayerControls(string id)
    {
        if (_playerIdentifier.Length == 0)
        {
            _playerIdentifier = id;

            _startScreen.ControllerSelected();
            return true;
        }
        return false;
    }

    public void BeginCountdown()
    {
        _actionBar.SetActive(true);
        _musicPanel.SetActive(true);
        _bottomPanel.SetActive(true);

        _startScreen.ClearSelf();
        _countdownText.gameObject.SetActive(true);

        _started = true;
    }

    public Color GetColourForButton(char index)
    {
        switch (index)
        {
            case 'A':
                return aColor;
            case 'B':
                return bColor;
            case 'X':
                return xColor;
            case 'Y':
                return yColor;
            default:
                return aColor;
        }
    }

    public bool CheckComplete(char input)
    {
        for (int i = 0; i < _actionAreas.Count; i++)
        {
            if(_actionAreas[i].buttonType == input)
            {
                break;
            }
            else if(i == _actionAreas.Count-1)
            {
                return false;
            }
        }

        for (int i = 0; i < _actionAreas.Count; i++)
        {
            if (_actionAreas[i].isActive && 
                _actionAreas[i].buttonType == input && 
                !_actionAreas[i].wasHit)
            {
                return false;
            }
        }

        //New button activation allowed
        _cycledToActivations--;
        if(_cycledToActivations == 0)
        {
            _cycledToActivations = 5;
            _activations++;

            foreach(char index in _key.Keys)
            {
                _key[index].SetEnabled(true);
            }
        }

        return true;
    }

    public void ResetAreas(char type)
    {
        for(int i = 0; i < _actionAreas.Count; i++)
        {
            if(_actionAreas[i].buttonType == type)
            {
                _actionAreas[i].wasHit = false;
            }
        }
    }

    public void MissedAction(char type)
    {
        _droppingKeys[type].CauseRespawn();
        _key[type].Disable();

        foreach (char c in _droppingKeys.Keys)
        {
            _droppingKeys[c].CauseReset();
        }

        int count = 0;
        foreach (char keyC in _key.Keys)
        {
            if (_key[keyC].HasSpawned())
            {
                count++;
            }
        }
        if (count == 0)
        {
            _activations++;
            foreach (char index in _key.Keys)
            {
                _key[index].SetEnabled(true);
            }
        }

        for (int i = 0; i < _actionAreas.Count; i++)
        {
            if(_actionAreas[i].buttonType == type)
            {
                GameObject.Destroy(_actionAreas[i].gameObject);

                _actionAreas.Remove(_actionAreas[i]);
                
                i--;
            }
        }

        int player = _playerIdentifier[_playerIdentifier.Length - 1] - '0';
        _selectPlayers.RemoveDamage(player, type);
    }

    public void GameState(bool paused)
    {
        foreach(char c in _droppingKeys.Keys)
        {
            _droppingKeys[c].SetPaused(paused);
        }
    }

    public void ApplyDamage(char type)
    {
        int player = _playerIdentifier[_playerIdentifier.Length-1] - '0';
        _selectPlayers.ApplyDamage(player, type);
    }

    public void UpdateDamage(int damage, char type)
    {
        float multiplier = 1f;
        foreach(char c in _key.Keys)
        {
            multiplier += (_key[c].HasSpawned() ? 0.5f : 0f);
        }
        damage = Mathf.CeilToInt(damage * multiplier);

        int player = _playerIdentifier[_playerIdentifier.Length-1] - '0';
        _selectPlayers.UpdateDamage(player, damage, type);
    }

    public void AttackDone(char type)
    {
        _droppingKeys[type].AttackDone();
    }

    public void NewGame()
    {
        foreach(char c in _droppingKeys.Keys)
        {
            _droppingKeys[c].NewGame();
        }

        foreach(char c in _key.Keys)
        {
            _key[c].NewGame();
        }

        _startScreen.NewGame();

        _activations = 1;
        _cycledToActivations = 5;
        _started = false;
        _playerIdentifier = "";
        _yAxisPressed = false;
        countdownTimer = 3f;
        selectedCharacter = false;
    }
}
