using UnityEngine;
using System;

public class GameTimer : SingletonPattern<GameTimer>
{
    // ===== CONFIG =====
    public const double REAL_SECONDS_PER_GAME_DAY = 0.5 * 60; // 900s
    public const double GAME_SECONDS_PER_DAY = 24 * 60 * 60; // 86400s

    public const double TIME_SCALE =
        GAME_SECONDS_PER_DAY / REAL_SECONDS_PER_GAME_DAY; // 96x

    // ===== BASE TIME =====
    private double _gameStartRealTime;
    private double _gameStartGameTime;

    protected override void Awake()
    {
        base.Awake();

        _gameStartRealTime = CurrentTime;
        _gameStartGameTime = 0;
    }

    // ===== REAL TIME =====
    public double CurrentTime => DateTime.UtcNow
        .Subtract(DateTime.UnixEpoch)
        .TotalSeconds;

    // ===== GAME TIME =====
    public double GameTimeSeconds
    {
        get
        {
            double realElapsed = CurrentTime - _gameStartRealTime;
            return _gameStartGameTime + realElapsed * TIME_SCALE;
        }
    }

    // ===== GAME TIME BREAKDOWN =====
    public int CurrentDay
    {
        get
        {
            return (int)(GameTimeSeconds / GAME_SECONDS_PER_DAY);
        }
    }

    public double TimeOfDaySeconds
    {
        get
        {
            return GameTimeSeconds % GAME_SECONDS_PER_DAY;
        }
    }

    public float TimeOfDay01
    {
        get
        {
            return (float)(TimeOfDaySeconds / GAME_SECONDS_PER_DAY);
        }
    }

    public int Hour
    {
        get
        {
            return (int)(TimeOfDaySeconds / 3600);
        }
    }

    public int Minute
    {
        get
        {
            return (int)((TimeOfDaySeconds % 3600) / 60);
        }
    }

    // ===== UTILS =====
    public bool IsDayTime
    {
        get
        {
            return Hour >= 6 && Hour < 18;
        }
    }

    public bool IsNightTime
    {
        get
        {
            return !IsDayTime;
        }
    }

    // ===== CONTROL =====
    public void SetGameTime(double gameSeconds)
    {
        _gameStartGameTime = gameSeconds;
        _gameStartRealTime = CurrentTime;
    }

    public void AddGameTime(double gameSeconds)
    {
        SetGameTime(GameTimeSeconds + gameSeconds);
    }
}
