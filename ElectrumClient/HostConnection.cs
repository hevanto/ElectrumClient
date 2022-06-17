using NBitcoin;
using System.Net.Security;
using System.Net.Sockets;
using System.Security.Cryptography.X509Certificates;

namespace ElectrumClient
{
    internal class HostConnection
    {
        private TcpClient? _tcpClient;
        private SslStream? _sslStream;
        private Stream? _stream;

        public HostConnection() : this(Resources.TestnetHost, Resources.TestnetPort, Resources.TestnetSSL, Network.TestNet) { }
        public HostConnection(string host, int port, bool useSSL, Network network)
        {
            Host = host;
            Port = port;
            UseSSL = useSSL;
            Network = network;

            _tcpClient = null;
            _sslStream = null;
            _stream = null;
        }
        public string Host { get; set; }
        public int Port { get; set; }
        public bool UseSSL { get; set; }
        public Network Network { get; set; }

        public Stream? Stream { get { return _stream; } }
        public bool Connected
        {
            get
            {
                return _tcpClient != null && _tcpClient.Connected && _stream != null;
            }
        }

        public async Task<bool> ConnectAsync()
        {
            if (Connected) Disconnect();

            _tcpClient = new TcpClient();
            await _tcpClient.ConnectAsync(Host, Port);
            if (_tcpClient == null || !_tcpClient.Connected) return false;

            if (UseSSL)
            {
                _sslStream = new SslStream(_tcpClient.GetStream(), true, new RemoteCertificateValidationCallback(CertificateValidationCallback));
                await _sslStream.AuthenticateAsClientAsync(Host);
                _stream = Stream.Synchronized(_sslStream);
            }
            else
                _stream = Stream.Synchronized(_tcpClient.GetStream());

            return true;
        }

        public void Disconnect()
        {
            if (!Connected) return;

            _stream?.Close();
            _tcpClient?.Close();

            if (_stream != null)
            {
                _stream?.Dispose();
                _stream = null;
            }

            if (_sslStream != null)
            {
                _sslStream.Dispose();
                _sslStream = null;
            }

            if (_tcpClient != null)
            {
                _tcpClient.Dispose();
                _tcpClient = null;
            }
        }

        public async Task WriteAsync(byte[] buffer, int offset, int count)
        {
            if (_stream == null) return;
            await _stream.WriteAsync(buffer, offset, count);
        }

        public async Task<int> ReadAsync(byte[] buffer, int offset, int count, CancellationToken ct)
        {
            if (_stream == null) return 0;
            return await _stream.ReadAsync(buffer, offset, count, ct);
        }

        private static bool CertificateValidationCallback(object sender, X509Certificate? cert, X509Chain? chain, SslPolicyErrors sslPolicyError)
        {
            // TODO: Implement
            return true;
        }
    }
}
