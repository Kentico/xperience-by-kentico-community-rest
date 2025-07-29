namespace Xperience.Community.Rest.Models
{
    /// <summary>
    /// The metadata for an Xperience by Kentico object type.
    /// </summary>
    /// <param name="objectType">The object type class name.</param>
    public class ObjectMeta(string objectType)
    {
        /// <summary>
        /// The object type class name.
        /// </summary>
        public string ObjectType { get; set; } = objectType;


        /// <summary>
        /// The display name.
        /// </summary>
        public string? DisplayName { get; set; }


        /// <summary>
        /// The name of the code name column, or null if not present.
        /// </summary>
        public string? CodeNameColumn { get; set; }


        /// <summary>
        /// The name of the ID column, or null if not present.
        /// </summary>
        public string? IdColumn { get; set; }


        /// <summary>
        /// The name of the GUID column, or null if not present.
        /// </summary>
        public string? GuidColumn { get; set; }


        /// <summary>
        /// The type of the class, from <see cref="CMS.DataEngine.ClassType"/>.
        /// </summary>
        public string? ClassType { get; set; }


        /// <summary>
        /// A list of the class fields and their metadata.
        /// </summary>
        public IEnumerable<FieldMeta> Fields { get; set; } = [];
    }
}
