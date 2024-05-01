using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Linq;

public class Location
{
    //General MAP SIZE
    public const int xSize = 15;
    public const int ySize = 13;

    //Static Array to get Map information
    public static Location[,] map=new Location[xSize, ySize];

    //Fields of each tile
    int X {get;}
	int Y { get; }
	
	private int _moveAP = 2;

    public int X3d {get => X-(Y-(Y&1))/2;}
	public int Y3d {get => Y;}
	public int Z3d {get => -X3d-Y3d;}

    public int AP => _moveAP;
	
	public Location (int x, int y) {
		X=x;
		Y=y;
	}

    public Location (int x, int y, int moveApCost)
    {
        X = x;
        Y = y;
        _moveAP = moveApCost;
    }

    // Start is called before the first frame update
    public static void LoadBattleMap()
    {
        if (map[0, 0] != null)
            return;
        //Fulfilling map
        for (int i =0; i<xSize; i++)
        {
            for (int j=0; j < ySize; j++)
            {
                map[i, j] = new Location(i, j);
            }
        }
    }
    public static Vector2Int Calc3dCoordinates (int x3d, int y3d, int z3d) => new (x3d + (y3d - (y3d&1)) / 2, y3d);

    public static int Distance (Location from, Location to) {

	    return (Mathf.Abs(from.X3d - to.X3d) + Mathf.Abs(from.Y3d - to.Y3d) + Mathf.Abs(from.Z3d - to.Z3d)) / 2;
	
    }

    public static int Distance (int[] from, int [] to)
    {
        int fromX3d = from[0] - (from[1] - (from[1] & 1)) / 2;
        int fromY3d = from[1];
        int fromZ3d = -fromX3d - fromY3d;
        int toX3d = to[0] - (to[1] - (to[1] & 1)) / 2;
        int toY3d = to[1];
        int toZ3d = -toX3d - toY3d;
        return (Mathf.Abs(fromX3d - toX3d) + Mathf.Abs(fromY3d - toY3d) + Mathf.Abs(fromZ3d - toZ3d)) / 2;
    }
    public static Location GetLocation (int[] pos)
    {
        foreach (Location loc in map)
        {
            if (loc.X == pos[0] && loc.Y == pos[1])
                return loc;
        }
        return null;
    }

    public static bool IsBusy(int x, int y) => IsBusy(x, y, GlobalUserInterface.Instance.BattleManager);
    public static bool IsBusy(int x, int y, BattleManager battleManager)
    {
        foreach (CombatUnit cC in battleManager.AllCombatCharacters)
            if (cC.pos[0] == x && cC.pos[1] == y)
                return true;
        return false;
    }

    public static int[] GetSpawnPosition()
    {
        int[] position = new int[2];
        int side;
        do
        {
            side = Random.Range(0, 4);
            switch (side)
            {
                case 0:
                    position[0] = 0;
                    position[1] = Random.Range(0, ySize);
                    break;
                case 1:
                    position[0] = Location.xSize - 1;
                    position[1] = Random.Range(0, ySize);
                    break;
                case 2:
                    position[0] = Random.Range(0, xSize);
                    position[1] = 0;
                    break;
                case 3:
                    position[0] = Random.Range(0, xSize);
                    position[1] = ySize - 1;
                    break;
            }
        } while (IsBusy(position[0], position[1]));
        return position;
    }
}
