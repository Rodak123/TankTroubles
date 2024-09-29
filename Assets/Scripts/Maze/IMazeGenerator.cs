using System.Collections.Generic;

public interface IMazeGenerator
{
    public IEnumerator<Maze> EnumerateGenerateMaze(int width, int height);
}
