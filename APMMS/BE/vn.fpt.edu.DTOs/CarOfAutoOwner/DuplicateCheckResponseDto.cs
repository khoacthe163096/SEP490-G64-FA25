namespace BE.vn.fpt.edu.DTOs.CarOfAutoOwner
{
    public class DuplicateCheckResponseDto
    {
        public bool LicensePlateExists { get; set; }
        public bool VinNumberExists { get; set; }
        public bool EngineNumberExists { get; set; }
    }
}

