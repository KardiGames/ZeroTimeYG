using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class ExampleTest
{
    // A Test behaves as an ordinary method
    [Test]
    public void ExampleTrueTest()
    {
       Assert.IsTrue(true);
    }

    [UnityTest]
    public IEnumerator ExampleEqualsUnityTest()
    {
        yield return null;
        Assert.AreEqual(1, 1);
    }
}
