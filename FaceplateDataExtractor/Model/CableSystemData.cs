﻿namespace FaceplateDataExtractor.Model
{
    public class CableSystemData
    {
        public SystemType SystemType { get; set; }
        public CableType CableType { get; set; }
        public int Quantity { get; set; }
        public string Destination { get; set; }

        public CableSystemData() {
            SystemType = SystemType.NONE;
            CableType = CableType.NONE;
            Quantity = 0;
            Destination = "";
        }
        public CableSystemData(SystemType systemType, CableType cableType, int quantity, string destination)
        {
            SystemType = systemType;
            CableType = cableType;
            Quantity = quantity;
            Destination = destination;
        }
    }
}
