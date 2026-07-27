namespace GymX.Domain.Contracts;

public interface ISoftDelete
{
    bool IsDeleted { get; set; }    
    DateTime? DeletedDate { get; set; }
}
