using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace EditorTests
{
    public class EditorTestScript
    {
        [Test]
        public void EditorTestScriptSimplePasses()
        {
            try
            {
                var go = new GameObject("TestObject");
                go.SetActive(false);
                Assert.IsFalse(go.activeSelf, "GameObject should be inactive.");
                Debug.Log("EditorTestScriptSimplePasses passed.");
            }
            catch (AssertionException e)
            {
                Debug.LogError($"EditorTestScriptSimplePasses failed: {e.Message}");
                throw;
            }
        }

        [UnityTest]
        public IEnumerator EditorTestScriptWithEnumeratorPasses()
        {
            var go = new GameObject("MovableObject");
            go.transform.position = Vector3.zero;
            Vector3 targetPosition = new Vector3(5, 0, 0);
            go.transform.position = targetPosition;
            yield return null;

            try
            {
                Assert.AreEqual(targetPosition, go.transform.position, "GameObject should move to new position.");
                Debug.Log("EditorTestScriptWithEnumeratorPasses passed.");
            }
            catch (AssertionException e)
            {
                Debug.LogError($"EditorTestScriptWithEnumeratorPasses failed: {e.Message}");
                throw;
            }
        }
    }
}