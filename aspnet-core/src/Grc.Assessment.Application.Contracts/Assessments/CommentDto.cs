using System;

namespace Grc.Assessments;

/// <summary>
/// Comment DTO
/// </summary>
public class CommentDto
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public string UserName { get; set; }
    public string Comment { get; set; }
    public DateTime CreationTime { get; set; }
    public bool IsInternal { get; set; }
}

