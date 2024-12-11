using UnityEngine;
using System;
using System.Collections.Generic;
using System.Net;
using Pong.Utils;

namespace Pong.Core
{
    /// <summary>
    /// Centralized message handling for registering and invoking message handlers.
    /// </summary>
    public static class MessageHandler
    {
        private static Dictionary<string, Action<string, IPEndPoint>> messageHandlers
            = new Dictionary<string, Action<string, IPEndPoint>>();

        /// <summary>
        /// Registers a handler for a specific message type.
        /// </summary>
        public static void RegisterHandler(string messageType, Action<string, IPEndPoint> handler)
        {
            if (messageHandlers.ContainsKey(messageType))
            {
                PongLogger.Warning("MessageHandler", $"Handler for {messageType} already exists. Replacing it.");
                UnregisterHandler(messageType);
            }

            messageHandlers[messageType] = handler;
            PongLogger.Info("MessageHandler", $"Handler registered for message type: {messageType}");
        }

        /// <summary>
        /// Unregisters a handler for a specific message type.
        /// </summary>
        public static void UnregisterHandler(string messageType)
        {
            if (messageHandlers.ContainsKey(messageType))
            {
                messageHandlers.Remove(messageType);
                PongLogger.Info("MessageHandler", $"Handler unregistered for message type: {messageType}");
            }
        }

        /// <summary>
        /// Handles a message by invoking the corresponding handler.
        /// </summary>
        public static void HandleMessage(string rawMessage, IPEndPoint sender)
        {
            if (string.IsNullOrEmpty(rawMessage)) return;

            string[] tokens = rawMessage.Split('|');
            string messageType = tokens[0];
            string messageData = tokens.Length > 1 ? tokens[1] : string.Empty;

            if (messageHandlers.TryGetValue(messageType, out var handler))
            {
                PongLogger.Verbose("MessageHandler", $"Invoking handler for message type: {messageType}");
                handler.Invoke(messageData, sender);
            }
            else
            {
                PongLogger.Warning("MessageHandler", $"No handler registered for message type: {messageType}. Message: {rawMessage}");
            }
        }
    }
}
