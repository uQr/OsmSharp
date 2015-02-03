using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OsmSharp.UI.Map.Styles.MapCSS.v0_2.Domain;
using OsmSharp.UI.Map.Styles.MapCSS.v0_2.Eval;

namespace OsmSharp.UI.Map.Styles.MapCSS.v0_2.Domain
{
    /// <summary>
    /// Strongly typed MapCSS v0.2 Declaration class.
    /// </summary>
    public class DeclarationInt : Declaration<DeclarationIntEnum, int>
	{
		/// <summary>
		/// Evalues the value in this declaration or returns the regular value when there is no eval function.
        /// </summary>
        /// <param name="mapCSSObject">Map CSS object.</param>
		/// <returns></returns>
		public override int Eval (MapCSSObject mapCSSObject)
		{
			if (!string.IsNullOrWhiteSpace (this.EvalFunction)) {
				return EvalInterpreter.Instance.InterpretInt (this.EvalFunction, mapCSSObject);
			}
			return this.Value;
		}
    }

    /// <summary>
    /// Strongly typed MapCSS v0.2 Declaration class.
    /// </summary>
    public enum DeclarationIntEnum
    {
        /// <summary>
        /// Fillcolor.
        /// </summary>
        FillColor,
        /// <summary>
        /// Z-index.
        /// </summary>
        ZIndex,
        /// <summary>
        /// Color.
        /// </summary>
        Color,
        /// <summary>
        /// Casing color.
        /// </summary>
        CasingColor,
        /// <summary>
        /// Extrude.
        /// </summary>
        Extrude,
        /// <summary>
        /// Extrude edge color.
        /// </summary>
        ExtrudeEdgeColor,
        /// <summary>
        /// Extrude face color.
        /// </summary>
        ExtrudeFaceColor,
        /// <summary>
        /// Icon width.
        /// </summary>
        IconWidth,
        /// <summary>
        /// Icon height.
        /// </summary>
        IconHeight,
        /// <summary>
        /// Font size.
        /// </summary>
        FontSize,
        /// <summary>
        /// Text color.
        /// </summary>
        TextColor,
        /// <summary>
        /// Text offset.
        /// </summary>
        TextOffset,
        /// <summary>
        /// Max width.
        /// </summary>
        MaxWidth,
        /// <summary>
        /// Text halo color.
        /// </summary>
        TextHaloColor,
        /// <summary>
        /// Text halo radius.
        /// </summary>
        TextHaloRadius
    }
}