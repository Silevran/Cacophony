using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;

public class SpawnedObject : MonoBehaviour 
{
    [SerializeField]
    private Image _border = null;

    [SerializeField]
    private AudioSource _source = null;
    [SerializeField]
    private List<AudioClip> _personalSound = new List<AudioClip>();
    public int soundIndex = 0;

    public SpawnKey spawner = null;

    private KeyboardControl _inputController = null;
    private Vector3 _startPoint = Vector3.zero;

    private float _moveRate = 1.5f;

    [SerializeField]
    private float speedupPerPass = 0f;

    private float _pauseTime = 1f;
    private bool _delayed = false;
    private bool _paused = false;
    private bool _attacking = false;

    public char type = '0';

    private int _activations = 1;
    private float _activationTime = 100f;
    private float _timer = 0f;

    private List<SelectArea> _currentArea = new List<SelectArea>();

    private int _damage = 0;
    [SerializeField]
    private int _damageBase = 0;
    private bool _cycle = false;

    public void SetController(KeyboardControl cont)
    {
        _inputController = cont;
        _inputController.RegisterObject(this, type);
    }

    public void SetStartPoint(Vector3 start)
    {
        _startPoint = start;
    }

    public void PlaySound()
    {
        _source.PlayOneShot(_personalSound[soundIndex]);
    }

    public void SetActivationTime(float time)
    {
        _activationTime = time;
    }
	
	// Update is called once per frame
	void Update () 
    {
        if (_paused || _attacking)
        {
            return;
        }

        if (_delayed)
        {
            _pauseTime -= Time.deltaTime;
            if(_pauseTime < 0f)
            {
                _pauseTime = 1f;
                _delayed = false;
            }
        }
        else
        {
            Vector3 pos = transform.localPosition;
            pos.y -= _moveRate;
            transform.localPosition = pos;
        }

        _timer += Time.deltaTime;
        if(_timer > _activationTime)
        {
            _timer = 0;
            _activations++;
        }

        spawner.UpdateActivationInfo(_timer / _activationTime, _activations);
	}

    void OnTriggerEnter2D(Collider2D collide)
    {
        if (collide.gameObject.name == "Destroyer")
        {
            if (_inputController.CheckComplete(type))
            {
                transform.position = _startPoint;
                _inputController.ResetAreas(type);

                _moveRate += speedupPerPass;

                if (_damage > 0)
                {
                    _inputController.ApplyDamage(type);
                    _attacking = true;
                }

                _cycle = true;

                _damage = 0;
            }
            else
            {
                _inputController.MissedAction(type);
            }
        }
        else
        {
            _currentArea.Add(collide.gameObject.GetComponent<SelectArea>());
            if (_currentArea[_currentArea.Count-1] != null)
            {
                if (_currentArea[_currentArea.Count - 1].buttonType == type)
                {
                    _border.enabled = true;
                }
            }
            else
            {
                _currentArea.RemoveAt(_currentArea.Count - 1);
            }
        }
    }

    void OnTriggerExit2D(Collider2D collide)
    {
        _border.enabled = false;

        for (int i = 0; i < _currentArea.Count; i++)
        {
            if(_currentArea[i].gameObject == collide.gameObject)
            {
                _currentArea.RemoveAt(i);
                break;
            }
        }
    }

    public bool OnButtonPressed()
    {
        if (_currentArea.Count <= 0 && _activations > 0)
        {
            _activations--;
            return false;
        }
        else if(_currentArea.Count <= 0)
        {
            return true;
        }

        bool returnValue = false;
        for (int i = 0; i < _currentArea.Count; i++)
        {
            if (!_currentArea[i].wasHit)
            {
                _currentArea[i].wasHit = true;
                _source.PlayOneShot(_personalSound[soundIndex]);
                _damage += (_cycle ? _damageBase : 1);

                _inputController.UpdateDamage(_damage, type);
                returnValue = true;
                _currentArea.RemoveAt(i);

                i--;
                if(_currentArea.Count == 0)
                {
                    return returnValue;
                }
            }
            else if (_activations > 0)
            {
                _activations--;
            }
            else
            {
                _damage = 0;
                returnValue = true;
            }

            _border.enabled = false;

            if(returnValue)
            {
                break;
            }
        }

        return returnValue;
    }

    public void SetMoveSpeed(float speed)
    {
        _moveRate = speed;
    }

    public bool CanActivate()
    {
        if(_activations > 0)
        {
            return true;
        }
        return false;
    }

    public void CauseRespawn()
    {
        spawner.Respawn();
        _inputController.RemoveObject(type);
        _damage = 0;
        _cycle = false;
        GameObject.Destroy(gameObject);
    }

    public void CauseReset()
    {
        transform.position = _startPoint;
        _inputController.ResetAreas(type);
        _delayed = true;
        _damage = 0;
        _cycle = false;
    }

    public void SetPaused(bool paused)
    {
        _paused = paused;
    }

    public void AttackDone()
    {
        _attacking = false;
    }

    public void NewGame()
    {
        GameObject.Destroy(gameObject);
    }
}
