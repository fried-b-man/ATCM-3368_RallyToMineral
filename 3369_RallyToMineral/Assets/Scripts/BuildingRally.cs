﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(ClickableUnit))]
public class BuildingRally : MonoBehaviour
{
    private ClickableUnit _building = null;
    private Vector3 _rallyPoint = Vector3.zero;
    public Vector3 RallyPoint { get { return _rallyPoint; } }


    private PlayerInput _gameManager = null;
    private bool _isSelected = false;

    [SerializeField] ParticleSystem _rallyParticles = null;

    private void Awake()
    {
        _building = GetComponent<ClickableUnit>();
        _gameManager = GameObject.Find("GameManager").GetComponent<PlayerInput>();
    }

    private void OnEnable()
    {
        _building.NewLocation += CreateRally;
        _gameManager.OnUnit += OnSelect;
    }

    private void OnDisable()
    {
        _building.NewLocation -= CreateRally;
    }

    void CreateRally(Vector3 location)
    {
        Debug.Log("New Rally at: " + location);
        _rallyParticles.transform.LookAt(location);
    }


    void OnSelect(ClickableUnit unit)
    {
        if (unit != _building)
        {
            _rallyParticles.Stop();
            _rallyParticles.Clear();
            return;
        }
            

        Debug.Log("This Building is Selected");
        _isSelected = true;

        _rallyParticles.Play();

    }
    //while selected, draw line from Transform.Position to RallyPoint
}
