public enum Specialization
{
    Mathematics,
    ComputerScience,
    Physics
}
public class Student
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public double Average { get; set; }
    public Specialization Specialization { get; set; }
}