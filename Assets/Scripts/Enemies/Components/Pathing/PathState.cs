using System.Security.Cryptography;
using System;

/// <summary>
/// Holds the current state of an entity's path along a spline, including position and randomness.
/// </summary>
public class PathState
{
    /// <summary>
    /// The next waypoint the entity should move toward.
    /// </summary>
    public UnityEngine.Vector3 NextWaypoint { get; set; }

    /// <summary>
    /// Index of the current spline segment.
    /// </summary>
    public int CurrentSplineIndex { get; set; }

    /// <summary>
    /// Index of the current knot (waypoint) in the spline.
    /// </summary>
    public int CurrentKnotIndex { get; set; }

    /// <summary>
    /// A seeded random generator for path-related decisions.
    /// </summary>
    public Random Seed { get; }

    // Constructor where all values already known
    public PathState(UnityEngine.Vector3 waypoint, int splineIndex, int knotIndex)
    {
        NextWaypoint = waypoint;
        CurrentSplineIndex = splineIndex;
        CurrentKnotIndex = knotIndex;
        Seed = new Random(GetSeed());
    }

    // Default constructor
    public PathState()
    {
        CurrentSplineIndex = 0;
        CurrentKnotIndex = 0;
        Seed = new Random(GetSeed());
    }

    // Try and get a better seed for better random generation
    private int GetSeed()
    {
        byte[] secureBytes = new byte[4];
        RandomNumberGenerator.Fill(secureBytes);
        int secureRandom = BitConverter.ToInt32(secureBytes, 0);

        Random pseudoRandom = new(Guid.NewGuid().GetHashCode());
        int pseudoRandomNumber = pseudoRandom.Next();

        return secureRandom ^ pseudoRandomNumber;
    }
}
