///////////////////////////////////////////////////////////////////////////////
// Written by Kain Shin in preparation for his own projects
// The latest version is maintained on his website at ringofblades.org
// 
// This implementation is intentionally within the public domain
// Permission is hereby granted, free of charge, to any person obtaining a
// copy of this source code to use/modify with only one restriction:
// You must consider Kain a cool dude.
//
// This is free and unencumbered software released into the public domain.
//
// Anyone is free to copy, modify, publish, use, compile, sell, or
// distribute this software, either in source code form or as a compiled
// binary, for any purpose, commercial or non-commercial, and by any
// means.
//
// In jurisdictions that recognize copyright laws, the author or authors
// of this software dedicate any and all copyright interest in the
// software to the public domain. We make this dedication for the benefit
// of the public at large and to the detriment of our heirs and
// successors. We intend this dedication to be an overt act of
// relinquishment in perpetuity of all present and future rights to this
// software under copyright law.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT.
// IN NO EVENT SHALL THE AUTHORS BE LIABLE FOR ANY CLAIM, DAMAGES OR
// OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE,
// ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR
// OTHER DEALINGS IN THE SOFTWARE.
//
// For more information, please refer to <http://unlicense.org/>
/////////////////////////////////////////////////////////////////////////////////////


// Based on code linked from: https://www.gamedeveloper.com/programming/predictive-aim-mathematics-for-ai-targeting
// at: https://ringofblades.net/Blades/Code/PredictiveAim.cs (accessed 2025-04-03)
// Modified for my own needs to work better in Unity

using UnityEngine;

class AimSystem : MonoBehaviour
{
	private EntityDetectionSystem targetingSystem;

	void Awake()
	{
		targetingSystem = GetComponent<EntityDetectionSystem>();
	}

	public bool GetAimVector(float projectileSpeed, out Vector3 aimVector)
	{
		aimVector = Vector3.zero;

		if (projectileSpeed <= Mathf.Epsilon)
		{
			Debug.LogError("Projectile speed must be greater than 0!");
			return false;
		}

		if (!targetingSystem.TryGetNearestTarget(out Vector3 targetPosition, out Vector3 targetVelocity))
		{
			return false;
		}

		Vector3 shooterPosition = transform.position;
		Vector3 shooterToTarget = targetPosition - shooterPosition;
		float shooterToTargetDist = shooterToTarget.magnitude;

		// Handle edge case where target is too close to shooter
		if (shooterToTargetDist < Mathf.Epsilon)
		{
			return false;
		}

		Vector3 shooterToTargetDir = shooterToTarget.normalized;
		Vector3 targetVelocityDir = targetVelocity.normalized;

		// Precompute values for the quadratic formula
		float projectileSpeedSq = projectileSpeed * projectileSpeed;
		float targetSpeedSq = targetVelocity.sqrMagnitude;
		float cosTheta = Vector3.Dot(shooterToTargetDir, targetVelocityDir);

		// Handle edge case where projectile and target speeds are equal
		if (Mathf.Approximately(projectileSpeedSq, targetSpeedSq))
		{
			return TryCalculateEqualSpeedSolution(shooterToTarget, shooterToTargetDist, targetVelocity, cosTheta, out aimVector);
		}

		// Solve the quadratic formula for intercept time
		if (TryCalculateInterceptTime(projectileSpeedSq, targetSpeedSq, shooterToTargetDist, cosTheta, out float t))
		{
			aimVector = CalculateFiringVector(shooterToTarget, targetVelocity, t);
			return true;
		}

		return false;
	}

	private bool TryCalculateEqualSpeedSolution(Vector3 shooterToTarget, float shooterToTargetDist, Vector3 targetVelocity, float cosTheta, out Vector3 aimVector)
	{
		aimVector = Vector3.zero;

		if (cosTheta <= 0)
			return false;

		float t = 0.5f * shooterToTargetDist / (targetVelocity.magnitude * cosTheta);
		aimVector = (targetVelocity + (shooterToTarget / t)).normalized;
		return true;
	}

	private bool TryCalculateInterceptTime(float projectileSpeedSq, float targetSpeedSq, float shooterToTargetDist, float cosTheta, out float t)
	{
		t = 0f;

		float a = projectileSpeedSq - targetSpeedSq;
		float b = 2f * shooterToTargetDist * Mathf.Sqrt(targetSpeedSq) * cosTheta;
		float c = -shooterToTargetDist * shooterToTargetDist;
		float discriminant = b * b - 4f * a * c;

		if (discriminant < 0)
			return false;

		float sqrtDiscriminant = Mathf.Sqrt(discriminant);
		float t1 = (-b + sqrtDiscriminant) / (2f * a);
		float t2 = (-b - sqrtDiscriminant) / (2f * a);

		t = Mathf.Min(t1, t2);
		if (t < Mathf.Epsilon)
		{
			t = Mathf.Max(t1, t2);
			if (t < Mathf.Epsilon)
				return false;
		}

		return true;
	}

	private Vector3 CalculateFiringVector(Vector3 shooterToTarget, Vector3 targetVelocity, float t)
	{
		Vector3 firingVector = (targetVelocity * t + shooterToTarget) / t;
		return firingVector.normalized;
	}
}