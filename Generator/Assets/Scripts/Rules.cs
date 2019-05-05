public static class Rules
{
    private static bool[,] rules = new bool[(int)DetailType.length, (int)Direction.length];

    public static void InitRules()
    {
        rules[(int)DetailType.Wheel, (int)Direction.Forward] = false;
        rules[(int)DetailType.Wheel, (int)Direction.Back] = false;
        rules[(int)DetailType.Wheel, (int)Direction.Up] = false;
        rules[(int)DetailType.Wheel, (int)Direction.AtSide] = true;

        rules[(int)DetailType.Platform, (int)Direction.Forward] = true;
        rules[(int)DetailType.Platform, (int)Direction.Back] = true;
        rules[(int)DetailType.Platform, (int)Direction.Up] = true;
        rules[(int)DetailType.Platform, (int)Direction.AtSide] = true;

        rules[(int)DetailType.Box, (int)Direction.Forward] = true;
        rules[(int)DetailType.Box, (int)Direction.Back] = true;
        rules[(int)DetailType.Box, (int)Direction.Up] = true;
        rules[(int)DetailType.Box, (int)Direction.AtSide] = true;

        rules[(int)DetailType.Cabin, (int)Direction.Forward] = true;
        rules[(int)DetailType.Cabin, (int)Direction.Back] = true;
        rules[(int)DetailType.Cabin, (int)Direction.Up] = true;
        rules[(int)DetailType.Cabin, (int)Direction.AtSide] = false;

        rules[(int)DetailType.Weapon, (int)Direction.Forward] = false;
        rules[(int)DetailType.Weapon, (int)Direction.Back] = false;
        rules[(int)DetailType.Weapon, (int)Direction.Up] = false;
        rules[(int)DetailType.Weapon, (int)Direction.AtSide] = false;

        rules[(int)DetailType.Empty, (int)Direction.Forward] = false;
        rules[(int)DetailType.Empty, (int)Direction.Back] = false;
        rules[(int)DetailType.Empty, (int)Direction.Up] = false;
        rules[(int)DetailType.Empty, (int)Direction.AtSide] = false;
    }

    public static bool GetRule(DetailType detailType, Direction direction)
    {
        return rules[(int)detailType, (int)direction];
    }
}