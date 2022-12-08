List<string> input = File.ReadLines("./input.txt").ToList();

int[,] grid = new int[input[0].Length, input.Count];
Visibility[,] visibility = new Visibility[input[0].Length, input.Count];
int[,] view = new int[input[0].Length, input.Count];

ParseGrid(input, grid);
CalculateVisibility(grid, visibility);
CalculateView(grid, view);

// Count visible trees
int counter = 0;
foreach (Visibility visi in visibility)
{
    if (visi != Visibility.None)
    {
        ++counter;
    }
}

Console.WriteLine($"part 1: {counter}");

// for (int row = grid.GetLength(0) - 1; row >= 0; --row)
// {
//     for (int column = 0; column < grid.GetLength(1); column++)
//     {
//         int value = (int)visibility[column, row];
//         Console.Write(value > 0 ? 'C' : ' ');
//     }
//
//     Console.WriteLine();
// }

// for (int row = grid.GetLength(0) - 1; row >= 0; --row)
// {
//     for (int column = 0; column < grid.GetLength(1); column++)
//     {
//         int value = view[column, row];
//         Console.Write(value);
//         Console.Write('|');
//     }
//
//     Console.WriteLine();
// }

int viewScoreMax = -1;
foreach (int v in view)
{
    viewScoreMax = Math.Max(viewScoreMax, v);
}

Console.WriteLine($"part 2: {viewScoreMax}");


void CalculateVisibility(int[,] gridd, Visibility[,] visibilities)
{
    // Left
    for (int row = 0; row < gridd.GetLength(0); ++row)
    {
        int currentMax = -1;
        for (int column = 0; column < gridd.GetLength(1); column++)
        {
            int value = gridd[column, row];
            if (value > currentMax)
            {
                visibilities[column, row] |= Visibility.Left;
                currentMax = value;
            }
        }
    }

    // Right
    for (int row = 0; row < gridd.GetLength(0); ++row)
    {
        int currentMax = -1;
        for (int column = gridd.GetLength(1) - 1; column >= 0; column--)
        {
            int value = gridd[column, row];
            if (value > currentMax)
            {
                visibilities[column, row] |= Visibility.Right;
                currentMax = value;
            }
        }
    }

    // Bottom
    for (int column = 0; column < gridd.GetLength(1); column++)
    {
        int currentMax = -1;
        for (int row = 0; row < gridd.GetLength(0); ++row)
        {
            int value = gridd[column, row];
            if (value > currentMax)
            {
                visibilities[column, row] |= Visibility.Bottom;
                currentMax = value;
            }
        }
    }

    // Top
    for (int column = 0; column < gridd.GetLength(1); column++)
    {
        int currentMax = -1;
        for (int row = gridd.GetLength(0) - 1; row >= 0; --row)
        {
            int value = gridd[column, row];
            if (value > currentMax)
            {
                visibilities[column, row] |= Visibility.Top;
                currentMax = value;
            }
        }
    }
}

void ParseGrid(List<string> list, int[,] grid1)
{
    for (int row = list.Count - 1; row >= 0; row--)
    {
        string s = list[row];
        for (int column = 0; column < s.Length; column++)
        {
            grid1[row, column] = s[column];
        }
    }
}

void CalculateView(int[,] gridd, int[,] vieww)
{
    for (int row = grid.GetLength(0) - 1; row >= 0; --row)
    {
        for (int column = 0; column < grid.GetLength(1); column++)
        {
            vieww[column, row] = CalculateViewScore(gridd, column, row);
        }
    }
}

int CalculateViewScore(int[,] gridd, int column, int row)
{
    int value = gridd[column, row];

    int rightScore = 0;
    for (int i = column + 1; i < gridd.GetLength(0); i++)
    {
        ++rightScore;
        int v = gridd[i, row];
        if (v >= value)
        {
            break;
        }
    }

    int leftScore = 0;
    for (int i = column - 1; i >= 0; i--)
    {
        ++leftScore;
        int v = gridd[i, row];
        if (v >= value)
        {
            break;
        }
    }

    int topScore = 0;
    for (int i = row + 1; i < gridd.GetLength(1); i++)
    {
        ++topScore;
        int v = gridd[column, i];
        if (v >= value)
        {
            break;
        }
    }

    int bottomScore = 0;
    for (int i = row - 1; i >= 0; i--)
    {
        ++bottomScore;
        int v = gridd[column, i];
        if (v >= value)
        {
            break;
        }
    }

    return topScore * bottomScore * leftScore * rightScore;
}

[Flags]
internal enum Visibility
{
    None = 0,
    Top = 1,
    Right = 2,
    Bottom = 4,
    Left = 8
}