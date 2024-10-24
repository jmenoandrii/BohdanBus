using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Screamer : MonoBehaviour
{
    [SerializeField]
    private bool _davidScreamerActive = false;
    private bool _davidShowBird = false;

    [SerializeField]
    private AudioSource _audioDavid;
    [SerializeField]
    private AudioSource _audioDavidBird;

    [SerializeField]
    private bool _blackShadowScreamerActive = false;
    [SerializeField]
    private GameObject _blackShadowObject;

    public void ActiveDavidScreamer()
    {
        _davidScreamerActive = true;
        _audioDavid.Play();
    }

    public void ActiveBlackShadow()
    {
        _blackShadowScreamerActive = true;
        _blackShadowObject.SetActive(true);
    }

    public void DeactiveBlackShadow()
    {
        _blackShadowScreamerActive = false;
        _blackShadowObject.SetActive(false);
    }

    private void Update()
    {
        if (_davidScreamerActive)
        {
            if (!_davidShowBird)
            {
                if (_audioDavid.isPlaying && Input.GetKeyDown(KeyCode.X))
                {
                    _davidShowBird = true;
                }
            }
            else if ((_davidShowBird && Input.GetKeyUp(KeyCode.X)) || (!_audioDavid.isPlaying && !_davidShowBird))
            {
                _audioDavidBird.Play();
                _davidScreamerActive = false;
            }
        }

        if (_blackShadowScreamerActive)
        {
            if (Input.GetKeyDown(KeyCode.X))
            {
                _blackShadowObject.SetActive(false);
            }
            else if (Input.GetKeyUp(KeyCode.X))
            {
                _blackShadowObject.SetActive(true);
            }
        }
    }
}
