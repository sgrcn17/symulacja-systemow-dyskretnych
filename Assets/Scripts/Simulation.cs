using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class Simulation : MonoBehaviour {
    [SerializeField] private int cellsCount;
    [SerializeField] private int lanesCount;
    [SerializeField] private int agentsCount;
    [SerializeField] private int lightsCount;
    [SerializeField] private float pSpeed;
    [SerializeField] private float pLane;
    [SerializeField] private int lightsDuration;
    
    [SerializeField] private GameObject cellObject;
    [SerializeField] private GameObject agentObject;
    
    private GameObject[,] _cells;
    private bool[,] _occupied;
    private GameObject[] _agents;
    
    private float _time = 0f;
    
    private void Start() {
        GenerateSimulation();
    }

    private void Update() {
        _time += Time.deltaTime;
        if (_time >= 1.0f) {
            _time -= 1.0f;
            Step();
        }
    }

    private void GenerateSimulation() {
        _occupied = new bool[cellsCount, lanesCount];
        _cells = new GameObject[cellsCount, lanesCount];
        for(int i = 0 ; i < cellsCount ; i++) {
            for(int j = 0 ; j < lanesCount ; j++) {
                _cells[i, j] = Instantiate(cellObject, new Vector3(i, 0, j), Quaternion.identity);
                _cells[i, j].transform.SetParent(this.transform);
            }
        }
        
        for (int i = 0; i < lightsCount; i++) {
            int lightCell = Random.Range(0, cellsCount);
            int lightLane = Random.Range(0, lanesCount);
            var cellComponent = _cells[lightCell, lightLane].GetComponent<Cell>();
            cellComponent.Initialization(lightsDuration, Random.value > 0.5f, true);
        }
        
        _agents = new GameObject[agentsCount];
        for(int i = 0 ; i < agentsCount ; i++) {
            int sCell = Random.Range(0, cellsCount);
            int sLane = Random.Range(0, lanesCount);
            int maxSpeed = Random.Range(3, 5);
            _agents[i] = Instantiate(agentObject, new Vector3(sCell, 0, sLane), Quaternion.identity);
            _agents[i].transform.SetParent(this.transform);
            
            var agentComponent = _agents[i].GetComponent<Agent>();
            agentComponent.Initialize(sCell, sLane, maxSpeed);
        }
    }

    private void Step() {
        for(int i = 0 ; i < cellsCount ; i++) {
            for(int j = 0 ; j < lanesCount ; j++) {
                _occupied[i, j] = false;
            }
        }
        
        foreach (var agent in _agents) {
            agent.TryGetComponent<Agent>(out var agentComponent);
            _occupied[agentComponent.currentCell, agentComponent.currentLane] = true;

            agentComponent.nextLane = agentComponent.currentLane;
        }
        
        for(int i = 0 ; i < cellsCount ; i++) {
            for(int j = 0 ; j < lanesCount ; j++) {
                _cells[i, j].TryGetComponent<Cell>(out var cellComponent);
                cellComponent.UpdateLight();
            }
        }

        foreach (var agent in _agents) {
            agent.TryGetComponent<Agent>(out var agentComponent);

            TryChangeLane(agentComponent);
            UpdateAgentSpeed(agentComponent);
            UpdateOccupiedCells(agentComponent);
        }

        foreach (var agent in _agents) {
            agent.TryGetComponent<Agent>(out var agentComponent);
            _occupied[agentComponent.currentCell, agentComponent.currentLane] = false;

            bool stopAtLight = false;
            for (int i = 1; i <= agentComponent.currentSpeed; i++) {
                int nextCell = agentComponent.currentCell + i;
                if (nextCell < cellsCount) {
                    _cells[nextCell, agentComponent.currentLane].TryGetComponent<Cell>(out var cellComponent);
                    if (cellComponent.isLight && !cellComponent.isGreen) {
                        agentComponent.currentSpeed = i - 1;
                        stopAtLight = true;
                        break;
                    }
                }
            }

            if (!stopAtLight) {
                int newCell = agentComponent.currentCell + agentComponent.currentSpeed;
                if (newCell >= cellsCount-1) {
                    agentComponent.ResetPosition();
                } else {
                    agentComponent.MoveTo(newCell, agentComponent.nextLane);
                }
            } else {
                agentComponent.MoveTo(agentComponent.currentCell, agentComponent.currentLane);
            }
        }
    }
    
    private void UpdateOccupiedCells(Agent agent) {
        for (int i = agent.currentCell + 1; i <= agent.currentCell + agent.currentSpeed; i++) {
            _occupied[i, agent.nextLane] = true;
        }
    }
    
    private void TryChangeLane(Agent agent) {
        float randomChance = Random.Range(0f, 1f);
        if (randomChance > pLane) return;

        randomChance = Random.Range(0f, 1f);
        if (randomChance < 0.5) {
            if (agent.currentLane > 0 && CanChangeLane(agent, agent.currentLane - 1)) {
                agent.nextLane = agent.currentLane - 1; // Change to left lane
            }
            else if (agent.currentLane < lanesCount - 1 && CanChangeLane(agent, agent.currentLane + 1)) {
                agent.nextLane = agent.currentLane + 1; // Change to right lane
            }
        } else {
            if (agent.currentLane < lanesCount - 1 && CanChangeLane(agent, agent.currentLane + 1)) {
                agent.nextLane = agent.currentLane + 1; // Change to right lane
            }
            else if (agent.currentLane > 0 && CanChangeLane(agent, agent.currentLane - 1)) {
                agent.nextLane = agent.currentLane - 1; // Change to left lane
            }
        }
    }

    private bool CanChangeLane(Agent agent, int targetLane) {
        for (int i = agent.currentCell; i <= agent.currentCell + agent.currentSpeed; i++) {
            if (i >= cellsCount || _occupied[i, targetLane]) {
                return false;
            }
        }
        return true;
    }
    private void UpdateAgentSpeed(Agent agent) {
        float randomChance = Random.Range(0f, 1f);

        int emptyCells = 0;
        for(int i = agent.currentCell + 1 ; i < cellsCount ; i++) {
            if (_occupied[i,agent.nextLane]) break;
            emptyCells++;
        }
        
        agent.currentSpeed++;
        agent.currentSpeed = Mathf.Min(agent.currentSpeed, agent.maxSpeed);
        agent.currentSpeed = Math.Min(agent.currentSpeed, emptyCells);
        
        if (randomChance < pSpeed) {
            agent.currentSpeed = Mathf.Max(agent.currentSpeed - 1, 0);
        }
    }
}
