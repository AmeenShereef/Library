namespace BookLibrary.Data.Entities
{
    public class AuditableEntity : IAuditableEntity
    {       
        public DateTime? CreationTime { get; set; }

        public long? CreatorUserId { get; set; }
       
        public DateTime? LastModificationTime { get; set; }

        public long? LastModifierUserId { get; set; }

    }

    public class AuditableEntityWithDelete : AuditableEntity
    {

        public bool IsDeleted { get; set; }

        public long? DeleterUserId { get; set; }

        public DateTime? DeletionTime { get; set; }

    }

    public interface IAuditableEntity
    {
        DateTime? CreationTime { get; set; }

        long? CreatorUserId { get; set; }

        DateTime? LastModificationTime { get; set; }

        long? LastModifierUserId { get; set; }
      
    }
}
