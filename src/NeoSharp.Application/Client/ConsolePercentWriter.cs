using System;

namespace NeoSharp.Application.Client
{
    public class ConsolePercentWriter : IDisposable
    {
        #region Variables

        long _maxValue, _value;
        Decimal _lastFactor;

        readonly IConsoleWriter _writer;
        readonly int _x, _y;

        #endregion

        #region Properties

        /// <summary>
        /// Value
        /// </summary>
        public long Value
        {
            get { return _value; }
            set
            {
                if (value == _value) return;

                _value = Math.Min(value, _maxValue);

                Invalidate();
            }
        }
        /// <summary>
        /// Maximum value
        /// </summary>
        public long MaxValue
        {
            get { return _maxValue; }
            set
            {
                if (value == _maxValue) return;

                _maxValue = value;

                if (_value > _maxValue)
                    _value = _maxValue;

                Invalidate();
            }
        }
        /// <summary>
        /// Percent
        /// </summary>
        public Decimal Percent
        {
            get
            {
                if (_maxValue == 0) return 0;
                return (_value * 100M) / _maxValue;
            }
        }

        #endregion

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="writer">Writer</param>
        /// <param name="value">Value</param>
        /// <param name="maxValue">Maximum value</param>
        public ConsolePercentWriter(IConsoleWriter writer, long value = 0, long maxValue = 100)
        {
            _writer = writer;
            _lastFactor = -1;
            _writer.GetCursorPosition(out _x, out _y);
            MaxValue = maxValue;
            Value = value;
            Invalidate();
        }
        /// <summary>
        /// Invalidate
        /// </summary>
        public void Invalidate()
        {
            var factor = Math.Round((Percent / 100M), 1);

            if (_lastFactor == factor)
            {
                _lastFactor = factor;
                return;
            }

            var fill = string.Empty.PadLeft((int)(10 * factor), '■');
            var clean = string.Empty.PadLeft(10 - fill.Length, '■');

            _writer.SetCursorPosition(_x, _y);
            _writer.Write("[");
            _writer.Write(fill, ConsoleOutputStyle.Input);
            _writer.Write(clean + "] (" + Percent.ToString("0.0").PadLeft(5, ' ') + "%)");
        }
        /// <summary>
        /// Free console
        /// </summary>
        public void Dispose()
        {
            _writer.WriteLine("");
        }
    }
}
