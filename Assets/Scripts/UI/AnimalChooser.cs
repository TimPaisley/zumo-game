using UnityEngine;
using System.Collections;

public class AnimalChooser : MonoBehaviour {
    public struct ChoosableAnimal {
        public AnimalController animal;
        public bool isChosen;
    }

    public int animalColumnCount = 2;
    
    private ChoosableAnimal[] animals;
    private PlayerController player;
    private AxisInput xAxisInput;
    private AxisInput yAxisInput;

    private int currentX = -1;
    private int currentY = -1;

    public void Initialize(ChoosableAnimal[] animals, PlayerController player) {
        this.animals = animals;
        this.player = player;
        xAxisInput = new AxisInput(player.input.xAxis);
        yAxisInput = new AxisInput(player.input.yAxis);
    }

	void Start () {
    }
	
	void Update () {
	}
}
