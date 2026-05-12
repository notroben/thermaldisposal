using UnityEngine;

public static class ServiceLocator
{
    private static GameManager _gameManager;
    public static GameManager GameManager => _gameManager;

    private static PlayerController _playerController;
    public static PlayerController PlayerController => _playerController;

    public static void RegisterGameManager(GameManager gm)
    {
        _gameManager = gm;
    }

    public static void RegisterPlayerController(PlayerController pc)
    {
        _playerController = pc;
    }
}
