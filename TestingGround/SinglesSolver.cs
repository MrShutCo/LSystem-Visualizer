using System.Collections;

namespace TestingGround;


public class Board : IEnumerable<(int X,int Y)>
{
    // state:
    //  '\0'  - empty state
    //  'X' - position is coloured
    //  'O' - position is set

    private int[,] Values { get; }
    public char[,] State { get; set; }
    public int N => Values.GetLength(0);

    public Board(int[,] board)
    {
        Values = board;
        State = new char[N, N];
    }

    public int GetValueAt((int, int) pos) => Values[pos.Item1, pos.Item2];
    public char GetStateAt((int, int) pos) => State[pos.Item1, pos.Item2];

    public List<(int, int)> GetNeighbours(int x, int y)
    {
        List<(int, int)> neighs = [];
        if (y != 0) neighs.Add((x, y-1)); // North
        if (y != N-1) neighs.Add((x, y+1)); // South
        if (x != 0) neighs.Add((x-1, y)); // West
        if (x != N-1) neighs.Add((x+1, y)); // East
        return neighs;
    }

    public bool IsConnected()
    {
        var openPositions = this.Where(p => GetStateAt(p) != 'X');
        HashSet<(int, int)> subgraph = [openPositions.First()];
        HashSet<(int, int)> frontier = [openPositions.First()];
        while (frontier.Count > 0)
        {
            var currNode = frontier.First();
            var unvisitedNeighbours = GetNeighbours(currNode.Item1, currNode.Item2)
                .Where(n => GetStateAt(n) != 'X' && !subgraph.Contains(n));
            foreach (var neighbour in unvisitedNeighbours)
            {
                subgraph.Add(neighbour);
                frontier.Add(neighbour);
            }
            frontier.Remove(currNode);
        }

        // If subgraph contains all open positions then we are connected
        return subgraph.Count == openPositions.Count();
    }
    
    public IEnumerator<(int X, int Y)> GetEnumerator()
    {
        for (int i = 0; i < N; i++)
        {
            for (int j = 0; j < N; j++)
            {
                yield return (j, i);
            }
        }
    }

    public List<(int, int)> PlaceX((int X, int Y) pos)
    {
        var neighs = GetNeighbours(pos.X, pos.Y)
            .Where(p => GetStateAt(p) == '\0')
            .ToList();

        neighs.ForEach(n => State[n.Item1, n.Item2] = 'O');
        
        return neighs;
    }

    public IEnumerable<(int X, int Y)> GetOpen() => this.Where(p => GetStateAt(p) == '\0'); 
    
    public List<(int,int)> PropagateO((int X,int Y) pos)
    {
        List<(int, int)> newXs = [];
        // Horizontal propagation
        for (int x = 0; x < N; x++)
        {
            if (State[x, pos.Y] == '\0' && Values[x, pos.Y] == Values[pos.X, pos.Y])
            {
                State[x, pos.Y] = 'X';
                newXs.Add((x,pos.Y));
            }
        }
        
        // Vertical propagation
        for (int y = 0; y < N; y++)
        {
            if (State[pos.X, y] == '\0' && Values[pos.X, y] == Values[pos.X, pos.Y])
            {
                State[pos.X, y] = 'X';
                newXs.Add((pos.X, y));
            }
        }

        return newXs;
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    public override string ToString()
    {
        var s = "";
        for (int i = 0; i < N; i++)
        {
            for (int j = 0; j < N; j++)
            {
                s += State[i,j] == '\0' ? Values[i,j].ToString() : State[i,j];
                s += ",";
            }
            s += "\n";
        }

        return s;
    }
}

public static class SinglesSolver
{
    
    public static void Singles(int[,] boardValues)
    {
        var board = new Board(boardValues);

        var uncheckedOs = GetInitialOs(board);
        var uncheckedXs = new List<(int, int)>();

        while (board.GetOpen().Any())
        {
            // Parse through all the unchecked X and O spots
            while (uncheckedOs.Count > 0 || uncheckedXs.Count > 0)
            {
                if (uncheckedOs.Count > 0)
                {
                    uncheckedXs.AddRange(board.PropagateO(uncheckedOs.First()));
                    uncheckedOs.RemoveAt(0);
                }

                if (uncheckedXs.Count > 0)
                {
                    uncheckedOs.AddRange(board.PlaceX(uncheckedXs.First()));
                    uncheckedXs.RemoveAt(0);
                }
            }
            
            // Validate board connectivity by setting open tile to X and checking if its still connected, if not then set to O
            foreach (var openSpot in board.GetOpen())
            {
                // Try set it to X and see if board is valid
                board.State[openSpot.X, openSpot.Y] = 'X';
                if (!board.IsConnected())
                {
                    board.State[openSpot.X, openSpot.Y] = 'O';
                    uncheckedOs.Add(openSpot);
                }
                // If not, revert change
                else
                {
                    board.State[openSpot.X, openSpot.Y] = '\0';
                }
            }

        }

        Console.WriteLine(board);
    }

    static List<(int, int)> GetInitialOs(Board board)
    {
        List<(int, int)> os = [];
        foreach (var pos in board)
        {
            var neigh = board.GetNeighbours(pos.X, pos.Y);
            // Check NS and EW
            if (neigh.Count == 4)
            {
                if (board.GetValueAt(neigh[0]) == board.GetValueAt(neigh[1]) ||
                    board.GetValueAt(neigh[2]) == board.GetValueAt(neigh[3]))
                {
                    os.Add(pos);
                    board.State[pos.X, pos.Y] = 'O';
                }
            }

            // Either check NS or EW, aka we are at a corner
            if (neigh.Count == 3)
            {
                // Left or right edge, check NS
                if ((pos.X == 0 || pos.X == board.N - 1) && board.GetValueAt(neigh[0]) == board.GetValueAt(neigh[1]))
                {
                    os.Add(pos);
                    board.State[pos.X, pos.Y] = 'O';
                }
                // Top or bottom edge, check EW
                if ((pos.Y == 0 || pos.Y == board.N - 1) && board.GetValueAt(neigh[1]) == board.GetValueAt(neigh[2]))
                {
                    os.Add(pos);
                    board.State[pos.X, pos.Y] = 'O';
                }
            }
        }

        return os;
    }
}