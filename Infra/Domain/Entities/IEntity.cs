namespace Infra.Domain.Entities
{
    public interface IEntity
    {
        public long Id { get; set; }
        public string Identifier { get; set; }
    }
}
