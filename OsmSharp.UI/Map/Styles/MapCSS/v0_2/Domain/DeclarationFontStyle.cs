using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OsmSharp.UI.Map.Styles.MapCSS.v0_2.Domain;

namespace OsmSharp.UI.Map.Styles.MapCSS.v0_2.Domain
{
    /// <summary>
    /// Strongly typed MapCSS v0.2 Declaration class.
    /// </summary>
    public class DeclarationFontStyle : Declaration<QualifierFontStyleEnum, FontStyleEnum>
    {

    }

    /// <summary>
    /// Strongly typed MapCSS v0.2 Declaration class.
    /// </summary>
    public enum QualifierFontStyleEnum
    {
        /// <summary>
        /// Fontstyle.
        /// </summary>
        FontStyle
    }

    /// <summary>
    /// Strongly typed MapCSS v0.2 Declaration class.
    /// </summary>
    public enum FontStyleEnum
    {
        /// <summary>
        /// Italic.
        /// </summary>
        Italic,
        /// <summary>
        /// Normal.
        /// </summary>
        Normal
    }
}
