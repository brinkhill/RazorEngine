﻿namespace RazorEngine.Text
{
    using System.Net;

    /// <summary>
    /// Represents a Html-encoded string.
    /// </summary>
    public class HtmlEncodedString : IEncodedString
    {
        #region Fields
        private readonly string _encodedString;
        #endregion

        #region Constructor
        /// <summary>
        /// Initialises a new instance of <see cref="HtmlEncodedString"/>
        /// </summary>
        /// <param name="rawString">The raw string to be encoded.</param>
        public HtmlEncodedString(string rawString)
        {
            if (!string.IsNullOrWhiteSpace(rawString))
                _encodedString = WebUtility.HtmlEncode(rawString);
        }
        #endregion

        #region Methods
        /// <summary>
        /// Gets the encoded string.
        /// </summary>
        /// <returns>The encoded string.</returns>
        public string ToEncodedString()
        {
            return _encodedString ?? string.Empty;
        }

        /// <summary>
        /// Gets the string representation of this instance.
        /// </summary>
        /// <returns>The string representation of this instance.</returns>
        public override string ToString()
        {
            return ToEncodedString();
        }
        #endregion
    }
}