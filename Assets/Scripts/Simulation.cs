using System.Collections.Generic;
using UnityEngine;

public class Simulation : MonoBehaviour {
    public int cellsCount;
    public int lanesCount;
    public int agentsCount;
    public int lightsCount;
    public float pSpeed;
    public float pLane;
    public int lightsDuration;

    [SerializeField] private GameObject cellObject;
    [SerializeField] private GameObject agentObject;

    private GameObject[,] _cells;
    private bool[,] _occupied;
    private List<GameObject> _agents;

    private float _time = 0f;

    public void Initialize(int _cellsCount, int _lanesCount, int _agentsCount, int _lightsCount, float _pSpeed, float _pLane, int _lightsDuration) {
        cellsCount = _cellsCount;
        lanesCount = _lanesCount;
        agentsCount = _agentsCount;
        lightsCount = _lightsCount;
        pSpeed = _pSpeed;
        pLane = _pLane;
        lightsDuration = _lightsDuration;
    }

    private void Update() {
        _time += Time.deltaTime;
        if (_time >= 1.0f) {
            _time -= 1.0f;
            Step();
        }
    }

    public void GenerateSimulation() {
        _time = 0;
        InitializeCells();
        InitializeLights();
        InitializeAgents();
    }

    private void InitializeCells() {
        if (_cells != null) {
            foreach (var cell in _cells) {
                Destroy(cell);
            }
        }

        _occupied = new bool[cellsCount, lanesCount];
        _cells = new GameObject[cellsCount, lanesCount];
        for (int i = 0; i < cellsCount; i++) {
            for (int j = 0; j < lanesCount; j++) {
                _cells[i, j] = Instantiate(cellObject, new Vector3(i, 0, j), Quaternion.identity);
                _cells[i, j].transform.SetParent(this.transform);
            }
        }
    }

    private void InitializeLights() {
        for (int i = 0; i < lightsCount; i++) {
            int lightCell = Random.Range(0, cellsCount);
            int lightLane = Random.Range(0, lanesCount);
            var cellComponent = _cells[lightCell, lightLane].GetComponent<Cell>();
            cellComponent.Initialization(lightsDuration, Random.value > 0.5f, true);
        }
    }

    private void InitializeAgents() {
        if (_agents != null) {
            foreach (var agent in _agents) {
                Destroy(agent);
            }
        }

        _agents = new List<GameObject>();
        for (int i = 0; i < agentsCount; i++) {
            CreateNewAgent(true);
        }
    }

    private void CreateNewAgent(bool initial = false) {
        int sLane = Random.Range(0, lanesCount);
        int sCell = initial ? Random.Range(0, cellsCount) : 0;
        int maxSpeed = Random.Range(3, 5);
        var newAgent = Instantiate(agentObject, new Vector3(sCell, 0, sLane), Quaternion.identity);
        newAgent.transform.SetParent(this.transform);

        newAgent.TryGetComponent<Agent>(out var agentComponent);
        agentComponent.Initialize(sCell, sLane, maxSpeed);

        _agents.Add(newAgent);
    }

    private void Step() {
        ResetOccupiedCells();
        UpdateLights();
        UpdateAgents();
        RemoveAndCreateAgents();
    }

    private void ResetOccupiedCells() {
        for (int i = 0; i < cellsCount; i++) {
            for (int j = 0; j < lanesCount; j++) {
                _occupied[i, j] = false;
            }
        }
    }

    private void UpdateOccupiedCells(Agent agentComponent) {
        _occupied[agentComponent.currentCell, agentComponent.currentLane] = true;
        MarkLaneChange(agentComponent, agentComponent.currentLane, agentComponent.nextLane);
    }

    private void UpdateLights() {
        for (int i = 0; i < cellsCount; i++) {
            for (int j = 0; j < lanesCount; j++) {
                _cells[i, j].TryGetComponent<Cell>(out var cellComponent);
                if (cellComponent is not null && cellComponent.isLight) {
                    cellComponent.UpdateLight();
                }
            }
        }
    }

    private void UpdateAgents() {
        foreach (var agent in _agents) {
            agent.TryGetComponent<Agent>(out var agentComponent);
            TryChangeLane(agentComponent);
            UpdateAgentSpeed(agentComponent);

            if (agentComponent.currentSpeed == 0) {
                agentComponent.nextLane = agentComponent.currentLane;
            }

            UpdateOccupiedCells(agentComponent);

            agentComponent.MoveTo(agentComponent.currentCell + agentComponent.currentSpeed, agentComponent.nextLane);
        }
    }

    private void TryChangeLane(Agent agent) {
        if (Random.Range(0f, 1f) > pLane) return;

        int targetLane = agent.currentLane;
        if (Random.Range(0f, 1f) < 0.5) {
            if (agent.currentLane > 0 && CanChangeLane(agent, agent.currentLane - 1)) {
                targetLane = agent.currentLane - 1;
            } else if (agent.currentLane < lanesCount - 1 && CanChangeLane(agent, agent.currentLane + 1)) {
                targetLane = agent.currentLane + 1;
            }
        } else {
            if (agent.currentLane < lanesCount - 1 && CanChangeLane(agent, agent.currentLane + 1)) {
                targetLane = agent.currentLane + 1;
            } else if (agent.currentLane > 0 && CanChangeLane(agent, agent.currentLane - 1)) {
                targetLane = agent.currentLane - 1;
            }
        }
        agent.nextLane = targetLane;
    }

    private bool CanChangeLane(Agent agent, int targetLane) {
        for (int i = agent.currentCell; i <= agent.currentCell + agent.currentSpeed; i++) {
            if (i >= cellsCount || _occupied[i, targetLane]) {
                return false;
            }
        }
        return true;
    }

    private void MarkLaneChange(Agent agent, int currentLane, int targetLane) {
        for (int i = agent.currentCell; i <= agent.currentCell + agent.currentSpeed / 2; i++) {
            if (i >= cellsCount) break;
            _occupied[i, currentLane] = true;
        }
        for (int i = agent.currentCell + agent.currentSpeed / 2; i <= agent.currentCell + agent.currentSpeed; i++) {
            if (i >= cellsCount) break;
            _occupied[i, targetLane] = true;
        }
    }

    private void UpdateAgentSpeed(Agent agent) {
        int emptyCells = 0;
        for (int i = agent.currentCell + 1; i < cellsCount; i++) {
            if (_occupied[i, agent.nextLane]) break;
            emptyCells++;
        }

        agent.currentSpeed = Mathf.Min(agent.currentSpeed + 1, agent.maxSpeed);
        agent.currentSpeed = Mathf.Min(agent.currentSpeed, emptyCells);

        if (Random.Range(0f, 1f) < pSpeed) {
            agent.currentSpeed = Mathf.Max(agent.currentSpeed - 1, 0);
        }
        for (int i = 1; i <= agent.currentSpeed; i++) {
            int nextCell = agent.currentCell + i;
            if (nextCell < cellsCount) {
                _cells[nextCell, agent.nextLane].TryGetComponent<Cell>(out var cellComponent);
                if (cellComponent.isLight && !cellComponent.isGreen) {
                    agent.currentSpeed = Mathf.Min(agent.currentSpeed, i - 1);
                    break;
                }
            }
        }
    }

    private void RemoveAndCreateAgents() {
        for (int i = _agents.Count - 1; i >= 0; i--) {
            var agent = _agents[i];
            agent.TryGetComponent<Agent>(out var agentComponent);
            _occupied[agentComponent.currentCell, agentComponent.currentLane] = false;

            int newCell = agentComponent.currentCell + agentComponent.currentSpeed;
            if (newCell >= cellsCount - 1) {
                Destroy(agent);
                _agents.RemoveAt(i);
                CreateNewAgent();
            } else if (!_occupied[newCell, agentComponent.nextLane]) {
                agentComponent.MoveTo(newCell, agentComponent.nextLane);
            } else {
                agentComponent.MoveTo(agentComponent.currentCell, agentComponent.currentLane);
            }
        }
    }
}