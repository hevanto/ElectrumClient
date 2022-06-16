﻿using ElectrumClient.Hashing;
using ElectrumClient.Request;
using ElectrumClient.Response;
using NBitcoin;
using System.Collections.Concurrent;
using System.Net.Security;
using System.Net.Sockets;
using System.Reflection;
using System.Security.Cryptography.X509Certificates;

namespace ElectrumClient
{
    public class Client : IDisposable
    {
        public const string DEFAULT_TETNET_HOST = "electrum2.cipig.net";
        public const int DEFAULT_TESTNET_PORT = 20068;
        public const bool DEFAULT_TESTNET_USESSL = true;

        public const string DEFAULT_MAINNET_HOST = "electrum2.cipig.net";
        public const int DEFAULT_MAINNET_PORT = 20000;
        public const bool DEFAULT_MAINNET_USESSL = true;

        private const string DONATION_ADDRESS = "bc1qg5vc0kn4997l2x8m3vnjgmekca49y8va55pje7";

        private static readonly int BUFFERSIZE = 32;
        private static readonly string CLIENT = "Hevanto ElectrumClient 1.0";
        private static readonly string PROTO_MIN = "1.4";
        private static readonly string PROTO_MAX = "1.4";

        public event EventHandler<ITip>? NewTip;
        public event EventHandler<IScriptHashStatus>? NewScriptHashStatus;

        private event EventHandler? _streamClosed;
        private event EventHandler<GenericResponse>? _responseReceived;

        public string ServerVersion { get; private set; }
        public string ProtocolVersion { get; private set; }

        private string _host;
        private int _port;
        private bool _useSSL;

        private TcpClient? _tcpClient;
        private Stream? _stream;

        private CancellationTokenSource _cancelSource;

        private BitSize _scriptHashBitSize;
        //private IHashFunction _scriptHashFunction;

        private int _msgIdCntr;

        private ConcurrentDictionary<int, string> _scriptHashSubscription;
        private ConcurrentDictionary<string, int> _scriptHashSubscriptionIndex;

        public static async Task<Client> ConnectToTestnet(
            string host = DEFAULT_TETNET_HOST,
            int port = DEFAULT_TESTNET_PORT,
            bool useSSL = DEFAULT_TESTNET_USESSL)
        {
            var clnt = new Client(host, port, useSSL);
            await clnt.ConnectAsync();
            return clnt;
        }

        public static async Task<Client> ConnectToMainnet(
            string host = DEFAULT_MAINNET_HOST,
            int port = DEFAULT_MAINNET_PORT,
            bool useSSL = DEFAULT_MAINNET_USESSL)
        {
            var clnt = new Client(host, port, useSSL);
            await clnt.ConnectAsync();
            return clnt;
        }

        public Client(string host, int port, bool useSSL)
        {
            ServerVersion = "";
            ProtocolVersion = "";

            _host = host;
            _port = port;
            _useSSL = useSSL;

            TimeoutMs = 30000;
            KeepAliveIntervalMs = 30000;

            _cancelSource = new CancellationTokenSource();

            _streamClosed += async (object? sender, EventArgs e) =>
            {
                _cancelSource.Cancel();
                Close();
                await ConnectAsync();
            };

            _scriptHashSubscription = new ConcurrentDictionary<int, string>();
            _scriptHashSubscriptionIndex = new ConcurrentDictionary<string, int>();

            _scriptHashBitSize = HashFunctionFactory.GetHashFunctionBitSize();
        }

        public bool Connected { get { return _tcpClient != null ? _tcpClient.Connected : false; } }

        public int TimeoutMs { get; set; }

        public int KeepAliveIntervalMs { get; set; }

        public async Task<bool> ConnectAsync()
        {
            _tcpClient = new TcpClient();
            await _tcpClient.ConnectAsync(_host, _port);
            if (!_tcpClient.Connected) { return false; }

            if (_useSSL)
            {
                var sslsTream = new SslStream(_tcpClient.GetStream(), true, new RemoteCertificateValidationCallback(CertificateValidationCallback));
                await sslsTream.AuthenticateAsClientAsync(_host);
                _stream = Stream.Synchronized(sslsTream);
            }
            else
                _stream = Stream.Synchronized(_tcpClient.GetStream());

            StartBackgroundTasks();
            await NegotiateProtocolAsync();
            if (ProtocolVersion == "")
            {
                Close();
                return false;
            }
            await GetFeaturesAsync();
            await SubscribeToNewTipEventsAsync();
            return true;
        }

        public void Close()
        {
            StopBackgroundTasks();
            _stream?.Close();
            _tcpClient?.Close();
        }

        public void Dispose()
        {
            if (Connected) Close();

            _stream?.Dispose();
            _tcpClient?.Dispose();

            _stream = null;
            _tcpClient = null;
        }

        public async Task<IAsyncResponse<IHexString, IError>> GetBlockHeaderAsync(long height)
        {
            var request = new BlockChainBlockHeaderRequest(height, null);
            request.MessageId = AllocMessageId();
            var resp = await SendAndWaitForResponse<BlockChainBlockHeaderRequest, AsyncResponse<Response.BlockHeader, Error, IBlockHeader, IError>>(request);
            return new AsyncResponse<IHexString, Error, IHexString, IError>(resp.ResultValue != null ? resp.ResultValue.Header : null, resp.ErrorValue);
        }

        public async Task<IAsyncResponse<IBlockHeader, IError>> GetBlockHeaderAsync(long height, long checkPointHeight)
        {
            var request = new BlockChainBlockHeaderRequest(height, checkPointHeight);
            request.MessageId = AllocMessageId();
            return await SendAndWaitForResponse<BlockChainBlockHeaderRequest, AsyncResponse<Response.BlockHeader, Error, IBlockHeader, IError>>(request);
        }

        public IHexString GetBlockHeader(long height, out IError? error) => GetBlockHeaderAsync(height).Result.ToSyncResponse(out error) ?? new HexString();
        public IBlockHeader GetBlockHeader(long height, long checkpointHeight, out IError? error) => GetBlockHeaderAsync(height, checkpointHeight).Result.ToSyncResponse(out error) ?? new Response.BlockHeader();


        public async Task<IAsyncResponse<IBlockHeaders, IError>> GetBlockHeadersAsync(long startHeight, long count)
        {
            var request = new BlockChainBlockHeadersRequest(startHeight, count, null);
            request.MessageId = AllocMessageId();
            return await SendAndWaitForResponse<BlockChainBlockHeadersRequest, AsyncResponse<BlockHeaders, Error, IBlockHeaders, IError>>(request);
        }

        public async Task<IAsyncResponse<IBlockHeaders, IError>> GetBlockHeadersAsync(long startHeight, long count, long checkPointHeight)
        {
            var request = new BlockChainBlockHeadersRequest(startHeight, count, checkPointHeight);
            request.MessageId = AllocMessageId();
            return await SendAndWaitForResponse<BlockChainBlockHeadersRequest, AsyncResponse<BlockHeaders, Error, IBlockHeaders, IError>>(request);
        }

        public IBlockHeaders GetBlockHeaders(long startHeight, long count, out IError? error) => GetBlockHeadersAsync(startHeight, count).Result.ToSyncResponse(out error) ?? new BlockHeaders();
        public IBlockHeaders GetBlockHeaders(long startHeight, long count, long checkPointHeight, out IError? error) => GetBlockHeadersAsync(startHeight, count, checkPointHeight).Result.ToSyncResponse(out error) ?? new BlockHeaders();

        public async Task<IAsyncResponse<IFee, IError>> EstimateFeeAsync(long targetConfirmation)
        {
            var request = new BlockChainEstimateFeeRequest(targetConfirmation);
            request.MessageId = AllocMessageId();
            var resp = await SendAndWaitForResponse<BlockChainEstimateFeeRequest, AsyncResponse<Fee, Error, IFee, IError>>(request);
            if (resp.ResultValue != null) resp.ResultValue.TargetConfirmation = targetConfirmation;
            return resp;
        }

        public IFee EstimateFee(long targetConfirmation, out IError? error) => EstimateFeeAsync(targetConfirmation).Result.ToSyncResponse(out error) ?? new Fee(targetConfirmation);

        public async Task<IAsyncResponse<Money, IError>> RelayFeeAsync()
        {
            var request = new BlockChainRelayFeeRequest();
            request.MessageId = AllocMessageId();
            var resp = await SendAndWaitForResponse<BlockChainRelayFeeRequest, AsyncResponse<Fee, Error, IFee, IError>>(request);
            return new AsyncResponse<Money, Error, Money, IError>(resp.ResultValue != null ? resp.ResultValue.Amount : null, resp.ErrorValue);
        }

        public Money RelayFee(out IError? error) => RelayFeeAsync().Result.ToSyncResponse(out error) ?? Money.Zero;

        public async Task<IAsyncResponse<IBalance, IError>> GetScriptBalanceAsync(IHash scriptHash)
        {
            var request = new BlockChainScriptHashGetBalanceRequest(scriptHash);
            request.MessageId = AllocMessageId();
            return await SendAndWaitForResponse<BlockChainScriptHashGetBalanceRequest, AsyncResponse<Balance, Error, IBalance, IError>>(request);
        }

        private async Task<IAsyncResponse<IBalance, IError>> GetScriptBalanceAsync(Script script) => await GetScriptBalanceAsync(HashFactory.Create(_scriptHashBitSize, script));
        public IBalance GetScriptBalance(IHash scriptHash, out IError? error) => GetScriptBalanceAsync(scriptHash).Result.ToSyncResponse(out error) ?? new Balance();
        public IBalance GetScriptBalance(Script script, out IError? error) => GetScriptBalance(HashFactory.Create(_scriptHashBitSize, script), out error);

        public async Task<IAsyncResponse<IList<ITransactionInfo>, IError>> GetScriptHistoryAsync(IHash scriptHash)
        {
            var request = new BlockChainScriptHashGetHistoryRequest(scriptHash);
            request.MessageId = AllocMessageId();
            var resp = await SendAndWaitForResponse<BlockChainScriptHashGetHistoryRequest, AsyncResponse<TransactionInfoList, Error, ITransactionInfoList, IError>>(request);
            return new AsyncResponse<IList<ITransactionInfo>, Error, IList<ITransactionInfo>, IError>(resp.ResultValue != null ? resp.ResultValue.List : null, resp.ErrorValue);
        }

        public async Task<IAsyncResponse<IList<ITransactionInfo>, IError>> GetScriptHistoryAsync(Script script) => await GetScriptHistoryAsync(HashFactory.Create(_scriptHashBitSize, script));
        public IList<ITransactionInfo> GetScriptHistory(IHash scriptHash, out IError? error) => GetScriptHistoryAsync(scriptHash).Result.ToSyncResponse(out error) ?? new List<ITransactionInfo>();
        public IList<ITransactionInfo> GetScriptHistory(Script script, out IError? error) => GetScriptHistory(HashFactory.Create(_scriptHashBitSize, script), out error);

        public async Task<IAsyncResponse<IList<IMempoolTransactionInfo>, IError>> GetScriptMempoolAsync(IHash scriptHash)
        {
            var request = new BlockChainScriptHashGetMempoolRequest(scriptHash);
            request.MessageId = AllocMessageId();
            var resp = await SendAndWaitForResponse<BlockChainScriptHashGetMempoolRequest, AsyncResponse<MempoolTransactionInfoList, Error, IMempoolTransationInfoList, IError>>(request);
            return new AsyncResponse<IList<IMempoolTransactionInfo>, Error, IList<IMempoolTransactionInfo>, IError>(resp.ResultValue != null ? resp.ResultValue.List : null, resp.ErrorValue);
        }

        public async Task<IAsyncResponse<IList<IMempoolTransactionInfo>, IError>> GetScriptMempoolAsync(Script script) => await GetScriptMempoolAsync(HashFactory.Create(_scriptHashBitSize, script));
        public IList<IMempoolTransactionInfo> GetScriptMempool(IHash scriptHash, out IError? error) => GetScriptMempoolAsync(scriptHash).Result.ToSyncResponse(out error) ?? new List<IMempoolTransactionInfo>();
        public IList<IMempoolTransactionInfo> GetScriptMempool(Script script, out IError? error) => GetScriptMempool(HashFactory.Create(_scriptHashBitSize, script), out error);


        public async Task<IAsyncResponse<IList<IUnspentOutput>, IError>> GetScriptUnspentOutputsAsync(IHash scriptHash)
        {
            var request = new BlockChainScriptHashListUnspent(scriptHash);
            request.MessageId = AllocMessageId();
            var resp = await SendAndWaitForResponse<BlockChainScriptHashListUnspent, AsyncResponse<UnspentOutputList, Error, IUnspentOutputList, IError>>(request);
            return new AsyncResponse<IList<IUnspentOutput>, Error, IList<IUnspentOutput>, IError>(resp.ResultValue != null ? resp.ResultValue.List : null, resp.ErrorValue);
        }

        public async Task<IAsyncResponse<IList<IUnspentOutput>, IError>> GetScriptUnspentOutputsAsync(Script script) => await GetScriptUnspentOutputsAsync(HashFactory.Create(_scriptHashBitSize, script));
        public IList<IUnspentOutput> GetScriptUnspentOutputs(IHash scriptHash, out IError? error) => GetScriptUnspentOutputsAsync(scriptHash).Result.ToSyncResponse(out error) ?? new List<IUnspentOutput>();
        public IList<IUnspentOutput> GetScriptUnspentOutputs(Script script, out IError? error) => GetScriptUnspentOutputs(HashFactory.Create(_scriptHashBitSize, script), out error);

        public async Task<IAsyncResponse<Hash<BitSize256>, IError>> SubscribeToScriptHashAsync(IHash scriptHash)
        {
            var request = new BlockChainScriptHashSubscribeRequest(scriptHash);
            request.MessageId = AllocMessageId();

            _scriptHashSubscription.TryAdd(request.MessageId, scriptHash.ToString());
            _scriptHashSubscriptionIndex.TryAdd(scriptHash.ToString(), request.MessageId);

            var resp = await SendAndWaitForResponse<BlockChainScriptHashSubscribeRequest, AsyncResponse<IScriptHashStatus, Error, IScriptHashStatus, IError>>(request);
            return new AsyncResponse<Hash<BitSize256>, Error, Hash<BitSize256>, IError>(resp.ResultValue != null ? resp.ResultValue.Hash : new Hash<BitSize256>(), resp.ErrorValue);
        }

        public async Task<IAsyncResponse<Hash<BitSize256>, IError>> SubscribeToScriptHashAsync(Script script) => await SubscribeToScriptHashAsync(HashFactory.Create(_scriptHashBitSize, script));
        public Hash<BitSize256> SubscribeToScriptHash(IHash scriptHash, out IError? error) => SubscribeToScriptHashAsync(scriptHash).Result.ToSyncResponse(out error) ?? new Hash<BitSize256>();
        public Hash<BitSize256> SubscribeToScriptHash(Script script, out IError? error) => SubscribeToScriptHash(HashFactory.Create(_scriptHashBitSize, script), out error);

        public async Task<IAsyncResponse<bool, IError>> UnsubscribeFromScriptHashAsync(IHash scriptHash)
        {
            var request = new BlockChainScriptHashUnsubscribeRequest(scriptHash);
            request.MessageId = AllocMessageId();

            int messageId = 0;
            if (_scriptHashSubscriptionIndex.TryGetValue(scriptHash.ToString(), out messageId))
            {
                string? sh = scriptHash.ToString();
                if (_scriptHashSubscription.TryRemove(messageId, out sh)) _scriptHashSubscriptionIndex.TryRemove(sh, out messageId);
            }

            var resp = await SendAndWaitForResponse<BlockChainScriptHashUnsubscribeRequest, AsyncResponse<BoolResponse, Error, IBool, IError>>(request);
            return new AsyncResponse<bool, Error, bool, IError>(resp.ResultValue != null ? resp.ResultValue.Value : false, resp.ErrorValue);
        }

        public async Task<IAsyncResponse<bool, IError>> UnsubscribeFromScriptHashAsync(Script script) => await UnsubscribeFromScriptHashAsync(HashFactory.Create(_scriptHashBitSize, script));
        public bool UnsubscribeFromScriptHash(IHash scriptHash, out IError? error) => UnsubscribeFromScriptHashAsync(scriptHash).Result.ToSyncResponse(out error);
        public bool UnsubscribeFromScriptHash(Script script, out IError? error) => UnsubscribeFromScriptHash(HashFactory.Create(_scriptHashBitSize, script), out error);

        public async Task<IAsyncResponse<Hash<BitSize256>, IError>> BroadcastTransactionAsync(IHexString rawTx)
        {
            var request = new BlockChainTransactionBroadcastRequest(rawTx);
            request.MessageId = AllocMessageId();
            var resp = await SendAndWaitForResponse<BlockChainTransactionBroadcastRequest, AsyncResponse<Transaction<BroadcastTransaction>, Error, ITransaction, IError>>(request);
            return new AsyncResponse<Hash<BitSize256>, Error, Hash<BitSize256>, IError>(resp.ResultValue != null ? resp.ResultValue.Hash : new Hash<BitSize256>(), resp.ErrorValue);
        }
        public async Task<IAsyncResponse<Hash<BitSize256>, IError>> BroadcastTransactionAsync(Transaction tx) => await BroadcastTransactionAsync(new HexString(tx));
        public Hash<BitSize256> BroadcastTransaction(IHexString rawTx, out IError? error) => BroadcastTransactionAsync(rawTx).Result.ToSyncResponse(out error) ?? new Hash<BitSize256>();
        public Hash<BitSize256> BroadcastTransaction(Transaction tx, out IError? error) => BroadcastTransaction(new HexString(tx), out error);

        public async Task<IAsyncResponse<IHexString, IError>> GetRawTransactionAsync(Hash<BitSize256> txHash)
        {
            var request = new BlockChainTransactionGetRequest(txHash, false);
            request.MessageId = AllocMessageId();
            var resp = await SendAndWaitForResponse<BlockChainTransactionGetRequest, AsyncResponse<Transaction<OnChainTransaction>, Error, ITransaction, IError>>(request);
            return new AsyncResponse<IHexString, Error, IHexString, IError>(resp.ResultValue != null ? resp.ResultValue.Hex : new HexString(), resp.ErrorValue);
        }
        public IHexString GetRawTransaction(Hash<BitSize256> txHash, out IError? error) => GetRawTransactionAsync(txHash).Result.ToSyncResponse(out error) ?? new HexString();
        
        public async Task<IAsyncResponse<ITransaction, IError>> GetTransactionAsync(Hash<BitSize256> txHash)
        {
            var request = new BlockChainTransactionGetRequest(txHash, true);
            request.MessageId = AllocMessageId();
            return await SendAndWaitForResponse<BlockChainTransactionGetRequest, AsyncResponse<Transaction<OnChainTransaction>, Error, ITransaction, IError>>(request);
        }
        public ITransaction GetTransaction(Hash<BitSize256> txHash, out IError? error) => GetTransactionAsync(txHash).Result.ToSyncResponse(out error) ?? new Transaction<OnChainTransaction>();

        public async Task<IAsyncResponse<IMerkleInfo, IError>> GetTransactionMerkleAsync(Hash<BitSize256> txHash, long height)
        {
            var request = new BlockChainTransactionGetMerkleRequest(txHash, height);
            request.MessageId = AllocMessageId();
            return await SendAndWaitForResponse<BlockChainTransactionGetMerkleRequest, AsyncResponse<MerkleInfo, Error, IMerkleInfo, IError>>(request);
        }

        public IMerkleInfo GetTransactionMerkle(Hash<BitSize256> txHash, long height, out IError? error) => GetTransactionMerkleAsync(txHash, height).Result.ToSyncResponse(out error) ?? new MerkleInfo();

        public async Task<IAsyncResponse<Hash<BitSize256>, IError>> GetTransactionFromBlockAsync(long height, long txPos)
        {
            var request = new BlockChainTransactionIdFromPosRequest(height, txPos, false);
            request.MessageId = AllocMessageId();
            var resp = await SendAndWaitForResponse<BlockChainTransactionIdFromPosRequest, AsyncResponse<TransactionInfo, Error, ITransactionInfo, IError>>(request);
            return new AsyncResponse<Hash<BitSize256>, Error, Hash<BitSize256>, IError>(resp.ResultValue != null ? resp.ResultValue.TxHash : new Hash<BitSize256>(), resp.ErrorValue);
        }

        public Hash<BitSize256> GetTransactionFromBlock(long height, long txPos, out IError? error) => GetTransactionFromBlockAsync(height, txPos).Result.ToSyncResponse(out error) ?? new Hash<BitSize256>();

        public async Task<IAsyncResponse<ITransactionInfo, IError>> GetTransactionFromBlockWithMerkleAsync(long height, long txPos)
        {
            var request = new BlockChainTransactionIdFromPosRequest(height, txPos, true);
            request.MessageId = AllocMessageId();
            return await SendAndWaitForResponse<BlockChainTransactionIdFromPosRequest, AsyncResponse<TransactionInfo, Error, ITransactionInfo, IError>>(request);
        }

        public ITransactionInfo GetTransactionFromBlockWihtMerkle(long height, long txPos, out IError? error) => GetTransactionFromBlockWithMerkleAsync(height, txPos).Result.ToSyncResponse(out error) ?? new TransactionInfo();

        public async Task<IAsyncResponse<IList<IFeeHistogramPoint>, IError>> GetMempoolFeeHistogramAsync()
        {
            var request = new MempoolGetFeeHistogramRequest();
            request.MessageId = AllocMessageId();
            var resp = await SendAndWaitForResponse<MempoolGetFeeHistogramRequest, AsyncResponse<FeeHistogram, Error, IFeeHistogram, IError>>(request);
            return new AsyncResponse<IList<IFeeHistogramPoint>, Error, IList<IFeeHistogramPoint>, IError>(resp.ResultValue != null ? resp.ResultValue.List : null, resp.ErrorValue);
        }

        public IList<IFeeHistogramPoint> GetMempoolFeeHistogram(out IError? error) => GetMempoolFeeHistogramAsync().Result.ToSyncResponse(out error) ?? new List<IFeeHistogramPoint>();

        public async Task<IAsyncResponse<string, IError>> GetServerBannerAsync()
        {
            var request = new ServerBannerRequest();
            request.MessageId = AllocMessageId();
            var resp = await SendAndWaitForResponse<ServerBannerRequest, AsyncResponse<StringResponse, Error, IString, IError>>(request);
            return new AsyncResponse<string, Error, string, IError>(resp.ResultValue != null ? resp.ResultValue.Value : null, resp.ErrorValue);
        }

        public string GetServerBanner(out IError? error) => GetServerBannerAsync().Result.ToSyncResponse(out error) ?? "";

        public async Task<IAsyncResponse<string, IError>> GetServerDonationAddressAsync()
        {
            var request = new ServerDonationAddressRequest();
            request.MessageId = AllocMessageId();
            var resp = await SendAndWaitForResponse<ServerDonationAddressRequest, AsyncResponse<StringResponse, Error, IString, IError>>(request);
            return new AsyncResponse<string, Error, string, IError>(resp.ResultValue != null ? resp.ResultValue.Value : null, resp.ErrorValue);
        }

        public string GetServerDonationAddress(out IError? error) => GetServerDonationAddressAsync().Result.ToSyncResponse(out error) ?? DONATION_ADDRESS;

        public string GetClientDonationAddress()
        {
            return DONATION_ADDRESS;
        }

        public async Task<IAsyncResponse<IServerFeatures, IError>> GetServerFeaturesAsync()
        {
            var request = new ServerFeaturesRequest();
            request.MessageId = AllocMessageId();
            return await SendAndWaitForResponse<ServerFeaturesRequest, AsyncResponse<ServerFeatures, Error, IServerFeatures, IError>>(request);
        }

        public IServerFeatures GetServerFeatures(out IError? error) => GetServerFeaturesAsync().Result.ToSyncResponse(out error) ?? new ServerFeatures();

        public async Task<IAsyncResponse<IList<IPeer>, IError>> GetServerPeersAsync()
        {
            var request = new ServerPeersSubscribeRequest();
            request.MessageId = AllocMessageId();
            var resp = await SendAndWaitForResponse<ServerPeersSubscribeRequest, AsyncResponse<PeerList, Error, IPeerList, IError>>(request);
            return new AsyncResponse<IList<IPeer>, Error, IList<IPeer>, IError>(resp.ResultValue != null ? resp.ResultValue.List : null, resp.ErrorValue);
        }

        public IList<IPeer> GetServerPeers(out IError? error) => GetServerPeersAsync().Result.ToSyncResponse(out error) ?? new List<IPeer>();

        public async Task<IAsyncResponse<bool, IError>> PingAsync()
        {
            var request = new ServerPingRequest();
            request.MessageId = AllocMessageId();
            var resp = await SendAndWaitForResponse<ServerPingRequest, AsyncResponse<GenericResponse, Error, IGenericResponse, IError>>(request);
            return new AsyncResponse<bool, Error, bool, IError>(resp.IsSuccess, resp.ErrorValue);
        }

        public bool Ping(out IError? error) => PingAsync().Result.ToSyncResponse(out error);

        private async Task<IAsyncResponse<IServerVersion, IError>> GetServerVersionAsync(string clientName, string protocolMin, string protocolMax)
        {
            if (protocolMax == "") protocolMax = protocolMin;
            var request = new ServerVersionRequest(clientName, protocolMin, protocolMax);
            request.MessageId = AllocMessageId();
            return await SendAndWaitForResponse<ServerVersionRequest, AsyncResponse<ServerVersion, Error, IServerVersion, IError>>(request);
        }
            
        private async Task<IAsyncResponse<bool, IError>> SubscribeToNewTipEventsAsync()
        {
            var request = new BlockChainHeadersSubscribeRequest();
            request.MessageId = AllocMessageId();
            var resp = await SendAndWaitForResponse<BlockChainHeadersSubscribeRequest, AsyncResponse<Tip, Error, ITip, IError>>(request);
            if (resp.ResultValue != null) NewTip?.Invoke(this, resp.ResultValue);
            return new AsyncResponse<bool, Error, bool, IError>(resp.IsSuccess, resp.ErrorValue);
        }

        private int AllocMessageId()
        {
            var msgId = Interlocked.Increment(ref _msgIdCntr);
            if (msgId == 0) return Interlocked.Increment(ref _msgIdCntr);
            return msgId;
        }

        private async Task<Resp> SendAndWaitForResponse<Req, Resp>(RequestBase request) where Resp : IIAsyncResponse, new()
        {
            if (_stream == null) throw new Exception("Client not connected");

            var requestData = request.GetRequestData<Req>();
            await _stream.WriteAsync(requestData, 0, requestData.Length);

            GenericResponse? responseMessage = null;
            using (var sph = new SemaphoreSlim(0, 1))
            {
                EventHandler<GenericResponse> r = delegate (object? sender, GenericResponse response)
                {
                    if (response.MessageId == request.MessageId)
                    {
                        responseMessage = response;
                        sph.Release();
                    }
                };

                _responseReceived += r;

                var t = sph.WaitAsync();
                if (await Task.WhenAny(t, Task.Delay(TimeoutMs)) == t)
                {
                    _responseReceived -= r;
                    if (responseMessage != null)
                    {
                        var resp = new Resp();
                        if (responseMessage.IsError)
                        {
                            resp.UnmarshallError(responseMessage.Raw);
                        }
                        else
                        {
                            resp.UnmarshallResult(responseMessage.Raw);
                        }
                        return resp;
                    }
                    else
                        throw new System.IO.IOException();
                }
                _responseReceived -= r;
            }
            throw new TimeoutException();
        }

        private void StartBackgroundTasks()
        {
            var ct = _cancelSource.Token;

            Task.Run(async () =>
            {
                await Reader(this, ct);
            });
            Task.Run(async () =>
            {
                await KeepAlive(this, ct);
            });
        }

        private void StopBackgroundTasks()
        {
            _cancelSource.Cancel();
        }

        private async Task NegotiateProtocolAsync()
        {
            var serverVersion = await GetServerVersionAsync(CLIENT, PROTO_MIN, PROTO_MAX);
            if (serverVersion.IsError || serverVersion.Result == null) return;
            ServerVersion = serverVersion.Result.SoftwareVersion;
            ProtocolVersion = serverVersion.Result.ProtocolVersion;
        }

        private async Task GetFeaturesAsync()
        {
            var features = await GetServerFeaturesAsync();
            if (features.IsError || features.Result == null) return;
            _scriptHashBitSize = HashFunctionFactory.GetHashFunctionBitSize(features.Result.HashFunction);
        }

        private static async Task Reader(Client client, CancellationToken ct)
        {
            if (client._stream == null) throw new Exception("Client not connected");

            while (!ct.IsCancellationRequested)
            {
                var receivedData = string.Empty;
                var buffer = new byte[BUFFERSIZE];

                using (MemoryStream memoryStream = new MemoryStream())
                {
                    int read;
                    while ((read = await client._stream.ReadAsync(buffer, 0, buffer.Length, ct)) > 0)
                    {
                        memoryStream.Write(buffer, 0, read);
                        if (read < BUFFERSIZE) break;
                    }
                    receivedData = System.Text.Encoding.UTF8.GetString(memoryStream.ToArray());
                }

                if (receivedData == "")
                {
                    client._streamClosed?.Invoke(client, new EventArgs());
                    break;
                }

                var packets = receivedData.Split('\n');
                foreach (string packet in packets)
                {
                    var genericResponse = GenericResponse.FromJson(packet);
                    if (genericResponse == null) continue;

                    switch (genericResponse.Method)
                    {
                        case "blockchain.headers.subscribe":
                            NotifyNewTip(client, genericResponse);
                            break;
                        case "blockchain.scripthash.subscribe":
                            NotifyNewScriptHashStatus(client, genericResponse);
                            break;
                        default:
                            NotifyResponseReceived(client, genericResponse);
                            break;
                    }
                }
            }
        }

        private static void NotifyResponseReceived(Client client, GenericResponse response)
        {
            client._responseReceived?.Invoke(client, response);
        }

        private static void NotifyNewTip(Client client, GenericResponse response)
        {
            var tipList = TipList.FromJson(response.Raw);
            foreach (Tip tip in tipList.Result)
            {
                if (tip.Height != 0)
                    client.NewTip?.Invoke(client, tip);
            }
        }

        private static void NotifyNewScriptHashStatus(Client client, GenericResponse response)
        {
            IScriptHashStatus? status;
            Type type = typeof(ScriptHashStatus<>).MakeGenericType(client._scriptHashBitSize.GetType());
            MethodInfo? fromJson = type.GetMethod("FromJson", BindingFlags.Static | BindingFlags.Public);

            string? scriptHash = "";
            if (client._scriptHashSubscription.TryGetValue(response.MessageId, out scriptHash))
                status = (IScriptHashStatus?)fromJson?.Invoke(null, new object?[] { scriptHash, response.Raw });
            else
                status = (IScriptHashStatus?)fromJson?.Invoke(null, new object?[] { null, response.Raw });
            
            if (status != null)
                client.NewScriptHashStatus?.Invoke(client, status);
        }

        private static async Task KeepAlive(Client client, CancellationToken ct)
        {
            if (client._stream == null) throw new Exception("Client not connected");

            var request = new ServerPingRequest();
            request.MessageId = 0;
            var requestData = request.GetRequestData<ServerPingRequest>();
            while (!ct.IsCancellationRequested)
            {
                try { await Task.Delay(client.KeepAliveIntervalMs, ct); } catch (Exception) { }
                if (!ct.IsCancellationRequested)
                {
                    await client._stream.WriteAsync(requestData, 0, requestData.Length);
                }
            }
        }

        private static bool CertificateValidationCallback(object sender, X509Certificate? cert, X509Chain? chain, SslPolicyErrors sslPolicyError)
        {
            // TODO: Implement
            return true;
        }
    }
}
