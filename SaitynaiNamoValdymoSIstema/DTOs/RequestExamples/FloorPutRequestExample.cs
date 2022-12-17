using SaitynaiNamoValdymoSIstema.DataDB;
using Swashbuckle.AspNetCore.Filters;

namespace SaitynaiNamoValdymoSIstema.DTOs.RequestExamples
{
    public class FloorPutRequestExample : IExamplesProvider<Floor>
    {
        public Floor GetExamples()
        {
            return new Floor()
            {
                Id = 1,
                WhichFloor = 1
            };
        }
    }
}
