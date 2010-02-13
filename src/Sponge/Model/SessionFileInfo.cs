using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using SIL.Sponge.Controls;
using SIL.Sponge.Model.MetaData;

namespace SIL.Sponge.Model
{
	#region SessionFileData class
	/// ----------------------------------------------------------------------------------------
	/// <summary>
	/// Class defining a field/value pair for a single session file field.
	/// </summary>
	/// ----------------------------------------------------------------------------------------
	[XmlType("field")]
	public class SessionFileData : IInfoPanelField
	{
		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Initializes a new instance of the <see cref="SessionFileData"/> class.
		/// </summary>
		/// ------------------------------------------------------------------------------------
		public SessionFileData()
		{
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Initializes a new instance of the <see cref="SessionFileData"/> class.
		/// </summary>
		/// ------------------------------------------------------------------------------------
		public SessionFileData(string fieldName, string displayText)
		{
			FieldName = fieldName;
			DisplayText = displayText;
		}

		[XmlAttribute("name")]
		public string FieldName { get; set; }

		[XmlText]
		public string Value { get; set; }

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Gets the field's text.
		/// </summary>
		/// ------------------------------------------------------------------------------------
		[XmlIgnore]
		public string DisplayText { get; set; }
	}

	#endregion
}
