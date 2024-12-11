using UnityEngine;
using System.Net;
using System.Net.Sockets;
using System;
using Pong.Utils;
using Pong.Core;

/// <summary>
/// A service to manage UDP communication, including sending and receiving messages.
/// </summary>
public class UDPService : MonoBehaviour
{
    private UdpClient udp;
    private IPEndPoint localEP;

    /// <summary>
    /// Starts the UDP server and listens on the specified port.
    /// </summary>
    /// <param name="port">The port to listen on.</param>
    /// <returns>True if the server starts successfully, false otherwise.</returns>
    public bool Listen(int port)
    {
        if (udp != null)
        {
            PongLogger.Warning("UDPService", "Socket already initialized! Close it first.");
            return false;
        }

        try
        {
            // Create and bind the UDP listener
            localEP = new IPEndPoint(IPAddress.Any, port);
            udp = new UdpClient(localEP);

            PongLogger.Info("UDPService", $"Server listening on port: {port}");
            return true;
        }
        catch (Exception ex)
        {
            PongLogger.Error("UDPService", $"Error creating UDP listener on port {port}: {ex.Message}");
            Close();
            return false;
        }
    }

    /// <summary>
    /// Initializes a UDP client.
    /// </summary>
    /// <returns>True if the client initializes successfully, false otherwise.</returns>
    public bool InitClient()
    {
        if (udp != null)
        {
            PongLogger.Warning("UDPService", "Socket already initialized! Close it first.");
            return false;
        }

        try
        {
            udp = new UdpClient();
            localEP = new IPEndPoint(IPAddress.Any, 0);
            udp.Client.Bind(localEP);

            PongLogger.Info("UDPService", "Client initialized and bound to a random port.");
            return true;
        }
        catch (Exception ex)
        {
            PongLogger.Error("UDPService", $"Error creating UDP client: {ex.Message}");
            Close();
            return false;
        }
    }

    /// <summary>
    /// Closes the UDP socket.
    /// </summary>
    public void Close()
    {
        if (udp != null)
        {
            udp.Close();
            udp = null;
            PongLogger.Info("UDPService", "UDP socket closed.");
        }
    }

    void Update()
    {
        if (udp != null)
        {
            ReceiveUDP();
        }
    }

    /// <summary>
    /// Receives UDP messages and routes them to the MessageHandler.
    /// </summary>
    private void ReceiveUDP()
    {
        while (udp.Available > 0)
        {
            IPEndPoint sourceEP = new IPEndPoint(IPAddress.Any, 0);

            try
            {
                byte[] data = udp.Receive(ref sourceEP);
                ParseString(data, sourceEP);
            }
            catch (Exception ex)
            {
                PongLogger.Warning("UDPService", $"Error receiving UDP message: {ex.Message}");
            }
        }
    }

    /// <summary>
    /// Parses received messages and delegates them to the MessageHandler.
    /// </summary>
    /// <param name="bytes">The received message bytes.</param>
    /// <param name="sender">The sender's IP endpoint.</param>
    private void ParseString(byte[] bytes, IPEndPoint sender)
    {
        string message;

        try
        {
            message = System.Text.Encoding.UTF8.GetString(bytes);
        }
        catch (Exception ex)
        {
            PongLogger.Error("UDPService", $"Error decoding message: {ex.Message}");
            return;
        }

        PongLogger.Verbose("UDPService", $"Message received from {sender}: {message}");

        try
        {
            MessageHandler.HandleMessage(message, sender);
        }
        catch (Exception ex)
        {
            PongLogger.Error("UDPService", $"Error handling message: {message}. Exception: {ex.Message}");
        }
    }

    /// <summary>
    /// Sends a message via UDP to a specified endpoint.
    /// </summary>
    /// <param name="message">The message to send.</param>
    /// <param name="destination">The target endpoint.</param>
    public void SendUDPMessage(string message, IPEndPoint destination)
    {
        byte[] bytes = System.Text.Encoding.UTF8.GetBytes(message);

        try
        {
            udp.Send(bytes, bytes.Length, destination);
            PongLogger.Verbose("UDPService", $"Message sent to {destination}: {message}");
        }
        catch (Exception ex)
        {
            PongLogger.Warning("UDPService", $"Error sending message to {destination}: {ex.Message}");
        }
    }
}
