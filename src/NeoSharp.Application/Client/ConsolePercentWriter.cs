using System;

namespace NeoSharp.Application.Client
{
    public class ConsolePercentWriter : IDisposable
    {
        #region Variables

        int _MaxValue, _Value;
        Decimal _LastFactor;

        readonly IConsoleWriter _Writer;
        readonly int _X, _Y;

        #endregion

        #region Properties

        /// <summary>
        /// Value
        /// </summary>
        public int Value
        {
            get { return _Value; }
            set
            {
                if (value == _Value) return;

                _Value = Math.Min(value, _MaxValue);

                Invalidate();
            }
        }
        /// <summary>
        /// Maximum value
        /// </summary>
        public int MaxValue
        {
            get { return _MaxValue; }
            set
            {
                if (value == _MaxValue) return;

                _MaxValue = value;

                if (_Value > _MaxValue)
                    _Value = _MaxValue;

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
                if (_MaxValue == 0) return 0;
                return (_Value * 100M) / _MaxValue;
            }
        }

        #endregion

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="writer">Writer</param>
        /// <param name="value">Value</param>
        /// <param name="maxValue">Maximum value</param>
        public ConsolePercentWriter(IConsoleWriter writer, int value = 0, int maxValue = 100)
        {
            _Writer = writer;
            _Writer.GetCursorPosition(out _X, out _Y);
            MaxValue = maxValue;
            Value = value;
            Invalidate();
        }
        /// <summary>
        /// Invalidate
        /// </summary>
        public void Invalidate()
        {
            Decimal factor = Math.Round((Percent / 100M), 1);

            if (_LastFactor == factor)
            {
                _LastFactor = factor;
                return;
            }

            string fill = string.Empty.PadLeft((int)(10 * factor), '■');
            string clean = string.Empty.PadLeft(10 - fill.Length, '■');

            _Writer.SetCursorPosition(_X, _Y);
            _Writer.Write("[", ConsoleOutputStyle.Output);
            _Writer.Write(fill, ConsoleOutputStyle.Input);
            _Writer.Write(clean + "] (" + Percent.ToString("0.0").PadLeft(5, ' ') + "%)", ConsoleOutputStyle.Output);
        }
        /// <summary>
        /// Free console
        /// </summary>
        public void Dispose()
        {
            _Writer.WriteLine("", ConsoleOutputStyle.Output);
        }
    }
}
