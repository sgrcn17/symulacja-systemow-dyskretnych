using UnityEngine;
using TMPro;

public class SimulationSettings : MonoBehaviour  {
    [SerializeField] private int cellsCount;
    [SerializeField] private int lanesCount;
    [SerializeField] private int agentsCount;
    [SerializeField] private int lightsCount;
    [SerializeField] private float pSpeed;
    [SerializeField] private float pLane;
    [SerializeField] private int lightsDuration;

    [SerializeField] private Simulation simulation;
    
    [SerializeField] private TMP_Text pSpeedText;
    [SerializeField] private TMP_Text pLaneText;

    private void Start() {
        StartSimulation();
    }
    
    public void StartSimulation() {
        simulation.Initialize(cellsCount, lanesCount, agentsCount, lightsCount, pSpeed, pLane, lightsDuration);
        simulation.GenerateSimulation();
    }
    
    public void UpdateCellsCount(string value) {
        cellsCount = int.Parse(value);
    }
    
    public void UpdateLanesCount(string value) {
        lanesCount = int.Parse(value);
    }
    
    public void UpdateAgentsCount(string value) {
        agentsCount = int.Parse(value);
    }
    
    public void UpdateLightsCount(string value) {
        lightsCount = int.Parse(value);
    }
    
    public void UpdatePSpeed(float value) {
        pSpeed = value;
        pSpeedText.text = "p speed: " + value.ToString("0.00");
    }
    
    public void UpdatePLane(float value) {
        pLane = value;
        pLaneText.text = "p lane: " + value.ToString("0.00");
    }
    
    public void UpdateLightsDuration(string value) {
        lightsDuration = int.Parse(value);
    }
}
