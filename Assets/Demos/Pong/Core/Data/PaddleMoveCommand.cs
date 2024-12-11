namespace Pong.Core.Data
{
    [System.Serializable]
    public class PaddleMoveCommand
    {
        public float Direction;
        public string State = "Stopped";
    }
}
