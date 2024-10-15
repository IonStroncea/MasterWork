
namespace ProxyLibrary.Handler
{
    /// <summary>
    /// Regular handler that sends data at once
    /// </summary>
    public class RegularHandler : AbstractHandler
    {
        /// <summary>
        /// Constructor. Sets tokens per turn
        /// </summary>
        /// <param name="tokensPerTurn">Tokens per turn</param>
        public RegularHandler(int tokensPerTurn) : base(tokensPerTurn)
        {
        }

        /// <inheritdoc/>
        public override void Send()
        {
            foreach (var item in _buffers)
            {
                item.SendData(_tokensPerTurn);
            }
        }
    }
}
