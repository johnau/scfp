using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FaceplateGeneratorCore.Model
{
    /// <summary>
    /// Immutable representation of a fixing hole
    /// </summary>
    public class FixingHole : DrawingObject
    {
        
        /// <summary>
        /// Type name of the fixing hole. ie. M4, or Main Fixing or something
        /// </summary>
        /// <remarks>
        /// Make into an object
        /// </remarks>
        public string Type { get; }

        public FixingHole(string id, string name, string type)
            : base(id, name)
        {
            Type = type;
        }
    }
}
