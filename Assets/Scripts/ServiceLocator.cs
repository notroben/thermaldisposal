using UnityEngine;

public static class ServiceLocator
{
    private static GameManager _gameManager;
    public static GameManager GameManager => _gameManager;

    private static PlayerController _playerController;
    public static PlayerController PlayerController => _playerController;

    private static AudioManager _audioManager;
    public static AudioManager AudioManager => _audioManager;

    public static void RegisterGameManager(GameManager gm)
    {
        _gameManager = gm;
    }

    public static void RegisterPlayerController(PlayerController pc)
    {
        _playerController = pc;
    }

    public static void RegisterAudioManager(AudioManager am)
    {
        _audioManager = am;
    }
}
