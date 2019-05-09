public static class Rules
{
    private static DetailType[,][] rules = new DetailType[(int)DetailType.length, (int)Direction.length][];

    public static void InitRules()
    {
        //Wheel
        rules[(int)DetailType.Wheel, (int)Direction.Forward] = new DetailType[] 
        { DetailType.Empty };

        rules[(int)DetailType.Wheel, (int)Direction.Back] = new DetailType[] 
        { DetailType.Empty };

        rules[(int)DetailType.Wheel, (int)Direction.Up] = new DetailType[] 
        { DetailType.Empty };

        rules[(int)DetailType.Wheel, (int)Direction.AtSide] = new DetailType[] 
        { DetailType.Wheel, DetailType.Empty };

        //Platform
        rules[(int)DetailType.Platform, (int)Direction.Forward] = new DetailType[] 
        { DetailType.Platform, DetailType.Empty };

        rules[(int)DetailType.Platform, (int)Direction.Back] = new DetailType[] 
        { DetailType.Platform, DetailType.Empty };

        rules[(int)DetailType.Platform, (int)Direction.Up] = new DetailType[] 
        { DetailType.Weapon, DetailType.Box, DetailType.Cabin, DetailType.Empty };

        rules[(int)DetailType.Platform, (int)Direction.AtSide] = new DetailType[] 
        { DetailType.Wheel, DetailType.Platform, DetailType.Empty };

        //Box
        rules[(int)DetailType.Box, (int)Direction.Forward] = new DetailType[]
        { DetailType.Box, DetailType.Empty };

        rules[(int)DetailType.Box, (int)Direction.Back] = new DetailType[]
        { DetailType.Box, DetailType.Empty };

        rules[(int)DetailType.Box, (int)Direction.Up] = new DetailType[]
        { DetailType.Weapon, DetailType.Box, DetailType.Empty };

        rules[(int)DetailType.Box, (int)Direction.AtSide] = new DetailType[]
        { DetailType.Box, DetailType.Empty };

        //Cabin
        rules[(int)DetailType.Cabin, (int)Direction.Forward] = new DetailType[]
        { DetailType.Box, DetailType.Empty };

        rules[(int)DetailType.Cabin, (int)Direction.Back] = new DetailType[]
        { DetailType.Box, DetailType.Empty };

        rules[(int)DetailType.Cabin, (int)Direction.Up] = new DetailType[]
        { DetailType.Weapon, DetailType.Empty };

        rules[(int)DetailType.Cabin, (int)Direction.AtSide] = new DetailType[]
        { DetailType.Empty };

        //Weapon
        rules[(int)DetailType.Weapon, (int)Direction.Forward] = new DetailType[]
        { DetailType.Empty };

        rules[(int)DetailType.Weapon, (int)Direction.Back] = new DetailType[]
        { DetailType.Empty };

        rules[(int)DetailType.Weapon, (int)Direction.Up] = new DetailType[]
        { DetailType.Empty };

        rules[(int)DetailType.Weapon, (int)Direction.AtSide] = new DetailType[]
        { DetailType.Empty };

        //Empty
        rules[(int)DetailType.Empty, (int)Direction.Forward] = new DetailType[0];
        rules[(int)DetailType.Empty, (int)Direction.Back] = new DetailType[0];
        rules[(int)DetailType.Empty, (int)Direction.Up] = new DetailType[0];
        rules[(int)DetailType.Empty, (int)Direction.AtSide] = new DetailType[0];
    }

    public static DetailType[] GetRule(DetailType detailType, Direction direction)
    {
        return rules[(int)detailType, (int)direction];
    }
}