using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Splines;

/// <summary>
/// Handles spline-based pathing logic and navigation for enemies.
/// </summary>
public class SplinePath : MonoBehaviour
{
    private SplineContainer splineContainer;
    private KnotLinkCollection knotLinkCollection;

    /// <summary>
    /// Read-only list of splines in the container.
    /// </summary>
    public IReadOnlyList<Spline> Splines { get => splineContainer.Splines; }

    private void Awake()
    {
        splineContainer = GetComponent<SplineContainer>();
        knotLinkCollection = splineContainer.KnotLinkCollection;
    }

    // Calculate world position to avoid issues with scaling
    /// <summary>
    /// Gets the world position of a specific knot in a spline.
    /// </summary>
    public Vector3 GetKnotWorldPosition(int splineIndex, int knotIndex)
    {
        if (splineContainer == null || splineIndex >= splineContainer.Splines.Count)
        {
            return Vector3.zero;
        }

        Vector3 localPosition = splineContainer.Splines[splineIndex][knotIndex].Position;
        return splineContainer.transform.TransformPoint(localPosition);
    }

    /// <summary>
    /// Gets the world position of the very first knot.
    /// </summary>
    public Vector3 GetFirstPosition()
    {
        return GetKnotWorldPosition(0, 0);
    }

    /// <summary>
    /// Selects the next knot to travel to from a list of options using randomness.
    /// </summary>
    public SplineKnotIndex SelectNextNode(List<SplineKnotIndex> options, System.Random rand)
    {
        if (options.Count == 1)
        {
            return options[0];
        }

        // Try and get uniform randomness using reservoir sampling
        int selectedIndex = 0;
        for (int i = 1; i < options.Count; i++)
        {
            if (rand.Next(i + 1) == 0)
            {
                selectedIndex = i;
            }
        }

        return options[selectedIndex];
    }

    private PathState CreatePathState(int splineIndex, int knotIndex)
    {
        Vector3 worldPosition = GetKnotWorldPosition(splineIndex, knotIndex);
        return new PathState(worldPosition, splineIndex, knotIndex);
    }


    /// <summary>
    /// Gets a new path state containing the next waypoint, modified indexes, and original seed
    /// </summary>
    public PathState GetNextPathState(PathState currentState)
    {
        List<SplineKnotIndex> possibleRoutes = GetAvailableRoutes(currentState);
        if (possibleRoutes.Count == 0)
        {
            return null;
        }

        SplineKnotIndex chosen = SelectNextNode(possibleRoutes, currentState.Seed);
        return CreatePathState(chosen.Spline, chosen.Knot);
    }

    /// <summary>
    /// Gets a list of available routes (next direct knot and linked knots).
    /// </summary>
    public List<SplineKnotIndex> GetAvailableRoutes(PathState state)
    {
        int currentSplineIndex = state.CurrentSplineIndex;
        int currentKnotIndex = state.CurrentKnotIndex;

        SplineKnotIndex currentKnot = new(currentSplineIndex, currentKnotIndex);
        List<SplineKnotIndex> routes = new();

        // Add next direct knot
        if (currentKnotIndex < Splines[currentSplineIndex].Count - 1)
        {
            routes.Add(new SplineKnotIndex(currentSplineIndex, currentKnotIndex + 1));
        }

        // Add linked knots
        if (knotLinkCollection.TryGetKnotLinks(currentKnot, out var links))
        {
            foreach (SplineKnotIndex link in links)
            {
                if (!link.Equals(currentKnot))
                {
                    routes.Add(link);
                }
            }
        }

        return routes;
    }
}