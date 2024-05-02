using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEditor.PackageManager;
using UnityEngine;
using UnityEngine.TestTools;

public class RootTesting
{
    [SerializeField] private GlobalUnitList unitList;

    public class GameResourceMock : GameResourceStructure { 
        public GameResourceMock() {
            this.type = GameResources.WINPOINTS;
            this.amount = 0;
        }
    }

    public class PlayerControllerMock : PlayerController
    {
        public PlayerControllerMock()
        {
            this.team = Teams.TEAM_A;
            this.resourcesCount = new GameResourceStructure[1];
            resourcesCount[0] = new GameResourceMock();     
        }
    }

    public class GameManagerMock : GameManager { 
        public GameManagerMock()
        {
            this.localPlayerController = new PlayerControllerMock();
            this.winpointsRequired = 10;
        }

        public GameObject WinState { get => this.winState; set => winState = value; }
        public float WinPoints { get => this.winpointsRequired; }
    }


    // A Test behaves as an ordinary method
    [Test]
    [TestCase(1)]
    [TestCase(10)]
    [TestCase(100)]
    public void RootTestingSimplePasses(float winpoints)
    {
        // Use the Assert class to test conditions
        GameObject gameManagerObject = new GameObject();
        GameObject player1 = new GameObject();
        
        GameManagerMock gameManager = gameManagerObject.AddComponent<GameManagerMock>();      // Mock gameManager
        gameManager.WinState = new GameObject();
        gameManager.WinState.SetActive(false);
        Assert.IsFalse(gameManager.WinState.activeInHierarchy);
        PlayerController player1Controller = player1.AddComponent<PlayerControllerMock>();
        player1Controller.getResource(GameResources.WINPOINTS).Amount = winpoints;

        gameManager.ValidateWin(player1Controller);
        Assert.IsTrue(gameManager.WinState.activeInHierarchy == (winpoints >= gameManager.WinPoints));
    }

    // A UnityTest behaves like a coroutine in Play Mode. In Edit Mode you can use
    // `yield return null;` to skip a frame.
    [UnityTest]
    public IEnumerator RootTestingWithEnumeratorPasses()
    {
        // Use the Assert class to test conditions.
        // Use yield to skip a frame.
        yield return null;
    }
}
