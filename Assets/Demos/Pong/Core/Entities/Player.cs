using UnityEngine;
using System.Net;

namespace Pong.Core.Entities
{
    public class Player
    {
        public IPEndPoint Endpoint { get; private set; }
        public string Name { get; private set; }
        public bool IsReady { get; private set; }

        public Player(IPEndPoint endpoint, string name)
        {
            Endpoint = endpoint;
            Name = name;
            IsReady = false;
        }

        public void SetEndpoint(IPEndPoint endpoint)
        {
            Endpoint = endpoint;
        }

        public void SetName(string name)
        {
            Name = name;
        }

        public void GenerateName()
        {
            Name = $"Player #{Mathf.FloorToInt(Random.Range(0, 100))}";
        }

        public void SetReady(bool ready)
        {
            IsReady = ready;
        }

        public override string ToString()
        {
            return $"{Name} ({Endpoint.Address}:{Endpoint.Port})";
        }
    }
}
