using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TimeAttendance.Model
{
    public class FaceBox
    {
        //
        // Summary:
        //     The X coordinate, in pixels, of the top left corner of the bounding box.
        public uint X { get; set; }
        //
        // Summary:
        //     The Y coordinate, in pixels, of the top left corner of the bounding box.
        public uint Y { get; set; }
        //
        // Summary:
        //     The Width, in pixels, of the bounding box.
        public uint Width { get; set; }
        //
        // Summary:
        //     The Height, in pixels, of the bounding box
        public uint Height { get; set; }
    }
}
