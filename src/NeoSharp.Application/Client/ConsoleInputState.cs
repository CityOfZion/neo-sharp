using System.Text;

namespace NeoSharp.Application.Client
{
    internal class ConsoleInputState
    {
        #region Private fields

        private IConsoleHandler _console;
        private int _startX = 0, _startY = 0;

        #endregion

        #region Public properties

        public int Cursor { get; set; } = 0;
        public int HistoryIndex { get; set; } = 0;
        public int StartX => _startX;
        public int StartY => _startY;
        public bool InsertMode { get; set; } = true;

        public StringBuilder Txt { get; set; } = new StringBuilder();
        public IAutoCompleteHandler Autocomplete { get; set; }

        #endregion

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="handler">Handler</param>
        public ConsoleInputState(IConsoleHandler handler)
        {
            _console = handler;
            GetCursorPosition();
        }

        /// <summary>
        /// Update cursor position
        /// </summary>
        public void GetCursorPosition()
        {
            _console.GetCursorPosition(out _startX, out _startY);
        }
    }
}