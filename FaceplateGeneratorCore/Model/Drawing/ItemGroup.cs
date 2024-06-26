﻿namespace FaceplateGeneratorCore.Model.Drawing
{
    /// <summary>
    /// Immutable representation of a group of items in a <see cref="PlateSection"/>
    /// </summary>
    /// <remarks>
    /// The most common object in an ItemGroup will be a <see cref="Socket"/>
    /// </remarks>
    public class ItemGroup : DrawingObject
    {

        public List<Socket> Sockets { get; }

        public ItemGroup(string id, string name)
            : base(id, name)
        {
            Sockets = [];
        }
    }
}
