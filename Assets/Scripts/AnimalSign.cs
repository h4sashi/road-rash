using UnityEngine;

[System.Serializable]
public class AnimalSign
{
    public Sprite Sign;
    public string Heading;
    public string Note;
}

[System.Serializable]
public class Nugget
{
    public string Note;
}

[CreateAssetMenu(fileName = "AnimalSigns", menuName = "Animal Signs")]
public class AnimalSignsSO : ScriptableObject
{
    public AnimalSign[] Signs = new AnimalSign[0];
    public Nugget[] Nuggets = new Nugget[0];

    public AnimalSign getSign()
    {
        return Signs[Random.Range(0, Signs.Length)];
    }
}