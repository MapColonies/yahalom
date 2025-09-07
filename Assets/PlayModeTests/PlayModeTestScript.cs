using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace PlayModeTests
{
    public class PlayModeTestScript
    {
        [Test]
        public void GameObject_IsCreatedWithCorrectName()
        {
            try
            {
                var expectedName = "TestObject";
                var go = new GameObject(expectedName);
                Assert.AreEqual(expectedName, go.name);
                Debug.Log("GameObject_IsCreatedWithCorrectName passed.");
            }
            catch (AssertionException e)
            {
                Debug.LogError($"GameObject_IsCreatedWithCorrectName failed: {e.Message}");
                throw;
            }
        }

        [UnityTest]
        public IEnumerator GameObject_MovesAfterOneFrame()
        {
            var go = new GameObject("MovingObject");
            go.transform.position = Vector3.zero;
            var targetPosition = new Vector3(0, 5, 0);
            go.transform.position = targetPosition;
            yield return null;

            try
            {
                Assert.AreEqual(targetPosition, go.transform.position);
                Debug.Log("GameObject_MovesAfterOneFrame passed.");
            }
            catch (AssertionException e)
            {
                Debug.LogError($"GameObject_MovesAfterOneFrame failed: {e.Message}");
                throw;
            }
        }

        [UnityTest]
        public IEnumerator GameObject_WithRigidbody_FallsWithGravity()
        {
            var go = GameObject.CreatePrimitive(PrimitiveType.Cube);
            var rb = go.AddComponent<Rigidbody>();
            go.transform.position = new Vector3(0, 10, 0);

            yield return new WaitForSeconds(0.5f);

            try
            {
                Assert.Less(go.transform.position.y, 10f, "Rigidbody should have fallen due to gravity.");
                Debug.Log("GameObject_WithRigidbody_FallsWithGravity passed.");
            }
            catch (AssertionException e)
            {
                Debug.LogError($"GameObject_WithRigidbody_FallsWithGravity failed: {e.Message}");
                throw;
            }
        }
    }
}
