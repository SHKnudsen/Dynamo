using System;
using System.Xml;
using Dynamo.Utilities;

namespace Dynamo.Graph
{
    public class WirePinModel : ModelBase
    {
 
        /// <summary>
        /// Creates NoteModel.
        /// </summary>
        /// <param name="x">X coordinate of note.</param>
        /// <param name="y">Y coordinate of note.</param>
        /// <param name="text">Text of note</param>
        /// <param name="guid">Unique id of note</param>
        public WirePinModel(double x, double y, Guid guid)
        {
            X = x;
            Y = y;
            GUID = guid;
        }

        #region Command Framework Supporting Methods

        protected override bool UpdateValueCore(UpdateValueParams updateValueParams)
        {
            string name = updateValueParams.PropertyName;
            string value = updateValueParams.PropertyValue;

            return base.UpdateValueCore(updateValueParams);
        }
        #endregion

        #region Serialization/Deserialization Methods

        protected override void SerializeCore(XmlElement element, SaveContext context)
        {
            var helper = new XmlElementHelper(element);
            helper.SetAttribute("guid", GUID);
            helper.SetAttribute("x", X);
            helper.SetAttribute("y", Y);
        }

        protected override void DeserializeCore(XmlElement nodeElement, SaveContext context)
        {
            var helper = new XmlElementHelper(nodeElement);
            GUID = helper.ReadGuid("guid", GUID);
            X = helper.ReadDouble("x", 0.0);
            Y = helper.ReadDouble("y", 0.0);

            // Notify listeners that the position of the note has changed, 
            // then parent group will also redraw itself.
            ReportPosition();
        }

        #endregion
    }
}
