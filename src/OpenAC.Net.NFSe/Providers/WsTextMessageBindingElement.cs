using System;
using System.IO;
using System.ServiceModel.Channels;
using System.Text;
using System.Xml;

namespace OpenAC.Net.NFSe.Providers
{
    public class WsTextMessageBindingElement : MessageEncodingBindingElement
    {
        #region Fields

        private MessageVersion msgVersion;
        private string mediaType;
        private string encoding;

        #endregion Fields

        #region Constructors

        /// <inheritdoc />
        private WsTextMessageBindingElement(WsTextMessageBindingElement binding)
            : this(binding.Encoding, binding.MediaType, binding.MessageVersion)
        {
            ReaderQuotas = new XmlDictionaryReaderQuotas();
            binding.ReaderQuotas.CopyTo(ReaderQuotas);
        }

        /// <inheritdoc />
        public WsTextMessageBindingElement(string encoding, string mediaType, MessageVersion msgVersion)
        {
            this.msgVersion = msgVersion ?? throw new ArgumentNullException(nameof(msgVersion));
            this.mediaType = mediaType ?? throw new ArgumentNullException(nameof(mediaType));
            this.encoding = encoding ?? throw new ArgumentNullException(nameof(encoding));
            ReaderQuotas = new XmlDictionaryReaderQuotas();
        }

        /// <inheritdoc />
        public WsTextMessageBindingElement(string encoding, string mediaType)
            : this(encoding, mediaType, MessageVersion.Soap11WSAddressingAugust2004)
        {
        }

        /// <inheritdoc />
        public WsTextMessageBindingElement(string encoding)
            : this(encoding, "text/xml")
        {
        }

        /// <inheritdoc />
        public WsTextMessageBindingElement()
            : this("UTF-8")
        {
        }

        #endregion Constructors

        #region Properties

        /// <inheritdoc />
        public override MessageVersion MessageVersion
        {
            get => msgVersion;

            set => msgVersion = value ?? throw new ArgumentNullException(nameof(value));
        }

        public string MediaType
        {
            get => mediaType;

            set => mediaType = value ?? throw new ArgumentNullException(nameof(value));
        }

        public string Encoding
        {
            get => encoding;

            set => encoding = value ?? throw new ArgumentNullException(nameof(value));
        }

        /// <summary>
        /// This encoder does not enforces any quotas for the unsecure messages. The
        /// quotas are enforced for the secure portions of messages when this encoder
        /// is used in a binding that is configured with security.
        /// </summary>
        public XmlDictionaryReaderQuotas ReaderQuotas { get; }

        #endregion Properties

        #region Methods

        public override MessageEncoderFactory CreateMessageEncoderFactory()
        {
            return new WsTextMessageEncoderFactory(MediaType, Encoding, MessageVersion);
        }

        public override BindingElement Clone()
        {
            return new WsTextMessageBindingElement(this);
        }

        public override IChannelFactory<TChannel> BuildChannelFactory<TChannel>(BindingContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            context.BindingParameters.Add(this);
            return context.BuildInnerChannelFactory<TChannel>();
        }

        public override bool CanBuildChannelFactory<TChannel>(BindingContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            return context.CanBuildInnerChannelFactory<TChannel>();
        }

        public override T GetProperty<T>(BindingContext context)
        {
            if (typeof(T) == typeof(XmlDictionaryReaderQuotas))
            {
                return (T)(object)ReaderQuotas;
            }

            return base.GetProperty<T>(context);
        }

        #endregion Methods
    }

    public class WsTextMessageEncoderFactory : MessageEncoderFactory
    {
        #region Constructors

        internal WsTextMessageEncoderFactory(string mediaType, string charSet,
            MessageVersion version)
        {
            MessageVersion = version;
            MediaType = mediaType;
            CharSet = charSet;
            Encoder = new WsTextMessageEncoder(this);
        }

        #endregion Constructors

        #region Properties

        /// <inheritdoc />
        public override MessageEncoder Encoder { get; }

        /// <inheritdoc />
        public override MessageVersion MessageVersion { get; }

        internal string MediaType { get; }

        internal string CharSet { get; }

        #endregion Properties
    }

    /// <inheritdoc />
    public class WsTextMessageEncoder : MessageEncoder
    {
        #region Fields

        private readonly WsTextMessageEncoderFactory factory;
        private readonly XmlWriterSettings writerSettings;

        #endregion Fields

        #region Constructors

        /// <inheritdoc />
        public WsTextMessageEncoder(WsTextMessageEncoderFactory factory)
        {
            this.factory = factory;

            writerSettings = new XmlWriterSettings { Encoding = Encoding.GetEncoding(factory.CharSet) };
            ContentType = $"{this.factory.MediaType}; charset={writerSettings.Encoding.HeaderName}";
        }

        #endregion Constructors

        #region Properties

        /// <inheritdoc />
        public override string ContentType { get; }

        /// <inheritdoc />
        public override string MediaType => factory.MediaType;

        /// <inheritdoc />
        public override MessageVersion MessageVersion => factory.MessageVersion;

        #endregion Properties

        #region Methods

        /// <inheritdoc />
        public override Message ReadMessage(ArraySegment<byte> buffer, BufferManager bufferManager, string contentType)
        {
            var msgContents = new byte[buffer.Count];
            Array.Copy(buffer.Array, buffer.Offset, msgContents, 0, msgContents.Length);
            bufferManager.ReturnBuffer(buffer.Array);

            var stream = new MemoryStream(msgContents);
            return ReadMessage(stream, int.MaxValue);
        }

        /// <inheritdoc />
        public override Message ReadMessage(Stream stream, int maxSizeOfHeaders, string contentType)
        {
            var reader = XmlReader.Create(stream);
            return Message.CreateMessage(reader, maxSizeOfHeaders, MessageVersion);
        }

        /// <inheritdoc />
        public override ArraySegment<byte> WriteMessage(Message message, int maxMessageSize, BufferManager bufferManager, int messageOffset)
        {
            var stream = new MemoryStream();
            var writer = XmlWriter.Create(stream, writerSettings);
            message.WriteMessage(writer);
            writer.Close();

            var messageBytes = stream.GetBuffer();
            var messageLength = (int)stream.Position;
            stream.Close();

            var totalLength = messageLength + messageOffset;
            var totalBytes = bufferManager.TakeBuffer(totalLength);
            Array.Copy(messageBytes, 0, totalBytes, messageOffset, messageLength);

            var byteArray = new ArraySegment<byte>(totalBytes, messageOffset, messageLength);
            return byteArray;
        }

        /// <inheritdoc />
        public override void WriteMessage(Message message, Stream stream)
        {
            var writer = XmlWriter.Create(stream, writerSettings);
            message.WriteMessage(writer);
            writer.Close();
        }

        #endregion Methods
    }
}