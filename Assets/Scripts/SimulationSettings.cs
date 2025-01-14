using UnityEngine;

public class SimulationSettings : MonoBehaviour  {
    [SerializeField] private int cellsCount;
    [SerializeField] private int lanesCount;
    [SerializeField] private int agentsCount;
    [SerializeField] private int lightsCount;
    [SerializeField] private float pSpeed;
    [SerializeField] private float pLane;
    [SerializeField] private int lightsDuration;

    [SerializeField] private Simulation simulation;

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
    
    public void UpdatePSpeed(string value) {
        pSpeed = float.Parse(value);
    }
    
    public void UpdatePLane(string value) {
        pLane = float.Parse(value);
    }
    
    public void UpdateLightsDuration(string value) {
        lightsDuration = int.Parse(value);
    }
}
