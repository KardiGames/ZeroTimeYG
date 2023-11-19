
public interface IWorldBuilding
{
    public int X { get; }

    public int Y { get; }
    public string Name { get; }

    public string ToJson();
    public void FromJson(string jsonString);

    public void ExitBuilding();
}

