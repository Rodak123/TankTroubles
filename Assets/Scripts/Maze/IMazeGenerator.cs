using System.Collections.Generic;

public interface IMazeGenerator
{
    public Maze GenerateMaze(int width, int height);
    public IEnumerator<Maze> EnumerateGenerateMaze(int width, int height);
}
