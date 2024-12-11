namespace Pong.Constants
{
    /// <summary>
    /// Defines constants for network message types.
    /// </summary>
    public static class MessageType
    {
        public const string Coucou = "coucou";
        public const string Welcome = "welcome!";
        public const string Disconnect = "disconnect";

        public const string ServerStopping = "SERVER_STOPPING";
        public const string GameStart = "GAME_START";
        public const string GameOver = "GAME_OVER";

        public const string PaddleLeftMove = "PADDLE_LEFT_MOVE";
        public const string PaddleLeftUpdate = "PADDLE_LEFT_UPDATE";
        public const string PaddleRightMove = "PADDLE_RIGHT_MOVE";
        public const string PaddleRightUpdate = "PADDLE_RIGHT_UPDATE";
        public const string BallUpdate = "BALL_UPDATE";
    }
}
