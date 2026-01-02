namespace FMMEditor.Models
{
    public class ClubOption(int id, string fullname)
    {
        public int Id { get; set; } = id;
        public string FullName { get; set; } = fullname;

        public override string ToString() => FullName;
    }
}
