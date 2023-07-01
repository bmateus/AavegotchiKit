using System.Collections.Generic;

using MetaMask.NativeWebSocket;

using UnityEngine;

namespace MetaMask.SocketIOClient
{

    public class WebSocketDispatcher : MonoBehaviour
    {

        private static WebSocketDispatcher instance;

        public static WebSocketDispatcher Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new GameObject("WebSocket Dispatcher").AddComponent<WebSocketDispatcher>();
                }
                return instance;
            }
        }

        private List<WebSocket> webSockets = new List<WebSocket>();

        private void Awake()
        {
            DontDestroyOnLoad(gameObject);
        }

#if !UNITY_WEBGL || UNITY_EDITOR
        private void Update()
        {
            DispatchMessageQueue();
        }

        public void DispatchMessageQueue()
        {
            for (int i = 0; i < this.webSockets.Count; i++)
            {
                this.webSockets[i].DispatchMessageQueue();
            }
        }
#endif

        public void AddWebSocket(WebSocket webSocket)
        {
            this.webSockets.Add(webSocket);
        }

        public void RemoveWebSocket(WebSocket webSocket)
        {
            this.webSockets.Remove(webSocket);
        }

    }

}