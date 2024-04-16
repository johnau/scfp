﻿namespace FaceplateGeneratorCore.Model
{
    /// <summary>
    /// Immutable representation of a Faceplate
    /// </summary>
    /// <remarks>
    /// A Faceplate comprises of a range of possible connectors and 
    /// configurations.  
    /// Connectors are arrange in a hierarchy of;
    /// Panel > Section > Group > Item.
    /// 
    /// TODO: Implement builder pattern - maintain a constant valid state
    /// </remarks>
    public class Faceplate
    {
        public List<FixingHole> FixingHoles { get; }
        public List<PlateSection> Sections { get; }
        public List<Obstruction> Obstructions { get; }

    }
}
