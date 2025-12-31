using UnityEngine;

public class GameTimer : SingletonPattern<GameTimer>
{
    public double CurrentTime => System.DateTime.UtcNow
    .Subtract(System.DateTime.UnixEpoch)
    .TotalSeconds;
}
