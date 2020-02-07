using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;

public class PortListener
{
    private Thread listenerThread;
    string receivedMessage;
    private TcpListener listener;

	public PortListener()
	{
        listener = new TcpListener(1000);
        listenerThread = new Thread(new ThreadStart(ListenForClient));
	}

    private void ListenForClient()
    {
        while (true)
        {
            Thread thread = new Thread(new ParameterizedThreadStart(HandleClientComm));
        }
    }

    private void HandleClientComm(object clientObj)
    {
        byte[] message = new byte[4096];
        TcpClient client = (TcpClient)clientObj;

        while (true)
        {

            var bytesRead = 0;

            try
            {
                bytesRead = client.GetStream().Read(message, 0, 4096);
            }
            catch
            {
                break;
            }

            if (bytesRead == 0)
            {
                break;
            }

            var encoding = new System.Text.ASCIIEncoding();
            receivedMessage = encoding.GetString(message);
        }
        
    }
}
