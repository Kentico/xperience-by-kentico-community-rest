using CMS.DataEngine;

namespace Xperience.Community.Rest.Models
{
    /// <summary>
    /// The metadata for a field of an object type.
    /// </summary>
    /// <param name="name">The field name.</param>
    public class FieldMeta(string name)
    {
        /// <summary>
        /// The field name.
        /// </summary>
        public string Name { get; set; } = name;


        /// <summary>
        /// The field caption.
        /// </summary>
        public string? Caption { get; set; }


        /// <summary>
        /// If <c>true</c>, the field does not allow <c>null</c>.
        /// </summary>
        public bool IsRequired { get; set; }


        /// <summary>
        /// If <c>true</c>, the field's value must be unique.
        /// </summary>
        public bool IsUnique { get; set; }


        /// <summary>
        /// The maximum size of the field value. If 0, the size is unset.
        /// </summary>
        public int Size { get; set; }


        /// <summary>
        /// The type of the field value, from <see cref="FieldDataType"/>.
        /// </summary>
        public string? DataType { get; set; }


        /// <summary>
        /// The default value.
        /// </summary>
        public string? DefaultValue { get; set; }
    }
}
