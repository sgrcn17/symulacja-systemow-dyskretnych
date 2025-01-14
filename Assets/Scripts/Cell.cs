using UnityEngine;

public class Cell : MonoBehaviour {

    public int duration;
    public bool isGreen = false;
    public bool isLight = false;

    [SerializeField] private Transform model;
    
    private int _time = 0;

    public void Initialization(int _duration, bool _isGreen, bool _isLight) {
        duration = _duration;
        isGreen = _isGreen;
        isLight = _isLight;
    }

    public void UpdateLight() {
        if (isLight) {
            _time++;
            if (_time >= duration) {
                _time -= duration;
                isGreen = !isGreen;
            }
            UpdateCellMaterial();
        }
    }

    private void UpdateCellMaterial() {
        model.TryGetComponent<Renderer>(out var renderer);
        if (isGreen) {
            renderer.material.color = Color.green;
        } else {
            renderer.material.color = Color.red;
        }
        
    }
}
