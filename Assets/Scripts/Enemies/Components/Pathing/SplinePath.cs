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

    // Originall private
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

    /// <summary>
    /// Gets a list of nearby knots suitable for teleportation, limited by steps and distance.
    /// Prioritizes forward/linked knots to keep teleports predictable yet varied.
    /// </summary>
    /// <param name="currentState">The current path state.</param>
    /// <param name="maxSteps">Max knots ahead to consider (e.g., 3 for short-range).</param>
    /// <param name="maxDistance">Max world-space distance from current position.</param>
    /// <returns>List of valid SplineKnotIndex targets, or empty if none found.</returns>
    public List<SplineKnotIndex> GetNearbyKnotsForTeleport(PathState currentState, int maxSteps, float maxDistance)
    {
        List<SplineKnotIndex> nearbyKnots = new();
        HashSet<SplineKnotIndex> visited = new(); // Avoid duplicates
        Queue<(SplineKnotIndex, int)> toProcess = new(); // BFS-like for step limiting

        // Start from current knot
        SplineKnotIndex currentKnot = new(currentState.CurrentSplineIndex, currentState.CurrentKnotIndex);
        toProcess.Enqueue((currentKnot, 0));
        visited.Add(currentKnot);

        Vector3 currentPosition = currentState.NextWaypoint;

        while (toProcess.Count > 0)
        {
            var (knot, step) = toProcess.Dequeue();
            if (step >= maxSteps) continue; // Enforce step limit

            // Get routes from this knot (direct next and linked)
            var routes = GetAvailableRoutes(new PathState { CurrentSplineIndex = knot.Spline, CurrentKnotIndex = knot.Knot });

            foreach (var route in routes)
            {
                if (visited.Contains(route)) continue;
                visited.Add(route);

                Vector3 routePosition = GetKnotWorldPosition(route.Spline, route.Knot);
                float distance = Vector3.Distance(currentPosition, routePosition);

                // Add if within distance and not the current knot
                if (distance <= maxDistance && distance > 0)
                {
                    nearbyKnots.Add(route);
                }

                // Queue for further exploration if under step limit
                if (step + 1 < maxSteps)
                {
                    toProcess.Enqueue((route, step + 1));
                }
            }
        }

        return nearbyKnots;
    }
}