﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourcePatch : MonoBehaviour
{
    [Header("Required")]
    [SerializeField] MineralGroup _group = null;
    private List<MinerController> _miningQueue = new List<MinerController>();
    /* Using LIST instead of QUEUE
     * SCV's can arrive in chaotic times to the resource,
     * Allows removing SCV's at RandomAccess when called away to do other tasks
     */
    public int MiningQueue { get { return _miningQueue.Count; } }
    public bool _activeMining = false;

    private Vector3 _closePoint = Vector3.zero;
    public Vector3 MiningPoint { get { return _closePoint; } }
    private HQController _nearestHQ = null;
    public HQController NearestHQ { get { return _nearestHQ; } }

    [Header("Settings")]
    [SerializeField] int _resourceValue = 5;

    private void Awake()
    {
        CheckForNearHQ();
    }

    private void Start()
    {
        StartCoroutine(PeriodicMiningCheck());
    }

    private void CheckForNearHQ()
    {
        HQController[] allHQ = FindObjectsOfType<HQController>();
        float minDist = Mathf.Infinity;
        foreach (HQController hq in allHQ)
        {
            float dist = (hq.transform.position - transform.position).magnitude;
            if (dist < minDist)
            {
                _nearestHQ = hq; //temporary save, final "closest" becomes final value
                minDist = dist; //set new value to compare
            }
        }

        transform.LookAt(_nearestHQ.transform, Vector3.up);
        _closePoint = transform.position + transform.forward;
    }

    public void AddToQueue(MinerController scv)
    {
        if(_miningQueue.Count <= _group.AverageQueue)   //if we are relatively empty compared to the mineral line
        {
            _miningQueue.Add(scv);
        }
        else
        {
            _group.FindNeighborPatch(scv);
        }
    }

    public void RemoveFromQueue(MinerController scv)
    {
        foreach(MinerController miner in _miningQueue)
        {
            if(miner == scv)
            {
                _miningQueue.Remove(scv);
            }
        }
    }

    IEnumerator PeriodicMiningCheck()
    {
        while (true)
        {
            yield return new WaitForSeconds(0.2f);
            UpdateMiningQueue();
        }
    }

    public void StopMining()
    {
        _activeMining = false;
        UpdateMiningQueue();
    }

    private void UpdateMiningQueue()
    {

        if (_miningQueue.Count <= 0)    //do not update queue if no miners in queue
            return;

        if (_activeMining)  //do not update queue if we are currently mining
            return;

        foreach (MinerController miner in _miningQueue) //find a miner
        {
            if ((miner.transform.position - _closePoint).magnitude > 1f)    //skip miners that are too far
                continue;

            miner.MineMinerals(_resourceValue); //tell that miner to mine
            _activeMining = true;

            break;  //ends foreach, does not end Routine
        }
    }
}
