namespace TaskManagement.Application.DTOs.Sprint
{
    public class BurndownDataDto
    {
        public string Date { get; set; } = string.Empty;
        public int IdealPoints { get; set; }
        public int RemainingPoints { get; set; }
    }
}
