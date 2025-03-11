using UnityEngine;

[CreateAssetMenu(fileName = "AnimalSigns", menuName = "Animal Signs")]
public class AnimalSignsSO : ScriptableObject
{
    public AnimalSign[] Signs = new AnimalSign[0];
    public AnimalSign getSign()
    {
        return Signs[Random.Range(0, Signs.Length)];
    }
}