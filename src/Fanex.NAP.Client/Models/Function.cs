namespace Fanex.NAP.Client.Models;

public class Function
{
    public short Id { get; set; }

    public string Name { get; set; }

    public string Key { get; set; }

    public short DisplayOrder { get; set; }

    public bool IsActive { get; set; }

    public string Environment { get; set; }

    public int ParentId { get; set; }

    public string ParentName { get; set; }
}