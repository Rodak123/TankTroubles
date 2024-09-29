using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapGenerationAnimator : MonoBehaviour
{
    private GameObject mazeWalls;

    private bool forceStopAnimation;

    public void AnimateMazeGeneration(IEnumerator<Maze> mazeEnumerator, float stepDelay, Func<Maze, GameObject> generateMazeWalls, Action onAnimationFinished)
    {
        StartCoroutine(AnimateMaze(mazeEnumerator, stepDelay, generateMazeWalls, onAnimationFinished));
    }

    private IEnumerator AnimateMaze(IEnumerator<Maze> mazeEnumerator, float stepDelay, Func<Maze, GameObject> generateMazeWalls, Action onAnimationFinished)
    {
        forceStopAnimation = false;
        while (mazeEnumerator.MoveNext())
        {
            if (mazeWalls != null) Destroy(mazeWalls);
            if (forceStopAnimation) break;
            mazeWalls = generateMazeWalls(mazeEnumerator.Current);
            mazeWalls.transform.parent = transform;
            yield return new WaitForSeconds(stepDelay);
        }
        if (!forceStopAnimation)
            onAnimationFinished?.Invoke();
    }

    public void DestroySelf()
    {
        forceStopAnimation = false;
        Destroy(mazeWalls);
        Destroy(gameObject);
    }

}
